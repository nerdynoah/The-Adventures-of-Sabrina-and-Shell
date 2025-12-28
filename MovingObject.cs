using BaseCharacter.Items;
using BaseCharacter.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class MovingProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] SphereCollider sphereCollider;
    private WorldRun run;
    private float gravity;
    private string Name;
    private HashSet<GameObject> gameObjects = new HashSet<GameObject>();
    private float LiveTime { get; set; }
    /// <summary>
    /// If the proj is an explosive, how big does the explosion get. (Use Sphere)
    /// </summary>
    private float ExplosiveSize { get; set; }
    /// <summary>
    /// If the proj is an explosive, How long does the explosvie last. (Use Sphere)
    /// </summary>
    private float ExplosiveTime { get; set; }
    /// <summary>
    /// If the proj is an explosive, what is the size the explosion starts at. (Use Sphere)
    /// </summary>
    private float SmallExplosiveSize { get; set; }
    /// <summary>
    /// Outer edge of explosion
    /// </summary>
    private float ExplosiveDamageMin { get; set; } = 0.5f;
    /// <summary>
    /// How many items can it travel through. Players/Enemies = -1, Everything else = -100;
    /// </summary>
    private int Piercing { get; set; }
    /// <summary>
    /// The farthest the bullet can travel before reaching minimum damage.
    /// </summary>
    private float MinDist { get; set; }
    /// <summary>
    /// The farthest the bullet can travel before its starts losing damage.
    /// </summary>
    private float MaxDist { get; set; }
    /// <summary>
    /// Size of the bullet. Uses vector3 if the bullet is not Sphere shaped. 
    /// </summary>
    private float Size { get; set; }
    /// <summary>
    /// Knockback direction, Z = Back, Y = lift, X = random. <code>Knockback *= (Weight * Damage + AdditionalWeight)/HitEntity.Weight</code>
    /// </summary>
    private Vector3 KnockBack { get; set; }
    /// <summary>
    /// Knockback Weight and Weight of object. 1000 = 1 mass.
    /// </summary>
    private float Weight { get; set; }
    /// <summary>
    /// Minimum percent fall off. A value from 0.0 to 1.0;
    /// </summary>
    private float MinPercentFalloff { get; set; }
    public GameObject SphereicalObject { get; private set; }
    public float Damage { get; private set; }
    public List<string> Attributes { get; private set; } = new List<string>();
    private WeaponClass wpnClass;
    private Vector3 StartingPos { get; set; }
    private Vector3 EndingPos { get; set; }
    private bool didExplode = false;
    private float ExplodeTimer = 0f;
    private float InvinciFrames = 0f;
    public void SetupProjectile(Projectile projectile, WeaponClass wpnClass, string name)
    {
        InvinciFrames = Time.deltaTime * 2f;
        LiveTime = Time.time + projectile.GetLiveTime(0);
        gravity = projectile.GetGravity();
        Weight = projectile.AdditionalWeight + projectile.Weight;
        MinPercentFalloff = projectile.MinPercentFalloff;
        Damage = projectile.Damage;
        Attributes = new List<string>(projectile.Attributes);
        Piercing = Mathf.Max(projectile.GetPercing(),1);
        //Finished
        SetExplosiveSize(projectile.GetExplosiveSize(), projectile.GetExplosiveTime(), projectile.GetSmallExplosionSize(), projectile.ExplosiveDamageMin);
        Size = projectile.GetSize();
        SetFallOff(projectile.GetDistance(false), projectile.GetDistance(true), projectile.MinPercentFalloff);
        KnockBack = projectile.KnockBack;
        transform.localScale = new Vector3(Size,Size,Size);
        this.wpnClass = wpnClass;
        Name = name;
        body.mass = Weight / 100;
    }
    public void SetFallOff(float minDist, float maxDist, float minPercent)
    {
        MinDist = minDist;
        MaxDist = maxDist;
        MinPercentFalloff = minPercent;
    }
    /// <summary>
    /// Gets the size of the object
    /// </summary>
    /// <returns>Vector3</returns>
    public float GetSize()
    {
        return Size;
    }
    /// <summary>
    /// Sets the size of the explosive
    /// </summary>
    /// <param name="smallSize">Smallest size of the explosvie</param>
    /// <param name="size">Largest size</param>
    /// <param name="explosiveTime">How long it takes to fully explode</param>
    private void SetExplosiveSize(float explosiveSize, float explosiveTime, float smallestExplosiveSize, float explosiveDamageMin)
    {
        ExplosiveSize = explosiveSize;
        ExplosiveTime = explosiveTime;
        SmallExplosiveSize = smallestExplosiveSize;
        ExplosiveDamageMin = explosiveDamageMin;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns><see cref="ExplosiveTime"/></returns>
    public float GetExplosiveTime()
    {
        return ExplosiveTime;
    }
    /// <summary>
    /// Gets the size of the explosion max
    /// </summary>
    /// <returns></returns>
    public float GetExplosiveSize()
    {
        return ExplosiveSize;
    }
    /// <summary>
    /// Gets the size of the inital blast
    /// </summary>
    /// <returns><see cref="SmallExplosiveSize"/></returns>
    public float GetSmallExplosionSize()
    {
        return SmallExplosiveSize;
    }
    /// <summary>
    /// If the object can travel through objects
    /// </summary>
    /// <returns><see cref="Piercing"/></returns>
    public int GetPercing()
    {
        return Piercing;
    }
    /// <summary>
    /// Gets Max distance or min distance
    /// </summary>
    /// <param name="max">true = max distance</param>
    /// <returns>Max/min Distance</returns>
    public float GetDistance(bool max)
    {
        if (max)
        {
            return MaxDist;
        }
        return MinDist;
    }
    /// <summary>
    /// Gets a value from 0.0 to 1.0;
    /// </summary>
    /// <returns><see cref="MinPercentFalloff"/></returns>
    public float GetMinFalloff()
    {
        return MinPercentFalloff;
    }
    /// <summary>
    /// Get knockback
    /// </summary>
    /// <param name="direct"></param>
    /// <returns></returns>
    public ForceKnockback GetKnockback()
    {
        float x = KnockBack.x * (UnityEngine.Random.value - 0.5f);
        Vector3 finalKnockback = new Vector3(x, KnockBack.y, KnockBack.z);

        // Use forward direction for knockback orientation
        return new ForceKnockback(finalKnockback, transform.position,Weight * 100);
    }
    public float Gravity
    {
        get
        {
            return gravity * run.Gravity;
        }
        set
        {
            gravity = value;
        }
    }
    private void Start()
    {
        run = WorldRun.Instance;
        StartingPos = transform.position;
    }
    private void Awake()
    {
        run = WorldRun.Instance;
        StartingPos = transform.position;
    }
    private void FixedUpdate()
    {
        body.AddForce(0, Gravity, 0, ForceMode.Acceleration);
        if (Time.time > LiveTime)
        {
            if (wpnClass == WeaponClass.Explosive)
            {
                DidExplode();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    private void Update()
    {
        if (didExplode)
        {
            Explode();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (wpnClass == WeaponClass.Bullet)
        {
            if (collision.collider.TryGetComponent<HurtBox>(out HurtBox box) && !gameObjects.Contains(collision.collider.gameObject))
            {
                //Vector3 distance = (collision.collider.transform.position - transform.position).normalized;
                float totalTravel = Vector3.Distance(StartingPos, transform.position);
                float travelRatio = Mathf.Clamp01((totalTravel - MinDist) / (MaxDist - MinDist));
                float damageMultiplier = Mathf.Lerp(1f, MinPercentFalloff, travelRatio);
                float finalDamage = Damage * damageMultiplier;
                gameObjects.Add(box.ApplyDamage(GetHashCode(), Name, 100, finalDamage, wpnClass));
                box.ApplyKnockback(GetHashCode(), Name, 10, GetKnockback());
                box.ApplyAttributes(GetHashCode(), Name, 20, CommandRequest.Attributes, Attributes.ToArray());
                Piercing -= 100;
            }
            else
            {
                Piercing--;
            }
            if (Piercing <= 0)
            {
                Destroy(gameObject);
            }
        }
        if (wpnClass == WeaponClass.Explosive)
        {
            if (collision.collider.TryGetComponent<HurtBox>(out HurtBox box) && !gameObjects.Contains(collision.collider.gameObject))
            {
                //Vector3 distance = (collision.collider.transform.position - transform.position).normalized;
                float totalTravel = Vector3.Distance(StartingPos, transform.position);

                // Fixed: Corrected damage falloff calculation
                float travelRatio = Mathf.Clamp01((totalTravel - MinDist) / (MaxDist - MinDist));
                float damageMultiplier = Mathf.Lerp(1f, MinPercentFalloff, travelRatio);
                float finalDamage = Damage * damageMultiplier;

                // Fixed: Explosive damage should decrease with distance from explosion center
                float distanceFromExplosion = Vector3.Distance(box.transform.position, transform.position);
                float explosiveDamageMultiplier = Mathf.Lerp(1f, ExplosiveDamageMin, (distanceFromExplosion / ExplosiveSize));
                finalDamage *= explosiveDamageMultiplier;

                gameObjects.Add(box.ApplyDamage(GetHashCode(), Name, 100, finalDamage, wpnClass));
                box.ApplyKnockback(GetHashCode(), Name, 10, GetKnockback());
                box.ApplyAttributes(GetHashCode(), Name, 20, CommandRequest.Attributes, Attributes.ToArray());
                Piercing -= 100;
            }
            else
            {
                Piercing--;
            }
            if (Piercing <= 0)
            {
                DidExplode();
            } 
        }
    }
    private void DidExplode()
    {
        if (!didExplode)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            ExplodeTimer = Time.time + ExplosiveTime;
            EndingPos = transform.position;
            Gravity = 0;
            transform.localScale = Vector3.one * SmallExplosiveSize;
            sphereCollider.isTrigger = true;
            gameObjects.Clear();
        }
        didExplode = true;
    }
    private void Explode()
    {
        transform.position = EndingPos;
        float progress = Mathf.Clamp01(1f - ((ExplodeTimer - Time.time) / ExplosiveTime));
        float temps = Mathf.Lerp(SmallExplosiveSize, ExplosiveSize, progress);
        transform.localScale = Vector3.one * temps;
        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (didExplode && other.TryGetComponent<HurtBox>(out HurtBox box))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(other.transform.position, transform.position);

            // Only apply damage if within current explosion size
            float currentSize = transform.localScale.x;
            if (distance <= currentSize)
            {
                float damageMultiplier = Mathf.Lerp(1f, ExplosiveDamageMin, (distance / currentSize));
                float explosionDamage = Damage * damageMultiplier;
                Vector3 knockbackDirection = body.velocity.normalized;
                Vector3 explosionDirection = (other.transform.position - transform.position).normalized;
                ForceKnockback knockback = new ForceKnockback(explosionDirection * KnockBack.magnitude, transform.position,Weight * 100);

                gameObjects.Add(box.ApplyDamage(GetHashCode(), Name, 100, explosionDamage, wpnClass));
                box.ApplyAttributes(GetHashCode(), Name, 20, CommandRequest.Attributes, Attributes.ToArray());
                box.ApplyKnockback(GetHashCode(), Name, 10, knockback);
            }
        }
    }
}
