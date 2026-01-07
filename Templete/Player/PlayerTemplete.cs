using BaseCharacter.Entity;
using BaseCharacter;
using BaseCharacter.Movement;
using BaseCharacter.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
public class CharacterTemplete : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] Texture icon;
    [SerializeField][Tooltip("Name of the character. Used in search via the AllLibary class. This will not override a Player's name unless specified otherwise")] private string Name;
    [SerializeField][TextArea(3, 8)] private string Description;
    [Header("Health")]
    [SerializeField][Min(1)] private int MaxHealth = 50;
    [SerializeField, Range(0.01f, 1f)] private float StartingHealthPercent = 1f;
    [SerializeField] private float IncrimentPerLevelHealth = 0.5f;
    [SerializeField] private WeaponClass[] resistances;
    [SerializeField][Range(0f, 2f)] private float[] resistancePower;
    [Header("Inventory")]
    [SerializeField][Min(0)][Tooltip("Not used in entites. Strictly used by the Player")] private int HotbarSize = 9;
    [SerializeField][Min(1)][Tooltip("How many InventoryItems can you hold")] private int InventorySize = 35;
    [Header("Weight")]
    [SerializeField][Min(100f)][Tooltip("1000 weight = 10 KG")] private float Weight = 5000f;
    [Header("Speed")]
    [SerializeField] private float Speed = 60f;
    [SerializeField][Min(0f)] private float incrementByLevelSpeed = 0.1f;
    [SerializeField][Range(0.00f,1.00f)][Tooltip("Your speed in the air")] private float AcclerationSpeed = 0.4f;
    [Header("Move System Keyboard")]
    [SerializeField][Min(0.002f)] private float timeToSpeedUp = 0.05f;
    [SerializeField][Min(0.002f)] private float timeToSpeedDown = 0.5f;
    [SerializeField][Min(0.001f)] private float Clamp = 0.8f;
    [SerializeField][Min(0.2f)][Tooltip("How much faster do you move holding foward")] private float fowardMultiplier = 1f;
    [SerializeField][Min(0f)][Tooltip("How is your speed adjusted while holding Foward/Backwords in the air.")] private float AirealFowardMultipler = 0.4f;
    [SerializeField][Min(0f)][Tooltip("How is your speed adjusted while Left/Right in the air.")] private float AirealSideMultipler = 0.36f;
    [SerializeField][Range(0.7f, 1f)][Tooltip("How much of your velocity is multiplied when holding backwords.")] private float BreakingSpeed = 0.92f;
    [Header("Aireal Movement")]
    [SerializeField][Tooltip("How fast do you accelerate in the air")] private float AccelerationMultiplier = 1.285f;
    [SerializeField] private float GroundDrag = 1f;
    [SerializeField] private float AirDrag = 0.128f;
    [SerializeField][Min(0f)][Tooltip("How much stronger is your foward movement in the air")] private float AirealPowerFoward = 1f;
    [SerializeField][Min(0f)][Tooltip("How much stronger is your side movement in the air")] private float AirealPowerSide = 0.8f;
    [Header("Jump")]
    [SerializeField][Tooltip("How high you jump")] private float JumpForce = 20f;
    [SerializeField][Min(0)][Tooltip("The amount of jumps you have")] private int AmountOfJumps = 1;
    [SerializeField][Min(0)][Tooltip("Aireal jumps when you run out of normal jumps. These cannot be used on the ground")] private int AmountOfMurderJumps;
    [SerializeField][Tooltip("Multiplier of jump force")] private float MurderJumpStrength;
    [SerializeField, Range(0.05f, 0.250f)][Tooltip("Delay before your next jump")] private float JumpDelay = 0.128f;
    [SerializeField][Min(0f)] private float incrementByLevelJump = 0.1f;
    [Header("GroundPound")]
    [SerializeField][Min(0)] private int AmountOfGroundPounds= 1;
    [SerializeField][Min(0f)] private float GpoundForce = 100;
    [SerializeField][Min(0f)] private float incrementByLevelGpounds = 0.1f;
    [Header("Vision")]
    [SerializeField] private CameraMode CameraMode = CameraMode.FirstPerson;
    [SerializeField][Min(0f)] private float Vision = 100f;
    [SerializeField] private float incrementByLevelVision = 0.1f;
    [Header("Aim")]
    [SerializeField][Tooltip("The higher the number, the more accurate you are.")] private float Aim = 100f;
    [SerializeField] private float incrementByLevelAim = 0.1f;
    [Header("Gravity")]
    [SerializeField][Tooltip("Leave a 1 by default")] private float gravityStrength = 1f;
    [SerializeField] private float gravityProtectionTime = 0.5f;

    private MovingSystemKeyboard move;
    private GRDPound gRDPound;
    private JumpSystem jumpSystem;
    private AirMovement airmovement;
    private StatHealth health;
    private Stat speed;
    private Stat jump;
    private Stat gPound;
    private Stat vision;
    private Stat aim;
    private Player player;
    public Character Character { get { return Init(); } }
    private Character Init()
    {
        move = new MovingSystemKeyboard(timeToSpeedUp, timeToSpeedDown, Clamp, fowardMultiplier, AirealFowardMultipler, AirealSideMultipler);
        gRDPound = new GRDPound((byte)AmountOfGroundPounds);
        jumpSystem = new JumpSystem(AmountOfJumps, AmountOfMurderJumps, MurderJumpStrength);
        airmovement = new AirMovement(AccelerationMultiplier, GroundDrag, AirDrag, AirealPowerSide, AirealPowerFoward);
        player = new Player(Name,Description,InventorySize,Weight,0);
        player.SetupHotbar(HotbarSize);
        health = new StatHealth("Health", MaxHealth, IncrimentPerLevelHealth, MaxHealth * StartingHealthPercent);
        health.SetResistance(resistances, resistancePower);
        speed = new Stat("Speed", Speed, incrementByLevelSpeed);
        jump = new Stat("Jump", JumpForce, incrementByLevelSpeed);
        gPound = new Stat("Gpound",GpoundForce, incrementByLevelSpeed);
        vision = new Stat("Vision", Vision, incrementByLevelSpeed);
        aim = new Stat("Aim", Aim, incrementByLevelAim);
        player.SetupStats(health, speed, jump, gPound, vision,aim);
        player.SetupMovement(gravityStrength,AcclerationSpeed,BreakingSpeed,gravityProtectionTime);
        return new Character(Name, player, move, gRDPound, jumpSystem, airmovement, JumpDelay);
    }

}