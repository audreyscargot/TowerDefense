using System;
using UnityEngine;
using UnityEngine.UI;

public class Interface_Handler_Menu : MonoBehaviour
{
    public GameObject Cursor;
    private Image CursorImage;

    public GameObject Icon;
    private Image IconImage;

    public Sprite NoHoverIcon;

    public void Start()
    {
        CursorImage = Cursor.GetComponent<Image>();
        CursorImage.enabled = false;

        IconImage = Icon.GetComponent<Image>();
    }
    public void CatImage(Sprite TargetImage)
    {
        IconImage.sprite = TargetImage;
    }
    
    public void MoveCursor(float TargetLoc, Sprite TargetImage)
    {
        ShowHide(true);
        Vector3 pos = Cursor.transform.localPosition;
        pos.y = TargetLoc;
        Cursor.transform.localPosition = pos;
        CatImage(TargetImage);
    }

    public void ShowHide(bool Show)
    {
        if (Show) CursorImage.enabled = true;
        else
        {
            CursorImage.enabled = false;
            IconImage.sprite = NoHoverIcon;
        }
    }
}