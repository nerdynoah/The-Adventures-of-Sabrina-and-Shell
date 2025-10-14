using UnityEngine;
using UnityEngine.UI;

public class ScaleUp : MonoBehaviour
{
    private float maxScale = 0.5f;
    private float? MaxUpForce = null;
    private float UpForce;
    [SerializeField] bool UseColorFromBelow = false;
    [SerializeField] Color color;
    [SerializeField] RawImage image;
    public void SetupScale(float maxUpForce)
    {
        MaxUpForce = maxUpForce;
    }

    public void AddScaleUI(float force)
    {
        if (MaxUpForce != null)
        {
            UpForce = Mathf.Max(UpForce, 0);
            UpForce += Mathf.Clamp(force, 0, (float)MaxUpForce);
            transform.localScale = new(transform.localScale.x, (UpForce / (float)MaxUpForce) * maxScale, transform.localScale.z);
        }
    }
    public void ApplyScale(float gravity)
    {
        if (MaxUpForce != null)
        {
            UpForce += gravity;
            UpForce = Mathf.Clamp(UpForce, 0, (float)MaxUpForce);
            transform.localScale = new(transform.localScale.x, (UpForce / (float)MaxUpForce) * maxScale, transform.localScale.z);
        }
    }
    public void HpAdjust(float maxHP, float currentHP, float exp)
    {
        transform.localScale = new Vector3(Mathf.Pow((currentHP/ maxHP), exp), transform.localScale.y, 0);
    }
    private void Start()
    {
        if (UseColorFromBelow)
        {
            image.color = color;
        }
    }
}
