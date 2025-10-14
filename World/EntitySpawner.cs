using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseCharacter.Items;
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
    [Header("Spawn Conditions")]
    [SerializeField][Min(1)] private int MaxSpawn;
    [SerializeField] private int SpawnPerTime;
    [SerializeField] private float ChanceOfSpawn;
    [SerializeField] private bool ApplyAttributesRandomly = true;
    [SerializeField] private PathMode[] OverRidePathMode;

    private List<EntityTemplete> enemyTMP;
    private List<AttributesTemplete> attributesTempletes;
    private List<InventoryItem> inventoryItems;

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
                inventoryItems.Add(AllLibary.ItemLibary.SearchLibaryForTemplete(Name[i]));
            }
            if (searchFor[i] == SearchFor.Attributes)
            {
                attributesTempletes.Add(AllLibary.ItemLibary.SearchLibaryForAttribute(Name[i]));
            }
        }
        for (int i = 0; i < enemyTMP.Count; i++)
        {
            enemyTMP[i].Player.AddItem(inventoryItems.ToArray());
            if (!ApplyAttributesRandomly)
            {
                enemyTMP[i].Player.ApplyAttribute(attributesTempletes);
                
            }
        }
    }
}
