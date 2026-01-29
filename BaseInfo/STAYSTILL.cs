using UnityEngine;

/// <summary>
/// This goddess forsaken class is the only thing that kept me from losing my mind when using rigibodies omfg.
/// <br></br>
/// Use this to force a child gameobject inside a parent with a rigibody to not move after colliding.
/// </summary>
public class STAYSTILL : MonoBehaviour
{
    [SerializeField] private float fixedRotation = 0f;
    [SerializeField] private float YfixedPlacement = 0f;
    [SerializeField] private float ZfixedPlacement = 0f;

    private Transform playerTransform;
    // Update is called once per frame
    void LateUpdate()
    {
        if (playerTransform == null) return;
        transform.SetPositionAndRotation(new Vector3(playerTransform.position.x + ZfixedPlacement, playerTransform.position.y + YfixedPlacement, playerTransform.position.z), Quaternion.Euler(0, 0, fixedRotation));
    }

    private void Start()
    {
        playerTransform = transform.parent;

        if (playerTransform == null)
        {
            Debug.LogError("FixedRotationCamera must be a child of the Player object!");
            return;
        }
        transform.rotation = Quaternion.Euler(0, 0, fixedRotation);
    }

}
