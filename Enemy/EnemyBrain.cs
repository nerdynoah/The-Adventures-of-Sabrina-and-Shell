using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private PathMode DefaultPathMode;
    [SerializeField] private WanderMode wanderMode;
    [SerializeField] private FiveSenses[] inspectionModes = new FiveSenses[Enum.GetValues(typeof(FiveSenses)).Length];
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
