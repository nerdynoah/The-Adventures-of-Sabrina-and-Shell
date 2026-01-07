using UnityEngine;
/// <summary>
/// Holds all of the Enumorators and Severeal methods protaining to the Camera and Player defaults.
/// </summary>
public class Enums
{
    #region Movement
    /// <summary>
    /// Keyboard movement inputs
    /// </summary>
    public enum MovingDirection
    {
        Up, Down, Left, Right
    }
    /// <summary>
    /// Pressing key, holding key, Releasing key, not pressing any key,
    /// </summary>
    public enum MoveStates
    {
        None = 0,
        OnPress = 1,
        OnHold = 2,
        OnRelease = 3,
    }
    #endregion
    #region Vision, Quests
    /// <summary>
    /// The stagge of the quest
    /// </summary>
    public enum QuestStage
    {
        Inactive,       // Not started
        Unavailable,     // Not yet available
        Active,         // Currently in progress
        Completed,      // Finished but not rewarded
        Failed,         // Failed conditions
        Rewarded        // Completed and rewarded
    }
    /// <summary>
    /// Entity Algorithm based off of Vision.
    /// </summary>
    public enum VisionType
    {
        Default,
        Air,
        Long,
        Ideal,
    }
    #endregion
    #region Attributes
    /// <summary>
    /// Attributes
    /// </summary>
    public enum Attributes
    {
        None = 0,
        /// <summary>
        /// Causes Poison Damage. 
        /// </summary>
        Poison = 1,
        /// <summary>
        /// Causess regen
        /// </summary>
        Regeneration,
        /// <summary>
        /// Lets you float around a bit.
        /// </summary>
        Flytation,
        /// <summary>
        /// Lowers your vision for a bit
        /// </summary>
        Blindness,
        /// <summary>
        /// Lets you heal a percent of your health back.
        /// </summary>
        Vampiric,
        /// <summary>
        /// You got shot with the grappler
        /// </summary>
        Grappled,
        /// <summary>
        /// Heals the Player
        /// </summary>
        Healing,
        /// <summary>
        /// Decreases Accuacy.
        /// </summary>
        Crying,
        /// <summary>
        /// Decreases damage from other players.
        /// </summary>
        Wounded,
        /// <summary>
        /// Alter Speed.
        /// </summary>
        Speed,
        /// <summary>
        /// Jump alter,
        /// </summary>
        Jump,
        /// <summary>
        /// Adjust your size.
        /// </summary>
        Bigger,

    }
    #endregion
    #region Weapons
    /// <summary>
    /// A description of the weapon design
    /// </summary>
    public enum WeaponDesign
    {
        Custom = 0,
        /// <summary>
        /// Fire shots by holding down left click.
        /// </summary>
        Standered = 1,
        SingleFire = 2,
        /// <summary>
        /// Fire a charged shot.
        /// </summary>
        Charged = 4,
    }
    public enum AmmoHoldDesign
    {
        SingleBullet,
        Magazine,
        CustomReloadMagazine,
        Endless,
    }
    public enum ChargeDesign
    {
        None,
        Charge,
        OverCharge,
    }
    public enum FirePatternDesign
    {
        SingleFire,
        ColmRowFire,
    }
    /// <summary>
    /// Projectile Type.  Not used for anything as of this very momment. UwU
    /// </summary>
    public enum ProjectileTypes
    {
        None,
        Melee,
        GenericExplosive,
        GenericBullet,
        GenericCast,
        Unique,
    }

    /// <summary>
    /// A list of weapon types. Used for resistances
    /// </summary>
    public enum WeaponClass
    {
        /// <summary>
        /// Use for melee weapons. Weapons that summon a hit box in front of you for a short time are melee weapons.
        /// </summary>
        Melee = 0,
        /// <summary>
        /// Anythign that summons a bullet and shoots it.
        /// </summary>
        Bullet = 1,
        /// <summary>
        /// Weapons that cause a projectile hitbox to change on impact or have an abonromal radius. Tend to cover large areas.
        /// </summary>
        Explosive = 2,
        /// <summary>
        /// Status effects, Lingering effects, Things like "Fire Damage, Poison, Etc..." are considered Magic.
        /// </summary>
        Magic = 3,
        /// <summary>
        /// Fall damage, Earthquakes, etc... Mainly things not summoned by players.
        /// </summary>
        World = 4,
    }
    public enum ProjectileType
    {

    }
    #endregion
    #region Items and Libary
    public enum SearchFor
    {
        InventoryItem,
        Entities,
        Quests,
        Attributes,
    }
    public enum ConsumableClass
    {
        None = 10,
        Normal = 11,
        Magic = 3,
        Explosive = 2,
    }
    public enum HoldingType
    {
        Single = 0,
        UnlmintedStackable = 1,
        LimitedStackable = 2,
    }
    public enum DuplicateReturn
    {
        True = 1,
        False = 0,
        /// <summary>
        /// Some items got added but the rest need added to a new slot.
        /// </summary>
        Incrimented = 2,
    }
    public enum InventoryAddReturn
    {
        NothingToAdd = 1,
        Sucess = 2,
        Fail = 4,
    }
    /// <summary>
    /// The different <see cref="BaseCharacter.Item"/> instantiations and their similar calls
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Uses <see cref="BaseCharacter.Items.Item"/>
        /// </summary>
        Item,
        /// <summary>
        /// Uses <see cref="BaseCharacter.Items.Weapon"/>
        /// </summary>
        Weapon,
        /// <summary>
        /// Uses <see cref="BaseCharacter.Items.Weapon"/> but for Melee weapons
        /// </summary>
        Melee,
        /// <summary>
        /// Uses <see cref="BaseCharacter.Items.Consumable"/>
        /// </summary>
        Consumable,
        /// <summary>
        /// Uses <see cref="BaseCharacter.Items.Armor"/>, will autoapply to your charature.
        /// </summary>
        Armor,
        /// <summary>
        /// Uses <see cref="BaseCharacter.Items.Projectile"/>
        /// </summary>
        Ammo,
        /// <summary>
        /// Is a replacable Empty item <see cref="BaseCharacter.Items.InventoryItem.GetIsEmptyItem()"/>
        /// </summary>
        Empty = 20
    }
    public enum LibraryObjects
    {
        None = 0,
        AttributeTemplete = 6,
        InventoryItem = 7,
        Entities = 8,
        Quests = 9,
        Character = 10,
    }
    public enum SwitchFromTo
    {
        None,
        Character,
        Inventory,
    }
    public enum RegexSearchType
    {
        None = 0,
        Copy,
        /// <summary>
        /// Whisper a message to a Player
        /// </summary>
        Whisper,
        /// <summary>
        /// Default settings
        /// </summary>
        Default,
        /// <summary>
        /// Clear items in a inventory
        /// </summary>
        Clear = 4,
        /// <summary>
        /// Give things from the <see cref="AllLibary.Libary"/>.
        /// </summary>
        Give = 5,
        //unused commands, only used for /help
        AttributeTemplete = 6,
        InventoryItem = 7,
        Entities = 8,
        Quests = 9,
        LibaryObjects = 10,
        Character = 11,
        //End of unused Commands
        New,
        /// <summary>
        /// Force a <see cref="BaseCharacter.Entity.Player"/> to jump
        /// </summary>
        Jump,
        /// <summary>
        /// List all possible usable things in the <see cref="AllLibary.Libary"/>
        /// </summary>
        List,
        /// <summary>
        /// Help with commands
        /// </summary>
        Help,
        /// <summary>
        /// Cause somthing to die.
        /// </summary>
        Die,
        /// <summary>
        /// Max out the amount of items currently in a <see cref="BaseCharacter.Items.InventorySystem"/>
        /// </summary>
        Max,
        /// <summary>
        /// Switch libary objects of 2 entities.
        /// </summary>
        Switch,
        Order,
    }
    public enum RegexOrderItems
    {
        KeepAsIs,
        Name,
        Type,
        Price,
        Size,
        Weight,
        Amount
    }
    public enum RegexModifier
    {
        None = 1,
        Target = 2, //@
        Amount = 3, //#
    }
    /// <summary>
    /// @ annotation
    /// </summary>
    public enum RegexTarget
    {
        /// <summary>
        /// @
        /// </summary>
        None = 0, //@
        /// <summary>
        /// @M
        /// </summary>
        Me = 1, //@M
        /// <summary>
        /// @L
        /// </summary>
        LookingAt = 2, // @L
        /// <summary>
        /// @D{value}
        /// </summary>
        Distance = 4, // @D
        /// <summary>
        /// @C
        /// </summary>
        Closest = 8, // @C

        //Combinding @CD will give it to the closest person within a distance.
    }
    #endregion
    #region Camera
    /// <summary>
    /// Mouse Movement
    /// </summary>
    public enum RotationAxis
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2,
    }
    /// <summary>
    /// Camera Mode
    /// </summary>
    public enum CameraMode
    {
        FirstPerson = 0,
        ThirdPerson = 1,
        TopDownPerspective = 2,
    }
    /// <summary>
    /// The camera mode
    /// </summary>
    static CameraMode camera = CameraMode.FirstPerson;
    /// <summary>
    /// Get camera mode
    /// </summary>
    /// <returns>CameraMode Value</returns>
    public static CameraMode GetCameraMode()
    {
        return camera;
    }

    /// <summary>
    /// Set the mode based on toggle
    /// </summary>
    /// <param name="toggle">True = Third, False = First</param>
    public static void SetCameraMode(bool toggle)
    {
        if (toggle)
        {
            camera = CameraMode.TopDownPerspective;
        }
        else
        {
            camera = CameraMode.FirstPerson;
        }
    }
    #endregion
    #region Inventory and UI
    public enum ColorChoiseInventory
    {
        Idle,
        IdleSelected,
        Hover,
        HoverSelected,
    }

    public enum ExtraDataType
    {
        /// <summary>
        /// Use if the UI element is to not be displayed
        /// </summary>
        Inactive,
        IsSelf,
        SliderRoles,
        SliderPlayers,
    }
    /// <summary>
    /// Animations Indicator
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Frozen image
        /// </summary>
        Idle = 0,
        /// <summary>
        /// Idle image
        /// </summary>
        IdleAnim = 1,
        /// <summary>
        /// Primary Fire
        /// </summary>
        Shoot,
        /// <summary>
        /// Running out of ammo.
        /// </summary>
        NoAmmo,
        /// <summary>
        /// run out of ammo, no animation
        /// </summary>
        NoAmmoIdle,
        /// <summary>
        /// Reload animaition
        /// </summary>
        Reload,
        /// <summary>
        /// Other
        /// </summary>
        Other,
        /// <summary>
        /// Charge Time
        /// </summary>
        Charge,
        SecondaryIdle,
        SecondaryIdleAnim,
        SecondaryShoot,
        SecondaryNoAmmo,
        SecondaryNoAmmoIdle,
        SecondaryReload,
        SecondaryOther,
        SecondaryCharge,
    }

    #endregion
    #region Queue
    /// <summary>
    /// Queue
    /// </summary>
    public enum QueueData
    {
        Int,
        Bool,
        String,
        Name
    }
    /// <summary>
    /// Use to send commands to the server to request data.
    /// </summary>
    public enum CommandRequest
    {
        /// <summary>
        /// Send an Attributes, 
        /// </summary>
        Attributes = 0,
        /// <summary>
        /// Damage
        /// </summary>
        Damage = 1,
        /// <summary>
        /// Knockback
        /// </summary>
        Knockback = 2,
    }
    #endregion
    #region Save data
    public static void SetLoadData(bool mode)
    {
        GetSaveData = mode;
    }
    /// <summary>
    /// Load the save data or create game.
    /// </summary>
    public static bool GetSaveData { get; private set; } = true;
    #endregion
    #region Classes
    /// <summary>
    /// The main classes of the FPS game
    /// </summary>
    public enum Classes
    {
        None,
        Vampire,
        Bird,
        LeatherBird,
        Elf,
        Human,

    }
    #endregion
    #region Speedrun
    public static float? SpeedRunTime { get; private set; }
    public static void StartSpeedRun()
    {
        SpeedRunTime ??= Time.time;
    }
    #endregion
    #region Health
    ///<summary>
    ///Choose health damage decrease percentage
    /// <list type = "number">
    /// <item>Option: Remove Amount from MaxHealth<code> Health -= (int)((float)value * (float)HealthMax); </code></item>
    /// <item>Option: Remove Amount from current health<code> Health = (int)((float)Health * (float)value);</code></item>
    /// <item>Option: Remove Health by adding Health + Max Heatlh, then devide by 2. Afterwords multiply that by the desired percentage.<code> Health = (int)((float)(((float)Health + (float)HealthMax) / (float)(2)) * value);</code></item>
    /// </list>
    ///</summary>
    public enum HealthDamagePercentage
    {
        /// <summary>
        /// Option: Remove Amount from MaxHealth<code> Health -= (int)((float)value * (float)HealthMax); </code>
        /// </summary>
        MaxHealth = 1,
        /// <summary>
        /// Option: Remove Amount from current health<code> Health = (int)((float)Health * (float)value);</code>
        /// </summary>
        CurrentHealth,
        /// <summary>
        /// Option: Remove Health by adding Health + Max Heatlh, then devide by 2. Afterwords multiply that by the desired percentage.<code> Health = (int)((float)(((float)Health + (float)HealthMax) / (float)(2)) * value);</code>
        /// </summary>
        MaxPlusCurrentHealth,
    }
    #endregion
    #region Clicks
    public enum MouseClick
    {
        Left = 0,
        Right = 1,
        Middle = 2,
    }
    #endregion
    #region Enemy Data
    public enum PathMode
    {
        /// <summary>
        /// Utilize custom data built into the gameobject.
        /// </summary>
        None = 0,
        /// <summary>
        /// Doesn't attack
        /// </summary>
        Peacefull,
        /// <summary>
        /// Doesn't attack unless attacked
        /// </summary>
        Provokative,
        /// <summary>
        /// Doesn't attack unless you get too close
        /// </summary>
        VisionaryProvoke,
        /// <summary>
        /// Uses its sensors to hunt you out.
        /// </summary>
        Hunter,
        /// <summary>
        /// Uses its sensers to attack ANYTHING it passes.
        /// </summary>
        Agreesive,
        /// <summary>
        /// Stays in one area, will only attack when a trigger is pushed.
        /// </summary>
        Gaurd,
    }
    public enum WanderMode
    {
        /// <summary>
        /// Cannot wander.
        /// </summary>
        None = 0,
        /// <summary>
        /// Doesn't move,
        /// </summary>
        Still,
        /// <summary>
        /// Doesn't move, but rotates
        /// </summary>
        StillButRotates,
        /// <summary>
        /// Moves around randomly
        /// </summary>
        RandomMovement,
        /// <summary>
        /// Moves to the closest thing in its sight, will trigger an inspection every time.
        /// </summary>
        SightMovement,
        /// <summary>
        /// Will move to smaller areas if not inspecting.
        /// </summary>
        LessAreaRandommMovement,
    }
    public enum FiveSenses
    {
        /// <summary>
        /// Inspects sounds
        /// </summary>
        Sound,
        /// <summary>
        /// Inspects vision
        /// </summary>
        Vision,
        /// <summary>
        /// Inspects smells
        /// </summary>
        Smells,
        /// <summary>
        /// Inspects touch
        /// </summary>
        Touch,
        /// <summary>
        /// Inspects whats its consuming.
        /// </summary>
        Taste
    }
    #endregion
}