using Audrey.Player.Script;
using UnityEngine;

public class PlayerOwnedHealth : PlayerHealth
{
    public override void Death()
    {
        Destroy(gameObject);
    }
}
