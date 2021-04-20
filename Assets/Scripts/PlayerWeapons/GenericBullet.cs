using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GenericBullet : NetworkBehaviour
{
    //Moves forward at a given speed. On trigger collision it instantiates the hit gfx (sparks/etc.) and destroys this object
    //Optionally, may need to stop the object moving and destroy a few frames later so the trail can fade out - see how it looks


    //public float damage;
    public float speed;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private GameObject hitGFXObject;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Invoke(nameof(DestroySelf), bulletLifetime);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider col)
    {
        //Check trigger tags
        Debug.Log($"Bullet collided with {col.gameObject.name}");

        //Instantiate the hit gfx


        //Destroy this object
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    //[ServerCallback]
    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

}
