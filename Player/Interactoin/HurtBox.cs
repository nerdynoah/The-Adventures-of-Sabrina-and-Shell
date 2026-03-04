using BaseCharacter;
using BaseCharacter.Effects;
using BaseCharacter.Movement;
using BaseCharacter.Structual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
/// <summary>
/// Apply Damage, Effects, Knockback, etc... via this Hurtbox.
/// </summary>
public class HurtBox : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private BoxCollider boxCollider;
    private List<QueueInfo> queue = new List<QueueInfo>();
    private bool haltQueue = false;
    /// <summary>
    /// Get all of the <see cref=QueueInfo"/> requested.
    /// </summary>
    /// <returns></returns>
    public List<QueueInfo> GetQueue()
    {
        if (queue.Count > 0 && !haltQueue)
        {
            return queue;
        }
        return null;
    }
    /// <summary>
    /// Clear the Queue
    /// </summary>
    public void ClearQueue()
    {
        queue.Clear();
    }
    /// <summary>
    /// Add damage to the <see cref="QueueInfo"/>, will be applied when the queue is called via <see cref="GetQueue()"/>
    /// </summary>
    /// <param name="id">use GetHashCode</param>
    /// <param name="name">Name of the person applying the damage</param>
    /// <param name="priority">Priority of function, the smaller it happens, the sooner the event is applied.</param>
    /// <param name="damage">How much damage</param>
    /// <param name="wpnclass">What <see cref="WeaponClass"/> is the damage. Used for resistances</param>
    public GameObject ApplyDamage(int id, string name, int priority, float damage, WeaponClass wpnclass)
    {
        queue.Add(new QueueInfo(id, name, priority, CommandRequest.Damage, damage, (int)wpnclass));
        Debug.Log($"Damage: {damage}");
        return gameObject;
    }
    /// <summary>
    /// Apply Attributes
    /// </summary>
    /// <param name="id">use GetHashCode</param>
    /// <param name="name">Name of the person applying the effect</param>
    /// <param name="priority">Priority of function, the smaller it happens, the sooner the event is applied.</param>
    /// <param name="effect">Custom effect.</param>
    public void ApplyAttributes(int id, string name, int priority, Effect[] effect)
    {
        queue.Add(new QueueInfo(id, name, priority, effect));
    }
    /// <summary>
    /// Apply a Command
    /// </summary>
    /// <param name="id">use GetHashCode</param>
    /// <param name="name"></param>
    /// <param name="priority">Name of the person applying the effect</param>
    /// <param name="request">The Request wanted</param>
    /// <param name="things">An array of names or a name which usually get searched for via the ItemLibrary.</param>
    public void ApplyAttributes(int id, string name, int priority, CommandRequest request, params string[] things)
    {
        queue.Add(new QueueInfo(id, name, priority, request, things));
    }
    /// <summary>
    /// Apply Knockback with the <see cref="ForceKnockback"/>
    /// </summary>
    /// <param name="id">use GetHashCode</param>
    /// <param name="name">Name of the person applying the knockback</param>
    /// <param name="priority">Priority of the knockback</param>
    /// <param name="knockback">Knockback applied from <see cref="ForceKnockback"/></param>
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
    /// <returns>Gets the radius (if applicable) or the boxsize maginitude based on what you have.</returns>
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
