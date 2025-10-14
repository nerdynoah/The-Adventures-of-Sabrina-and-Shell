using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Money : MonoBehaviour
{
    protected int Value { get; set; }
    protected Vector3 EndLocation { get; set; }
    protected Vector3 StartLocation { get; set; }
    private bool HasCollided { get; set; } = false;
    private bool HasGivenForce { get; set; } = false;
    private float PowerReaction { get; set; } = 1;
    private float LiftReaction { get; set; } = 1;
    private float Speed { get; set; } = 0;
    private float StartTime { get; set; }
    [SerializeField] Rigidbody body;
    
    /// <summary>
    /// Setup a coin with a launch
    /// </summary>
    /// <param name="worth">Money</param>
    /// <param name="speed">How fast the coin moves</param>
    /// <param name="blast">Intial blast</param>
    /// <param name="distance">End location distance</param>
    /// <param name="startLocation">Where the thing is that drops the coin</param>
    /// <param name="power">Power after collision</param>
    /// <param name="lift">LIFT after collisions</param>
    public void SetupCoin(int worth, float speed, float distance, Vector3 blast, Vector3 startLocation, float power, float lift)
    {
        PowerReaction = power;
        LiftReaction = lift;
        Vector3 end = new Vector3(distance, distance, distance);
        EndLocation = startLocation + end * (Random.value + 0.015f);
        transform.position = startLocation;
        Speed = speed;
        body.AddForce(blast);
        Value = worth;
        StartTime = Time.time;
    }
    private void GiveForce()
    {
        Vector3 direction = (EndLocation - StartLocation).normalized;
        Vector3 bounce = PowerReaction * direction;
        bounce = new Vector3(bounce.x, LiftReaction, bounce.z);
        body.AddForce(bounce);
        HasGivenForce = true;

    }
    private void MoveTowardsTarget()
    {
        Vector3 direction = (EndLocation - StartLocation).normalized;
        transform.Translate(Speed * Time.deltaTime * direction, Space.World);
    }


    // Update is called once per frame
    void Update()
    {
        if (!HasCollided)
        {
            MoveTowardsTarget();
        }
        else if (!HasGivenForce)
        {
            GiveForce();
        }
        if (Time.time > StartTime + 20f)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time + 1f > StartTime)
        {
            HasCollided = true;
        }
    }
    public void FixedUpdate()
    {
        body.AddForce(Physics.gravity,ForceMode.Acceleration);
    }
}
