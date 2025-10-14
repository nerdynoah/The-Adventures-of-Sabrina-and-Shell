using UnityEngine;

public class STAYSTILL : MonoBehaviour
{
    [SerializeField] private float fixedRotation = 0f;
    [SerializeField] private float YfixedPlacement = 0f;

    private Transform playerTransform;
    // Update is called once per frame
    void LateUpdate()
    {
        if (playerTransform == null) return;
        transform.SetPositionAndRotation(new Vector3(playerTransform.position.x, playerTransform.position.y + YfixedPlacement, playerTransform.position.z), Quaternion.Euler(0, 0, fixedRotation));
    }

    private void Start()
    {
        playerTransform = transform.parent;

        if (playerTransform == null)
        {
            Debug.LogError("FixedRotationCamera must be a child of the player object!");
            return;
        }
        transform.rotation = Quaternion.Euler(0, 0, fixedRotation);
    }

}
