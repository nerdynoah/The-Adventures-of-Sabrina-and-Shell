using BaseCharacter;
using BaseCharacter.Items;
using BaseCharacter.Structual;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Blocks : MonoBehaviour
{
    private Rigidbody body;
    private HurtBox hurtBox;
    private InventoryItem item;
    private readonly float GAMEDELAY = 0.025f;
    private float delay = 0;
    private bool toDestory = false;
    public float Weight { get; private set; }
    // Start is called before the first frame update
    void Start()
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
        try
        {
            body.mass = Mathf.Max(Weight * 10, 10);
        }
        catch (System.Exception e)
        {
            body = GetComponent<Rigidbody>();
            body.mass = Mathf.Max(Weight * 10, 10); 
            Debug.LogWarning(e);
        }
        
    }
    public InventoryItem GetInventoryItem(bool destoryItem)
    {
        Debug.Log($"Getting item: {item.GetName()}");
        if (!toDestory)
        {
            if (destoryItem)
            {
                Destroy(gameObject, 0.07f);
                toDestory = true;
                return item;
            }
            return item;
        }
        return null;
    }
}
