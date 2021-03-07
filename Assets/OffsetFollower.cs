using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetFollower : MonoBehaviour
{
    //This is simply a shitty way to parent an object to another object, but ignoring rotation/scale changes

    public Transform targetTrans;
    public Vector3 offset;

    void LateUpdate()
    {
        this.transform.position = targetTrans.position + offset;
    }
}
