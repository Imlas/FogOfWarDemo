using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyBaseClass : NetworkBehaviour
{
    [SerializeField] public int pubIntField;
    [SerializeField] public int PubIntProperty { get; set; }

    [SerializeField] protected int protIntField;

    [SerializeField] protected int ProtIntProp { get; set; }

    [SerializeField] private int privIntField;
    [SerializeField] private int PrivIntProp { get; set; }


}
