using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Damageable : NetworkBehaviour
{
    [SyncVar(hook = nameof(HealthUpdated))] [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;


    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth = maxHealth;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
    }

    public void HealthUpdated(float _old, float _new)
    {
        Debug.Log($"{this.gameObject.name} taking damage - new health {_new}");
        //Update health bar
        //Floating combat text

        //Needs to handle both non-player characters dying, and players "losing"
        //For now well just destroy the object
        if(_new <= 0f)
        {
            if (this.gameObject.CompareTag(TagManager.baddieTag))
            {
                //Score points

                //Have VFX shown for a death thing (should that be stored in this class? a deathExplosion VFX?)

                NetworkManager.Destroy(this.gameObject);

            }
            else if(this.gameObject.CompareTag(TagManager.playerTag))
            {
                // Disable control of the player
                // Display some sort of game over message (or figure out how this'll be done)
            }
        }
    }
}
