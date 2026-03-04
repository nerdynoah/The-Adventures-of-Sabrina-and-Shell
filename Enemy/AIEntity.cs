using BaseCharacter;
using BaseCharacter.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class AIEntity : MonoBehaviour
{
    [SerializeField] private EntityTemplete entityTemplete;
    [SerializeField] private Walking walking;
    [SerializeField] private GameObject cameraPrefab;
    public Character EntityCharacter
    {
        get
        {
            return entityTemplete.character;
        }
    }
    /// <summary>
    /// Assume the <see cref="Walking"/> script will always call this function.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="swapToo"></param>
    /// <param name="swapInventoryies"></param>
    /// <returns></returns>
    public void SwapPerspectives(Walking original, EntityTemplete swapToo, bool swapInventoryies = false)
    {
        if (walking == null)
        {
            walking = gameObject.AddComponent<Walking>();
        }
        if (swapInventoryies)
        {
            SwapInventories.SwapInventorySystems(original.Player,swapToo.Player);
        }
        if (swapToo.Player.GetHotbarSize() < 1)
        {
            swapToo.Player.SetupHotbar(Mathf.Min(original.Player.GetHotbarSize(), swapToo.Player.GetInventorySize(), original.Player.GetHotbarSlot()));
        }
        swapToo.Player.SetPendingItem(original.Player.GetPendingItem());
        walking.Setup(swapToo);
        Instantiate(cameraPrefab,transform);
    }
    public void MakeEntityDestroyCamera(Walking original, GameObject destroy)
    {
        List<Enums.AttackWho> who = new()
            {
                AttackWho.NonAllys,
                AttackWho.Attackers
            };
        if (entityTemplete == null)
        {
            entityTemplete = gameObject.AddComponent<EntityTemplete>();
        }
        entityTemplete.SetupEntity(original.Character, who.ToArray());
        Destroy(destroy);
    }
}