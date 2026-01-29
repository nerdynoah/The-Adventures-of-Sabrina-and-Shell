using UnityEngine;
using BaseCharacter;

public class SPINNN : MonoBehaviour
{
    [SerializeField] int rng = 1000;
    [SerializeField] float multiplier = 0.001f;
    void FixedUpdate()
    {
        FindRotation(rng, multiplier);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    /// <summary>
    /// Rotates from a value from 1-<paramref name="rotationRNG"/> * <paramref name="mult"/>
    /// </summary>
    /// <param name="rotationRNG"></param>
    /// <param name="mult"></param>
    protected void FindRotation(int rotationRNG, float mult)
    {
        transform.Rotate(0, Methods.RandomValuePositive(rotationRNG) * mult, 0);
    }
   
}
