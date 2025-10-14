using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    [SerializeField] private KeyCode moveDown = KeyCode.F;
    [SerializeField]  private KeyCode moveUp = KeyCode.R;
    [SerializeField] private KeyCode moveLeft = KeyCode.D;
    [SerializeField] private KeyCode moveRight = KeyCode.G;
    [SerializeField] private KeyCode Interact = KeyCode.T;
    [SerializeField] private KeyCode Crouch = KeyCode.UpArrow;
    [SerializeField] private float RotationSpeed;
    public void SetRotationSpeed(float rotationSpeed)
    {
        RotationSpeed = rotationSpeed;
    }
    public float GetRotationSpeed()
    {
        return RotationSpeed;
    }
    /// <summary>
    /// Get the keys:<br></br>
    /// 0. Down
    /// <list type="number">
    /// <item>Up</item>
    /// <item>Left</item>
    /// <item>Right</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public KeyCode[] GetMoveKeys()
    {
        KeyCode[] keys = new KeyCode[4];
        keys[0] = moveDown;
        keys[1] = moveUp;
        keys[2] = moveLeft;
        keys[3] = moveRight;
        return keys;
    }
    public KeyCode GetInteractKey()
    {
        return Interact;
    }
}
