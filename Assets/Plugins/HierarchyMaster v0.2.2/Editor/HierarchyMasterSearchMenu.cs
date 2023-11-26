using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HierarchyMaster
{
    public class HierarchyMasterSearchMenu : EditorWindow
    {
        private string componentFilter;
        private string matchValue;
        private string replacementValue = string.Empty;
        private string[] evaluationOperators = new string[] { "NotNull", "IsNull", "Regex", "==", "!=", "<", ">", "<=", ">=" };
        private int evaluationOperatorIndex = 0;
        private bool includeChildrenWhenFiltering;
        private List<GameObject> filterResults = new List<GameObject>();
        private int selectedMatchIndex = 0;
        private Vector2 scroll = Vector2.zero;
        UnityEngine.Object replacementObj = null;
        private bool runComponentFilter = false;
        private bool runObjectReplace = false;
        private bool runValueReplace = false;
        private bool addObject = false;
        private bool runRemoveComponent = false;


        //*======================
        //* EDITOR WINDOW ENTRY
        //*======================
        [MenuItem("CustomAddOns/Open HierarchyMasterSearch")]
        public static void Init()
        {
            EditorWindow.GetWindow<HierarchyMasterSearchMenu>();
        }


        //*======================
        //* UNITY FUNCTIONS
        //*======================
        public void OnSelectionChange()
        {
            Repaint();
        }

        public void OnGUI()
        {
            ProcessUserKeyPresses(); ProcessUserInput();
            DrawMenuAndGetUIInput(); ProcessUserInput();
        }


        //*======================
        //* PRIVATE FUNCTIONS
        //*======================
        private void DrawMenuAndGetUIInput()
        {
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            {
                RenderSearchScopeInfoPanel();
                RenderComponentFilterPanel();
                RenderReplacePanel();
                RenderSelectionStepThroughPanel();
            }
            EditorGUILayout.EndScrollView();
        }

        private void RenderSearchScopeInfoPanel()
        {
            Color originalBGColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUI.backgroundColor = originalBGColor;
                bool noGameObjectsSelected = Selection.activeGameObject == null;
                EditorGUILayout.BeginHorizontal();
                {

                    string suffix = (this.includeChildrenWhenFiltering) ? " (Child-Inclusive)" : string.Empty;
                    if (noGameObjectsSelected)
                    {
                        EditorGUILayout.LabelField("Scope: Entire scene.", EditorStyles.boldLabel);
                        includeChildrenWhenFiltering = false;
                    }
                    else
                    {
                        if (Selection.gameObjects.Length > 1)
                        {
                            EditorGUILayout.LabelField("Scope: " + Selection.gameObjects.Length + " gameobjects." + suffix, EditorStyles.boldLabel);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Scope: " + Selection.activeObject.name + " " + suffix, EditorStyles.boldLabel);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (!noGameObjectsSelected)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        includeChildrenWhenFiltering = EditorGUILayout.Toggle(includeChildrenWhenFiltering, GUILayout.MaxWidth(12.5f));
                        EditorGUILayout.LabelField("Include Children: ", GUILayout.MaxWidth(80));
                        if (GUILayout.Button("Clear Selection", GUILayout.ExpandWidth(false)))
                        {
                            ClearHierarchySelections();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderComponentFilterPanel()
        {
            Color originalBGColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUI.backgroundColor = originalBGColor;

                    EditorGUILayout.LabelField("Component:", GUILayout.MaxWidth(75));
                    GUI.SetNextControlName("ComponentFilter1"); componentFilter = EditorGUILayout.TextField(GUIContent.none, componentFilter);
                    GUI.SetNextControlName("ComponentFilter2"); evaluationOperatorIndex = EditorGUILayout.Popup(evaluationOperatorIndex, evaluationOperators, GUILayout.MaxWidth(60));
                    if (EvaluationOperatorIsNotNullComparison())
                    {
                        GUI.SetNextControlName("ComponentFilter3"); matchValue = EditorGUILayout.TextField(GUIContent.none, matchValue, GUILayout.MaxWidth(150));
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    this.runComponentFilter = GUILayout.Button("Filter", GUILayout.ExpandWidth(false));
                    if (GUILayout.Button("Reflect", GUILayout.ExpandWidth(false)))
                    {
                        Selection.activeGameObject.Reflect(componentFilter);
                    }
                    runRemoveComponent = GUILayout.Button("Remove Component", GUILayout.ExpandWidth(false));
                    if (IsUsingRegex())
                    {
                        if (GUILayout.Button("^.*$", GUILayout.ExpandWidth(false))) { matchValue = "^.*$"; GUIUtility.keyboardControl = 0; }
                        if (GUILayout.Button("(?<=)", GUILayout.ExpandWidth(false))) { matchValue = "(?<=" + matchValue + ")"; GUIUtility.keyboardControl = 0; }
                        if (GUILayout.Button("(?=)", GUILayout.ExpandWidth(false))) { matchValue = "(?=" + matchValue + ")"; GUIUtility.keyboardControl = 0; }
                    }
                    if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false))) { componentFilter = string.Empty; matchValue = string.Empty; GUIUtility.keyboardControl = 0; }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderReplacePanel()
        {
            Color originalBGColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUI.backgroundColor = originalBGColor;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Replace with Object:", GUILayout.MaxWidth(125));
                    GUI.SetNextControlName("ObjectReplacement"); replacementObj = EditorGUILayout.ObjectField(replacementObj, typeof(UnityEngine.Object), true, GUILayout.MaxWidth(200));
                    addObject = GUILayout.Button("Add", GUILayout.ExpandWidth(false));
                    runObjectReplace = GUILayout.Button("Replace", GUILayout.ExpandWidth(false));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Replace with Value:", GUILayout.MaxWidth(125));
                    GUI.SetNextControlName("ValueReplacement"); replacementValue = EditorGUILayout.TextField(replacementValue, GUILayout.MaxWidth(200));
                    runValueReplace = GUILayout.Button("Replace", GUILayout.ExpandWidth(false));
                    if (IsUsingRegex())
                    {
                        if (GUILayout.Button("$&", GUILayout.ExpandWidth(false))) { replacementValue = "$&"; GUIUtility.keyboardControl = 0; }

                        if (GUILayout.Button("Num Suffix", GUILayout.ExpandWidth(false)))
                        {
                            try { HierarchyMasterUtils.NumberSelectedGameObjects(matchValue, replacementValue); } catch { }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderSelectionStepThroughPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                {
                    if (Selection.gameObjects.Length > 0)
                    {
                        selectedMatchIndex--;
                        selectedMatchIndex = (selectedMatchIndex < 0) ? Selection.gameObjects.Length - 1 : selectedMatchIndex;
                        EditorGUIUtility.PingObject(Selection.gameObjects[selectedMatchIndex]);
                    }
                }

                if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                {
                    if (Selection.gameObjects.Length > 0)
                    {
                        selectedMatchIndex++;
                        selectedMatchIndex = selectedMatchIndex % Selection.gameObjects.Length;
                        EditorGUIUtility.PingObject(Selection.gameObjects[selectedMatchIndex]);
                    }
                }

                if (Selection.gameObjects.Length > 1)
                {
                    if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                    {
                        GameObject highlightedGO = Selection.gameObjects[selectedMatchIndex];
                        Selection.objects = new UnityEngine.Object[0];
                        Selection.activeGameObject = highlightedGO;
                        EditorGUIUtility.PingObject(highlightedGO);
                    }
                }

                if (Selection.gameObjects.Length == 1)
                {
                    if (GUILayout.Button("List Components", GUILayout.ExpandWidth(false)))
                    {
                        GameObject activeGameObject = Selection.activeGameObject;
                        Component[] components = activeGameObject.GetComponents<Component>();
                        for (int i = 0; i < components.Length; i++)
                        {
                            if (components[i] == null)
                            {
                                Debug.Log("Missing Component");
                            }
                            else
                            {
                                Debug.Log(components[i].GetType().ToString());
                            }
                        }
                    }
                }

                EditorGUILayout.LabelField("Highlight: ", GUILayout.MaxWidth(56));
                selectedMatchIndex = Mathf.Clamp(selectedMatchIndex, 0, Selection.gameObjects.Length - 1);
                string selectionDisplay = (Selection.activeGameObject == null) ? "None" : (selectedMatchIndex + 1) + "/" + Selection.gameObjects.Length.ToString() + " (" + Selection.gameObjects[selectedMatchIndex].name + ")";

                EditorGUILayout.LabelField(selectionDisplay);
            }
            EditorGUILayout.EndHorizontal();
        }


        //*======================//
        //* LOGIC POWERING THE UI
        //*======================//
        private void ProcessUserInput()
        {
            if (runComponentFilter) { ExecuteComponentFilter(); }
            if (addObject) { ExecuteAddObject(); }
            if (runRemoveComponent) { ExecuteComponentRemoval(); }
            if (runObjectReplace) { ExecuteReplacements(); }
            if (runValueReplace) { ExecuteReplacements(); }

            runComponentFilter = false;
            runObjectReplace = false;
            runValueReplace = false;
            addObject = false;
            runRemoveComponent = false;
        }

        private void ExecuteComponentFilter()
        {
            if (componentFilter == string.Empty) { return; }

            SearchEntry currentSearchEntry = new SearchEntry(componentFilter, includeChildrenWhenFiltering, evaluationOperatorIndex, matchValue);
            bool isAddSearchToHistory = true;

            if (searchHistory.Count > 0)
            {
                SearchEntry prevSearchEntry = this.searchHistory[searchHistory.Count - 1];

                if (currentSearchEntry.Equals(prevSearchEntry))
                {
                    isAddSearchToHistory = false;
                }
            }

            if (isAddSearchToHistory)
            {
                this.searchHistory.Add(currentSearchEntry);
            }
            this.searchHistoryIndex = searchHistory.Count - 1;

            filterResults.Clear();
            selectedMatchIndex = 0;
            try { HierarchyMasterUtils.ComponentFilterOnSelectedGameObjects(componentFilter, filterResults, includeChildrenWhenFiltering, evaluationOperators[evaluationOperatorIndex], matchValue); } catch { }
            Selection.objects = filterResults.ToArray();
        }

        private void ExecuteAddObject()
        {
            if (replacementObj == null || replacementObj.GetType() == typeof(GameObject))
            {
                GameObject[] selectedGOs = Selection.gameObjects;
                for (int i = 0; i < selectedGOs.Length; i++)
                {
                    GameObject iSelectedGO = selectedGOs[i];
                    GameObject go = (replacementObj == null) ? new GameObject() : GameObject.Instantiate(replacementObj, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(iSelectedGO.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    selectedGOs[i] = go;
                    Undo.RegisterCreatedObjectUndo(go, "Create child GameObject");
                }

                Selection.objects = selectedGOs;
            }
            else
            {
                Debug.LogWarning("Only adding of Prefabs are supported at this time");
            }
        }

        private void ExecuteComponentRemoval()
        {
            GameObject[] currentSelectedGOs = Selection.gameObjects;

            if (this.componentFilter == "<null>")
            {
                for (int i = 0; i < currentSelectedGOs.Length; i++)
                {
                    GameObject go = currentSelectedGOs[i];
                    HierarchyMasterUtils.RemoveNullComponentsOnGameObject(go);
                }
                Selection.objects = currentSelectedGOs;
                return;
            }

            for (int i = 0; i < currentSelectedGOs.Length; i++)
            {
                GameObject gameObject = currentSelectedGOs[i];
                Component component = gameObject.GetComponent(this.componentFilter);
                if (component != null)
                {
                    HierarchyMasterUtils.RemoveComponentFromGameObject(component, gameObject);
                }
            }
        }

        private void ClearHierarchySelections()
        {
            Selection.objects = new UnityEngine.Object[0];
        }

        private struct SearchEntry
        {
            public string componentFilter;
            public bool includeChildrenWhenFiltering;
            public int evaluationOperatorIndex;
            public string matchValue;

            public SearchEntry(string componentFilter, bool includeChildrenWhenFiltering, int evaluationOperatorIndex, string matchValue)
            {
                this.componentFilter = componentFilter;
                this.includeChildrenWhenFiltering = includeChildrenWhenFiltering;
                this.evaluationOperatorIndex = evaluationOperatorIndex;
                this.matchValue = matchValue;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is SearchEntry))
                {
                    return false;
                }

                SearchEntry mys = (SearchEntry)obj;
                bool isEqual = true;
                isEqual &= this.componentFilter == mys.componentFilter;
                isEqual &= this.includeChildrenWhenFiltering == mys.includeChildrenWhenFiltering;
                isEqual &= this.evaluationOperatorIndex == mys.evaluationOperatorIndex;
                isEqual &= this.matchValue == mys.matchValue;

                return isEqual;
            }
        };

        private List<SearchEntry> searchHistory = new List<SearchEntry>();
        private int searchHistoryIndex = -1;

        private void ProcessUserKeyPresses()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        if (Event.current.keyCode == (KeyCode.Escape))
                        {
                            e.Use();
                            ClearHierarchySelections();
                        }
                        if (Event.current.keyCode == (KeyCode.UpArrow))
                        {
                            e.Use();

                            if (searchHistory.Count > 0)
                            {
                                this.searchHistoryIndex--;
                                if (this.searchHistoryIndex < 0) { this.searchHistoryIndex = this.searchHistory.Count - 1; }

                                SearchEntry searchEntry = this.searchHistory[searchHistoryIndex];
                                this.componentFilter = searchEntry.componentFilter;
                                this.evaluationOperatorIndex = searchEntry.evaluationOperatorIndex;
                                this.includeChildrenWhenFiltering = searchEntry.includeChildrenWhenFiltering;
                                this.matchValue = searchEntry.matchValue;
                            }
                            GUI.FocusControl("ComponentFilter");
                        }
                        if (Event.current.keyCode == (KeyCode.DownArrow))
                        {
                            e.Use();

                            if (searchHistory.Count > 0)
                            {
                                this.searchHistoryIndex++;
                                if (this.searchHistoryIndex >= this.searchHistory.Count) { this.searchHistoryIndex = 0; }

                                SearchEntry searchEntry = this.searchHistory[searchHistoryIndex];
                                this.componentFilter = searchEntry.componentFilter;
                                this.evaluationOperatorIndex = searchEntry.evaluationOperatorIndex;
                                this.includeChildrenWhenFiltering = searchEntry.includeChildrenWhenFiltering;
                                this.matchValue = searchEntry.matchValue;

                                GUI.FocusControl("ComponentFilter");
                            }
                        }
                        if (Event.current.keyCode == (KeyCode.Return) && Event.current.shift)
                        {
                            e.Use(); runComponentFilter = true;
                            // if (GUI.GetNameOfFocusedControl().StartsWith("ComponentFilter")) { e.Use(); runComponentFilter = true; }
                            // if (GUI.GetNameOfFocusedControl().StartsWith("ObjectReplacement")) { e.Use(); runObjectReplace = true; }
                            // if (GUI.GetNameOfFocusedControl().StartsWith("ValueReplacement")) { e.Use(); runValueReplace = true; }
                        }
                        break;
                    }
            }
        }

        private void ExecuteReplacements()
        {
            string[] callSequence = componentFilter.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (callSequence.Length > 0)
            {
                string componentName = callSequence[0];
                GameObject[] selectedGOs = Selection.gameObjects;

                Undo.RegisterCompleteObjectUndo(selectedGOs, "Replace");

                for (int i = 0; i < selectedGOs.Length; i++)
                {
                    GameObject go = selectedGOs[i];
                    Component component = go.GetComponent(componentName);
                    if (component == null) { continue; }

                    if (runValueReplace) { ReplaceFieldWithValue(callSequence, component); }
                    if (runObjectReplace)
                    {
                        if (callSequence.Length == 1) //If single element, it's an attempt to replace component
                        {
                            ReplaceComponent(component);
                        }
                        else
                        {
                            ReplaceFieldWithObject(callSequence, component);
                        }
                    }
                }
            }
        }


        //*======================
        //* UTIL FUNCTIONS
        //*======================
        private void ReplaceComponent(Component component)
        {
            component.ReplaceComponent(replacementObj);
        }

        private void ReplaceFieldWithObject(string[] callSequence, Component component)
        {
            component.SetValue(callSequence, replacementObj);
        }

        private void ReplaceFieldWithValue(string[] componentFilterArray, Component component)
        {
            if (IsUsingRegex())
            {
                object retrievedValue;
                bool fieldExists = component.GetValue(componentFilterArray, out retrievedValue);
                if (fieldExists)
                {
                    string fieldValue = retrievedValue as string;
                    fieldValue = Regex.Replace(fieldValue, matchValue, replacementValue);
                    component.SetValue(componentFilterArray, fieldValue);
                }
                else
                {
                    Debug.LogWarning("Unable to locate field/property");
                    return;
                }
            }
            else
            {
                component.SetValue(componentFilterArray, replacementValue);
            }
        }

        private bool IsUsingRegex()
        {
            return (evaluationOperators[evaluationOperatorIndex] == "Regex");
        }

        private bool EvaluationOperatorIsNotNullComparison()
        {
            return evaluationOperatorIndex > 1 && evaluationOperatorIndex < 9;
        }
    }
}