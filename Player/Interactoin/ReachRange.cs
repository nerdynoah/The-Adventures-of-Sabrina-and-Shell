using BaseCharacter.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
/// <summary>
/// The reach range for interactions via pressing 'E' in most games.
/// </summary>
public class ReachRange : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private SphereCollider sphereCollider;
    private BuyableObject buyobj;
    private readonly List<Blocks> item = new();
    public bool GetItem { get; private set; }
    private float Reach { get; set; }
    private float Interaction { get; set; }
    private string TagName { get; set; }
    private float crossHairReach;
    public float CrossHairReach { get { return crossHairReach; } set { crossHairReach = Mathf.Max(Reach, value); } }
    /// <summary>
    /// Sets the value of Reach + Sets the radius of your reach.
    /// </summary>
    /// <param name="reach">How big</param>
    public void SetReach(float reach)
    {
        Reach = reach;
        sphereCollider.radius = reach;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns><see cref="Reach"/></returns>
    public float GetReach()
    {
        return Reach;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Get the tagname of the object your colliding with in the interaction range.</returns>
    public string GetTag()
    {
        return TagName;
    }
    /// <summary>
    /// Get the object being bought
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetBuyAbleObject(int money)
    {
        if (buyobj != null)
        {
            InventoryItem buy = buyobj.BuyProduct(money);
            if (buy != null)
            {
                return buy;
            }
        }
        return null;
    }

    private void OnTriggerStay(UnityEngine.Collider other)
    {
        if (other.TryGetComponent(out BuyableObject buy))
        {
            buyobj = buy;
            TagName = other.tag;
        }
        if (other.TryGetComponent(out Blocks inv))
        {
            if (!item.Contains(inv))
            {
                item.Add(inv);
                //Debug.Log(inv.GetInventoryItem(false).GetName()); //I am unable to run this line of code... BUT, when I pick up the item, I get the full item back data and all wtf is wrong is programming why do we do this.
            }
        }
    }
    /// <summary>
    /// Attempts to add items to the inventory
    /// </summary>
    /// <param name="inventorySystem">A Inventory System</param>
    /// <returns>If returns false, then the inventory was full.</returns>
    public InventoryAddReturn AddItems(InventorySystem inventorySystem)
    {
        
        if (item.Count == 0)
            return InventoryAddReturn.NothingToAdd;

        Debug.Log($"Interact Pressed, EmptyItems: {inventorySystem.GetAmountOfEmptyItems()}, Items in range: {item.Count}");

        int itemsAdded = 0;
        int emptySlots = inventorySystem.GetAmountOfEmptyItems();

        // Check if we have enough space for all items
        if (emptySlots >= item.Count)
        {
            // Add all items
            for (int i = item.Count - 1; i >= 0; i--)
            {
                if (item[i] != null)
                {
                    InventoryItem inventoryItem = item[i].GetInventoryItem(true);
                    if (inventoryItem != null)
                    {
                        if (inventorySystem.AddItem(inventoryItem))
                        {
                            itemsAdded++;
                        }
                    }
                }
            }
            item.Clear();
            return itemsAdded > 0 ? InventoryAddReturn.Sucess : InventoryAddReturn.Fail;
        }
        else
        {
            // Partial add - fill available slots
            for (int i = item.Count - 1; i >= 0 && emptySlots > 0; i--)
            {
                if (item[i] != null)
                {
                    InventoryItem inventoryItem = item[i].GetInventoryItem(true);
                    if (inventoryItem != null)
                    {
                        if (inventorySystem.AddItem(inventoryItem))
                        {
                            item.RemoveAt(i);
                            itemsAdded++;
                            emptySlots--;
                        }
                    }
                }
            }
            return itemsAdded > 0 ? InventoryAddReturn.Sucess : InventoryAddReturn.Fail;
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        buyobj = null;
        TagName = null;
        if (other.TryGetComponent(out Blocks block))
        {
            item.Remove(block);
        }
    }


}
