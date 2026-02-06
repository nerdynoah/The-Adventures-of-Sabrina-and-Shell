using BaseCharacter.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoUIChest : Blocks
{
    [SerializeField] protected string[] SpawnWithItems;
    [SerializeField] protected int[] AmountInSpawn;
    [SerializeField] protected int Spaces;
    protected int cycle = 0;

    new void Start()
    {
        invSystem = new InventorySystem(Spaces);
        for (int i = 0; i < SpawnWithItems.Length; i++)
        {
            try
            {
                AddItemRequest request = new AddItemRequest(SpawnWithItems[i], AmountInSpawn[i]);
                invSystem.AddInventorySpaces(1);
                invSystem.AddItem(request.GetItem());
            }
            catch
            {   
                AddItemRequest request = new AddItemRequest(SpawnWithItems[i], 1);
                invSystem.AddInventorySpaces(1);
                invSystem.AddItem(request.GetItem());
            }
        }       
        base.Start();
    }
    
    public override List<InventoryItem> GetInventoryItem(bool destoryItem)
    {
        if (destoryItem)
        {
            List<InventoryItem> item = new List<InventoryItem>
            {
                new InventoryItem(invSystem.GetInventoryItem(cycle))
            };
            invSystem.DeleteItem(cycle++);
            return item;
        }
        try
        {
            if (cycle < SpawnWithItems.Length)
            {
                return new List<InventoryItem>
                {
                    new InventoryItem(invSystem.GetInventoryItem(cycle))
                };
            }
                return new List<InventoryItem>
                {
                    new InventoryItem(0)
                };
        }
        catch 
        {
            return new List<InventoryItem>
                {
                    new InventoryItem(0)
                };
        }
    }

}
