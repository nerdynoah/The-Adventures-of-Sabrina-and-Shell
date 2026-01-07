using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseCharacter.Items;
using BaseCharacter;
using static Enums;
public class EntitySpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private BoxCollider box;
    [SerializeField] private SearchFor[] searchFor;
    [Tooltip("Things to spawn")]
    [SerializeField] private string[] Name;
    [Header("Leveling")]
    [SerializeField] private int MaxLevel;
    [SerializeField] private int MinLevel;
    [Header("Spawn Conditions Entity")]
    [SerializeField][Min(1)] private int MaxSpawn;
    [SerializeField] private int SpawnPerTime;
    [Space(15)]
    [SerializeField] private bool RefreshMaxSpawnAfterReachingMax;
    [SerializeField] private float TimeToRefresh;
    [SerializeField] private float ChanceOfSpawn;
    [Space(15)]
    [SerializeField] private bool ApplyAttributesRandomly = false;
    [SerializeField] private PathMode[] OverRidePathMode;

    private List<EntityTemplete> enemyTMP = new();
    private List<AttributesTemplete> attributesTempletes = new();
    private List<InventoryItem> inventoryItems = new();
    private float theTime = 0;
    private int spawns = 0;

    void Start()
    {
        for (int i = 0; i < searchFor.Length; i++)
        {
            if (searchFor[i] == SearchFor.Entities)
            {
                enemyTMP.Add(AllLibary.ItemLibary.SearchLibaryForEntity(Name[i]));
            }
            if (searchFor[i] == SearchFor.InventoryItem)
            {
                inventoryItems.Add(AllLibary.ItemLibary.SearchLibaryForInventoryItem(Name[i]));
            }
            if (searchFor[i] == SearchFor.Attributes)
            {
                attributesTempletes.Add(AllLibary.ItemLibary.SearchLibaryForAttribute(Name[i]));
            }
        }
    }
    public void Update()
    {
        if (spawns < MaxSpawn && Time.time > theTime && UnityEngine.Random.value < ChanceOfSpawn)
        {
            for (int i = 0; i < enemyTMP.Count; i++)
            {
                Vector3 spawnLocation = transform.position + new Vector3(Methods.GetRandomNegativePositive(box.size.x), Methods.GetRandomNegativePositive(box.size.y), Methods.GetRandomNegativePositive(box.size.z));
                GameObject entity = Instantiate(enemyTMP[i].gameObject, spawnLocation, transform.rotation);
                EntityTemplete temp = entity.GetComponent<EntityTemplete>();
                temp.Player.ApplyAttribute(attributesTempletes[i]);
            }
            spawns++;
            if (spawns >= MaxSpawn)
            {
                theTime = Time.time + TimeToRefresh;
            }
            else
            {
                theTime = Time.time + SpawnPerTime;
            }
        }
        else if (spawns >= MaxSpawn && Time.time > theTime && RefreshMaxSpawnAfterReachingMax)
        {
            spawns = 0;
        }
    }
}
