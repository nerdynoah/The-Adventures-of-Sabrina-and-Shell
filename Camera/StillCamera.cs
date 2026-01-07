using UnityEngine;
using System;

public class FixedRotationCamera : MonoBehaviour
{
    [Tooltip("The fixed rotation you want the camera to maintain (in degrees)")]
    [SerializeField] private float fixedRotation = 0f;

    [Tooltip("Smoothing factor for camera movement (0 = no smoothing)")]
    [SerializeField] private float smoothing = 5f;

    private Transform playerTransform;
    private Vector3 offset;
    private Vector3 targetPosition;

    private void Start()
    {
        playerTransform = transform.parent;

        if (playerTransform == null)
        {
            Debug.LogError("FixedRotationCamera must be a child of the Player object!");
            return;
        }

        offset = transform.position - playerTransform.position;
        transform.rotation = Quaternion.Euler(0, 0, fixedRotation);
    }

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        // Calculate target position
        targetPosition = playerTransform.position + offset;

        // Smoothly move to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);

        // Maintain fixed rotation
        transform.rotation = Quaternion.Euler(0, 0, fixedRotation);
        
    }
}