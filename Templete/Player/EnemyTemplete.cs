using BaseCharacter;
using BaseCharacter.Entity;
using BaseCharacter.FiveSenses;
using BaseCharacter.Items;
using BaseCharacter.Movement;
using BaseCharacter.Structual;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class EntityTemplete : MonoBehaviour, IHasCharacter
{
    [Header("Entity Body")]
    [SerializeField] private GameObject Body;
    [SerializeField] private CapsuleCollider outline; 
    [SerializeField] private string Name;
    [SerializeField][TextArea(4,8)] private string Desc;
    [SerializeField] private string PresetCharacterName;
    [Header("Level and EXP")]
    [SerializeField][Min(0)] private int BaseLevel = 0;
    [SerializeField] private int BaseEXPDrop = 1;
    [SerializeField] private int increasePerLevel = 1;
    [Header("Health")]
    [SerializeField][Range(0f, 1f)] private float StartingHealthPercent = 1;
    [Header("Items, Effect, and Things")]
    [SerializeField] private string[] FindInventoryItemsInLibary;
    [SerializeField] private int[] Amount;
    [Space(15)]
    [SerializeField] private string[] FindAttributeInLibary;
    [SerializeField][Range(0, 32)] private int ExtraItemSlots;
    [SerializeField][Min(0.02f)] private float PASSIVECHECK = 0.2f;
    [SerializeField][Range(0.1f, 20f)] private float timeNotShootingReload = 1f;
    [Space(15)]
    [SerializeField] private bool ApplyAttributesRandomly = false;
    [SerializeField][Range(1, 10001)] private int ApplyAttributesRNGrate = 2500;
    [Header("Sensors")]
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private Rigidbody body;
    [SerializeField] private MovementEnemy movement;
    [SerializeField] private SummonStench summonStench;
    [Header("Mood and AI")]
    [SerializeField] private AttackWho[] whoToAttack = new AttackWho[Enum.GetValues(typeof(AttackWho)).Length];
    [SerializeField] private FiveSenses fiveSenses;
    public Character character = new Character(Classes.Vampire);
    private MovingSystemKeyboard moveSys => character.MoveSys;
    private JumpSystem JumpSys => character.JumpSys;
    private GRDPound gPound => character.Gpound;
    private AirMovement airMent => character.AirMent;
    public Player Player { get => character.Player; private set { character.SetPlayer(value); } }
    private WorldRun worldRun;
    private float FastestFall;
    private bool isGrounded = true;
    private float maxSlopeAngle = 80;
    private float TimeNotShootingReload;
    private float timeToPassiveCheck = 0;
    public bool Ready { get; private set; } = false;
    public float GetEyeUpdateRate { get { return fiveSenses.GetEyeSightUpdateRate; } }
    private float Gravity
    {
        get
        {
            return Player.Gravity * worldRun.Gravity;
        }
    }
    public List<string> Attackers { get; private set; } = new List<string>();
    public void Init()
    {
        Player.FillNullInventory();
        Player.SetupHotbar(Player.GetInventorySize(),0);
        body.mass = Player.Weight / 100f;
        //jumpSys = new JumpSystem(AllowedJumps, 0,MidAirJumpsMultiplyer);
        for (int i = 0; i < FindInventoryItemsInLibary.Length; i++)
        {
            Player.AddItem(AllLibary.ItemLibary.SearchLibaryForInventoryItem(FindInventoryItemsInLibary[i]));
        }
        for (int i = 0; i < FindAttributeInLibary.Length; i++)
        {
            if (ApplyAttributesRandomly && Methods.RandomValuePositive(ApplyAttributesRNGrate) < ApplyAttributesRNGrate) //Apply attributes
            {
                Player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(FindAttributeInLibary[i]));
            }
            else if (!ApplyAttributesRandomly)
            {
                Player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(FindAttributeInLibary[i]));
            }
        }
        body.mass = Player.Weight / 100f;
        character.SetEnemy(StartingHealthPercent);
    }
    private void Start()
    {
        worldRun = WorldRun.Instance;
        character = new Character(AllLibary.ItemLibary.SearchLibaryForCharacter(PresetCharacterName),new Player(Name, ExtraItemSlots + FindInventoryItemsInLibary.Length));
        Init();
        Ready = true;
    }
    private void Awake()
    {
        worldRun = WorldRun.Instance;
        character = new Character(AllLibary.ItemLibary.SearchLibaryForCharacter(PresetCharacterName));
        Init();
        Ready = true;
    }
    private void GetHurtBoxData()
    {
        List<QueueInfo> apply = hurtBox.GetQueue();
        if (apply != null)
        {
            for (int i = 0; i < apply.Count; i++)
            {
                if (apply[i].Request == CommandRequest.Attributes)
                {
                    Player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(apply[i].DataString));
                }
                if (apply[i].Request == CommandRequest.Damage)
                {
                    Player.Health.DamagePlayer(apply[i].DataFloat[0], (WeaponClass)(int)apply[i].DataFloat[1]);
                    Attackers.Add(apply[i].Name);
                }
                if (apply[i].Request == CommandRequest.Knockback)
                {
                    body.AddForce(apply[i].Knockback.GetKnockback(Player.Weight, transform.position));
                    body.AddForce(0, apply[i].Knockback.GetYKnockback(Player.Weight), 0);
                }
            }
            Debug.Log($"Health == {Player.Health.Max}");
            hurtBox.ClearQueue();
        }
    }
    public string GetName() { return Name; }
    private void Update()
    {
        if (Time.time > TimeNotShootingReload)
        {
            for (int i = 0; i < Player.GetInventorySize(); i++)
            {
                Reload(Player.GetInventory()[i]);
            }
        }
        if (Time.time > timeToPassiveCheck)
        {
            Player.CheckPassive();
            timeToPassiveCheck = Time.time + PASSIVECHECK;
        }
        GetHurtBoxData();
        if (!Player.Health.GetIsAlive())
        {
            Destroy(gameObject);
            return;
        }
    }
    /// <summary>
    /// Checks if your grounded.
    /// </summary>
    /// <remarks>
    /// Code partially made from https://discussions.unity.com/t/how-do-i-properly-detect-if-the-Player-is-grounded/1545895/5
    /// </remarks>
    public bool CheckGrounded()
    {
        if (body.velocity.y > -1 * Gravity)
        {
            return false;
        }
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 1f);
        for (int i = 0; i < hits.Length; i++)
        {
            if (Mathf.Approximately(hits[i].distance, 0))
            {
                continue;
            }
            // Check if we're on stable ground
            float angle = Vector3.Angle(Vector3.up, hits[i].normal);
            bool isStableOnGround = angle <= maxSlopeAngle;
            FallDamage(isStableOnGround, Player.GravityProtectionTime, 100, 10);
            return isStableOnGround;
        }
        return false;
    }
    private void CalculateBestFall(float value)
    {
        if (value > 1)
        {
            FastestFall += value * Time.deltaTime;
        }
        else if (value < FastestFall)
        {
            FastestFall = value;
        }
        if (value > FastestFall)
        {
            FastestFall += 6 * Time.deltaTime;
        }
    }
    private float smellDelay = 0;
    private float SMELLDELAY = 5f;
    ///Thank you to https://discussions.unity.com/t/addforce-vs-addrelativeforce-vs-rigidbody-velocity/400189 for helping me understand Unity forces.
    private void FixedUpdate()
    {
        CalculateBestFall(body.velocity.y);

        Vector3 delta = moveSys.GetSimpleMvmDeltas(isGrounded);
        Vector3 direct = transform.eulerAngles;
        //Vector3 forwardDirection = new Vector3(direct.x, 0, direct.z).normalized;
        Vector3 MoveDirection = direct * delta.z + Quaternion.Euler(0, 90, 0) * direct * delta.x;
        //float horizontalMagnitude = new Vector3(body.velocity.x, 0, body.velocity.z).magnitude;
        Vector3 Speed = Time.fixedDeltaTime * Player.Speed * MoveDirection;
        Vector3 Rotation;
        body.drag = isGrounded ? airMent.GroundDrag : airMent.AirDrag;
        body.AddRelativeForce(Speed, ForceMode.VelocityChange);
        if (!isGrounded)
        {
            //body.AddRelativeForce(Rotation, ForceMode.VelocityChange);
            TookDamage = false;
        }
        body.AddRelativeForce(new Vector3(0, Gravity, 0), ForceMode.Acceleration);
        //Debug.Log($"Body.Velocity {body.velocity}");
        Player.ApplyStatAdjustments();
        Player.ApplyAllEffects();
        /*
        if (fiveSenses.GetNose().ShouldCreateNewScent(character) && Time.time > smellDelay)
        {
            summonStench.SummonBubble(character, transform.position);
            smellDelay = Time.time + SMELLDELAY;
            summonStench.ToString();
        }
        */
    }

    private bool TookDamage = false;
    /// <summary>
    /// Calculates fall damage based on -Body.Velocity.y. <code>(Damage = Gravity * <paramref name="gravProt"/> - <see cref="BaseCharacter.Entity.GetGroundPound()"/> - <see cref="BaseCharacter.Entity.Jump"/>)/(<paramref name="div"/>)</code>
    /// </summary>
    /// <param name="isGrounded">Is grounded</param>
    /// <param name="gravProt">How many seconds of protection from gravity if moving at GroundPoundSpeed do you get</param>
    /// <param name="div">Devide damage</param>
    private void FallDamage(bool isGrounded, float gravProt, float div = 100, float secondaryThresh = 10)
    {
        if (isGrounded && !TookDamage)
        {
            float damageProt = Gravity * gravProt - Player.GroundPound - Player.Jump - Player.Speed;
            Debug.Log(-FastestFall + $"Threshold: {-damageProt}");
            float damage = Mathf.Max(-FastestFall + damageProt, 0);
            if (damage > secondaryThresh)
            {
                damage = Mathf.Pow(damage, 1.2f);
            }
            damage /= div;
            Debug.Log(damage);
            Player.Health.DamagePlayer(damage, WeaponClass.World, false, 1f);
            TookDamage = true;
            FastestFall = 0;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = CheckGrounded();
        if (!isGrounded)
        {
            gPound.Reset();
        }
        else
        {

        }
    }
    public List<AttackWho> WhoToAttack()
    {
        List<AttackWho> list = new List<AttackWho>();
        if (whoToAttack.Contains<AttackWho>(AttackWho.AttackGenocide))
        {
            list.Add(AttackWho.AttackGenocide);
            return list;
        }
        if (whoToAttack.Contains<AttackWho>(AttackWho.AttackGenocide))
        {
            list.Add(AttackWho.AttackGenocide);
            return list;
        }
        list.AddRange(whoToAttack);
        if (list.Count < 1 || list.Contains(AttackWho.None))
        {
            return null;
        }
        return list;
    }

    public bool GetName(string name)
    {
        return ((IName)character).GetName(name);
    }

    public string GetCharName()
    {
        return character.CharacterName;
    }

    public bool GetCharName(string name)
    {
        return (character.CharacterName == name);
    }

    public Player GetPlayer()
    {
        return Player;
    }

    public bool IsPlayablePlayer()
    {
        return false;
    }

    public string GetDesc()
    {
        return ((INameDesc)character).GetDesc();
    }

    public bool GetDesc(string name)
    {
        return ((INameDesc)character).GetDesc(name);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public Vector3 GetOffsetToSelf(float size)
    {
        Ray placementRay = new Ray (new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, 0f),transform.forward);
        float spawnForwardOffset = Mathf.Max((Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.z) + Mathf.Abs(body.velocity.y)) / 12f, 0.01f + size);
        float lookingArea = transform.eulerAngles.x;
        if (lookingArea > 0 && lookingArea < 100f) //0-90 value
        {
            lookingArea /= 80f;
        }
        spawnForwardOffset += Mathf.Lerp(0.2f, 0.8f + outline.height, lookingArea); //Adjust the value in the middle
        return placementRay.origin + placementRay.direction * spawnForwardOffset;
    }

    public void RotateTo(Vector3 loc)
    {
        Vector3 direction = (loc - transform.position).normalized;
        transform.Rotate(direction);
    }
    public void ShootHold(InventoryItem item, MouseClick click)
    {
        if (item.GetItemType() == ItemType.Weapon)
        {
            Weapon rockTMP = item.GetItem<Weapon>();
            float speed = Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.z);
            //Debug.Log($"canFire: {rockTMP.GetCanFire(false)} and is standered {rockTMP.WeaponDesign == WeaponDesign.Standered}");
            if (rockTMP.WeaponDesign == WeaponDesign.Standered && rockTMP.GetCanFire(true))
            {
                rockTMP.ConsumeAmmo(1);
                rockTMP.ApplyAttackDelay();
                Debug.Log(Player.Aiming);
                TimeNotShootingReload = Time.time + timeNotShootingReload;
                if (!rockTMP.UsingPattern)
                {
                    RaycastHit hit = movement.GetRayPoint(rockTMP, Player.Aiming);
                    Vector3 direction = (hit.point - transform.position).normalized;
                    Debug.DrawLine(transform.position, hit.point, Color.blue, 6f);
                    Projectile projectile = rockTMP.GetProjectile((int)click);
                    Vector3 offset;
                    GameObject tempBullet = Instantiate(projectile.SphereicalObject);
                    Debug.Log($"immune to object: {projectile.IsShooterImmune}");
                    if (projectile.IsShooterImmune)
                    {
                        tempBullet.GetComponent<MovingProjectile>().SetImmune(hurtBox.gameObject);
                        offset = GetOffsetToSelf(rockTMP.GetProjectile((int)click).GetSize() / 2);
                    }
                    else
                    {
                        offset = GetOffsetToSelf(rockTMP.GetProjectile((int)click).GetSize() / 2 + hurtBox.GetSize());
                    }
                    if (rockTMP.GetWeaponClass() == WeaponClass.Melee)
                    {
                        tempBullet.GetComponent<MovingProjectile>().SetupMeleeProjectile(projectile, rockTMP.GetWeaponClass(), Player.Weight, Player.GetName());
                    }
                    else
                    {
                        tempBullet.GetComponent<MovingProjectile>().SetupProjectile(projectile, rockTMP.GetWeaponClass(), Player.GetName());
                    }
                    tempBullet.transform.position = offset;
                    tempBullet.GetComponent<Rigidbody>().AddForce(speed * direction);
                    tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetSpeed() * direction);
                    tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetYeet() * new Vector3(0, 1, 0));
                }
                else
                {
                    Vector2[] pattern = rockTMP.GetBulletPattern();
                    Debug.Log("Making bullets");
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        RaycastHit hit = movement.GetRayPoint(rockTMP, Player.Aiming);
                        Vector3 offset = GetOffsetToSelf(rockTMP.GetProjectile((int)click).GetSize() + hurtBox.GetSize());
                        Vector3 direction = (hit.point - transform.position).normalized;
                        Debug.DrawLine(offset, hit.point, Color.blue, 6f);
                        Projectile projectile = rockTMP.GetProjectile((int)click);
                        GameObject tempBullet = Instantiate(projectile.SphereicalObject);
                        if (projectile.IsShooterImmune)
                        {
                            tempBullet.GetComponent<MovingProjectile>().SetImmune(hurtBox.gameObject);
                            offset = GetOffsetToSelf(hurtBox.GetSize());
                        }
                        else
                        {
                            offset = GetOffsetToSelf(rockTMP.GetProjectile((int)click).GetSize() + hurtBox.GetSize());
                        }
                        if (rockTMP.GetWeaponClass() == WeaponClass.Melee)
                        {
                            tempBullet.GetComponent<MovingProjectile>().SetupMeleeProjectile(projectile, rockTMP.GetWeaponClass(), Player.Weight, Player.GetName());
                        }
                        else
                        {
                            tempBullet.GetComponent<MovingProjectile>().SetupProjectile(projectile, rockTMP.GetWeaponClass(), Player.GetName());
                        }
                        tempBullet.transform.position = offset;
                        tempBullet.GetComponent<Rigidbody>().AddForce(speed * direction);
                        tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetSpeed() * direction);
                        tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetYeet() * new Vector3(0, 1, 0));
                        tempBullet.GetComponent<Rigidbody>().AddForce(body.velocity.y * new Vector3(0, 1, 0));
                    }

                }
            }
        }
    }
    public void Reload(InventoryItem item)
    {
        if (item.GetItemType() == ItemType.Weapon)
        {
            Weapon rockTmp = item.GetItem<Weapon>();
            if (rockTmp.AmmoHold == AmmoHoldDesign.SingleBullet || rockTmp.AmmoHold == AmmoHoldDesign.Magazine || rockTmp.AmmoHold == AmmoHoldDesign.CustomReloadMagazine)
            {
                //Debug.Log("start reloading");
                rockTmp.ActivatePassiveReload();
            }
        }
    }
}