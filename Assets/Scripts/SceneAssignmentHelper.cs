using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SceneAssignmentHelper : MonoBehaviour
{
    private static SceneAssignmentHelper _instance;

    public static SceneAssignmentHelper Instance { get { return _instance; } }

    public TextMeshProUGUI uiAmmoText;

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
