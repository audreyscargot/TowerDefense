using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonVisualMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite HoverBackground;
    public Sprite UnhoverBackground;
    public Sprite IconImage;
    public GameObject ITObject;
    private Interface_Handler_Menu InterfaceHandler;
    public float TargetLoc;
    private Image BackgroundImage;
    private void Start()
    {
        InterfaceHandler = ITObject.GetComponent<Interface_Handler_Menu>();
        BackgroundImage = gameObject.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InterfaceHandler.MoveCursor(TargetLoc,IconImage);
        BackgroundImage.sprite = HoverBackground;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InterfaceHandler.ShowHide(false);
        BackgroundImage.sprite = UnhoverBackground;
    }
}
