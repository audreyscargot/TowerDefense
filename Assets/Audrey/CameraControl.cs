using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform target;
    public Vector2 offset;

    private void Update()
    {
        transform.position = target.position * offset;
    }
}
