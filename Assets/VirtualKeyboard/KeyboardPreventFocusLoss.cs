using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardPreventFocusLoss : MonoBehaviour, IPointerDownHandler
{
    private GameObject currentSelectedGO;
    private TMP_InputField focusedInputField;

    protected void Update()
    {
        if (EventSystem.current == null) { return; }
        if (EventSystem.current.currentSelectedGameObject != currentSelectedGO)
        {
            currentSelectedGO = EventSystem.current.currentSelectedGameObject;
            this.focusedInputField = (currentSelectedGO != null) ? currentSelectedGO.GetComponent<TMP_InputField>() : null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (focusedInputField != null)
        {
            this.focusedInputField.ActivateInputField();
            focusedInputField.selectionAnchorPosition = focusedInputField.caretPosition;
        }
    }
}
