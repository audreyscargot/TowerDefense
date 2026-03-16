using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //SETINGS
    public Vector2 IconOffset = new Vector2(0, 0);

    // REFERENCES
    protected InteractionPlayer InteractionPlayer;
    protected GameObject spawnedObject;
    public GameObject SpawnPrefab;
    protected InteractionPlayer EnteredPlayer;

    
    protected void ShowInteractButton()
    {
        InteractionPlayer.InteractionObjScript = this;
        spawnedObject = Instantiate(
            SpawnPrefab,
            transform.position + (Vector3)IconOffset,
            Quaternion.identity
        );
    }
    
    protected void OnTriggerEnter2D(Collider2D other)
    {
        InteractionPlayer = other.GetComponent<InteractionPlayer>();

        if (InteractionPlayer)
        {
            EnteredPlayer = InteractionPlayer;
            if (InteractionPlayer.InteractionObjScript == null && spawnedObject == null)
                ShowInteractButton();
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (InteractionPlayer)
        {
            if (InteractionPlayer.InteractionObjScript == this)
            { 
                InteractionPlayer.InteractionObjScript = null;
                InteractionPlayer = null;
            }
            
            EnteredPlayer = null;
            if (spawnedObject)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
        }
    }

    public virtual void Update()
    { 
        if (EnteredPlayer && !InteractionPlayer.InteractionObjScript) ShowInteractButton();
    }

    public virtual void Action()
    {
        Debug.Log("Interacted");
    }
}