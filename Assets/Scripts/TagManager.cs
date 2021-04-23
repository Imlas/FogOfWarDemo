using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    private static TagManager _instance;
    public static TagManager Instance { get { return _instance; } }

    public static string playerTag = "Player";
    public static string baddieTag = "Baddie";
    public static string mainVirtualCamera = "MainVirtCam";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
