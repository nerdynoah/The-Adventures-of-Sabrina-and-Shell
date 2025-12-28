using BaseCharacter;
using BaseCharacter.Entities;
using BaseCharacter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Enums;
/// <summary>
/// Holds premade <see cref="WeaponTemplete"/>, <see cref="Quest"/>, <see cref="Entities"/>, <see cref="AttributesTemplete"></see>. Use to add your own abilities, roles, etc...
/// </summary>
public class AllLibary : MonoBehaviour
{
    private Quest cashCow;
    private Quest mrFaceClear;
    private Quest finalBoss;
    private readonly Libary libary = new();
    [SerializeField] private WeaponTemplete[] weaponTempletes;
    [SerializeField] private AmmoTemplete[] ammoTempletes;
    [SerializeField] private EntityTemplete[] entityTempletes;
    [SerializeField] private CharacterTemplete[] characterTempletes;

    /// <summary>
    /// Stores Dictionaries of <see cref="InventoryItem"/>, <see cref="AttributesTemplete"/>, <see cref="Quest"/>, <see cref="Character"/>, 
    /// </summary>
    private class Libary
    {
        /// <summary>
        /// A list of weaponsTemplete files
        /// </summary>
        private Dictionary<string,InventoryItem> Inventory { get; set; } = new Dictionary<string, InventoryItem>();
        private List<Quest> Quests { get; set; } = new List<Quest>();
        private Dictionary<string, EntityTemplete> Entities { get; set; } = new Dictionary<string, EntityTemplete>();
        private Dictionary<string, AttributesTemplete> Attributes { get; set; } = new Dictionary<string, AttributesTemplete>();
        private Dictionary<string, Character> Persons { get; set; } = new Dictionary<string, Character>(); 
        /// <summary>
        /// New <see cref="InventoryItem"/> object to a searchable libary.
        /// </summary>
        /// <param name="item">Inventory Item</param>
        /// <summary>
        /// New <see cref="InventoryItem"/> object to a searchable libary.
        /// </summary>
        /// <param name="item">Inventory Item</param>
        public void AddInventoryItem(InventoryItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("Attempted to add null InventoryItem");
                return;
            }
            
            string itemName = item.GetName().ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("Attempted to add InventoryItem with null or empty name");
                return;
            }

            if (Inventory.ContainsKey(itemName))
            {
                Debug.LogWarning($"Already found a object named {itemName}. Names are always lowercased");
                return;
            }

            Debug.Log($"Adding Item {itemName}");
            Inventory.Add(itemName, item);
        }
        public void AddCharacter(params Character[] items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                AddCharacter(item);
            }
        }
        public void AddCharacter(Character item)
        {
            if (item == null)
            {
                Debug.LogWarning("Attempted to add null InventoryItem");
                return;
            }

            string itemName = item.GetName().ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("Attempted to add InventoryItem with null or empty name");
                return;
            }

            if (Inventory.ContainsKey(itemName))
            {
                Debug.LogWarning($"Already found a object named {itemName}. Names are always lowercased");
                return;
            }

            Debug.Log($"Adding Item {itemName}");
            Persons.Add(itemName, item);
        }
        public void AddInventoryItem(params InventoryItem[] items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                AddInventoryItem(item);
            }
        }

        public void AddQuest(Quest quest)
        {
            if (quest == null)
            {
                Debug.LogWarning("Attempted to add null Quest");
                return;
            }

            Quests.Add(quest);
            SortByName();
        }
        public void AddEntities(EntityTemplete entity)
        {
            if (entity == null)
            {
                Debug.LogWarning("Attempted to add null EntityTemplete");
                return;
            }

            string entityName = entity.GetName().ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(entityName))
            {
                Debug.LogWarning("Attempted to add EntityTemplete with null or empty name");
                return;
            }

            if (Entities.ContainsKey(entityName))
            {
                Debug.LogWarning($"Already found a object named {entityName}");
                return;
            }

            Entities.Add(entityName, entity);
        }
        public void AddAttribute(AttributesTemplete value)
        {
            if (value == null)
            {
                Debug.LogWarning("Attempted to add null AttributesTemplete");
                return;
            }

            string attributeName = value.GetName().ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(attributeName))
            {
                Debug.LogWarning("Attempted to add AttributesTemplete with null or empty name");
                return;
            }

            if (Attributes.ContainsKey(attributeName))
            {
                Debug.LogWarning($"Already found a object named {attributeName}");
                return;
            }

            Attributes.Add(attributeName, value);
        }
        public void AddAttribute(params AttributesTemplete[] value)
        {   
            foreach (var attri in value)
            {
                AddAttribute(attri);
            }
        }
        /// <summary>
        /// Get an attribute by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AttributesTemplete GetAttribute(string name)
        {
            name = name.ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(name) || !Attributes.ContainsKey(name))
            {
                Debug.LogWarning($"Attribute '{name}' not found in library");
                return null;
            }
            return Attributes[name];
        }
        /// <summary>
        /// Get WeaponTemplete by the name
        /// </summary>
        /// <param name="name">Name of the WeaponTemplete</param>
        /// <returns>WeaponTemplete</returns>
        public InventoryItem GetInventoryItem(string name)
        {
            name = name.ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(name) || !Inventory.ContainsKey(name))
            {
                Debug.LogWarning($"InventoryItem '{name}' not found in library");
                return null;
            }

            InventoryItem item = Inventory[name];
            Debug.Log($"Getting inventoryItem by {name} -> {item?.GetName()}");
            return item;
        }
        public Character GetCharacter(string name)
        {
            name = name.ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(name) || !Persons.ContainsKey(name))
            {
                Debug.LogWarning($"Character '{name}' not found in library");
                return null;
            }

            Character item = Persons[name];
            Debug.Log($"Getting Character by {name} -> {item?.GetName()}");
            return item;
        }
        public List<string> GetInventoryItemNames() => Inventory.Keys.ToList();
        public List<string> GetEffectNames() => Attributes.Keys.ToList();
        public List<string> GetEntitityNames() => Entities.Keys.ToList();
        public List<string> GetCharacterNames() => Persons.Keys.ToList();
        public List<InventoryItem> GetAllItems() 
        {
            List<InventoryItem> items = Inventory.Values.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                items[i] = new InventoryItem(items[i]);
            }
            return items;
        }
        public List<AttributesTemplete> GetAllEffects() => Attributes.Values.ToList();
        public List<EntityTemplete> GetAllEntites() => Entities.Values.ToList();

        public Quest GetQuest(string name)
        {
            int left = 0;
            int right = Quests.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                int comparison = string.Compare(Quests[mid].GetName(), name, StringComparison.OrdinalIgnoreCase);

                Debug.Log($"Looking for {name}, compared to {Quests[mid].GetName()}");

                if (comparison == 0) // Match found
                {
                    return Quests[mid];
                }
                else if (comparison < 0) // Search right half
                {
                    left = mid + 1;
                }
                else // Search left half
                {
                    right = mid - 1;
                }
            }
            return null;
        }
        public EntityTemplete GetEntities(string name)
        {
            name = name.ToLower().Trim(Methods.charsToTrim);
            if (string.IsNullOrEmpty(name) || !Entities.ContainsKey(name))
            {
                Debug.LogWarning($"Entity '{name}' not found in library");
                return null;
            }
            return Entities[name];
        }
        public List<Quest> GetQuestList()
        {
            return Quests;
        }
        /// <summary>
        /// Sort the list
        /// </summary>
        public void SortByName()
        {
            Quests = Quests.OrderBy(quest => quest.GetName()).ToList();
        }
    }
    /// <summary>
    /// Setup the weapons of the game.
    /// </summary>
    /*
    private void SetupWeapons()
    {
        //Scout
        //Pump Shotgun
        pumpShotgun.SetupWeapon("Pump Shotgun", WeaponClass.Projectile, WeaponDesign.Close, 0.4f, 0.45f, 3.1f, 115f, 1400, true, 0.8f, 6, 6, 5, 0.5f);
        pumpShotgun.SetupKnockback(new Vector3(0f, 0f, 0f), -5f, 0);
        pumpShotgun.SetupProjectile(0f, 0f, 90f, 100f, new Vector3(0.25f, 0.25f, 0.25f), 0.1f, 60f, 1f, 1);
        libary.AddWeaponTemplete(pumpShotgun);
        //Gravity Blast
        gravityBlast.SetupWeapon("Gravity Blast", WeaponClass.Explosive, WeaponDesign.AreaOfDenial, 2.1f, 0.4f, 45, 4f, 50);
        gravityBlast.SetupKnockback(new Vector3(0, 0f, 0f), 1f, -800f);
        gravityBlast.SetupProjectile(-0.05f, 9.8f, 60f, 30f, new Vector3(1.5f, 1.5f, 1.5f), 4.8f, 8f, 0.4f);
        libary.AddWeaponTemplete(gravityBlast);

        //Stricker
        striker.SetupMeleeWeapon("Striker", WeaponClass.Melee, WeaponDesign.RapidFire, 0.2f, 0.185f, 11f, 0.05f, new Vector3(1.2f, 3f, 8f),50);
        striker.SetupKnockback(new Vector3(2f, 15f, 4f), -10f, 25f);
        striker.SetupAttribute(Attributes.Flytation, 9f, 1f, 1000f, true);
        libary.AddWeaponTemplete(striker);
        //Hurler
        //Rocket Launcher
        rocketLauncher.SetupWeapon("Rocket Launcher", WeaponClass.Explosive, WeaponDesign.Rifle, 0.55f, 0.55f, 35f, 0, 8000,true, 0.8f, 8);
        rocketLauncher.SetupKnockback(new Vector3(0, 23f, 3.11f), 15f, 12f);
        rocketLauncher.SetupProjectile(0f, -1f, 20f, 60f, new Vector3(1f, 1f, 1f), 6f, 12f, 0.8f);
        rocketLauncher.SetupAttribute(Attributes.Flytation, 5f, 0.33f, 5f, true);
        libary.AddWeaponTemplete(rocketLauncher);
        
        //Grenade
        grenade.SetupWeapon("Grenade", WeaponClass.Explosive, WeaponDesign.AreaOfDenial, 12f, 0.7f, 99f, 0f, 2099);
        grenade.SetupKnockback(new Vector3(0.1f, 0.105f, 1.15f), 1.005f, 2f);
        grenade.SetupProjectile(50f, -9.8f, 40f, 6.9f, new Vector3(0.46f, 0.46f, 0.46f), 12f, 40f, 5f);
        grenade.SetupAttribute(Attributes.Flytation, 5f, 0.472f, 1000f, true);
        libary.AddWeaponTemplete(grenade);
        //Sniper
        sniper.SetupWeapon("Sniper", WeaponClass.Projectile, WeaponDesign.Persision, 0.8f, 1.6f, 80f, 0f, 6464,false, 2.0835f,3);
        sniper.SetupKnockback(new Vector3(5f, 11f, 0f), 6f, 15f);
        sniper.SetupProjectile(0, -3.2f, 160f, 100f, new Vector3(1.15f, 1.15f, 1.15f), 0.8f, 1025f, 25f, 30);
        sniper.SetupAttribute(Attributes.Wounded, 0.5f, 3.3f, 2f, false);
        sniper.SetupAttribute(Attributes.Crying, 33f, 6f, 0.05f, true);

        libary.AddWeaponTemplete(sniper);

        //Axe
        axe.SetupMeleeWeapon("Axe", WeaponClass.Melee, WeaponDesign.SingleFire, 0.8f, 0.28f, 66f, 0f, new Vector3(1.2f, 2.6f, 10f),1000);
        axe.SetupKnockback(new Vector3(1.5f, 10f, 5f), 15f, 2f);
        axe.SetupAttribute(Attributes.Wounded, 0.8f, 0.5f, 1f, false);
        axe.SetupAttribute(Attributes.Crying, 50f, 1.8f, 0.15f, true);
        libary.AddWeaponTemplete(axe);
        //Revovler
        revolver.SetupWeapon("Revolver", WeaponClass.Projectile, WeaponDesign.Stream, 0.33f, 0.33f, 22f, 6f, 20, true, 0.6f, 10);
        revolver.SetupKnockback(new Vector3(0, 0, 1.8f), 0, 0);
        revolver.SetupProjectile(0, 0, 160f, 100f, new Vector3(0.6f, 0.6f, 0.6f), 0.625f, 90f, 1f, 199);
        libary.AddWeaponTemplete(revolver);
        //Golem
        //Onion Rifle
        onionRifle.SetupWeapon("Onion Rifle", WeaponClass.Projectile, WeaponDesign.Rifle, 0.28f, 0.28f, 12f, 30f, 6645, false, 3.25f, 20);
        onionRifle.SetupKnockback(new Vector3(0, 0, 0), 0, -0.5f);
        onionRifle.SetupProjectile(18f, -2f, 90f,150f,new Vector3(0.42f,0.42f,0.42f),0.78f,172f,20f,30);
        onionRifle.SetupAttribute(Attributes.Crying, 2f, 15f, 0.0165f, true);
        libary.AddWeaponTemplete(onionRifle);

        //Assassin
        baggetRifle.SetupWeapon("Baguette Rifle", WeaponClass.Projectile, WeaponDesign.Stream, 0.38f, 0.3f, 5.3f ,40f, 3586,1, 7, 2f);
        baggetRifle.SetupKnockback(new Vector3(0.5f, 1f, 0.6f), -2f, 10f);
        baggetRifle.SetupProjectile(0f, 0f, 38f, 100f, new Vector3(0.66f, 0.66f, 0.66f), 0.8f, 500f, 0f, 500);
        libary.AddWeaponTemplete(baggetRifle);

        swissStriker.SetupWeapon("Swiss Striker", WeaponClass.Projectile, WeaponDesign.RapidFire, 0.08f, 0.16f, 13f, 38, 900, false, 4.68f, 7);
        swissStriker.SetupKnockback(new Vector3(0f, 1f, 0f), 2.5f, 3f);
        swissStriker.SetupProjectile(0, 0, 70f, 100f, new Vector3(0.2f, 0.2f, 0.2f),0.88f,60f,0f,10);
        swissStriker.SetupSecondaryProjectile(30f, -3.2f, 40f, 7f, new Vector3(0.5f,0.5f,0.5f),0.5f,10f, 0.9f);
        libary.AddWeaponTemplete(swissStriker);

        furrySword.SetupMeleeWeapon("Furry Sword", WeaponClass.Melee, WeaponDesign.RapidFire, 0.4f, 0.185f, 50f, 0, new Vector3(1.35f, 3f, 12f),150);
        furrySword.SetupKnockback(new Vector3(0.25f, 30f, 1.01f), 0f, 30f);
        furrySword.SetupAttribute(Attributes.Fire, 2f, 12f, 0.345f, false);
        libary.AddWeaponTemplete(furrySword);
        //Flyer
        //Pistol
        pistol.SetupWeapon("Pistol", WeaponClass.Projectile, WeaponDesign.Stream, 0.17f, 0.2f, 13.5f, 16f, 200,false, 1.2f, 15);
        pistol.SetupKnockback(new(0, 1f, 1.1f), 1f, 1f);
        pistol.SetupProjectile(0, 0, 120f, 15f, new(0.45f, 0.45f, 0.45f), 0.4f, 70f, 3.5f, 2);
        libary.AddWeaponTemplete(pistol);

        //Blaster
        blaster.SetupWeapon("Bird Blaster", WeaponClass.Explosive, WeaponDesign.AreaOfDenial, 1.6f, 1.6f, 70f, 9f, 882,1.9f, 0.247f, 5, 0f, 0.001f,1.45f);
        blaster.SetupKnockback(new Vector3(2f, 10.4f, 1.3f), 35f, 15f);
        blaster.SetupProjectile(0, 0, 20f, 50f, new Vector3(4f, 4f, 4f), 4f, 13f, 0.3f);
        libary.AddWeaponTemplete(blaster);
        //SMG
        smg.SetupWeapon("SMG", WeaponClass.Projectile, WeaponDesign.Close, 0.01893f, 0.036f, 3.62f, 137f, 460,false, 2.96f, 90);
        smg.SetupKnockback(new Vector3(1, 1, 1), -0.3f, 0f);
        smg.SetupProjectile(0, 0, 50f, 80f, new Vector3(0.075f, 0.075f, 0.075f), 0.5f, 150f, 18f, 99);
        libary.AddWeaponTemplete(smg);

        jetpack.SetupItem("Jetpack", 6999);
        regenerationPassive.SetupItem("Passive Regeneration 2", 360000);

        libary.AddWeaponTemplete(jetpack);
        libary.AddWeaponTemplete(regenerationPassive);
        //Dugglin
        dugglinCharge.SetupMeleeWeapon("Dugglin Charge",WeaponClass.Melee,WeaponDesign.RapidFire,0.8f,0.8f,12f,0f,new Vector3(3f,1,0.5f),1030);
        dugglinCharge.SetupKnockback(new Vector3(1.3f, 12f, 2.8f), 2.1f, 25f);
        dugglinCharge.SetupAttribute(Attributes.Fire, 2f, 5, 0.5f, true);
        libary.AddWeaponTemplete(dugglinCharge);

        //New Libary
        
        
        
    } //Used in 3D games
    */
    /// <summary>
    /// Its easier to write each effect with code
    /// </summary>
    private void SetupAttributes() //I found it easier to create attributes in code since its only 1 string, 1 enumorator, and 3 floats.
    {
        //Single Level Effect
        libary.AddAttribute(new AttributesTemplete("Speed Boost Fire", Attributes.Speed, 1.15f, 10f, 0));
        libary.AddAttribute(new AttributesTemplete("Speed Boost Sparkle", Attributes.Speed, 2f, 5f, 0));
        libary.AddAttribute(new AttributesTemplete("Speed Boost", Attributes.Speed, 1f, 20f, -1));
        libary.AddAttribute(new AttributesTemplete("Speed Boost FlameThrower", Attributes.Speed, 1.1f, 3.5f, 0));
        libary.AddAttribute(new AttributesTemplete("Poison Damage 1", Attributes.Poison, 0.1f, 4f, 0.15f));
        libary.AddAttribute(new AttributesTemplete("Poison Damage 2", Attributes.Poison, 0.2f, 5f, 0.12f));
        libary.AddAttribute(new AttributesTemplete("Poison Damage 3", Attributes.Poison, 0.3f, 6f, 0.1f));
        libary.AddAttribute(new AttributesTemplete("Crying 1", Attributes.Crying, 1f, 5f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 2", Attributes.Crying, 2f, 4.9f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 3", Attributes.Crying, 3f, 4.8f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 4", Attributes.Crying, 4f, 4.7f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 5", Attributes.Crying, 5f, 4.5f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 6", Attributes.Crying, 7f, 4.3f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 7", Attributes.Crying, 10f, 4.15f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 8", Attributes.Crying, 12.5f, 4f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 9", Attributes.Crying, 15f, 3.85f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 10", Attributes.Crying, 17.5f, 3.65f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 11", Attributes.Crying, 20f, 3.35f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 12", Attributes.Crying, 22.5f, 3f, 1f));
        libary.AddAttribute(new AttributesTemplete("Crying 13", Attributes.Crying, 25f, 2.55f, 1f));
        libary.AddAttribute(new AttributesTemplete("Weaping 1", Attributes.Crying, 30f, 30f, 1f));
        libary.AddAttribute(new AttributesTemplete("Weaping 2", Attributes.Crying, 50f, 27f, 1f));
        libary.AddAttribute(new AttributesTemplete("Weaping 3", Attributes.Crying, 70f, 25f, 1f));
        libary.AddAttribute(new AttributesTemplete("Weaping 4", Attributes.Crying, 90f, 22f, 1f));
        libary.AddAttribute(new AttributesTemplete("Weaping 5", Attributes.Crying, 110f, 20f, 1f));
        libary.AddAttribute(new AttributesTemplete("Flytation 1", Attributes.Flytation, 1f, 1.5f, 35f));
        libary.AddAttribute(new AttributesTemplete("Flytation 2", Attributes.Flytation, 3f, 1.75f, 45f));
        libary.AddAttribute(new AttributesTemplete("Flytation 3", Attributes.Flytation, 5f, 2f, 60f));
        libary.AddAttribute(new AttributesTemplete("High Jump 15", Attributes.Jump, 1.15f, 15f, 0f));
        libary.AddAttribute(new AttributesTemplete("High Jump 35", Attributes.Jump, 1.35f, 15f, 0f));
        libary.AddAttribute(new AttributesTemplete("High Jump 60", Attributes.Jump, 1.60f, 15f, 0f));
        libary.AddAttribute(new AttributesTemplete("High Jump 100", Attributes.Jump, 2f, 15f, 0f));
        libary.AddAttribute(new AttributesTemplete("Regeneration 1", Attributes.Regeneration, 0.18f, 5, 0.2f));
        libary.AddAttribute(new AttributesTemplete("Regeneration 2", Attributes.Regeneration, 0.2f, 7, 0.2f));
        libary.AddAttribute(new AttributesTemplete("Regeneration 3", Attributes.Regeneration, 0.225f, 8, 0.15f));
        libary.AddAttribute(new AttributesTemplete("Regeneration Sniper", Attributes.Regeneration, 1.2f, 5, 0.6f));
        libary.AddAttribute(new AttributesTemplete("Flytation", Attributes.Flytation, 2, 8, 1));
        

        //Multi-Level Effect
        libary.AddAttribute(new AttributesTemplete("Fire Damage 1", Attributes.Poison, 0.5f, 8f, 1f, "Speed Boost Fire"));
        libary.AddAttribute(new AttributesTemplete("Fire Damage 2", Attributes.Poison, 0.7f, 8f, 0.9f, "Speed Boost Fire"));
        libary.AddAttribute(new AttributesTemplete("Fire Damage FlameThrower", Attributes.Poison, 0.2f, 3f, 0.1f));
        libary.AddAttribute(new AttributesTemplete("Spark Fire Damage 1", Attributes.Poison, 0.1f, 4f, 0.15f, "Speed Boost Sparkle"));
        libary.AddAttribute(new AttributesTemplete("Spark Fire Damage 2", Attributes.Poison, 0.15f, 3.558f, 0.1f, "Speed Boost Sparkle"));
    }
    private void SetupAmmo()
    {
        if (libary == null) {
            Debug.LogError("WeaponTempletes array is null!");
            return;
        }
        for (int i = 0; i < ammoTempletes.Length; i++)
        {
            if (ammoTempletes[i] == null)
            {
                Debug.LogAssertion($"WeaponTemplate at index {i} is null!");
                continue;
            }
            if (ammoTempletes[i].GetAmmo() == null)
            {
                Debug.LogAssertion($"AmmoTemplete item is null");
            }
            libary.AddInventoryItem(ammoTempletes[i].GetAmmo());
        }
    }
    private void SetupItems()
    {
        if (weaponTempletes == null)
        {
            Debug.LogAssertion("WeaponTempletes array is null!");
            return;
        }
        for (int i = 0; i < weaponTempletes.Length; i++)
        {
            if (weaponTempletes[i] == null)
            {
                Debug.LogAssertion($"WeaponTemplate at index {i} is null!");
                continue;
            }
            if (weaponTempletes[i].GetItem() == null)
            {
                Debug.LogAssertion($"WeaponTemplete inventoryItem at {i} index is null!!!!");
            }
            libary.AddInventoryItem(weaponTempletes[i].GetItem());
        }
    }
    private void SetupCharacter()
    {
        if (characterTempletes == null)
        {
            Debug.LogAssertion("WeaponTempletes array is null!");
            return;
        }
        for (int i = 0; i < characterTempletes.Length; i++)
        {
            if (characterTempletes[i] == null)
            {
                Debug.LogAssertion($"WeaponTemplate at index {i} is null!");
                continue;
            }
            if (characterTempletes[i].Character == null)
            {
                Debug.LogAssertion($"WeaponTemplete inventoryItem at {i} index is null!!!!");
            }
            libary.AddCharacter(characterTempletes[i].Character);
        }
    }
    private void SetupEntities()
    {
        for (int i = 0; i < entityTempletes.Length; i++)
        {
            libary.AddEntities(entityTempletes[i]);
        }
    }
    public AttributesTemplete[] SearchLibaryForAttribute(params string[] names)
    {
        if (names == null) return Array.Empty<AttributesTemplete>();

        AttributesTemplete[] work = new AttributesTemplete[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            Debug.Log($"Name of attribute: {names[i]}");
            work[i] = libary.GetAttribute(names[i]);
        }
        return work;
    }

    public AttributesTemplete SearchLibaryForAttribute(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Attribute name is null or empty");
            return null;
        }

        var work = libary.GetAttribute(name);
        if (work != null)
        {
            Debug.Log($"Found {work.GetName()}");
            return work;
        }
        Debug.Log("No Attribute found");
        return null;
    }
    public InventoryItem[] SearchLibaryForTemplete(params string[] names)
    {
        if (names == null) return Array.Empty<InventoryItem>();

        InventoryItem[] work = new InventoryItem[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            try
            {
                Debug.Log($"Name of object: {names[i]}");
                var temp = libary.GetInventoryItem(names[i]);
                if (temp != null)
                {
                    work[i] = new InventoryItem(temp); // Use copy constructor
                    Debug.Log($"Found item named {work[i]?.GetName()}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"No item named {names[i]} found OR {e}");
                work[i] = null;
            }
        }
        return work;
    }
    public InventoryItem SearchLibaryForTemplete(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Item name is null or empty");
            return null;
        }

        Debug.Log($"Name of object: {name}");
        var temp = libary.GetInventoryItem(name);
        if (temp != null)
        {
            return new InventoryItem(temp); // Use copy constructor
        }
        Debug.Log("No Item found");
        return null;
    }
    public Quest[] SearchLibaryForQuest(params string[] name)
    {
        Quest[] work = new Quest[name.Length];
        for (int i = 0; i < name.Length; i++)
        {
            Debug.Log($"Name of object: {name[i]}");
            Quest temp = libary.GetQuest(name[i]);
            if (temp != null)
            {
                work[i] = temp;
            }
        }
        return work;
    }
    public Quest SearchLibaryForQuest(string name)
    {
        Debug.Log($"Name of object: {name}");
        Quest temp = libary.GetQuest(name);
        if (temp != null)
        {
            return temp;
        }
        Debug.Log("No Quest found");
        return null;
    }
    public EntityTemplete SearchLibaryForEntity(string name)
    {
        Debug.Log($"Name of the object: {name}");
        EntityTemplete temp = libary.GetEntities(name);
        if (temp != null)
        {
            return temp;
        }
        Debug.Log("No Entities found");
        return null;
    }
    public EntityTemplete[] SearchLibaryForEntities(params string[] name)
    {
        EntityTemplete[] work = new EntityTemplete[name.Length];
        for (int i = 0; i < name.Length; i++)
        {
            Debug.Log($"Name of object: {name[i]}");
            EntityTemplete temp = libary.GetEntities(name[i]);
            if (temp != null)
            {
                work[i] = temp;
            }
        }
        return work;
    }
    public Character[] SearchLibaryForCharacter(params string[] name)
    {
        Character[] work = new Character[name.Length];
        for (int i = 0; i < name.Length; i++)
        {
            Debug.Log($"Name of object: {name[i]}");
            Character temp = libary.GetCharacter(name[i]);
            if (temp != null)
            {
                work[i] = temp;
            }
        }
        return work;
    }
    public Character SearchLibaryForCharacter(string name)
    {
        return libary.GetCharacter(name);
    }
    public void AddAttribute(params AttributesTemplete[] templetes)
    {
        libary.AddAttribute(templetes);
    }
    public static AllLibary ItemLibary { get; private set; }
    private void Awake()
    {
        if (ItemLibary == null)
        {
            ItemLibary = this;
            DontDestroyOnLoad(gameObject);
            SetupAttributes(); //Run first
            SetupAmmo();
            SetupItems(); //Run Items
            //Run Quests;
            SetupEntities(); //Run Entities
            SetupCharacter();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #region Get All Names
    public List<string> GetInventoryItemNames()
    {
        Debug.Log(libary.GetInventoryItemNames());
        return libary.GetInventoryItemNames();
    }
    public List<string> GetEffectNames()
    {
        return libary.GetEffectNames();
    }
    public List<string> GetEntitityNames()
    {
        return libary.GetEntitityNames();
    }
    public List<InventoryItem> GetAllItems() => libary.GetAllItems();
    public List<AttributesTemplete> GetAllEffects() => libary.GetAllEffects();
    public List<EntityTemplete> GetAllEntities() => libary.GetAllEntites();
    #endregion
}
