using UnityEngine;
using static Enums;

public class Vision : MonoBehaviour
{
    [SerializeField] private VisionType type;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private Rigidbody body;
    private bool isCollide = false;
    /// <summary>
    /// Set the size of the visionbox
    /// </summary>
    /// <param name="size"></param>
    public void SetSize(float size)
    {
        sphereCollider.radius = size / 2f;
    }
    public bool GetIsColliding()
    {
        return isCollide;
    }
    private void OnCollisionStay(Collision collision)
    {
        isCollide = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isCollide = false;
    }

}
