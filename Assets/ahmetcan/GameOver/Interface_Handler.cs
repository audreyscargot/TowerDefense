using System;
using UnityEngine;
using UnityEngine.UI;

public class Interface_Handler : MonoBehaviour
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

    public void MoveCursor(float TargetLoc, Sprite TargetImage)
    {
        ShowHide(true);
        Vector3 pos = Cursor.transform.localPosition;
        pos.y = TargetLoc;
        Cursor.transform.localPosition = pos;
        Debug.Log(Cursor.transform.localPosition);
        IconImage.sprite = TargetImage;
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