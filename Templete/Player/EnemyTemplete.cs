using BaseCharacter;
using BaseCharacter.Entities;
using BaseCharacter.Items;
using BaseCharacter.Movement;
using BaseCharacter.Structual;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using static Enums;

public class EntityTemplete : MonoBehaviour
{
    [Header("Entity Body")]
    [SerializeField] private GameObject Body;
    [SerializeField] private CapsuleCollider outline; 
    [SerializeField] private string Name;
    [SerializeField][TextArea(4,8)] private string Desc;
    [Header("Level and EXP")]
    [SerializeField][Min(0)] private int BaseLevel = 0;
    [SerializeField] private int BaseEXPDrop = 1;
    [SerializeField] private int increasePerLevel = 1;
    [Header("Health")]
    [SerializeField][Min(1)] private int MaxHealth;
    [SerializeField][Range(0f, 1f)] private float StartingHealthPercent = 1;
    [SerializeField][Min(0f)] private float AdranalineDistance = 0f;
    [SerializeField][Min(0f)] private float Adranaline = 0f;
    [Space(22)]
    [SerializeField] private WeaponClass[] wpnClass = new WeaponClass[Enum.GetValues(typeof(WeaponClass)).Length];
    [SerializeField] private int[] Resistances = new int[Enum.GetValues(typeof(WeaponClass)).Length];
    [Header("Movement")]
    [SerializeField] private float Speed = 35f;
    [SerializeField] private float RotationSpeed = 7f;
    [SerializeField][Range(0.7f,0.9999f)] private float BreakingSpeed = 0.97f;
    [SerializeField][Range(0f,180f)] private float maxSlopeAngle = 79f;
    [Tooltip("100 weight = 1 KG")]
    [SerializeField][Min(10f)] private float Weight = 1000f;
    [Header("Jumps")]
    [SerializeField] private float JumpAmount = 32f;
    [SerializeField][Min(0)] private int AllowedJumps = 1;
    [SerializeField][Min(0)] private int MidAirJumps = 1;
    [SerializeField][Range(0f,2f)] private float MidAirJumpsMultiplyer = 0.8f;
    [Header("Gravity")]
    [SerializeField] private float GravityMultiplied = 3f;
    [SerializeField] private float GroundPound = 100f;
    [SerializeField][Min(0)] private byte AmountOfGroundPounds = 1;
    [SerializeField] private float FallDamageProtectionTime = 1f;
    [Header("Sight and AIM")]
    [SerializeField][Min(0f)] private float Vision = 80f;
    [SerializeField][Min(0f)] private float AIM = 0f;
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
    public Player player { get; private set; }
    private JumpSystem jumpSys;
    private MovingSystemKeyboard moveSys;
    private GRDPound gPound;
    private AirMovement airMent;
    private WorldRun worldRun;
    private float FastestFall;
    private bool isGrounded = true;
    private readonly float JUMPDELAY = 0.125f;
    private float jumpDelay;
    private float Gravity
    {
        get
        {
            return player.Gravity * worldRun.Gravity;
        }
    }
    
    public void Init()
    {
        moveSys = new MovingSystemKeyboard(0.1f, 0.6f, 0.8f, 1.05f, 0.38f, 0.35f);
        gPound = new GRDPound(AmountOfGroundPounds);
        airMent = new AirMovement(1.285f, 0.68f, 0.185f);
        player = new(Name, Desc, MaxHealth, StartingHealthPercent, Weight, Vision, AIM, ExtraItemSlots, Adranaline);
        player.SetupMovement(Speed, JumpAmount, GroundPound, GravityMultiplied, RotationSpeed, BreakingSpeed, FallDamageProtectionTime);
        for (int i = 0; i < Enum.GetValues(typeof(WeaponClass)).Length; i++)
        {
            player.SetupResistance(wpnClass[i], Resistances[i]);
        }
        player.SetupHotbar(player.GetInventorySize(),0);
        body.mass = player.Weight / 100f;
        jumpSys = new JumpSystem(AllowedJumps, 0,MidAirJumpsMultiplyer);
        for (int i = 0; i < FindInventoryItemsInLibary.Length; i++)
        {
            player.AddItem(AllLibary.ItemLibary.SearchLibaryForTemplete(FindInventoryItemsInLibary[i]));
        }
        for (int i = 0; i < FindAttributeInLibary.Length; i++)
        {
            if (ApplyAttributesRandomly && Methods.RandomValue(ApplyAttributesRNGrate) < ApplyAttributesRNGrate) //Apply attributes
            {
                player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(FindAttributeInLibary[i]));
            }
            else if (!ApplyAttributesRandomly)
            {
                player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(FindAttributeInLibary[i]));
            }
        }
        body.mass = player.Weight / 100f;
    }
    protected Vector3 interestLocation;
    private void Start()
    {
        worldRun = WorldRun.Instance;
        Init();
        interestLocation = transform.position;
    }
    private void Awake()
    {
        worldRun = WorldRun.Instance;
        Init();
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
                    player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(apply[i].DataString));
                }
                if (apply[i].Request == CommandRequest.Damage)
                {
                    player.Health.DamagePlayer(apply[i].DataFloat[0], (WeaponClass)(int)apply[i].DataFloat[1]);
                }
                if (apply[i].Request == CommandRequest.Knockback)
                {
                    body.AddForce(apply[i].Knockback.GetKnockback(player.Weight, transform.position));
                    body.AddForce(0, apply[i].Knockback.GetYKnockback(player.Weight), 0);
                }
            }
            hurtBox.ClearQueue();
        }
    }
    public string GetName() { return Name; }
    private bool moveGpound = false;
    private bool moveJump = false;
    private bool moveFoward = true;
    private bool moveBackwords = false;
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool moveBreak = false;
    private void Update()
    {
        GetHurtBoxData();
        if (moveJump)
        {
            Jump(isGrounded);
        }
        if (moveGpound)
        {
            Crouching(isGrounded);
        }
        KeyPress();
        if (!player.Health.GetIsAlive())
        {
            Destroy(gameObject);
            return;
        }
        WanderToPoint();
        MoveAtRandom();
    }
    
    private void WanderToPoint()
    {
        if (Vector3.Distance(transform.position, interestLocation) < outline.radius + 1)
        {
            interestLocation = brain.GetInFront(player.Vision);
        }
    }
    private void MoveAtRandom()
    {
        int num = UnityEngine.Random.Range(0, 10001);
        if (num > 100)
        {
            moveFoward = true;
        }
        else
        {
            moveFoward = false;
        }
        if (num < 300)
        {
            moveBackwords = true;
        }
        if (num > 500)
        {
            moveBackwords = false;
        }
        if (num > 8000)
        {
            moveLeft = true;
        }
        if (num < 7000)
        {
            moveLeft = false;
        }
        if (num > 6000 && num < 8000)
        {
            moveRight = true;
        }
        else if (num > 5000)
        {
            moveRight = false;
        }
        if (num > 9999)
        {
            moveGpound = true;
        }
        else
        {
            moveGpound = false;
        }
        if (num > 3000 && num < 6000)
        {
            moveJump = true;
        }
        else
        {
            moveJump = false;
        }
    }
    private void KeyPress()
    {
        if (moveFoward)
        {
            moveSys.HandleKeyInput(MoveStates.OnHold, MovingDirection.Up);
        }
        else
        {
            moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Up);
        }
        if (moveBackwords)
        {
            moveSys.HandleKeyInput(MoveStates.OnHold, MovingDirection.Down);
        }
        else
        {
            moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Down);
        }
        if (moveLeft)
        {
            moveSys.HandleKeyInput(MoveStates.OnHold, MovingDirection.Left);
        }
        else
        {
            moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Left);
        }
        if (moveRight)
        {
            moveSys.HandleKeyInput(MoveStates.OnHold, MovingDirection.Right);
        }
        else
        {
            moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Right);
        }
    }
    /// <summary>
    /// Checks if your grounded.
    /// </summary>
    /// <remarks>
    /// Code partially made from https://discussions.unity.com/t/how-do-i-properly-detect-if-the-player-is-grounded/1545895/5
    /// </remarks>
    private bool CheckGrounded()
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
            FallDamage(isStableOnGround, player.GravityProtectionTime, 100, 10);
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

        if (moveBackwords && !isGrounded)
        {
            body.velocity = new Vector3(body.velocity.x * player.BreakingSpeed, body.velocity.y, body.velocity.z * player.BreakingSpeed);
        }
        Vector3 Speed = Time.fixedDeltaTime * player.Speed * MoveDirection;
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
        player.ApplyStatAdjustments();
    }

    private bool TookDamage = false;
    /// <summary>
    /// Calculates fall damage based on -Body.Velocity.y. <code>(Damage = Gravity * <paramref name="gravProt"/> - <see cref="BaseCharacter.Player.GetGroundPound()"/> - <see cref="BaseCharacter.Player.Jump"/>)/(<paramref name="div"/>)</code>
    /// </summary>
    /// <param name="isGrounded">Is grounded</param>
    /// <param name="gravProt">How many seconds of protection from gravity if moving at GroundPoundSpeed do you get</param>
    /// <param name="div">Devide damage</param>
    private void FallDamage(bool isGrounded, float gravProt, float div = 100, float secondaryThresh = 10)
    {
        if (isGrounded && !TookDamage)
        {
            float damageProt = Gravity * gravProt - player.GroundPound - player.Jump - player.Speed;
            Debug.Log(-FastestFall + $"Threshold: {-damageProt}");
            float damage = Mathf.Max(-FastestFall + damageProt, 0);
            if (damage > secondaryThresh)
            {
                damage = Mathf.Pow(damage, 1.2f);
            }
            damage /= div;
            Debug.Log(damage);
            player.Health.DamagePlayer(damage, WeaponClass.World, false, 1f);
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
    /// <summary>
    /// Crouching
    /// </summary>
    /// <param name="isGrounded"></param>
    private void Crouching(bool isGrounded)
    {
        if (moveGpound && !isGrounded && gPound.CanPound())
        {
            body.velocity = new Vector3(body.velocity.x, -Mathf.Abs(body.velocity.y), body.velocity.z);
            body.AddForce(new Vector3(0, -Mathf.Abs(player.GroundPound), 0), ForceMode.VelocityChange);
        }
        if (isGrounded)
        {
            gPound.Reset();
        }
    }
    /// <summary>
    /// Attempts to jump.
    /// </summary>
    /// <param name="isGrounded"></param>
    /// <returns></returns>
    private float Jump(bool isGrounded, float jump = 1)
    {
        float value = jumpSys.Jump(isGrounded, MoveStates.OnPress);
        jumpDelay = JUMPDELAY + Time.time;
        moveJump = false;
        return value;
    }

}