using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBullet : MonoBehaviour
{
    //Moves forward at a given speed. On trigger collision it instantiates the hit gfx (sparks/etc.) and destroys this object
    //Optionally, may need to stop the object moving and destroy a few frames later so the trail can fade out - see how it looks


    //public float damage;
    public float speed;
    private bool hasHit = false;
    //[SerializeField] private GameObject hitGFXObject;
    [SerializeField] private float fadeTime;



    void OnTriggerEnter(Collider col)
    {
        if (!hasHit)
        {
            //TODO - need to actually check tag/layer of object
            //Check trigger tags
            //Debug.Log($"Bullet collided with {col.gameObject.name}");

            //Instantiate the hit gfx


            //Switch hasHit so the same bullet can't repeatedly trigger
            hasHit = true;

            //Destroy this object
            Destroy(this.gameObject, fadeTime);

        }
    }



    private void Update()
    {
        if (!hasHit)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }

}
