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
    [SerializeField] private InventoryItem item;
    private readonly float GAMEDELAY = 0.025f;
    private float delay = 0;
    private bool toDestory = false;
    private float Weight { get { return weight; } set { try { body.mass = value; } catch { } weight = value; } }
    private float weight;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        hurtBox = GetComponent<HurtBox>();
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > Time.time)
        {
            GetHurtBoxData();
        }
        
    }
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
                        body.AddForce(apply[i].Knockback.GetKnockback(weight, transform.position));
                    }
                }
                hurtBox.ClearQueue();
            }
        }
    }
    public void SetupBox(InventoryItem item, float Weight)
    {
        this.item = item;
        this.Weight = Mathf.Max(Weight,1);
        try
        {
            body.mass = Weight;
        }
        catch (System.Exception e)
        {
            body = GetComponent<Rigidbody>();
            body.mass = Weight;
            Debug.LogWarning(e);
        }
        
    }
    public InventoryItem GetInventoryItem(bool destoryItem)
    {
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
