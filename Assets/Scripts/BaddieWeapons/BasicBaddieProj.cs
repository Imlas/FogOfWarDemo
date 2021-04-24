using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BasicBaddieProj : NetworkBehaviour
{

    public float damage;
    public float speed;
    public Vector3 target;
    public bool useTarget = false;
    private bool hasHit = false;

    [SerializeField] private GameObject hitGFXObject;
    //[SerializeField] private float fadeTime;


    [ServerCallback]
    void OnTriggerEnter(Collider col)
    {
        Debug.Log($"BaddieProj collided with {col.gameObject.name}");
        if (!hasHit)
        {
            //...probably a better way to structure this, but it works

            //Check trigger tags
            //If it hit a playerDamageable, then deal damage
            if (col.CompareTag(TagManager.playerTag))
            {

                Damageable player = col.gameObject.GetComponentInParent<Damageable>();
                player.TakeDamage(damage);
            }

            if(col.CompareTag(TagManager.playerTag) || col.CompareTag(TagManager.losBlockerTag))
            {
                //Instantiate the hit gfx


                VFXSpawner.Instance.RPCSpawnVFX(this.netIdentity, VFXType.ARifleBulletHit, col.ClosestPoint(this.transform.position), this.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
                //^ Inverting the rotation isn't quite right - I need to just spin it 180deg around the y axis

                //Switch hasHit so the same bullet can't repeatedly trigger
                hasHit = true;

                //Destroy this object
                Destroy(this.gameObject);

            }

        }
    }

    public void SetTarget(Vector3 _target)
    {
        target = _target;
        useTarget = true;
    }

    [ServerCallback]
    private void Update()
    {
        if (!hasHit)
        {
            if (!useTarget)
            {
                transform.position += transform.forward * Time.deltaTime * speed;
            }
            else
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, step);
                if (Vector3.SqrMagnitude(target - this.transform.position) < 0.001f)
                {
                    hasHit = true;
                    Destroy(this.gameObject);
                }
            }
        }
    }

}
