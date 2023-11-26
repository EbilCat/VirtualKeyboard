using CoreDev.Framework;
using CoreDev.Observable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardButton : MonoBehaviour, ISpawnee, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private KeyCode key;
    [SerializeField] private string textOverride = string.Empty;

    [SerializeField] private Image outline;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color pressedColor = Color.yellow;
    [SerializeField] private Color defaultColor = new Color(0, 0.75f, 0, 1);
    [SerializeField] private float defaultZOffset_Local = -5;
    

    private VirtualKeyboardDO virtualKeyboardDO;
    private TextMeshProUGUI text;
    private OBool keyDown;


//*====================
//* UNITY
//*====================
    protected virtual void OnDestroy()
    {
        this.UnbindDO(this.virtualKeyboardDO);
    }

    protected void OnValidate()
    {
        this.name = "KeyboardButton_" +this.key;
        this.text = this.GetComponentInChildren<TextMeshProUGUI>();
        this.text.text = (textOverride.Length == 0) ? key.ToString() : textOverride;
        this.SetColorDefault();
    }


//*====================
//* BINDING
//*====================
    public void BindDO(IDataObject dataObject)
    {
        if (dataObject is VirtualKeyboardDO && this.virtualKeyboardDO == null)
        {
            this.virtualKeyboardDO = dataObject as VirtualKeyboardDO;
            this.text = this.GetComponentInChildren<TextMeshProUGUI>();
            this.text.text = (textOverride.Length == 0) ? key.ToString() : textOverride;

            this.keyDown = this.virtualKeyboardDO.GetObservable(key);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is VirtualKeyboardDO && this.virtualKeyboardDO == dataObject as VirtualKeyboardDO)
        {
            this.keyDown = null;
            this.virtualKeyboardDO = null;
        }
    }


//*====================
//* EventSystems - Interfaces
//*====================
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.keyDown == null) return;
        this.keyDown.Value = true;
        this.SetColorPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.keyDown == null) return;
        this.keyDown.Value = false;
        this.SetColorDefault();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.SetColorHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.SetColorDefault();
    }


//*====================
//* PRIVATE
//*====================
    private void SetColorHover()
    {
        this.text.color = this.hoverColor;
        this.outline.color = this.hoverColor;
    }

    private void SetColorPressed()
    {
        this.text.color = this.pressedColor;
        this.outline.color = this.pressedColor;
        Vector3 pos_Local = this.transform.localPosition;
        pos_Local.z = 0.0f;
        this.transform.localPosition = pos_Local;
    }
    
    private void SetColorDefault()
    {
        this.text.color = this.defaultColor;
        this.outline.color = this.defaultColor;

        Vector3 pos_Local = this.transform.localPosition;
        pos_Local.z = this.defaultZOffset_Local;
        this.transform.localPosition = pos_Local;
    }
}