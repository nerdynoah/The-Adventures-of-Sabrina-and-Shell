using BaseCharacter.Items;
using BaseCharacter.Structual;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : Blocks
{
    protected InventorySystem invSystem;
    [SerializeField] protected string[] SpawnWithItems;
    [SerializeField] protected int[] AmountInSpawn;
    [SerializeField] protected int ID;
    [SerializeField] protected int Spaces;
    protected int cycle = 0;
    protected void Start()
    {
        invSystem = new InventorySystem(Spaces);
        for (int i = 0; i < SpawnWithItems.Length; i++)
        {
            try
            {
                AddItemRequest request = new AddItemRequest(SpawnWithItems[i], AmountInSpawn[i]);
                invSystem.AddItem(request.GetItem());
            }
            catch
            {   
                AddItemRequest request = new AddItemRequest(SpawnWithItems[i], 1);
                invSystem.AddItem(request.GetItem());
            }
        }       
        base.Start();
    }
    public override InventoryItem GetInventoryItem(bool destoryItem)
    {
        if (destoryItem)
        {
            InventoryItem item =  new InventoryItem(invSystem.GetInventoryItem(cycle));
            invSystem.DeleteItem(cycle++);
            return item;
        }
        try
        {
            if (cycle < SpawnWithItems.Length)
            {
                return new InventoryItem(invSystem.GetInventoryItem(cycle++));
            }
            return new InventoryItem(0);
        }
        catch 
        {
            return new InventoryItem(0);
        }
    }

}
