using BaseCharacter.Entities;
using BaseCharacter.Structual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public static class SaveData
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_save.json");
    private static string RegexPath => Path.Combine(Application.persistentDataPath, "regex_save.json");
    public static void SaveGame(Player player, WorldLocation location)
    {
        float extra = 0;
        List<string> inventory = new();
        List<int> amount = new();
        //List<Effect> effects = new List<Effect>();
        for (int i = 0; i < player.GetInventorySize(); i++)
        {
            inventory.Add(player.GetInventoryItem(i).GetName());
            amount.Add(player.GetInventoryItem(i).GetHeldAmount());
        }
        var saveData = new PlayerSaveData
        {
            Name = player.Name,
            Desc = player.GetDesc(),
            IsAlive = true,
            Health = player.Health.GetHPInfo()[0] + extra,
            HealthBase = player.Health.GetHPInfo()[1],
            AdranalineDistance = player.AdranalineDistance,
            Adranaline = player.Adranaline,
            SpeedBase = player.GetSpeedBase(),
            JumpBase = player.JumpBase.Level1Stat,
            GroundPoundBase = player.GetGroundPound(true),
            GravityBase = player.GravityBase,
            WeightBase = player.WeightBase,
            VisionBase = player.VisionBase.Level1Stat,
            Aim = player.Aim.Level1Stat,
            Reach = player.GetReach(),
            Resistances = player.GetResistances(),
            SizeOfInventory = player.GetInventorySize(),
            BaseSizeOfInventory = player.GetInventorySize(),
            PendingItem = player.GetPendingItem(),
            CurrentHotBarSlot = player.GetHotbarSlot(),
            HotbarSize = player.GetHotbarSize(),
            Inventory = inventory,
            Amount = amount,
            Money = player.GetMoneyInt(),
            Level = player.GameLevel,
            World = location.Name,
            Location = location.Location,
            RotationSpeedbase = player.GetRotationSpeed(1),
            BreakingSpeed = player.BreakingSpeed,
            GravityProtectionTime = player.GravityProtectionTime,
        };
        string json = JsonUtility.ToJson(obj: saveData, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved to: " + SavePath);
    }
    public static bool TryLoadGame(out PlayerSaveData loadedData)
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                loadedData = JsonUtility.FromJson<PlayerSaveData>(json);

                if (SceneManager.GetActiveScene().name != loadedData.World)
                {
                    SceneManager.LoadScene(loadedData.World);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading save file: {ex.Message}");
                loadedData = null;
                return false;
            }
        }
        loadedData = null;
        return false;
    }
    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }
    }
    public static void CreateRegexSaveFile()
    {
        List<int> targets = new()
        {
            1,
            2
        };
        var regexSaveData = new RegexSaveData
        {
            RegexTarget = targets,
            DefaultItemAmount = 1,
            LibaryDefaultObject = 7
        };

        string json = JsonUtility.ToJson(obj: regexSaveData, prettyPrint: true);
        File.WriteAllText(RegexPath, json);
    }
    public static bool AddAttributes(string name, Attributes attributes, float strength, float time, float option, string otherInterventions)
    {
        if (File.Exists(RegexPath))
        {
            try
            {
                List<string> NameOfAttributes = new List<string>();
                List<int> AttributeEnum = new List<int>();
                List<float> Strength = new List<float>();
                List<float> Time = new List<float>();
                List<float> Option = new List<float>();
                List<string> OtherAttributesApplied = new();
                
                string json = File.ReadAllText(RegexPath);
                RegexSaveData loadedData = JsonUtility.FromJson<RegexSaveData>(json);
                NameOfAttributes.Add(name);
                NameOfAttributes.AddRange(loadedData.NameOfAttribute);
                OtherAttributesApplied.AddRange(loadedData.OtherAttributesApplied);
                OtherAttributesApplied.Add(otherInterventions);
                AttributeEnum.AddRange(loadedData.AttributeEnum);
                Strength.AddRange(loadedData.Strength);
                Time.AddRange(loadedData.Time);
                Option.AddRange(loadedData.Option);
                AttributeEnum.Add((int)attributes);
                Strength.Add(strength);
                Time.Add(time);
                Option.Add(option);


                var regexSaveData = new RegexSaveData
                {
                    RegexTarget = loadedData.RegexTarget,
                    DefaultItemAmount = loadedData.DefaultItemAmount,
                    LibaryDefaultObject = loadedData.LibaryDefaultObject,
                    NameOfAttribute = NameOfAttributes,
                    OtherAttributesApplied = OtherAttributesApplied,
                    Strength = Strength,
                    Time = Time,
                    Option = Option,
                    AttributeEnum = AttributeEnum,

                };
                AllLibary.ItemLibary.AddAttribute(new AttributesTemplete(name, attributes, strength, time, option, otherInterventions));
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading regex file: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"Creating new save file, please run the command again");
            CreateRegexSaveFile();
        }
        return false;
    }
    public static bool GetAttributesToLibary()
    {
        if (File.Exists(RegexPath))
        {
            try
            {
                string json = File.ReadAllText(RegexPath);
                RegexSaveData loadedData = JsonUtility.FromJson<RegexSaveData>(json);
                for (int i = 0; i < loadedData.NameOfAttribute.Count; i++)
                {
                    try //TODO: Make it so you can combind multiple effects
                    {
                        AllLibary.ItemLibary.AddAttribute(new AttributesTemplete(loadedData.NameOfAttribute[i], (Attributes)loadedData.AttributeEnum[i], loadedData.Strength[i], loadedData.Time[i], loadedData.Option[i], loadedData.OtherAttributesApplied[i]));
                    }
                    catch
                    {
                        try
                        {
                            AllLibary.ItemLibary.AddAttribute(new AttributesTemplete(loadedData.NameOfAttribute[i], (Attributes)loadedData.AttributeEnum[i], loadedData.Strength[i], loadedData.Time[i], loadedData.Option[i]));
                        }
                        catch
                        {
                            continue;
                        }
                        continue;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading regex file: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"Creating new save file, please run the command again");
            CreateRegexSaveFile();
        }
        return false;
    }
    public static string GetDefaults()
    {
        if (File.Exists(RegexPath))
        {
            try
            {
                string json = File.ReadAllText(RegexPath);
                RegexSaveData loadedData = JsonUtility.FromJson<RegexSaveData>(json);
                string endResult = $"/Default item #{loadedData.DefaultItemAmount}";
                //TODO: FINISH TARGET FINDING
                /*
                foreach(Attributes target in loadedData.RegexTarget.Select(v => (Attributes)v))
                {
                    endResult += ""
                }
                */
                return endResult;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading regex file: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Creating new save file, please run the command again");
            CreateRegexSaveFile();
        }
        //TODO: Add @ml at the end of this string
        return "/Default #1";
    }
}

[Serializable]
public class PlayerSaveData
{
    #region Player Data
    public uint ID;
    /// <summary>
    /// Name of the player
    /// </summary>
    public string Name ;
    public string Desc ;
    /// <summary>
    /// Money
    /// </summary>
    public int Money ;
    /// <summary>
    /// Are you alive
    /// </summary>
    public bool IsAlive ;
    /// <summary>
    /// Your current health
    /// </summary>
    public float Health ;
    /// <summary>
    /// Your max health
    /// </summary>
    public int HealthMax ;
    /// <summary>
    /// Your base Health on spawn
    /// </summary>
    public int HealthBase ;
    //TODO: Setup Adranaline
    /// <summary>
    /// Adranaline time based systems. 
    /// </summary>
    public float Adranaline ;
    /// <summary>
    /// How far away does it activate
    /// </summary>
    public float AdranalineDistance ;
    /// <summary>
    /// Your base Speed on spawn
    /// </summary>
    public float SpeedBase ;
    public float RotationSpeedbase;
    public float GroundPoundBase ;
    public float JumpBase ;
    public float GravityBase ;
    public float WeightBase ;
    public float VisionBase ;
    public float Aim = 0;
    public float Reach ;
    public float[] Resistances = new float[Enum.GetValues(typeof(WeaponClass)).Length];
    public int Level = 0;
    public float GravityProtectionTime;
    public float BreakingSpeed;
    #endregion
    #region Inventory
    /// <summary>
    /// How many items can you hold
    /// </summary>
    public int SizeOfInventory ;

    /// <summary>
    /// The base size of your inventory (without expansions)
    /// </summary>
    public int BaseSizeOfInventory ;

    /// <summary>
    /// What you have in your inventory
    /// </summary>
    public List<string> Inventory = new List<string>();
    public List<string> ProjectileData = new List<string>();
    public List<int> Amount = new List<int>();
    /// <summary>
    /// The selected item used for swapping.
    /// </summary>
    public int PendingItem = -1;

    /// <summary>
    /// Current Inventory Slot
    /// </summary>
    public int CurrentHotBarSlot = 0;

    /// <summary>
    /// Hotbar Size.
    /// </summary>
    public int HotbarSize = 6;
    #endregion
    #region World Data
    public Vector3 Location = new Vector3();
    public string World;
    #endregion
    public Player GetSavePlayerData()
    {
        return new(ID, Name, Desc, Money,
            IsAlive, HealthMax, Health, HealthBase, Adranaline, AdranalineDistance,
            SpeedBase, GroundPoundBase, JumpBase, GravityBase, WeightBase, VisionBase, Aim, Reach, Resistances, Level,
            SizeOfInventory, BaseSizeOfInventory, PendingItem, CurrentHotBarSlot, HotbarSize, RotationSpeedbase);
    }
}
[Serializable]
public class RegexSaveData
{
    public int LibaryDefaultObject;
    #region Annotations
    public List<int> RegexTarget;
    public int DefaultItemAmount;
    #endregion
    #region Saved Attrbiutes
    public List<string> NameOfAttribute = new List<string>();
    public List<int> AttributeEnum = new List<int>();
    public List<float> Strength = new List<float>();
    public List<float> Time = new List<float>();
    public List<float> Option = new List<float>();
    public List<string> OtherAttributesApplied = new();
    public List<int> OtherAttributesIndex = new();
    #endregion
}