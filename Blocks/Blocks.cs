using BaseCharacter.Items;
using BaseCharacter.Structual;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Blocks : MonoBehaviour
{
    protected Rigidbody body;
    protected HurtBox hurtBox;
    protected InventorySystem invSystem = new InventorySystem();
    protected readonly float GAMEDELAY = 0.025f;
    protected float delay = 0;
    protected bool toDestory = false;
    public float Weight { get; private set; }
    [SerializeField] private bool IsPickable = true;
    // Start is called before the first frame update
    protected void Start()
    {
        body = GetComponent<Rigidbody>();
        hurtBox = GetComponent<HurtBox>();
    }

    // Update is called once per frame
    void Update()
    {
        GetHurtBoxData();
    }
    /// <summary>
    /// Get knockback data.
    /// </summary>
    private void GetHurtBoxData()
    {
        if (Time.time > delay)
        {
            List<QueueInfo> apply = hurtBox.GetQueue();
            if (apply != null)
            {
                for (int i = 0; i < apply.Count; i++)
                {
                    if (apply[i].Request == CommandRequest.Knockback)
                    {
                        body.AddForce(apply[i].Knockback.GetKnockback(Mathf.Max(Weight * 1000,5500), transform.position));
                    }
                }
                hurtBox.ClearQueue();
            }
            delay = Time.time + GAMEDELAY;
        }
    }
    /// <summary>
    /// Setup box during summoning to set mass and give its item data.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="Weight"></param>
    public void SetupBox(InventoryItem item, float Weight)
    {
        invSystem.AddInventorySpaces(1);
        invSystem.FillNullInventory(invSystem.GetInventorySize()-1);
        invSystem.AddItem(item);
        this.Weight = Weight;
        IsPickable = true;
        if (Weight < 1)
        {
            try
            {
                body.mass = Mathf.Max(Weight * 50, 1);
            }
            catch (System.Exception e)
            {
                body = GetComponent<Rigidbody>();
                body.mass = Mathf.Max(Weight * 50, 1);
                Debug.LogWarning(e);
            }
        }
        else
        {
            try
            {
                body.mass = Mathf.Max(Weight, 25);
            }
            catch (System.Exception e)
            {
                body = GetComponent<Rigidbody>();
                body.mass = Mathf.Max(Weight, 25);
                Debug.LogWarning(e);
            }
        }
        
    }
    public virtual List<InventoryItem> GetInventoryItem(bool destoryItem)
    {
        if (IsPickable)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            for (int i = 0; i < invSystem.GetInventorySize(); i++)
            {
                items.Add(new InventoryItem(invSystem.GetInventoryItem(i)));
                Debug.Log(invSystem.GetInventoryItem(i).GetName());
                if (destoryItem)
                {
                    invSystem.DeleteItem(i);
                }
            }
            if (!toDestory)
            {
                if (destoryItem)
                {
                    Destroy(gameObject, 0.07f);
                    toDestory = true;       
                }
            }
            return items;
        }
        return new List<InventoryItem>();
    }
}
