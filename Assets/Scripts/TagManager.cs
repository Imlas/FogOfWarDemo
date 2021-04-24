using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    //B/c this is just a storage for static strings, this doesn't even need to be a singleton (nor exist on a game object)

    private static TagManager _instance;
    public static TagManager Instance { get { return _instance; } }

    public static string playerTag = "Player";
    public static string baddieTag = "Baddie";
    public static string mainVirtualCamera = "MainVirtCam";
    public static string losBlockerTag = "LoSBlocker";

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
