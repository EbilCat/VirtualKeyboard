using CoreDev.Framework;
using CoreDev.Observable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardOutput : MonoBehaviour, ISpawnee
{
    private VirtualKeyboardDO virtualKeyboardDO;
    private GameObject currentSelectedGO;

    private TMP_InputField focusedInputField;


//*====================
//* UNITY
//*====================
    protected virtual void OnDestroy()
    {
        this.UnbindDO(this.virtualKeyboardDO);
    }

    protected void Update()
    {
        if (EventSystem.current == null) { return; }
        if (EventSystem.current.currentSelectedGameObject != currentSelectedGO)
        {
            currentSelectedGO = EventSystem.current.currentSelectedGameObject;
            this.focusedInputField = (currentSelectedGO != null) ? currentSelectedGO.GetComponent<TMP_InputField>() : null;
        }
    }


//*====================
//* BINDING
//*====================
    public void BindDO(IDataObject dataObject)
    {
        if (dataObject is VirtualKeyboardDO && this.virtualKeyboardDO == null)
        {
            this.virtualKeyboardDO = dataObject as VirtualKeyboardDO;

            this.virtualKeyboardDO.Button1.RegisterForChanges(OnButton1Changed);
            this.virtualKeyboardDO.Button2.RegisterForChanges(OnButton2Changed);
            this.virtualKeyboardDO.Button3.RegisterForChanges(OnButton3Changed);
            this.virtualKeyboardDO.Button4.RegisterForChanges(OnButton4Changed);
            this.virtualKeyboardDO.Button5.RegisterForChanges(OnButton5Changed);
            this.virtualKeyboardDO.Button6.RegisterForChanges(OnButton6Changed);
            this.virtualKeyboardDO.Button7.RegisterForChanges(OnButton7Changed);
            this.virtualKeyboardDO.Button8.RegisterForChanges(OnButton8Changed);
            this.virtualKeyboardDO.Button9.RegisterForChanges(OnButton9Changed);
            this.virtualKeyboardDO.Button0.RegisterForChanges(OnButton0Changed);
            this.virtualKeyboardDO.ButtonBackSpace.RegisterForChanges(OnButtonBackSpaceChanged);

            this.virtualKeyboardDO.ButtonA.RegisterForChanges(OnButtonAChanged);
            this.virtualKeyboardDO.ButtonB.RegisterForChanges(OnButtonBChanged);
            this.virtualKeyboardDO.ButtonC.RegisterForChanges(OnButtonCChanged);
            this.virtualKeyboardDO.ButtonD.RegisterForChanges(OnButtonDChanged);
            this.virtualKeyboardDO.ButtonE.RegisterForChanges(OnButtonEChanged);
            this.virtualKeyboardDO.ButtonF.RegisterForChanges(OnButtonFChanged);
            this.virtualKeyboardDO.ButtonG.RegisterForChanges(OnButtonGChanged);
            this.virtualKeyboardDO.ButtonH.RegisterForChanges(OnButtonHChanged);
            this.virtualKeyboardDO.ButtonI.RegisterForChanges(OnButtonIChanged);
            this.virtualKeyboardDO.ButtonJ.RegisterForChanges(OnButtonJChanged);
            this.virtualKeyboardDO.ButtonK.RegisterForChanges(OnButtonKChanged);
            this.virtualKeyboardDO.ButtonL.RegisterForChanges(OnButtonLChanged);
            this.virtualKeyboardDO.ButtonM.RegisterForChanges(OnButtonMChanged);
            this.virtualKeyboardDO.ButtonN.RegisterForChanges(OnButtonNChanged);
            this.virtualKeyboardDO.ButtonO.RegisterForChanges(OnButtonOChanged);
            this.virtualKeyboardDO.ButtonP.RegisterForChanges(OnButtonPChanged);
            this.virtualKeyboardDO.ButtonQ.RegisterForChanges(OnButtonQChanged);
            this.virtualKeyboardDO.ButtonR.RegisterForChanges(OnButtonRChanged);
            this.virtualKeyboardDO.ButtonS.RegisterForChanges(OnButtonSChanged);
            this.virtualKeyboardDO.ButtonT.RegisterForChanges(OnButtonTChanged);
            this.virtualKeyboardDO.ButtonU.RegisterForChanges(OnButtonUChanged);
            this.virtualKeyboardDO.ButtonV.RegisterForChanges(OnButtonVChanged);
            this.virtualKeyboardDO.ButtonW.RegisterForChanges(OnButtonWChanged);
            this.virtualKeyboardDO.ButtonX.RegisterForChanges(OnButtonXChanged);
            this.virtualKeyboardDO.ButtonY.RegisterForChanges(OnButtonYChanged);
            this.virtualKeyboardDO.ButtonZ.RegisterForChanges(OnButtonZChanged);

            this.virtualKeyboardDO.ButtonEnter.RegisterForChanges(OnButtonEnterChanged);
            this.virtualKeyboardDO.ButtonSpace.RegisterForChanges(OnButtonSpaceChanged);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is VirtualKeyboardDO && this.virtualKeyboardDO == dataObject as VirtualKeyboardDO)
        {
            this.virtualKeyboardDO?.Button1.UnregisterFromChanges(OnButton1Changed);
            this.virtualKeyboardDO?.Button2.UnregisterFromChanges(OnButton2Changed);
            this.virtualKeyboardDO?.Button3.UnregisterFromChanges(OnButton3Changed);
            this.virtualKeyboardDO?.Button4.UnregisterFromChanges(OnButton4Changed);
            this.virtualKeyboardDO?.Button5.UnregisterFromChanges(OnButton5Changed);
            this.virtualKeyboardDO?.Button6.UnregisterFromChanges(OnButton6Changed);
            this.virtualKeyboardDO?.Button7.UnregisterFromChanges(OnButton7Changed);
            this.virtualKeyboardDO?.Button8.UnregisterFromChanges(OnButton8Changed);
            this.virtualKeyboardDO?.Button9.UnregisterFromChanges(OnButton9Changed);
            this.virtualKeyboardDO?.Button0.UnregisterFromChanges(OnButton0Changed);
            this.virtualKeyboardDO?.ButtonBackSpace.UnregisterFromChanges(OnButtonBackSpaceChanged);

            this.virtualKeyboardDO?.ButtonA.UnregisterFromChanges(OnButtonAChanged);
            this.virtualKeyboardDO?.ButtonB.UnregisterFromChanges(OnButtonBChanged);
            this.virtualKeyboardDO?.ButtonC.UnregisterFromChanges(OnButtonCChanged);
            this.virtualKeyboardDO?.ButtonD.UnregisterFromChanges(OnButtonDChanged);
            this.virtualKeyboardDO?.ButtonE.UnregisterFromChanges(OnButtonEChanged);
            this.virtualKeyboardDO?.ButtonF.UnregisterFromChanges(OnButtonFChanged);
            this.virtualKeyboardDO?.ButtonG.UnregisterFromChanges(OnButtonGChanged);
            this.virtualKeyboardDO?.ButtonH.UnregisterFromChanges(OnButtonHChanged);
            this.virtualKeyboardDO?.ButtonI.UnregisterFromChanges(OnButtonIChanged);
            this.virtualKeyboardDO?.ButtonJ.UnregisterFromChanges(OnButtonJChanged);
            this.virtualKeyboardDO?.ButtonK.UnregisterFromChanges(OnButtonKChanged);
            this.virtualKeyboardDO?.ButtonL.UnregisterFromChanges(OnButtonLChanged);
            this.virtualKeyboardDO?.ButtonM.UnregisterFromChanges(OnButtonMChanged);
            this.virtualKeyboardDO?.ButtonN.UnregisterFromChanges(OnButtonNChanged);
            this.virtualKeyboardDO?.ButtonO.UnregisterFromChanges(OnButtonOChanged);
            this.virtualKeyboardDO?.ButtonP.UnregisterFromChanges(OnButtonPChanged);
            this.virtualKeyboardDO?.ButtonQ.UnregisterFromChanges(OnButtonQChanged);
            this.virtualKeyboardDO?.ButtonR.UnregisterFromChanges(OnButtonRChanged);
            this.virtualKeyboardDO?.ButtonS.UnregisterFromChanges(OnButtonSChanged);
            this.virtualKeyboardDO?.ButtonT.UnregisterFromChanges(OnButtonTChanged);
            this.virtualKeyboardDO?.ButtonU.UnregisterFromChanges(OnButtonUChanged);
            this.virtualKeyboardDO?.ButtonV.UnregisterFromChanges(OnButtonVChanged);
            this.virtualKeyboardDO?.ButtonW.UnregisterFromChanges(OnButtonWChanged);
            this.virtualKeyboardDO?.ButtonX.UnregisterFromChanges(OnButtonXChanged);
            this.virtualKeyboardDO?.ButtonY.UnregisterFromChanges(OnButtonYChanged);
            this.virtualKeyboardDO?.ButtonZ.UnregisterFromChanges(OnButtonZChanged);

            this.virtualKeyboardDO.ButtonEnter.RegisterForChanges(OnButtonEnterChanged);
            this.virtualKeyboardDO?.ButtonSpace.UnregisterFromChanges(OnButtonSpaceChanged);

            this.virtualKeyboardDO = null;
        }
    }


//*====================
//* CALLBACKS
//*====================
    private void OnButton1Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("1"); }
    private void OnButton2Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("2"); }
    private void OnButton3Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("3"); }
    private void OnButton4Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("4"); }
    private void OnButton5Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("5"); }
    private void OnButton6Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("6"); }
    private void OnButton7Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("7"); }
    private void OnButton8Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("8"); }
    private void OnButton9Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("9"); }
    private void OnButton0Changed(ObservableVar<bool> obj) { if (obj.Value) this.InputText("0"); }
    private void OnButtonBackSpaceChanged(ObservableVar<bool> obj) { if (obj.Value) this.BackSpace(); }

    private void OnButtonAChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("A"); }
    private void OnButtonBChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("B"); }
    private void OnButtonCChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("C"); }
    private void OnButtonDChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("D"); }
    private void OnButtonEChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("E"); }
    private void OnButtonFChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("F"); }
    private void OnButtonGChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("G"); }
    private void OnButtonHChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("H"); }
    private void OnButtonIChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("I"); }
    private void OnButtonJChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("J"); }
    private void OnButtonKChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("K"); }
    private void OnButtonLChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("L"); }
    private void OnButtonMChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("M"); }
    private void OnButtonNChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("N"); }
    private void OnButtonOChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("O"); }
    private void OnButtonPChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("P"); }
    private void OnButtonQChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("Q"); }
    private void OnButtonRChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("R"); }
    private void OnButtonSChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("S"); }
    private void OnButtonTChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("T"); }
    private void OnButtonUChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("U"); }
    private void OnButtonVChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("V"); }
    private void OnButtonWChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("W"); }
    private void OnButtonXChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("X"); }
    private void OnButtonYChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("Y"); }
    private void OnButtonZChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText("Z"); }

    private void OnButtonEnterChanged(ObservableVar<bool> obj) { if (obj.Value) this.Enter(); }
    private void OnButtonSpaceChanged(ObservableVar<bool> obj) { if (obj.Value) this.InputText(" "); }


//*====================
//* PRIVATE
//*====================
    private void InputText(string text)
    {
        if (this.focusedInputField != null)
        {
            // this.focusedInputField.Select();
            this.focusedInputField.ActivateInputField();

            this.DeleteHighlightedText();

            int caretPos = focusedInputField.selectionAnchorPosition;
            focusedInputField.text = focusedInputField.text.Insert(caretPos, text);
            focusedInputField.caretPosition = caretPos + text.Length;
        }
    }

    private void Enter()
    {
        if (this.focusedInputField != null)
        {
            //To be implemented later
        }
    }

    private void BackSpace()
    {
        if (this.focusedInputField != null)
        {
            // this.focusedInputField.Select();
            this.focusedInputField.ActivateInputField();

            if (this.DeleteHighlightedText() == 0)
            {
                int caretPos = focusedInputField.selectionAnchorPosition;
                if (caretPos > 0)
                {
                    focusedInputField.text = focusedInputField.text.Remove(caretPos - 1, 1);
                    focusedInputField.caretPosition = caretPos - 1;
                }
            }
        }
    }

    private void Delete()
    {
        if (this.focusedInputField != null)
        {
            // this.focusedInputField.Select();
            this.focusedInputField.ActivateInputField();

            if (this.DeleteHighlightedText() == 0)
            {
                int caretPos = focusedInputField.selectionAnchorPosition;
                focusedInputField.text = focusedInputField.text.Remove(caretPos, 1);
                focusedInputField.caretPosition = caretPos - 1;
            }
        }
    }

    private int DeleteHighlightedText()
    {
        if (this.focusedInputField != null)
        {
            string inputFieldtext = focusedInputField.text;

            int lowerBound = Mathf.Min(focusedInputField.selectionStringFocusPosition, focusedInputField.selectionAnchorPosition);
            int upperBound = Mathf.Max(focusedInputField.selectionStringFocusPosition, focusedInputField.selectionAnchorPosition);
            int removalCount = upperBound - lowerBound;
            inputFieldtext = inputFieldtext.Remove(lowerBound, removalCount);

            int caretPos = upperBound;
            caretPos -= removalCount;
            focusedInputField.text = inputFieldtext;
            focusedInputField.caretPosition = caretPos;

            return removalCount;
        }
        return 0;
    }


    // protected bool SendSubmitEventToSelectedObject()
    // {
    //     if (eventSystem.currentSelectedGameObject == null)
    //         return false;

    //     var data = GetBaseEventData();
    //     if (input.GetButtonDown(m_SubmitButton))
    //         ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

    //     if (input.GetButtonDown(m_CancelButton))
    //         ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
    //     return data.used;
    // }
}
