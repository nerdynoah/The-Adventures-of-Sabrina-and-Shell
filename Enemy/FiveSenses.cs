using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class FiveSenses : MonoBehaviour
{
    [Header("Eyesight")]
    [SerializeField] private Eye[] eyeSight;
    [SerializeField][Min(0.02f)] private float eyeSightUpdateRate = 0.02f;
    [SerializeField][Min(0)] private float personalSpace;
    [Header("Hearing")]
    [SerializeField] private Ear[] ears;
    [Header("Smell")]
    [SerializeField] private Nose nose;
    [SerializeField] private SummonStench summonStench;
    [Header("Touch")]
    [SerializeField] private Collider col;
    public float GetEyeSightUpdateRate { get { return eyeSightUpdateRate; } }
    public Nose GetNose()
    {
        return nose;
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {

    }

}
