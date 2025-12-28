using BaseCharacter.Items;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


public class AmmoTemplete : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] Texture icon;
    [SerializeField] private string Name;
    [SerializeField][TextArea(5, 8)] private string Description;
    [SerializeField] private int Price;
    [SerializeField][Min(0)] private int MaxHoldable;
    [SerializeField] private float Weight = 8f;
    [Header("Projectile Data")]
    [SerializeField] private ProjectileTemplete[] projectiles;

    public InventoryItem GetAmmo()
    {
        List<Projectile> projectiling = new();
        foreach (var projectile in projectiles)
        {
            projectiling.Add(projectile.GetProjectile());
        }
        Item item = new Item(Name, Description, projectiling);
        return new InventoryItem(item, 1, HoldingType.LimitedStackable, Price, Weight, icon, MaxHoldable);
    }
}
