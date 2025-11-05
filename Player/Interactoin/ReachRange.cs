using BaseCharacter;
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
    private readonly List<Blocks> item = new List<Blocks>();
    public bool GetItem { get; private set; }
    private float Reach { get; set; }
    private float Interaction { get; set; }
    private string TagName { get; set; }
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out BuyableObject buy))
        {
            buyobj = buy;
            TagName = other.tag;
        }
        if (other.TryGetComponent(out Blocks inv))
        {
            item.Add(inv);
        }
    }
    /// <summary>
    /// Attempts to add items to the inventory
    /// </summary>
    /// <param name="inventorySystem">A Inventory System</param>
    /// <returns>If returns false, then the inventory was full.</returns>
    public InventoryAddReturn AddItems(InventorySystem inventorySystem)
    {
        Debug.Log($"Interact Pressed, EmptyItems: {inventorySystem.GetAmountOfEmptyItems()}, Items to add");
        if (item.Count > 0 && inventorySystem.GetAmountOfEmptyItems() >= item.Count)
        {
            foreach (Blocks item in item)
            {
                inventorySystem.AddItem(item.GetInventoryItem(true));
            }
            item.Clear();
            return InventoryAddReturn.Sucess;
        }
        if (item.Count > 0 && inventorySystem.GetAmountOfEmptyItems() < item.Count)
        {
            for (int i = 0; i < item.Count; i++)
            {
                if (!inventorySystem.AddItem(item[i].GetInventoryItem(true)))
                {
                    return InventoryAddReturn.Fail;
                }
                item.RemoveAt(i);
            }
        }
        return InventoryAddReturn.NothingToAdd;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        buyobj = null;
        TagName = null;
        item.Remove(other.GetComponent<Blocks>());
    }


}
