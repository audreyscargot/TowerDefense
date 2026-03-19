using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MenuCat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    private List<string> texts = new List<string>()
    {
        "meow",
        "meow...",
        "y-you know cats can't talk right ?",
        "okay you won.. this game is made by: Sartore Enzo, Adrien Audrey, Demir Ahmetcan"
    };
    private bool isTyping = false;
    public Sprite UnhoverBackground;
    private Coroutine typingCoroutine;
    public Sprite CatPet1;
    public Sprite CatPet2;
    public Sprite CatTouched;
    public Sprite CatTalking;
    public Sprite CatTalking2;
    public int CreditCount = 0;
    private Image BackgroundImage;

    //credit
    public GameObject SpeakFrame;
    public GameObject SpeakTextObj;
    
    private bool MouseIn = false;
    private Vector2 lastMousePos;
    public float moveThreshold = 5;

    private bool toggle = false;
    private bool Touched = false;
    private bool CanChangeImage = true;

    private AudioSource CatVoice;
    IEnumerator TypeText(string fullText)
    {
        isTyping = true;

        TextMeshProUGUI msg = SpeakTextObj.GetComponent<TextMeshProUGUI>();
        msg.text = "";
        CanChangeImage = false;

        foreach (char c in fullText)
        {
            CanChangeImage = !CanChangeImage;
            BackgroundImage.sprite = CanChangeImage ? CatTalking : CatTalking2;
            msg.text += c;
            CatVoice.Play();
            yield return new WaitForSeconds(0.06f);
        }

        yield return new WaitForSeconds(1f);

        SpeakFrame.SetActive(false);
        if (!MouseIn) BackgroundImage.sprite = UnhoverBackground;

        CanChangeImage = true;
        isTyping = false;
    }
    
    public void Clicked()
    {
        Touched = true;
        if (!CanChangeImage) return;
        BackgroundImage.sprite = CatTouched;
    }
    
    public void ClickedCredit()
    {
        if (isTyping) return; 

        BackgroundImage.sprite = CatTalking;
        SpeakFrame.SetActive(true);

        StartCoroutine(TypeText(texts[CreditCount]));

        if (CreditCount >= 3) CreditCount = 0;
        else CreditCount += 1;
    }
    
    private void Start()
    {
        BackgroundImage = GetComponent<Image>();
        SpeakFrame.SetActive(false);
        CatVoice = gameObject.GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseIn = true;
        lastMousePos = Mouse.current.position.ReadValue();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseIn = false;
        Touched = false;
        if (!CanChangeImage) return;
        BackgroundImage.sprite = UnhoverBackground;
    }

    private void Update()
    {
        if (!MouseIn || !CanChangeImage) return;

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