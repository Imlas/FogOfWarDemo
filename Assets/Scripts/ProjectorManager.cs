using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorManager : MonoBehaviour
{
    public GameObject projector;

    // Start is called before the first frame update
    void Awake()
    {
        projector.SetActive(true);
    }

}
