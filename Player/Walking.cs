using BaseCharacter;
using BaseCharacter.Items;
using BaseCharacter.Movement;
using BaseCharacter.Structual;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Walking : MonoBehaviour
{
    [Header("Colliion and Phyiscs")]
    [SerializeField] private ReachRange reaching;
    [SerializeField] private Vision vision;
    [SerializeField] private Rigidbody body;
    [SerializeField] private HurtBox hurtBox;
    [Header("player Structure data")]
    [SerializeField] private PlayerShadow shadow;
    [SerializeField] private GameObject[] DistanceFeet;
    [SerializeField] private CapsuleCollider playerBody;
    [SerializeField] private ControlsScheme controls;
    [Header("Camera")]
    [SerializeField] private Movement movement;
    [Header("UI elements")]
    [SerializeField] private InvManager invManager;
    [SerializeField] private ScaleUp scaleUp;
    [SerializeField] private ScaleUp HpBar;
    [SerializeField] private GroundPoundUI scaleDown;
    [Header("MessageBoxes")]
    [SerializeField] private MsgBox Health;
    [SerializeField] private MsgBox Money;
    [SerializeField] private MsgBox Ammo;
    [SerializeField] private MsgBox gay;
    
    [Header("Chat Messages")]
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private MsgBox generic;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 2.2f;
    [SerializeField] private float groundContactOffset = 1f;
    [SerializeField] private float maxSlopeAngle = 80f;
    //Movement
    private MovingSystemKeyboard moveSys;
    private JumpSystem jumpSys;
    private GRDPound gPound;
    private AirMovement airMent;

    private Player player;
    private readonly float JUMPDELAY = 0.128f;
    private readonly float GAMEDELAY = 0.1f;
    private float jumpDelay;
    private bool InMenu { get; set; } = false;
    private bool InInventory { get; set; } = false;
    private bool isGrounded;
    private WorldRun worldRun;
    private float Gravity
    {
        get
        {
            return player.GetGravity() * worldRun.Gravity;
        }
    }
    private float FastestFall;
    void Start()
    {
        worldRun = WorldRun.Instance;
        Debug.Log(GetSaveData);
        SlashRegex.SetChatDefaultRegexLimited(SaveData.GetDefaults());
        SaveData.GetAttributesToLibary();
        moveSys = new MovingSystemKeyboard(0.1f,0.6f,0.8f,1.05f,0.38f,0.35f);
        jumpSys = new JumpSystem(3, 1, 0.35f);
        gPound = new GRDPound(1);
        airMent = new AirMovement(1.285f,0.68f,0.28f); //Default air = 0.185f
        int hotbar = 9;
        int ammo = 18;
        int shoe = 2;
        int legs = 2;
        int chest = 1;
        int hands = 2;
        int head = 1;
        int armor = shoe + legs + chest + hands + head;
        int extraItem = 0;
        int invoSlots = hotbar + ammo + armor + extraItem;
        try
        {
            if (SaveData.TryLoadGame(out PlayerSaveData get))
            {
                player = get.GetSavePlayerData();
                player.FillNullInventory();
                transform.position = get.Location;

                List<string> names = get.Inventory;
                foreach (string name in names) Debug.Log(name);
                for (int i = 0; i < names.Count && i < names.Count; i++)
                {
                    if (names[i] == null || names[i] == string.Empty || names[i] == "" || names[i] == " ")
                    {
                        continue;
                    }
                    try
                    {
                        player.AddItem(AllLibary.ItemLibary.SearchLibaryForTemplete(names[i]),i);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Error in finding item, Continue the search {e}");
                        continue;
                    }   
                }
                player.SetReachRange(get.Reach);
                player.AddMoney(get.Money);
                player.SetupMovement(get.SpeedBase, get.JumpBase, get.GroundPoundBase, get.GravityBase, get.RotationSpeedbase, get.BreakingSpeed, get.GravityProtectionTime);
            }
            else
            {
                player = new Player("RPG", "Super cool", 20, 1f, 5000f,30f,1f, invoSlots, 10f);
                (WeaponClass, float)[] resist = new(WeaponClass,float)[Enum.GetValues(typeof(WeaponClass)).Length];
                for (int i = 0; i < resist.Length; i++)
                {
                    resist[i].Item1 = (WeaponClass)i;
                    resist[i].Item2 = 1;
                }
                player.SetupResistances(resist);
                player.SetReachRange(3);
                player.AddMoney(500);
                player.SetupMovement(40f, 24f, 100f, 1f, 0.4f, 0.97f, 1f);
            }
        }
        catch (Exception e)
        {
            Debug.LogAssertion($"An error occured whlie reading Save data: {e}");
            player = new Player("RPG", "Super cool", 20, 1f, 5000f, 30f, 1f, invoSlots, 10f);
            (WeaponClass, float)[] resist = new (WeaponClass, float)[Enum.GetValues(typeof(WeaponClass)).Length];
            for (int i = 0; i < resist.Length; i++)
            {
                resist[i].Item1 = (WeaponClass)i;
                resist[i].Item2 = 1;
            }
            player.SetupResistances(resist);
            player.SetReachRange(3);
            player.AddMoney(500);
            player.SetupMovement(40f, 24f, 100f, 1f, 0.4f, 0.92f, 1f);
        }
        player.SetupHotbar(hotbar, 0);
        invManager.SetupInventorySize(player.GetInventorySize(), player.GetHotbarSize(), 0, 0.75f, 85f, 3, 0.135f, 50f,0.5f,80f);
        List<int> hpInfo = player.Health.GetHPInfo();

        body = GetComponent<Rigidbody>();
        //UI control
        Health.SetupHealthUI(hpInfo[0], hpInfo[1], 5.2f, 9.01f);
        //Text
        Money.SetText(player.GetMoney(),true);
        //Bubbles
        reaching.SetReach(player.GetReach());
        vision.SetSize(player.Vision);
        SetLoadData(true);
        //Inventory
        StartSpeedRun();

        //UI scaling
        scaleDown.SetupScale(Mathf.Max(Mathf.Abs(Gravity * 2f),player.GroundPoundBase.Max * 4.5f,98));
        scaleUp.SetupScale(player.JumpBase.Max * 80f);
        player.ScrollItem(0);
        // Inventory UI visuals.
        invManager.RefreshFullInventory(player);
        invManager.SetSelectedItem(player.GetHotbarSlot());
        invManager.InventoryToggle();
        invManager.RefreshFullInventory(player);
        invManager.InventoryToggle();
        body.mass = player.Weight / 100f;
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
                    body.AddForce(0,apply[i].Knockback.GetRawKnockback().y,0);
                    //Debug.Log(apply[i].Knockback.GetKnockback(player.Weight, transform.position));
                }
            }
            hurtBox.ClearQueue();
        }
    }
    private float timeGameDelay;
    public Vector3 GetCurrentRotationFromMovement()
    {
        Vector3 direct = movement.GetRotation();
        Vector3 currentVelocity = body.velocity;
        Vector3 delta = moveSys.GetSimpleMvmDeltas(isGrounded);
        delta = delta.normalized;
        Vector3 MoveDirection = Quaternion.Euler(0, 90, 0) * direct * delta.z + Quaternion.Euler(0, 180, 0) * direct * delta.x;
        delta = new Vector3(MoveDirection.x, 0, MoveDirection.z);
        float horizontalMagnitude = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
        Vector3 rotatedMovement = delta * horizontalMagnitude;

        rotatedMovement.y = Mathf.Max(currentVelocity.y, 0);
        return rotatedMovement;
    }
    public Vector3 GetCurrentRotationFromMouse()
    {
        Vector3 direct = movement.GetRotation();
        Vector3 delta = body.velocity;
        Vector3 MoveDirection = Quaternion.Euler(0, 90, 0) * direct * delta.z + Quaternion.Euler(0, 180, 0) * direct * delta.x;
        return MoveDirection;
    }
    void Update()
    {
        InInventory = invManager.GetFullInventoryOpen();
        InventoryItem item = player.GetInventoryItemCurrentHotbar();
        if(item.GetPassiveData(player))
        {
            if (InInventory)
            {
                invManager.RefreshFullInventory(player);
            }
            else
            {
                invManager.RefreshHotbarOnly(player);
            }
        }
        if (Input.GetKey(controls.interact))
        {
            reaching.AddItems(player);
        }
        if (timeGameDelay < Time.time)
        {
            player.CheckPassive();
            body.mass = player.Weight / 100f;
            timeGameDelay = Time.time + GAMEDELAY;
            if (InInventory)
            {
                invManager.RefreshFullInventory(player);
            }
            else
            {
                invManager.RefreshHotbarOnly(player);
            }
        }
        Ammo.SetAmmo(item);
        if (Input.GetKeyDown(controls.primaryFire) && !InMenu)
        {
            ShootPress(item);
        }
        if (Input.GetKeyUp(controls.primaryFire) && !InMenu)
        {
            ShootRelease(item);
        }
        isGrounded = CheckGrounded();
        GetHurtBoxData();
        KeyCheck();
        float jump = 0;
        if (jumpDelay < Time.time)
        {
            jump = Jump(isGrounded);
        }
        if (jump != 0 && !isGrounded)
        {
            //scaleUp.AddScaleUI(Mathf.Pow(jump * player.GetJump(),2));
            gPound.Reset();
            if (controls.GetIsAMovementKeyPressed())
            {
                body.velocity = GetCurrentRotationFromMovement();
            }
            else
            {
                body.velocity = new Vector3 (body.velocity.x,Mathf.Max(body.velocity.y,0),body.velocity.z);
            }
        }
        Crouching(isGrounded);
        body.AddRelativeForce(new Vector3(0, jump * player.Jump, 0), ForceMode.VelocityChange);
        scaleDown.SetScaleUI(body.velocity.y);
        if (Input.GetKey(controls.primaryFire) && !InMenu && !InInventory)
        {
            ShootHold(item,MouseClick.Left);
            invManager.SetCount(player.GetHotbarSlot(), item.GetHeldAmountString());
        }
        if (Input.GetKey(controls.secondaryFire) && !InMenu && !InInventory)
        {
            ShootHold(item, MouseClick.Right);
            invManager.SetCount(player.GetHotbarSlot(), item.GetHeldAmountString());
        }
        if (Input.GetKey(controls.reload) && !InMenu && !InInventory)
        {
            Reload(item);
        }
        if (Input.GetKeyDown(controls.throwItem) && !Input.GetKey(KeyCode.LeftControl)) { ThrowItem(false); }
        if (Input.GetKeyDown(controls.throwItem) && Input.GetKey(KeyCode.LeftControl)) { ThrowItem(true); }
        HotbarKeys();
        UtilizeInventory();
        ApplyEffects();
        ChatBox();
        Health.SetHP(player.Health.GetHPInfo()[0]);
        //HpBar.HpAdjust(10f,player.GetHPInfo()[0],1f);
        
    }
    private void ApplyEffects()
    {
        player.ApplyFireDamage();
        player.ApplyCrying();
        player.ApplyRegeneration();
        player.ApplyFlytation();
    }
    private void ChatBox()
    {
        if (Input.GetKeyDown(controls.OpenChat) && !InInventory && InMenu == false)
        {
            InMenu = true;
            chatManager.OpenBox();
            chatManager.SelectInputField();
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyDown(controls.CommandChat) && InMenu == false)
        {
            InInventory = false;
            invManager.CloseInventoryOpenHotbar();
            InMenu = true;
            chatManager.OpenBox();
            chatManager.SelectInputField();
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && InMenu == true && !InInventory)
        {
            chatManager.ScrollMsgs(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && InMenu == true && !InInventory)
        {
            chatManager.ScrollMsgs(1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //SlashRegex.GetSlashSearchType(text: chatManager.GetInputText(), matches: out MatchCollection commands);
            chatManager.GetInputText().TrimEnd();
            SlashRegex.GetChatBoxRegex(text: chatManager.GetInputText(), inventorySize: player.GetInventorySize(), out List<string> attributes, out List<AddItemRequest> items, out List<string> msgs, out bool clear, out float jump);
            string text = string.Concat(msgs.ToArray());
            msgs.Clear();
            Debug.Log(text);
            if (jump != 0)
            {
                body.AddRelativeForce(new Vector3(0, jump, 0), ForceMode.VelocityChange);
            }
            if (clear)
            {
                player.FillNullInventory();
            }
            player.ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(attributes.ToArray()));
            for (int i = 0; i < items.Count; i++)
            {
                player.AddItem(items[i].GetItem());
            }
            chatManager.ClearAndCloseTextBox();
            Cursor.lockState = CursorLockMode.Locked;
            invManager.RefreshFullInventory(player);
            InMenu = false;
            chatManager.AddText(text, 2.2f);
            body.mass = player.Weight / 100f;
        }
        if (Input.GetKeyDown(KeyCode.Period) && !InInventory && InMenu == false)
        {
            SaveData.SaveGame(player, new WorldLocation(Methods.GetCurrentSceneName(), transform.position));
        }
        if (Input.GetKeyDown(KeyCode.Comma) && !InInventory && InMenu == false)
        {
            SaveData.DeleteSave();
        }
    }
    private void ThrowItem(bool max)
    {
        InventoryItem item;
        if (max)
        {
            item = player.ThrowItems(player.GetHotbarSlot());
        }
        else
        {
            item = player.ThrowItem(player.GetHotbarSlot());
        }
        GameObject objection = Instantiate(new GameObject(),transform.position,transform.rotation);
        BoxCollider box = objection.AddComponent<BoxCollider>();
        box.includeLayers = 9;
        box.size = player.GetInventoryItemCurrentHotbar().Size;
        objection.AddComponent<MeshFilter>();
        objection.AddComponent<MeshRenderer>().material = (player.GetInventoryItemCurrentHotbar().GetGenericMaterial());
        Rigidbody tempbd = objection.AddComponent<Rigidbody>();
        Blocks blocks = objection.AddComponent<Blocks>();
        blocks.SetupBox(item, item.Weight * 10);
        tempbd.freezeRotation = true;
        
        tempbd.AddForce(transform.rotation.eulerAngles.normalized * player.GetReach() * 100);
    }
    private void HotbarKeys()
    {
        if (!InMenu)
        {
            if (Input.GetKeyDown(controls.slot1))
            {
                player.SetHotbarSlot(0);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot2))
            {
                player.SetHotbarSlot(1);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot3))
            {
                player.SetHotbarSlot(2);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot4))
            {
                player.SetHotbarSlot(3);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot5))
            {
                player.SetHotbarSlot(4);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot6))
            {
                player.SetHotbarSlot(5);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot7))
            {
                player.SetHotbarSlot(6);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot8))
            {
                player.SetHotbarSlot(7);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot9))
            {
                player.SetHotbarSlot(8);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
            if (Input.GetKeyDown(controls.slot10))
            {
                player.SetHotbarSlot(9);
                invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
            }
        }
    }
    private void UtilizeInventory()
    {
        if (Input.GetKeyDown(controls.Inventory) && InMenu == false)
        {
            invManager.RefreshFullInventory(player);
            invManager.InventoryToggle();
            InInventory = invManager.GetFullInventoryOpen();
            if (InInventory)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (Input.GetKeyDown(controls.moveKey) && InMenu == false)
        {
            int sel = player.GetPendingItemAndClear();
            int hotbarslot = player.GetHotbarSlot();
            Debug.Log(sel + " was previously selected");
            if (sel < 0)
            {
                player.SelectItem(hotbarslot);
                invManager.SelectItem(hotbarslot);
            }
            else
            {
                Debug.Log($"Swapped {sel} & {hotbarslot}");
                // Perform the actual swap
                player.SwapItem(sel, hotbarslot);
                //Update textures
                invManager.RemoveSelectItem(hotbarslot);
                invManager.RefreshFullInventory(player);
                invManager.SetSelectedItem(player.GetHotbarSlot());
            }
        }
        int?[] pend = invManager.GetPending();
        if (pend[0] == null || pend[1] == null)
        {

        }
        else
        {
            int[] swap = new int[2];
            swap[0] = (int)pend[0];
            swap[1] = (int)pend[1];
            Debug.Log($"Swapped {pend[0]} & {pend[1]}");
            player.GetInventoryItem((int)pend[0]).GetIsDuplication(true, player.GetInventoryItem((int)pend[1]));
            // Perform the actual swap
            player.SwapItem(swap[0], swap[1]);
            //Update textures
            invManager.RemoveSelectItem(swap[0]);
            invManager.RemoveSelectItem(swap[1]);
            invManager.RefreshFullInventory(player);
            invManager.SetSelectedItem(player.GetHotbarSlot());
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            player.ScrollItem(-1);
            invManager.SetSelectedItem(player.GetHotbarSlot(),player.GetInventoryItemCurrentHotbar().GetName());
        }
        if (scroll < 0f)
        {
            player.ScrollItem(1);
            invManager.SetSelectedItem(player.GetHotbarSlot(), player.GetInventoryItemCurrentHotbar().GetName());
        }
    }
    private void ShootPress(InventoryItem item)
    {
        
    }
    private void ShootRelease(InventoryItem item)
    {

    }
    private void ShootHold(InventoryItem item, MouseClick click)
    {
        if (item.GetItemType() == ItemType.Weapon)
        {
            Weapon rockTMP = item.GetItem<Weapon>();
            float speed = Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y) + Mathf.Abs(body.velocity.z);
            //Debug.Log($"canFire: {rockTMP.GetCanFire(false)} and is standered {rockTMP.WeaponDesign == WeaponDesign.Standered}");
            if (rockTMP.WeaponDesign == WeaponDesign.Standered && rockTMP.GetCanFire(true))
            {
                rockTMP.ConsumeAmmo(1);
                rockTMP.ApplyAttackDelay();
                if (!rockTMP.UsingPattern)
                {
                    RaycastHit hit = movement.GetRayPoint(rockTMP, player.Aiming);
                    Vector3 offset = GetOffsetToSelf(rockTMP.GetProjectile((int)click).GetSize() + hurtBox.GetSize());
                    Debug.DrawLine(transform.position, hit.point, Color.blue, 6f);
                    Projectile projectile = rockTMP.GetProjectile((int)click);
                    GameObject tempBullet = Instantiate(projectile.SphereicalObject);
                    tempBullet.GetComponent<MovingProjectile>().SetupProjectile(projectile, rockTMP.GetWeaponClass(), player.GetName());
                    tempBullet.transform.position = offset;
                    Vector3 direction = (hit.point - offset).normalized;
                    tempBullet.GetComponent<Rigidbody>().AddForce(speed * direction);
                    tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetSpeed() * direction);
                    tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetYeet() * new Vector3(0, 1, 0));
                }
                else
                {
                    Vector2[] pattern = rockTMP.GetBulletPattern();
                    Vector3 forward = movement.GetRotation();
                    Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
                    Vector3 up = Vector3.Cross(forward, right);
                    Debug.Log("Making bullets");
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        RaycastHit hit = movement.GetRayPoint(rockTMP, player.Aiming);
                        Vector3 offset = GetOffsetToSelf(rockTMP.GetProjectile((int)click).GetSize());
                        Debug.DrawLine(offset, hit.point, Color.blue, 6f);
                        Projectile projectile = rockTMP.GetProjectile((int)click);
                        GameObject tempBullet = Instantiate(projectile.SphereicalObject);
                        tempBullet.GetComponent<MovingProjectile>().SetupProjectile(projectile,rockTMP.GetWeaponClass(),player.GetName());
                        tempBullet.transform.position = offset;
                        Vector3 direction = (hit.point - offset).normalized;
                        tempBullet.GetComponent<Rigidbody>().AddForce(speed * direction);
                        tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetSpeed() * direction);
                        tempBullet.GetComponent<Rigidbody>().AddForce(projectile.GetYeet() * new Vector3(0, 1, 0));
                    }
                   
                }
            }
        }
    }
    private void Reload(InventoryItem item)
    {
        if (item.GetItemType() == ItemType.Weapon)
        {
            Weapon rockTmp = item.GetItem<Weapon>();
            if (rockTmp.AmmoHold == AmmoHoldDesign.SingleBullet || rockTmp.AmmoHold == AmmoHoldDesign.Magazine || rockTmp.AmmoHold == AmmoHoldDesign.CustomReloadMagazine)
            {
                Debug.Log("start reloading");
                rockTmp.ActivatePassiveReload();
            }
        }
    }
    public Vector3 GetOffsetToSelf(float size)
    {
        Ray placementRay = movement.GetCamera().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        float spawnForwardOffset = Mathf.Max((Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.z) + Mathf.Abs(body.velocity.y)) / 12f, 0.05f + size);
        float lookingArea = movement.LookingYDirection();
        if (lookingArea > 0 && lookingArea < 100f) //0-90 value
        {
            lookingArea /= 70f;
        }
        spawnForwardOffset += Mathf.Lerp(0.2f, 0.8f + playerBody.height, lookingArea); //Adjust the value in the middle
        return placementRay.origin + placementRay.direction * spawnForwardOffset;
    }

    /// <summary>
    /// Handle Keyboard inputs for movement.
    /// </summary>
    private void KeyCheck()
    {
        if (!InMenu)
        {
            if (Input.GetKeyDown(controls.moveUp))
            {
                moveSys.HandleKeyInput(MoveStates.OnPress, MovingDirection.Up);
            }
            if (Input.GetKeyDown(controls.moveDown))
            {
                moveSys.HandleKeyInput(MoveStates.OnPress, MovingDirection.Down);
            }
            if (Input.GetKeyDown(controls.moveLeft))
            {
                moveSys.HandleKeyInput(MoveStates.OnPress, MovingDirection.Left);
            }
            if (Input.GetKeyDown(controls.moveRight))
            {
                moveSys.HandleKeyInput(MoveStates.OnPress, MovingDirection.Right);
            }
            if (Input.GetKeyUp(controls.moveUp))
            {
                moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Up);
            }
            if (Input.GetKeyUp(controls.moveDown))
            {
                moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Down);
            }
            if (Input.GetKeyUp(controls.moveLeft))
            {
                moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Left);
            }
            if (Input.GetKeyUp(controls.moveRight))
            {
                moveSys.HandleKeyInput(MoveStates.OnRelease, MovingDirection.Right);
            }
        }
        else
        {
            moveSys.HandleKeyInput(MoveStates.None, MovingDirection.Up);
            moveSys.HandleKeyInput(MoveStates.None, MovingDirection.Down);
            moveSys.HandleKeyInput(MoveStates.None, MovingDirection.Left);
            moveSys.HandleKeyInput(MoveStates.None, MovingDirection.Right);
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
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, groundCheckDistance + playerBody.height/2f);
        for (int i = 0; i < hits.Length; i++)
        {
            // Skip if collider hits self
            if (hits[i].collider == playerBody)
            {
                continue;
            }
            if (Mathf.Approximately(hits[i].distance ,0))
            {
                continue;
            }
            // Check if we're on stable ground
            float angle = Vector3.Angle(Vector3.up, hits[i].normal);
            bool isStableOnGround = angle <= maxSlopeAngle;
            FallDamage(isStableOnGround, player.GravityProtectionTime,100,30);
            return isStableOnGround;
        }
        return false;

    }
    /// <summary>
    /// Crouching
    /// </summary>
    /// <param name="isGr+ounded"></param>
    private void Crouching(bool isGrounded)
    {
        if (Input.GetKeyDown(controls.crouch) && !isGrounded && gPound.CanPound() && !InMenu)
        {
            body.velocity = new Vector3(body.velocity.x,-Mathf.Abs(body.velocity.y), body.velocity.z);
            body.AddForce(new Vector3(0,-Mathf.Abs(player.GroundPound),0),ForceMode.VelocityChange);
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
        if (Input.GetKeyDown(controls.jump) && !InMenu)
        {
            float value = jumpSys.Jump(isGrounded, MoveStates.OnPress);
            scaleUp.AddScaleUI(value * player.Jump * jump * Mathf.Abs(Gravity));
            jumpDelay = JUMPDELAY + Time.time;
            return value;
        }
        if (Input.GetKeyUp(controls.jump) || InMenu)
        {
            jumpDelay = JUMPDELAY + Time.time;
            return jumpSys.Jump(isGrounded, MoveStates.OnRelease);
        }
        return 0;
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
        if (Input.GetKey(controls.moveLeft))
        {
            airMent.DirectionChangePress(player.GetRotationSpeed(1));
            dirPressed = true;
        }
        if (Input.GetKey(controls.moveRight))
        {
            airMent.DirectionChangePress(player.GetRotationSpeed(1));
            dirPressed = true;
        }
        if (!dirPressed)
        {
            airMent.DirectionalChangeNoPress(player.GetRotationSpeed(0.33f));
        }
        Vector3 delta = moveSys.GetSimpleMvmDeltas(isGrounded);
        Vector3 direct = movement.GetRotation();
        Vector3 forwardDirection = new Vector3(direct.x, 0, direct.z).normalized;
        Vector3 MoveDirection = direct * delta.z + Quaternion.Euler(0, 90, 0) * direct * delta.x;
        float horizontalMagnitude = new Vector3(body.velocity.x, 0, body.velocity.z).magnitude;
        
        if ((Input.GetKey(controls.breaking) || Input.GetKey(controls.breaking2))&& !isGrounded)
        {
            body.velocity = new Vector3(body.velocity.x * player.BreakingSpeed, body.velocity.y,body.velocity.z * player.BreakingSpeed) ;
        }
        if (Input.GetKey(controls.moveDown))
        {
            body.velocity = new Vector3(body.velocity.x * 0.98f, body.velocity.y, body.velocity.z * 0.98f);
        }
        Vector3 Speed = Time.fixedDeltaTime * player.GetSpeed() * MoveDirection;
        Vector3 Rotation = airMent.GetDirection(1) * Speed;
        body.drag = isGrounded ? airMent.GroundDrag : airMent.AirDrag;
        body.AddRelativeForce(Speed, ForceMode.VelocityChange);
        if (!isGrounded)
        {
            body.AddRelativeForce(Rotation, ForceMode.VelocityChange);
            TookDamage = false;
        }
        body.AddForce(new Vector3(0, Gravity, 0),ForceMode.Acceleration);
        scaleUp.ApplyScale(Gravity);
        //Debug.Log($"Body.Velocity {body.velocity}");
        player.ApplyStatAdjustments();
    }
    
    private bool TookDamage = false;
    /// <summary>
    /// Calculates fall damage based on -Body.Velocity.y. <code>(Damage = Gravity * <paramref name="gravProt"/> - <see cref="BaseCharacter.Player.GroundPound"/> - <see cref="BaseCharacter.Player.Jump"/>)/(<paramref name="div"/>)</code>
    /// </summary>
    /// <param name="isGrounded">Is grounded</param>
    /// <param name="gravProt">How many seconds of protection from gravity if moving at GroundPoundSpeed do you get</param>
    /// <param name="div">Devide damage</param>
    private void FallDamage(bool isGrounded, float gravProt, float div = 100, float secondaryThresh = 200, float div2 = 75)
    {
        if (isGrounded && !TookDamage)
        {
            float damageProt = Gravity * gravProt - player.GroundPound - player.Jump - player.GetSpeed();
            Debug.Log(-FastestFall + $"Threshold: {-damageProt}");
            float damage = Mathf.Max(-FastestFall + damageProt, 0);
            if (damage > secondaryThresh)
            {
                damage /= div2;
            }
            else
            {
                damage /= div;
            }
            Debug.Log(damage);
            player.Health.DamagePlayer(damage,WeaponClass.World,false,1f);
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
