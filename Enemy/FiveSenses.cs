using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class FiveSenses : MonoBehaviour
{
    [SerializeField] private Enums.FiveSenses[] inspectionModes = new Enums.FiveSenses[Enum.GetValues(typeof(Enums.FiveSenses)).Length];
    
    private List<Vector3> InterestLocations = new List<Vector3>();
    private int interestLocationIndex = 0;


    public Vector3 GetInFront(float vision)
    {
        if (inspectionModes.Contains(Enums.FiveSenses.Vision))
        {
            Ray ray = new Ray();
            Physics.Raycast(ray, out RaycastHit hit, vision);
            return hit.point;
        }
        return transform.position;
    }
    
}
