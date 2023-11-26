using TMPro;
using UnityEngine;

public class VirtualKeyboardForceFocusTextField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    protected void Update()
    {
        this.inputField.ActivateInputField();
        inputField.selectionAnchorPosition = inputField.caretPosition;
    }
}
