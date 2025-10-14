using UnityEngine;
using static Enums;

public class Vision : MonoBehaviour
{
    [SerializeField] private VisionType type;
    [SerializeField] private SphereCollider sphereCollider;
    /// <summary>
    /// Set the size of the visionbox
    /// </summary>
    /// <param name="size"></param>
    public void SetSize(float size)
    {
        sphereCollider.radius = size / 2f;
    }

}
