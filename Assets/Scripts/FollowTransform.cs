using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform targetTrans;

    // Update is called once per frame
    // Theoretically this would work better with giving this script a guaranteed later execution time
    // But doing it in LateUpdate would be overkill (and maybe screw with other things, idk)
    void Update()
    {
        if(targetTrans != null)
        {
            this.transform.SetPositionAndRotation(targetTrans.position, targetTrans.rotation);
        }
    }
}
