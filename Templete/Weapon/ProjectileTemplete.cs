using BaseCharacter.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTemplete : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Mulitplied by the gravity of the world")]
    [SerializeField] private float Gravity;
    [SerializeField][Min(0f)] private float Speed;
    [SerializeField][Min(0.05f)] private float LiveTime;
    [Tooltip("-100 on collision with hurtboxes, -1 on collision of anything else.")]
    [SerializeField][Min(1)] private int Piercing;
    [SerializeField][Min(0.0001f)] private float Size;
    [Tooltip("Lob a shot")]
    [SerializeField] private float Yeet;
    [Header("Falloff")]
    [SerializeField][Min(0)] private float StartFallOffDistace;
    [SerializeField] private float DamageAfterMaxFallOff;
    [SerializeField] private float EndFallOffDistance;
    [Header("Explosions")]
    [Tooltip("Smallest explosion size")]
    [SerializeField] private float SmallExplosiveSize;
    [Tooltip("Biggest explosion")]
    [SerializeField] private float ExplosiveSize;
    [Tooltip("How long the explosion lasts")]
    [SerializeField] private float ExplosiveTime;
    [Tooltip("Minimum damage during an explosion (outer edge of blast)")]
    [SerializeField] private float ExplosiveMinPercentFalloff;
    [Header("Knockback")]
    [Tooltip("1000 = 1 mass. Knockback is heavly effected by weight.")]
    [SerializeField][Min(1)] private float Weight = 1000f;
    [Tooltip("Z = Pull/Push, Y = Lift, X = Left/Right (Random)")]
    [SerializeField] private Vector3 KnockBack;
    [Header("On hit")]
    [Tooltip("Type in name of attributes")]
    [SerializeField] private string[] Attributes;
    [SerializeField] private GameObject SphereicalObject;
    [SerializeField] private float Damage;
    [SerializeField] private bool ShooterIsImmune = false;
    /// <summary>
    /// 
    /// </summary>
    /// <returns>A new <see cref="Projectile"/></returns>
    public Projectile GetProjectile()
    {
        Attributes ??= new string[0];
        return new Projectile(
            Gravity, Yeet, Speed, LiveTime,
            Piercing, Size, Weight, Damage,
            StartFallOffDistace, EndFallOffDistance, DamageAfterMaxFallOff,
            ExplosiveSize, ExplosiveTime, SmallExplosiveSize, ExplosiveMinPercentFalloff,
            KnockBack, SphereicalObject, ShooterIsImmune, Attributes);
    }
}
