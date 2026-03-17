using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonVisualCredits : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite HoverBackground;
    public Sprite UnhoverBackground;
    private Image BackgroundImage;
    private void Start()
    {
        BackgroundImage = gameObject.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BackgroundImage.sprite = HoverBackground;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BackgroundImage.sprite = UnhoverBackground;
    }
}
