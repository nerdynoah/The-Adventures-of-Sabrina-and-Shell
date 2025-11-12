using BaseCharacter;
using BaseCharacter.Items;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

//[CreateAssetMenu(fileName = "Weapon", menuName = "Game/Templetes")]
public class WeaponTemplete : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] Texture icon;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    [SerializeField] private string Name;
    [SerializeField][TextArea(3,8)] private string Description;
    [SerializeField] private int Price;
    [SerializeField] private float Weight = 100f;
    [SerializeField] private WeaponDesign design;
    [Header("Stats")]
    [SerializeField] private float Damage;
    [Tooltip("Leave the Aim value above 20 to ensure when players level up, weapons don't become perfectly accurate. Every 10 Levels gives 4 AIM points")]
    [SerializeField] private float Aim;
    [SerializeField][Min(0)] private float AttackDelay;
    [SerializeField] private int SizeOfObject;
    [SerializeField][Min(0)] private int StackableAmount = 0;
    [Header("Holdable Magazene")]
    [SerializeField] private bool UsesAmmo = true;
    [SerializeField][Min(1)] private int AmountOfAmmo;
    [SerializeField] private bool OneAtATimeReload = true;
    [SerializeField] private int AmountPerReload = 1;
    [SerializeField] private float ReloadSpeed;
    [SerializeField] private string[] AllowedAmmo;
    [SerializeField][Min(0)] private int AdditinoalPiercing = 0;
    [Header("Spread")]
    [SerializeField] private bool UsesSpread;
    [SerializeField] private int Rows;
    [SerializeField] private int Columns;
    [SerializeField] private float Distance;
    [Header("Protectile")]
    [Tooltip("Alter how the Projectile works. Also effects resistances.")]
    [SerializeField] private WeaponClass WeaponClass;
    [SerializeField] private ProjectileTemplete[] projectile;
    [Header("Effects")]
    [SerializeField] private string[] FindAttributeInLibary;
    [Header("Animation")]
    [SerializeField] private Texture[] Animations;
    [SerializeField] private AudioClip[] effects;
    [SerializeField] private int[] cuts;
    [SerializeField] private AnimationType[] type;

    private Weapon rockTMP;
    private Weapon Init()
    {
        if (UsesAmmo)
        {
            rockTMP = new Weapon(Name, WeaponClass, AttackDelay, AttackDelay, Damage, Aim, OneAtATimeReload, ReloadSpeed, AmountOfAmmo);
            rockTMP.SetAcceptableAmmo(AdditinoalPiercing, AllowedAmmo);
        }
        else
        {
            rockTMP = new Weapon(Name, WeaponClass,AttackDelay, AttackDelay, Damage, Aim);
            rockTMP.SetupProjectile(projectile[0].GetProjectile());
        }
        if (UsesSpread)
        {
            rockTMP.SetBulletPattern(Rows, Columns, Distance);
        }
        rockTMP.SetEffect(FindAttributeInLibary);
        rockTMP.SetAnimations(Animations, cuts, type, effects);
        return rockTMP;
    }

    public InventoryItem GetItem()
    {
        HoldingType type = HoldingType.Single;
        if (StackableAmount > 1)
        {
            type = HoldingType.LimitedStackable;
        }
        else
        {
            type = HoldingType.Single;
        }
        return new InventoryItem(Init(), type, SizeOfObject, Price, Weight, icon, mesh, material, StackableAmount);
    }
}
