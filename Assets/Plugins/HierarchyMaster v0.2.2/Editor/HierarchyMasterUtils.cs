using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HierarchyMaster
{
    public static class HierarchyMasterUtils
    {
        //*/////////////////////
        //* REFLECTION
        //*/////////////////////
        public static List<Member> GetCallSequenceObjects(this Component component, string[] callSequence, out bool callSequenceValid)
        {
            List<Member> callObjects = new List<Member>();
            object currentObj = component;
            callSequenceValid = false;

            for (int i = 1; i < callSequence.Length; i++) //Index "i" starts from 1 because 0 is always a Component
            {
                string fieldName = callSequence[i];
                Member memberVar = new Member(currentObj, fieldName);
                callSequenceValid = memberVar.IsValid;
                if (callSequenceValid == false || currentObj == null) { break; }
                currentObj = memberVar.GetValue();
                callObjects.Add(memberVar);
            }

            return callObjects;
        }


        //*//////////////////////////
        //* GET VALUE ON COMPONENT
        //*//////////////////////////
        public static bool GetValue(this Component component, string[] callSequence, out object result, int fieldStartIndex = 1)
        {
            bool fieldExists = true;
            result = component;
            for (int i = fieldStartIndex; i < callSequence.Length; i++)
            {
                string fieldName = callSequence[i];
                fieldExists = result.GetValue(fieldName, out result);
                if (fieldExists == false || result == null) { break; }
            }
            return fieldExists;
        }

        private static bool GetValue(this object obj, string fieldName, out object result)
        {
            bool fieldExists = false;
            result = null;
            if (IsFieldNameOfCollectionType(fieldName))
            {
                fieldExists = GetValueFromCollection(obj, fieldName, out result);
            }
            else
            {
                fieldExists = GetValueFromMember(obj, fieldName, out result);
            }
            return fieldExists;
        }

        private static bool GetValueFromCollection(object obj, string fieldName, out object result)
        {
            bool fieldExists = false;
            string collectionName;
            int arrayIndex;
            CollectionNameAndArrayIndexFromString(fieldName, out collectionName, out arrayIndex);

            try
            {
                object collectionObj;
                fieldExists = GetValueFromMember(obj, collectionName, out collectionObj);
                System.Collections.IList collection = collectionObj as System.Collections.IList;
                result = collection[arrayIndex];
                return fieldExists;
            }
            catch (Exception)
            {
                fieldExists = false;
                result = null;
                return fieldExists;
            }
        }

        private static bool GetValueFromMember(object obj, string fieldName, out object result)
        {
            bool fieldExists = false; result = null;
            Type objType = obj.GetType();
            PropertyInfo propertyInfo = objType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (propertyInfo != null)
            {
                fieldExists = true;
                try
                {
                    //Throws exception if Property isn't implemented. 
                    //Happens to Unity's deprecated properties.
                    result = propertyInfo.GetValue(obj, null);
                    return fieldExists;
                }
                catch { return fieldExists; }
            }

            FieldInfo fieldInfo = objType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldExists = true;
                result = fieldInfo.GetValue(obj);
            }
            return fieldExists;
        }


        //*//////////////////////////
        //* SET VALUE ON COMPONENT
        //*//////////////////////////
        public static void SetValue(this Component component, string[] callSequence, object intendedValueObj)
        {
            bool fieldExists = false;
            List<Member> callSequenceObjs = component.GetCallSequenceObjects(callSequence, out fieldExists);

            if (fieldExists == false)
            {
                Debug.LogWarning("Unable to locate field/property");
                return;
            }

            for (int i = callSequenceObjs.Count - 1; i >= 0; i--)
            {
                Member targetObj = callSequenceObjs[i];
                targetObj.AssignValue(intendedValueObj);
                if (targetObj.IsValueType == false) { break; }
                intendedValueObj = targetObj.ContainingObj;
            }
        }

        private static void SetElementInCollection<T>(object member, string fieldName, T intendedValue)
        {
            string collectionName;
            int arrayIndex;
            CollectionNameAndArrayIndexFromString(fieldName, out collectionName, out arrayIndex);

            try
            {
                object result;
                bool collectionExists = GetValueFromMember(member, collectionName, out result);
                if (collectionExists)
                {
                    System.Collections.IList collection = result as System.Collections.IList;
                    collection[arrayIndex] = intendedValue;
                }
                else
                {
                    Debug.LogWarning("Unable to locate collection");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to edit collection:" + e.Message);
            }
        }

        private static void SetField(this object targetObj, FieldInfo fieldInfo, object intendedValue)
        {
            PrepareUndo(targetObj);
            try
            {
                if (intendedValue.GetType() == typeof(string))
                {
                    intendedValue = ConvertStringToReflectedObject(fieldInfo.FieldType, intendedValue as string);
                }
                fieldInfo.SetValue(targetObj, intendedValue);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to set value: " + e.Message);
            }
        }

        private static void SetProperty(this object targetObj, PropertyInfo propertyInfo, object intendedValue)
        {
            PrepareUndo(targetObj);
            try
            {
                if (intendedValue.GetType() == typeof(string))
                {
                    intendedValue = ConvertStringToReflectedObject(propertyInfo.PropertyType, intendedValue as string);
                }

                Type typeOfAssignment = intendedValue.GetType();
                if (typeOfAssignment.IsValueType)
                {
                    object boxed = targetObj;
                    propertyInfo.SetValue(boxed, intendedValue, null);
                    targetObj = boxed;
                }
                propertyInfo.SetValue(targetObj, intendedValue, null);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to set value: " + e.Message);
            }
        }


        //*/////////////////////
        //* UTILITY FUNCTIONS
        //*/////////////////////
        private static void PrepareUndo(object obj)
        {
            UnityEngine.Object undoObj = obj as UnityEngine.Object;
            if (undoObj != null)
            {
                Undo.RecordObject(undoObj, "Change Values");
                return;
            }
        }

        public static void ReplaceComponent(this Component componentBeingReplaced, UnityEngine.Object replacementObject)
        {
            GameObject targetGO = componentBeingReplaced.gameObject;
            MonoScript monoScript = replacementObject as MonoScript;
            if (monoScript != null)
            {
                Undo.DestroyObjectImmediate(componentBeingReplaced);
                AttachScriptToGameObject(monoScript, targetGO);
                return;
            }

            Component replacementComponent = replacementObject as Component;
            if (replacementComponent != null)
            {
                Undo.DestroyObjectImmediate(componentBeingReplaced);
                Undo.AddComponent(targetGO, replacementComponent.GetType());
                return;
            }
        }

        public static void Reflect(this GameObject go, string componentFilter)
        {
            string[] componentFilterArray = componentFilter.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Component component = go.GetComponent(componentFilterArray[0]);

            if (component != null)
            {
                object retrievedValue;
                bool fieldExists = component.GetValue(componentFilterArray, out retrievedValue);
                if (fieldExists == false)
                {
                    Debug.LogWarning("Unable to locate field/property");
                    return;
                }
                PrintFieldsAndPropertiesOfType(component, retrievedValue, componentFilter);
            }
            else
            {
                Debug.Log("Invalid Component name");
            }
        }

        public static void PrintFieldsAndPropertiesOfType(Component component, object obj, string varPath)
        {
            Type componentType = obj.GetType();
            FieldInfo[] fieldInfos = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fieldInfos.Length > 0)
            {
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    string fieldName = fieldInfos[i].Name;
                    object result;
                    bool fieldExists = GetValue(obj, fieldName, out result);
                    if (fieldExists == true)
                    {
                        Type fieldType = fieldInfos[i].FieldType;
                        UnityEngine.Object goPing = (fieldType.IsSubclassOf(typeof(UnityEngine.Object))) ? (UnityEngine.Object)result : component;
                        Debug.Log(varPath + "." + fieldName + " = " + result, goPing);
                    }
                    else
                    {
                        Debug.LogWarning("Unable to locate field/property");
                        return;
                    }
                }
            }

            PropertyInfo[] propertyInfos = componentType.GetProperties();
            if (propertyInfos.Length > 0)
            {
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    string propertyName = propertyInfos[i].Name;
                    object propertyValue;
                    bool fieldExists = GetValue(obj, propertyName, out propertyValue);
                    if (fieldExists)
                    {
                        Type propertyType = propertyInfos[i].PropertyType;
                        UnityEngine.Object goPing = (propertyType.IsSubclassOf(typeof(UnityEngine.Object))) ? (UnityEngine.Object)propertyValue : component;
                        Debug.Log(varPath + "." + propertyName + " = " + propertyValue, goPing);
                    }
                    else
                    {
                        Debug.LogWarning("Unable to locate field/property");
                        // return;
                    }
                }
            }
        }

        public static bool IsNull(this object obj)
        {
            if (obj == null) { return true; }
            if (obj.GetType().IsSubclassOf(typeof(UnityEngine.Object)))
            {
                UnityEngine.Object unityObj = (UnityEngine.Object)obj;
                bool isNull = !unityObj;
                return isNull;
            }
            return false;
        }

        private static void CollectionNameAndArrayIndexFromString(string fieldName, out string collectionName, out int arrayIndex)
        {
            string arrayIndexStr = Regex.Match(fieldName, @"(?<=\[)\d*(?=\])").Value;
            arrayIndex = int.Parse(arrayIndexStr);
            collectionName = fieldName.Replace("[" + arrayIndexStr + "]", string.Empty);
        }

        public static void RegexFilterOnSelectedGameObjects(string regexFilter, List<GameObject> filterResults, bool searchChildren)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            UnityEngine.Object[] currentSelectedGOs = (selectedGameObjects.Length > 0) ? selectedGameObjects : Resources.FindObjectsOfTypeAll(typeof(GameObject));

            for (int i = 0; i < currentSelectedGOs.Length; i++)
            {
                GameObject go = currentSelectedGOs[i] as GameObject;
                if (IsSceneObject(go) == false) { continue; }
                RegexFilter(go, regexFilter, filterResults, searchChildren);
            }
        }

        public static void RegexFilter(GameObject go, string regexFilter, List<GameObject> filterResults, bool searchChildren)
        {
            if (Regex.IsMatch(go.name, regexFilter)) { filterResults.Add(go); }

            if (searchChildren)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform childTransform = go.transform.GetChild(i);
                    RegexFilter(childTransform.gameObject, regexFilter, filterResults, searchChildren);
                }
            }
        }

        public static void ComponentFilterOnSelectedGameObjects(string fields, List<GameObject> matches, bool searchChildren, string compareOperator, string expectedValue)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            object[] currentSelectedGOs = (selectedGameObjects.Length > 0) ? selectedGameObjects : Resources.FindObjectsOfTypeAll(typeof(GameObject));

            for (int i = 0; i < currentSelectedGOs.Length; i++)
            {
                GameObject go = currentSelectedGOs[i] as GameObject;
                if (IsSceneObject(go) == false) { continue; }
                ComponentFilter(go, fields, matches, searchChildren, compareOperator, expectedValue);
            }
        }

        private static bool IsSceneObject(GameObject go)
        {
            return (PrefabUtility.GetPrefabParent(go) != null || PrefabUtility.GetPrefabObject(go) == null);
        }

        public static void ComponentFilter(GameObject go, string fields, List<GameObject> matches, bool searchChildren, string compareOperator, string expectedValue)
        {
            bool haveMatch = ComponentMatchesExpectedValues(fields, go, compareOperator, expectedValue);
            if (haveMatch == true) { matches.Add(go); }

            if (searchChildren)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform childTransform = go.transform.GetChild(i);
                    ComponentFilter(childTransform.gameObject, fields, matches, searchChildren, compareOperator, expectedValue);
                }
            }
        }

        public static bool ComponentMatchesExpectedValues(string fields, GameObject go, string compareOperator, string expectedValue)
        {
            if (fields == "<null>")
            {
                return go.HasMissingComponents();
            }

            return go.HasComponentAndRequiredFields(fields, compareOperator, expectedValue);
        }

        private static bool HasComponentAndRequiredFields(this GameObject go, string fields, string compareOperator, string expectedValue)
        {
            string[] codeSegments = fields.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string componentString = codeSegments[0]; //Element 0 should always be a "UnityEngine.Component" type
            Component component = go.GetComponent(componentString);
            object evaluatedValue = component;

            if (component != null)
            {
                bool fieldExists = component.GetValue(codeSegments, out evaluatedValue);
                if (fieldExists == false) { return false; }
            }

            return EvaluateValueUsingCompareOp(evaluatedValue, compareOperator, expectedValue);
        }

        private static bool HasMissingComponents(this GameObject go)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null) { return true; }
            }
            return false;
        }

        public static void NumberSelectedGameObjects(string regexFilter, string numPrefix)
        {
            GameObject[] selected = Selection.gameObjects;
            for (int i = 0; i < selected.Length; i++)
            {
                RegexRename(selected[i].gameObject, regexFilter, numPrefix + i.ToString());
                selected[i].transform.SetSiblingIndex(i);
            }
        }

        public static void RegexRename(GameObject go, string search, string replace)
        {
            Undo.RecordObject(go, "Rename Operation");
            go.name = Regex.Replace(go.name, search, replace);
        }

        public static void ChildInclusiveRegexRename(Transform t, string regexFilter, string regexReplace)
        {
            RegexRename(t.gameObject, regexFilter, regexReplace);

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                ChildInclusiveRegexRename(child, regexFilter, regexReplace);
            }
        }

        public static void RemoveNullComponentsOnGameObject(GameObject go)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            EditorUtility.SetDirty(go);            
        }

        public static void AttachScriptToGameObject(MonoScript script, GameObject gameObject)
        {
            ThrowExceptionIfScriptIsNotMonoBehavior(script);
            Type classType = script.GetClass();
            Undo.AddComponent(gameObject, classType);
        }

        private static bool IsFieldNameOfCollectionType(string fieldName)
        {
            return Regex.IsMatch(fieldName, @"\[\d*\]");
        }

        public static void RemoveComponentFromGameObject(Component componentToRemove, GameObject gameObject)
        {
            Undo.DestroyObjectImmediate(componentToRemove);
        }

        private static void ThrowExceptionIfScriptIsNotMonoBehavior(MonoScript script)
        {
            Type classType = script.GetClass();
            if (classType.IsSubclassOf(typeof(MonoBehaviour)) == false)
            {
                throw new Exception("Script " + script.name + " is not a MonoBehavior and cannot be attached to game object.");
            }
        }

        public static object ConvertStringToReflectedObject(Type type, string str)
        {
            try
            {
                return Convert.ChangeType(str, type);
            }
            catch
            {
                try
                {
                    return Enum.Parse(type, str);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static bool EvaluateValueUsingCompareOp(object valueToEvaluate, string compareOperator, string expectedValue)
        {
            bool evaluatedValueIsNull = valueToEvaluate.IsNull();
            if (compareOperator == "IsNull") { return evaluatedValueIsNull; }
            if (compareOperator == "NotNull") { return !evaluatedValueIsNull; }

            Type evaluatedType;
            if (evaluatedValueIsNull == true)
            {
                return false;
            }
            else
            {
                evaluatedType = valueToEvaluate.GetType();
            }

            if (evaluatedType == typeof(bool))
            {
                if (compareOperator == "==")
                {
                    return (bool)valueToEvaluate == bool.Parse(expectedValue);
                }
                if (compareOperator == "!=") { return (bool)valueToEvaluate != bool.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(byte))
            {
                if (compareOperator == "==") { return (byte)valueToEvaluate == byte.Parse(expectedValue); }
                if (compareOperator == "!=") { return (byte)valueToEvaluate != byte.Parse(expectedValue); }
                if (compareOperator == "<") { return (byte)valueToEvaluate < byte.Parse(expectedValue); }
                if (compareOperator == ">") { return (byte)valueToEvaluate > byte.Parse(expectedValue); }
                if (compareOperator == "<=") { return (byte)valueToEvaluate <= byte.Parse(expectedValue); }
                if (compareOperator == ">=") { return (byte)valueToEvaluate >= byte.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(char))
            {
                if (compareOperator == "==") { return (char)valueToEvaluate == char.Parse(expectedValue); }
                if (compareOperator == "!=") { return (char)valueToEvaluate != char.Parse(expectedValue); }
                if (compareOperator == "<") { return (char)valueToEvaluate < char.Parse(expectedValue); }
                if (compareOperator == ">") { return (char)valueToEvaluate > char.Parse(expectedValue); }
                if (compareOperator == "<=") { return (char)valueToEvaluate <= char.Parse(expectedValue); }
                if (compareOperator == ">=") { return (char)valueToEvaluate >= char.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(decimal))
            {
                if (compareOperator == "==") { return (decimal)valueToEvaluate == decimal.Parse(expectedValue); }
                if (compareOperator == "!=") { return (decimal)valueToEvaluate != decimal.Parse(expectedValue); }
                if (compareOperator == "<") { return (decimal)valueToEvaluate < decimal.Parse(expectedValue); }
                if (compareOperator == ">") { return (decimal)valueToEvaluate > decimal.Parse(expectedValue); }
                if (compareOperator == "<=") { return (decimal)valueToEvaluate <= decimal.Parse(expectedValue); }
                if (compareOperator == ">=") { return (decimal)valueToEvaluate >= decimal.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(double))
            {
                if (compareOperator == "==") { return (double)valueToEvaluate == double.Parse(expectedValue); }
                if (compareOperator == "!=") { return (double)valueToEvaluate != double.Parse(expectedValue); }
                if (compareOperator == "<") { return (double)valueToEvaluate < double.Parse(expectedValue); }
                if (compareOperator == ">") { return (double)valueToEvaluate > double.Parse(expectedValue); }
                if (compareOperator == "<=") { return (double)valueToEvaluate <= double.Parse(expectedValue); }
                if (compareOperator == ">=") { return (double)valueToEvaluate >= double.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(float))
            {
                if (compareOperator == "==") { return Mathf.Approximately((float)valueToEvaluate, float.Parse(expectedValue)); }
                if (compareOperator == "!=") { return (float)valueToEvaluate != float.Parse(expectedValue); }
                if (compareOperator == "<") { return (float)valueToEvaluate < float.Parse(expectedValue); }
                if (compareOperator == ">") { return (float)valueToEvaluate > float.Parse(expectedValue); }
                if (compareOperator == "<=") { return (float)valueToEvaluate <= float.Parse(expectedValue); }
                if (compareOperator == ">=") { return (float)valueToEvaluate >= float.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(int))
            {
                if (compareOperator == "==") { return (int)valueToEvaluate == int.Parse(expectedValue); }
                if (compareOperator == "!=") { return (int)valueToEvaluate != int.Parse(expectedValue); }
                if (compareOperator == "<") { return (int)valueToEvaluate < int.Parse(expectedValue); }
                if (compareOperator == ">") { return (int)valueToEvaluate > int.Parse(expectedValue); }
                if (compareOperator == "<=") { return (int)valueToEvaluate <= int.Parse(expectedValue); }
                if (compareOperator == ">=") { return (int)valueToEvaluate >= int.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(long))
            {
                if (compareOperator == "==") { return (long)valueToEvaluate == long.Parse(expectedValue); }
                if (compareOperator == "!=") { return (long)valueToEvaluate != long.Parse(expectedValue); }
                if (compareOperator == "<") { return (long)valueToEvaluate < long.Parse(expectedValue); }
                if (compareOperator == ">") { return (long)valueToEvaluate > long.Parse(expectedValue); }
                if (compareOperator == "<=") { return (long)valueToEvaluate <= long.Parse(expectedValue); }
                if (compareOperator == ">=") { return (long)valueToEvaluate >= long.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(sbyte))
            {
                if (compareOperator == "==") { return (sbyte)valueToEvaluate == sbyte.Parse(expectedValue); }
                if (compareOperator == "!=") { return (sbyte)valueToEvaluate != sbyte.Parse(expectedValue); }
                if (compareOperator == "<") { return (sbyte)valueToEvaluate < sbyte.Parse(expectedValue); }
                if (compareOperator == ">") { return (sbyte)valueToEvaluate > sbyte.Parse(expectedValue); }
                if (compareOperator == "<=") { return (sbyte)valueToEvaluate <= sbyte.Parse(expectedValue); }
                if (compareOperator == ">=") { return (sbyte)valueToEvaluate >= sbyte.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(short))
            {
                if (compareOperator == "==") { return (short)valueToEvaluate == short.Parse(expectedValue); }
                if (compareOperator == "!=") { return (short)valueToEvaluate != short.Parse(expectedValue); }
                if (compareOperator == "<") { return (short)valueToEvaluate < short.Parse(expectedValue); }
                if (compareOperator == ">") { return (short)valueToEvaluate > short.Parse(expectedValue); }
                if (compareOperator == "<=") { return (short)valueToEvaluate <= short.Parse(expectedValue); }
                if (compareOperator == ">=") { return (short)valueToEvaluate >= short.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(uint))
            {
                if (compareOperator == "==") { return (uint)valueToEvaluate == uint.Parse(expectedValue); }
                if (compareOperator == "!=") { return (uint)valueToEvaluate != uint.Parse(expectedValue); }
                if (compareOperator == "<") { return (uint)valueToEvaluate < uint.Parse(expectedValue); }
                if (compareOperator == ">") { return (uint)valueToEvaluate > uint.Parse(expectedValue); }
                if (compareOperator == "<=") { return (uint)valueToEvaluate <= uint.Parse(expectedValue); }
                if (compareOperator == ">=") { return (uint)valueToEvaluate >= uint.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(ulong))
            {
                if (compareOperator == "==") { return (ulong)valueToEvaluate == ulong.Parse(expectedValue); }
                if (compareOperator == "!=") { return (ulong)valueToEvaluate != ulong.Parse(expectedValue); }
                if (compareOperator == "<") { return (ulong)valueToEvaluate < ulong.Parse(expectedValue); }
                if (compareOperator == ">") { return (ulong)valueToEvaluate > ulong.Parse(expectedValue); }
                if (compareOperator == "<=") { return (ulong)valueToEvaluate <= ulong.Parse(expectedValue); }
                if (compareOperator == ">=") { return (ulong)valueToEvaluate >= ulong.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType == typeof(short))
            {
                if (compareOperator == "==") { return (short)valueToEvaluate == short.Parse(expectedValue); }
                if (compareOperator == "!=") { return (short)valueToEvaluate != short.Parse(expectedValue); }
                if (compareOperator == "<") { return (short)valueToEvaluate < short.Parse(expectedValue); }
                if (compareOperator == ">") { return (short)valueToEvaluate > short.Parse(expectedValue); }
                if (compareOperator == "<=") { return (short)valueToEvaluate <= short.Parse(expectedValue); }
                if (compareOperator == ">=") { return (short)valueToEvaluate >= short.Parse(expectedValue); }
                return false;
            }
            else if (evaluatedType.IsEnum)
            {
                bool match = valueToEvaluate.Equals(Enum.Parse(evaluatedType, expectedValue));
                if (compareOperator == "==") { return match; }
                if (compareOperator == "!=") { return !match; }
                return false;
            }
            else
            {
                try
                {
                    if (compareOperator == "==") { return ((string)valueToEvaluate == expectedValue); }
                    if (compareOperator == "!=") { return ((string)valueToEvaluate != expectedValue); }
                    if (compareOperator == "Regex") { return Regex.IsMatch((string)valueToEvaluate, expectedValue); }
                }
                catch
                {
                    Debug.LogError("Type comparison for \"" + evaluatedType.ToString() + "\" has not been implemented yet.");
                }
                return false;
            }
        }

        [MenuItem("CustomAddOns/Ping selected Game objects")]
        public static void PingSelectedObjects()
        {
            GameObject[] currentSelectedGOs = Selection.gameObjects;

            for (int i = 0; i < currentSelectedGOs.Length; i++)
            {
                EditorGUIUtility.PingObject(currentSelectedGOs[i]);
            }
        }







        public class Member
        {
            private PropertyInfo propertyInfo;
            private FieldInfo fieldInfo;
            private object memberValue;

            public object ContainingObj { get; private set; }
            public bool IsValueType { get { return ContainingObj.GetType().IsValueType; } }
            public bool IsValid { get { return (propertyInfo != null || fieldInfo != null); } }

            public Member(object containingObj, string memberName)
            {
                this.ContainingObj = containingObj;
                this.propertyInfo = containingObj.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                this.fieldInfo = containingObj.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            public void AssignValue(object valueObj)
            {
                if (propertyInfo != null)
                {
                    ContainingObj.SetProperty(propertyInfo, valueObj);
                }

                if (fieldInfo != null)
                {
                    ContainingObj.SetField(fieldInfo, valueObj);
                }
            }

            public object GetValue()
            {
                object retrievedValue = null;
                if (propertyInfo != null)
                {
                    try
                    {
                        //Throws exception if Property isn't implemented. 
                        //Happens to Unity's deprecated properties.
                        retrievedValue = propertyInfo.GetValue(ContainingObj, null);
                    }
                    catch { retrievedValue = null; }
                }

                if (fieldInfo != null)
                {
                    retrievedValue = this.fieldInfo.GetValue(ContainingObj);
                }
                return retrievedValue;
            }
        }

    }
}