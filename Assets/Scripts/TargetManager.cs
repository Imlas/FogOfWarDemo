using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private static TargetManager _instance;

    public static TargetManager Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> targets;

    public string targetTag;


    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        //Initially populate all game objects with the Target tag
        RefreshTargetList();
    }

    void AddTarget(GameObject newTarget)
    {
        targets.Add(newTarget);
    }

    void RemoveTarget(GameObject oldTarget)
    {
        targets.Remove(oldTarget);
    }

    public List<GameObject> GetTargets()
    {
        return targets;
    }

    void RefreshTargetList()
    {
        GameObject[] targetArray;
        targetArray = GameObject.FindGameObjectsWithTag(targetTag);
        targets = new List<GameObject>(targetArray);
        Debug.Log($"{targets.Count} game objects found with the {targetTag} tag");
    }

}
