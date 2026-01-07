using BaseCharacter;
using BaseCharacter.Entity;
using BaseCharacter.Items;
using BaseCharacter.Movement;
using BaseCharacter.Structual;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class EntityTemplete : MonoBehaviour
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
    [Space(22)]
    [SerializeField] private WeaponClass[] wpnClass = new WeaponClass[Enum.GetValues(typeof(WeaponClass)).Length];
    [SerializeField] private int[] Resistances = new int[Enum.GetValues(typeof(WeaponClass)).Length];
    [Header("Items, Effect, and Things")]
    [SerializeField] private string[] FindInventoryItemsInLibary;
    [SerializeField] private string[] FindAttributeInLibary;
    [SerializeField][Range(0, 32)] private int ExtraItemSlots;
    [Space(10)]
    [SerializeField] private bool ApplyAttributesRandomly = false;
    [SerializeField][Range(1, 10001)] private int ApplyAttributesRNGrate = 2500;
    [Header("Sensors")]
    [SerializeField] private Vision caughtBox;
    [SerializeField] private Vision TooCloseBox;
    [SerializeField] private Vision IdealAttackingRange;
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private Rigidbody body;
    [Header("Intelligence")]
    [SerializeField] private FiveSenses brain;
    [SerializeField] private PathMode DefaultPathMode;
    [SerializeField] private WanderMode wanderMode;
    public Character character = new Character(Classes.Vampire);
    private MovingSystemKeyboard moveSys => character.MoveSys;
    private JumpSystem jumpSys => character.JumpSys;
    private GRDPound gPound => character.Gpound;
    private AirMovement airMent => character.AirMent;
    public Player Player { get => character.Player; private set { character.SetPlayer(value); } }
    private WorldRun worldRun;
    private float FastestFall;
    private bool isGrounded = true;
    private float maxSlopeAngle = 80;
    public bool Ready { get; private set; } = false;
    private float Gravity
    {
        get
        {
            return Player.Gravity * worldRun.Gravity;
        }
    }
    
    public void Init()
    {
        for (int i = 0; i < Enum.GetValues(typeof(WeaponClass)).Length; i++)
        {
            Player.SetupResistance(wpnClass[i], Resistances[i]);
        }
        Player.SetupHotbar(Player.GetInventorySize(),0);
        body.mass = Player.Weight / 100f;
        //jumpSys = new JumpSystem(AllowedJumps, 0,MidAirJumpsMultiplyer);
        for (int i = 0; i < FindInventoryItemsInLibary.Length; i++)
        {
            Player.AddItem(AllLibary.ItemLibary.SearchLibaryForInventoryItem(FindInventoryItemsInLibary[i]));
        }
        for (int i = 0; i < FindAttributeInLibary.Length; i++)
        {
            if (ApplyAttributesRandomly && Methods.RandomValue(ApplyAttributesRNGrate) < ApplyAttributesRNGrate) //Apply attributes
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
                }
                if (apply[i].Request == CommandRequest.Knockback)
                {
                    body.AddForce(apply[i].Knockback.GetKnockback(Player.Weight, transform.position));
                    body.AddForce(0, apply[i].Knockback.GetYKnockback(Player.Weight), 0);
                }
            }
            hurtBox.ClearQueue();
        }
    }
    public string GetName() { return Name; }
    private void Update()
    {
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

    ///Thank you to https://discussions.unity.com/t/addforce-vs-addrelativeforce-vs-rigidbody-velocity/400189 for helping me understand Unity forces.
    private void FixedUpdate()
    {
        CalculateBestFall(body.velocity.y);

        bool dirPressed = false;
        

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

}