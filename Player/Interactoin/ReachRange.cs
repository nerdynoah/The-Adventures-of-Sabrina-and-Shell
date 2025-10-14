using BaseCharacter.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The reach range for interactions via pressing 'E' in most games.
/// </summary>
public class ReachRange : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] BuyableObject buyobj;
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
        BuyableObject buy = other.GetComponent<BuyableObject>();
        if (buy != null)
        {
            buyobj = buy;
        }
        TagName = other.tag;
        Debug.Log(TagName);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        buyobj = null;
        TagName = null;
    }


}
