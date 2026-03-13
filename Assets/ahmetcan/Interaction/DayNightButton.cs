using System;
using UnityEngine;

public class DayNightButton : Interactable
{

  

    public override void Action()
    {
       GameManager.PlayerMoved();
    }
}
