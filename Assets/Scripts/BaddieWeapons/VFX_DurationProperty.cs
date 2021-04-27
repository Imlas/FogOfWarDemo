using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFX_DurationProperty : MonoBehaviour
{
    [SerializeField] public float duration = 5.0f;
    [SerializeField] public bool smooth = false;
    [SerializeField] public bool destroyOnEnd = false;

    private float rate;
    private float count;
    private VisualEffect visualEffect;

    private void Start()
    {
        visualEffect = GetComponent<VisualEffect>();

        if (!visualEffect.HasFloat("Rate"))
            throw new System.Exception("Visual Effect missing exposed parameter type float with name 'Rate'");

        rate = visualEffect.GetFloat("Rate");

        if (destroyOnEnd)
            Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (visualEffect.HasFloat("Rate"))
        {
            count += Time.deltaTime;
            if (smooth)
            {
                if (count < duration)
                {
                    rate -= count;
                    visualEffect.SetFloat("Rate", rate);
                }
            }
            else
            {
                if (count >= duration)
                {
                    visualEffect.SetFloat("Rate", 0);
                }
            }
        }
    }
}