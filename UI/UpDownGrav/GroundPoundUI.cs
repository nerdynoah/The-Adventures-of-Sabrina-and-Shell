using UnityEngine;

public class GroundPoundUI : MonoBehaviour
{
    // Start is called before the first frame update

    private float? MaxDownForce = null;
    private float maxScale = 2f;

    public void SetupScale(float maxUpForce)
    {
        MaxDownForce = maxUpForce;
    }

    public void SetScaleUI(float force)
    {
        if (MaxDownForce != null)
        {
            force *= -1;
            force = Mathf.Clamp(force, 0, (float)MaxDownForce);
            transform.localScale = new(transform.localScale.x, -1 * (force / (float)MaxDownForce) * maxScale, transform.localScale.z);
        }
    }

}
