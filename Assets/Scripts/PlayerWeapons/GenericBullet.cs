using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GenericBullet : NetworkBehaviour
{
    public float damage;
    public float speed;
    [SerializeField] private float bulletLifetime = 5f;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Invoke(nameof(DestroySelf), bulletLifetime);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider col)
    {
        Debug.Log($"Bullet collided with {col.gameObject.name} for {damage} damage");
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

}
