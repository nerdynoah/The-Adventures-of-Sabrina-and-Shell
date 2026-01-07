using BaseCharacter;
using BaseCharacter.Items;
using BaseCharacter.Structual;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Blocks : MonoBehaviour
{
    protected Rigidbody body;
    protected HurtBox hurtBox;
    protected InventoryItem item;
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
                        //body.AddForce(apply[i].Knockback.GetKnockback(weight * 1000, transform.position));
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
        this.item = item;
        this.Weight = Weight;
        IsPickable = true;
        if (Weight < 1)
        {
            try
            {
                body.mass = Mathf.Max(Weight * 10, 1);
            }
            catch (System.Exception e)
            {
                body = GetComponent<Rigidbody>();
                body.mass = Mathf.Max(Weight * 10, 1);
                Debug.LogWarning(e);
            }
        }
        else
        {
            try
            {
                body.mass = Mathf.Max(Weight, 10);
            }
            catch (System.Exception e)
            {
                body = GetComponent<Rigidbody>();
                body.mass = Mathf.Max(Weight, 10);
                Debug.LogWarning(e);
            }
        }
        
    }
    public virtual InventoryItem GetInventoryItem(bool destoryItem)
    {
        if (IsPickable)
        {
            Debug.Log($"Getting item: {item.GetName()}");
            if (!toDestory)
            {
                if (destoryItem)
                {
                    Destroy(gameObject, 0.07f);
                    toDestory = true;
                    return new InventoryItem(item);
                }
                return new InventoryItem(item);
            }
        }
        return null;
    }
}
