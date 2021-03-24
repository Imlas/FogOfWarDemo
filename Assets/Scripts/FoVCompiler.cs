using System.Collections;
using System.Collections.Generic;
using Mirror;
using System.Linq;
using UnityEngine;
using System;

public class FoVCompiler : NetworkBehaviour
{
    //This is a server-side manager that keeps track of all of the gameObjects with a list of visible targets
    // (for now, this is just the two players, but in the future could include other units/scouting items/etc.)
    //Every frame it collects the appropriate list of Fadable GOs, and fades them as appropriate

    private static FoVCompiler _instance;

    public static FoVCompiler Instance { get { return _instance; } }

    [SerializeField] private List<FieldOfView> fovProviders;

    [SerializeField] private List<GameObject> visibleFadables;
    [SerializeField] private List<GameObject> allFadables;
    [SerializeField] private List<GameObject> invisFadables;

    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;




    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            fovProviders.Clear();
            visibleFadables.Clear();
        }
    }

    [Server]
    public void AddFoVProvider(GameObject newProvider)
    {
        //Debug.Log("Adding provider start");
        fovProviders.Add(newProvider.GetComponent<FieldOfView>());

        //if (TryGetComponent<FieldOfView>(out FieldOfView fov))
        //{
        //    fovProviders.Add(fov);
        //    Debug.Log("FoV found, adding to list");
        //}
    }

    [Server]
    public void RemoveFoVProvider(GameObject oldProvider)
    {
        fovProviders.Remove(oldProvider.GetComponent<FieldOfView>());

        //if (TryGetComponent(out FieldOfView fov))
        //{
        //    fovProviders.Remove(fov);
        //}
    }




    [ServerCallback]
    private void LateUpdate() //...not sure if this should be lateupdate or update but enforce FoVCompiler to run after FieldOfView
    {
        visibleFadables.Clear();

        foreach (FieldOfView fov in fovProviders)
        {
            visibleFadables.AddRange(fov.visibleTargets);
        }

        //Now we remove duplicates
        visibleFadables = visibleFadables.Distinct().ToList();

        ToggleVisibilityOfFadables();

    }

    //For now, this will look at the list of Baddies gotten from BaddieManager, ones that are in visibleFadables it will fade in, everything else will fade out
    private void ToggleVisibilityOfFadables()
    {
        allFadables = BaddieManager.Instance.getBaddies();

        invisFadables = allFadables.Except(visibleFadables).ToList();

        //I would really love to figure out a solution to not getting the component of everything in these lists every frame
        foreach(GameObject visObj in visibleFadables)
        {
            visObj.GetComponent<Fadable>().RPCFadeIn(fadeInTime);
        }

        foreach(GameObject invisObj in invisFadables)
        {
            invisObj.GetComponent<Fadable>().RPCFadeOut(fadeOutTime);
        }
    }
}
