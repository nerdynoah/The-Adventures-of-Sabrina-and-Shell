using BaseCharacter.Items;
using BaseCharacter.Movement;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnemy : MonoBehaviour, IRayShoot
{
    [SerializeField] private EntityTemplete EntityTemplete;
    [SerializeField] private MeshRenderer render;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Get the raypoint of where your looking.
    /// </summary>
    /// <param name="rockTMP">The weapon to be used during the process to cause innacuracsyes</param>
    /// <param name="aim">Player aim</param>
    /// <returns>Raycast hits</returns>
    public RaycastHit GetRayPoint(Weapon rockTMP, float aim)
    {
        Ray ray;
        Ray ogRay;
        Vector3 ogpoint = transform.position;
        //Debug.Log($"Aim: {rockTMP.GetSphereAccuracy(true, aim)}");
        Vector3 point = new Vector3(ogpoint.x + rockTMP.GetSphereAccuracy(true, aim), ogpoint.y + rockTMP.GetSphereAccuracy(true, aim), ogpoint.z + rockTMP.GetSphereAccuracy(true, aim));
        Vector3 mousePosition = Input.mousePosition;
        Vector3 ThemousePosition = new Vector3(mousePosition.x + rockTMP.GetSphereAccuracy(true, aim), mousePosition.y + rockTMP.GetSphereAccuracy(true, aim), mousePosition.z + rockTMP.GetSphereAccuracy(true, aim));
        ray = new Ray(point, transform.forward);
        int includeMask = (1 << 0) | (1 << 10);
        Physics.Raycast(ray, out RaycastHit hit, 1000, includeMask);
        return hit;
    }
    public RaycastHit GetRayPoint(float rngAim, float aim)
    {
        Ray ray;
        Vector3 ogpoint = transform.position;
        Vector3 point = new Vector3(ogpoint.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        Vector3 mousePosition = Input.mousePosition;
        Vector3 ThemousePosition = new Vector3(mousePosition.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        ray = new Ray(point, transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 10);
        return hit;
    }
    public RaycastHit GetRayPoint(float rngAim, float aim, int layers)
    {
        Ray ray;
        Vector3 ogpoint = transform.position;
        Vector3 point = new Vector3(ogpoint.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        Vector3 mousePosition = Input.mousePosition;
        Vector3 ThemousePosition = new Vector3(mousePosition.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        ray = new Ray(point, transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layers);
        return hit;
    }
}
