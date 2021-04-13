using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GenericBullet : NetworkBehaviour
{
    public float damage;
    [SerializeField] private float bulletLifetime = 5f;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Invoke(nameof(DestroySelf), bulletLifetime);
    }

    [ServerCallback]
    void OnCollisionEnter(Collision col)
    {
        Debug.Log($"Bullet collided with {col.gameObject.name} for {damage} damage");
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }

}
