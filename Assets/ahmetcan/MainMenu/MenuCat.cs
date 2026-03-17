using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuCat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite UnhoverBackground;

    public Sprite CatPet1;
    public Sprite CatPet2;
    public Sprite CatTouched;
    
    private Image BackgroundImage;

    private bool MouseIn = false;
    private Vector2 lastMousePos;
    public float moveThreshold = 10f;

    private bool toggle = false;
    private bool Touched = false;
    public void Clicked()
    {
        Touched = true;
        BackgroundImage.sprite = CatTouched;
    }
    
    private void Start()
    {
        BackgroundImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseIn = true;
        lastMousePos = Mouse.current.position.ReadValue();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BackgroundImage.sprite = UnhoverBackground;
        MouseIn = false;
        Touched = false;
    }

    private void Update()
    {
        if (!MouseIn) return;

        Vector2 currentMousePos = Mouse.current.position.ReadValue();

        float distance = Vector2.Distance(currentMousePos, lastMousePos);

        if (distance > moveThreshold && !Touched)
        {
            toggle = !toggle;
            BackgroundImage.sprite = toggle ? CatPet1 : CatPet2;

            lastMousePos = currentMousePos;
        }
    }
}