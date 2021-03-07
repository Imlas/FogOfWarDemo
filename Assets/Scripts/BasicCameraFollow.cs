using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraFollow : MonoBehaviour
{

    public Transform player;

    public void LateUpdate()
    {
        this.transform.position = new Vector3(player.position.x, this.transform.position.y, player.position.z);
    }


}
