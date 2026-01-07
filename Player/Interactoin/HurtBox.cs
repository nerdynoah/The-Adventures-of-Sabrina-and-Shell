using BaseCharacter;
using BaseCharacter.Effects;
using BaseCharacter.Movement;
using BaseCharacter.Structual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private BoxCollider boxCollider;
    private List<QueueInfo> queue = new List<QueueInfo>();
    private bool haltQueue = false;

    public List<QueueInfo> GetQueue()
    {
        if (queue.Count > 0 && !haltQueue)
        {
            return queue;
        }
        return null;
    }
    public void ClearQueue()
    {
        queue.Clear();
    }
    /// <summary>
    /// Damage 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="damage"></param>
    public GameObject ApplyDamage(int id, string name, int priority, float damage, WeaponClass wpnclass)
    {
        queue.Add(new QueueInfo(id, name, priority, CommandRequest.Damage, damage, (int)wpnclass));
        return gameObject;
    }
    public void ApplyAttributes(int id, string name, int priority, Effect[] effect)
    {
        queue.Add(new QueueInfo(id, name, priority, effect));
    }
    public void ApplyAttributes(int id, string name, int priority, CommandRequest request, params string[] things)
    {
        queue.Add(new QueueInfo(id, name, priority, request, things));
    }
    public void ApplyKnockback(int id, string name, int priority, ForceKnockback knockback)
    {
        queue.Add(new QueueInfo(id, name, priority, knockback));
    }
    /// <summary>
    /// Stop/Start the Queue
    /// </summary>
    /// <param name="halt"></param>
    public void StartOrHaltQueue(bool halt)
    {
        haltQueue = halt;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetSize()
    {
        try
        {
            return capsuleCollider.radius;
        }
        catch
        {
            try
            {
                return boxCollider.size.magnitude;
            }
            catch
            {
                return 1;
            }
            
        }

    }
    public void Awake()
    {
        if (capsuleCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }
    }
    public void Start()
    {
        if (capsuleCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }
    }
}
