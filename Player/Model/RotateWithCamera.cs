using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{

    [SerializeField] Movement movement;

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.LookRotation(movement.GetAnimatedRotation());
    }
}
