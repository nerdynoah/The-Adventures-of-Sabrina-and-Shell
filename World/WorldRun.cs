using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldRun : MonoBehaviour
{
    public static WorldRun Instance { get; private set; }
    private const float GRAVITY = -9.806f;
    [Tooltip("9.806f * Adjustment")]
    [SerializeField] private float gravityAdj = 1;
    /// <summary>
    /// Gravity of the current world.
    /// </summary>
    public float Gravity {
        get{
            return GRAVITY * gravityAdj;
        } 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple WorldRun instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
