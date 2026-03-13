using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //SETINGS
    public bool AnimatedIcon = false;
    public Vector2 IconOffset = new Vector2(0,0);
    
    // REFERENCES
    private InteractionPlayer InteractionPlayer;
    public GameObject GameManagerObj;
    protected GameManager GameManager;
    private GameObject spawnedObject;
    private GameObject SpawnPrefab;

    private void Start()
    {
        GameManager = GameManagerObj.GetComponent<GameManager>();
        if (AnimatedIcon) SpawnPrefab = GameManager.InteractIconAnimated;
        else SpawnPrefab = GameManager.InteractIconNormal;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractionPlayer = other.GetComponent<InteractionPlayer>();

        if (InteractionPlayer)
        {
            InteractionPlayer.InteractionObjScript = this;
            if (GameManager && spawnedObject == null)
            {
                spawnedObject = Instantiate(
                    SpawnPrefab,
                    transform.position + (Vector3)IconOffset,
                    Quaternion.identity
                );
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractionPlayer = other.GetComponent<InteractionPlayer>();

        if (InteractionPlayer && InteractionPlayer.InteractionObjScript == this)
        {
            InteractionPlayer.InteractionObjScript = null;

            if (spawnedObject)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
        }
    }

    public virtual void Action()
    {
        Debug.Log("Interacted");
    }
}