using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GenericBullet : NetworkBehaviour
{
    public float damage;

    [ServerCallback]
    void OnCollisionEnter(Collision col)
    {
        Debug.Log($"Bullet collided with foo for foo damage");
        NetworkManager.Destroy(this.gameObject);
    }

}
