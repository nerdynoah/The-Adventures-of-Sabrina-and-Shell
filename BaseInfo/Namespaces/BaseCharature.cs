using BaseCharacter.Effect;
using BaseCharacter.Items;
using BaseCharacter.Movement;
using BaseCharacter.Stats;
using BaseCharacter.Structual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using static Enums;

namespace BaseCharacter
{
    /// <summary>
    /// Setup wallet methods
    /// </summary>
    public interface IWallet
    {
        public void AddMoney(int amount);
        public int SpendMoney(int amount);
    }
    /// <summary>
    /// Get money as a int and as a string
    /// </summary>
    public interface IMoney
    {
        /// <summary>
        /// Get how much money you have
        /// </summary>
        /// <returns></returns>
        public int GetMoneyInt();
        /// <summary>
        /// Get how much money you have formatted as a string
        /// </summary>
        /// <returns>Dollars.Cents</returns>
        public string GetMoney();
    }
    /// <summary>
    /// Basic health setup
    /// </summary>
    public interface IHealth
    {
        public List<int> GetHPInfo();
        /// <summary>
        /// Usually used to heal the player.
        /// </summary>
        /// <param name="amount"></param>
        public void Heal(float amount);
        /// <summary>
        /// Damage the player with a set float value
        /// </summary>
        /// <param name="value">Decrease by a set value</param>
        public void DamagePlayer(float value);
        /// <summary>
        /// Damage the player with some extra data
        /// </summary>
        /// <param name="value">Damage amount</param>
        /// <param name="weapon">Weapon type.</param>
        /// <param name="fake">Doesn't damage the player</param>
        /// <returns></returns>
        public float DamagePlayer(float value, WeaponClass weapon, bool fake = false);
        /// <summary>
        /// Damage the player with some extra data, Along with the ability to make a minimum health Barriar
        /// </summary>
        /// <param name="value">Damage amount</param>
        /// <param name="weapon">Weapon type.</param>
        /// <param name="fake">Doesn't damage the player</param>
        /// <returns></returns>
        public float DamagePlayer(float value, WeaponClass weapon, bool fake, float lowestHealth);
        /// <summary>
        /// Damage the player with a value from 0.0 to 1.0, this will turn into a percent. 
        /// You can choose between 3 different types of decreasing. 
        /// </summary>
        /// <param name="value">A value from 0.0 to 1.0</param>
        /// <param name="DecreaesType">A value that must be be 1, 2, or 3. Look at the list provided to see what each option does.</param>
        public void DamagePlayer(float value, HealthDamagePercentage DecreaesType);
        public bool GetIsAlive();
    }
    /// <summary>
    /// Required methods as many of the <see cref="BaseCharacter.Items"/> require this info.
    /// </summary>
    public interface INameDesc
    {
        public string GetName();
        public string GetDesc();
        public bool GetName(string name);
        public bool GetDesc(string name);
    }
    public interface IMovement
    {

    }
    public static class SlashRegex
    {
        /// <summary>
        /// Detects where each / and whitespace is. The word in the / will be identifiable as a "command".<br></br> Everything else after the first whitespace will be stored as a "param". You can have multiple / and whitespaces. <br></br> Several annotations exist, such as @#$%, anything after them is considered "text". <br></br> Example:
        /// <code>
        /// /default #1 @ml
        /// /give item Shotgun
        /// /give item Shotgun Pistol 2 /Libary Effect Fire_Damage_1 @m
        /// /give item Shotgun Pistol #4 #1
        /// /give effect Fire_Damage_2 @l
        /// /jump #500
        /// /clear @d
        /// </code>
        /// </summary>
        public static Regex SlashCommands { get; private set; } = new Regex(@"(?<=^|\s)\/(?<command>\w+)(?:\s+(?<params>(?<param>[^\/@#$%&*\-\s]+)(?:(?:\s+|,\s*)(?<param>[^\/@#$%&\-*\s]+))*(?=\s|$))?)?(?:\s*)(?<annotations>(?<annotation>[@#$%&*\-])(?<text>[\w]*)(?:\s*(?<annotation>[@#$%&*\-]+)*(?<text>[\w]+))*)?\b", RegexOptions.Compiled);

        //public static Regex SlashCommands { get; private set; } = new Regex(@"(?<=^|\s)\/(?<command>\w+)(?:\s+)(?<params>(?<param>[^/@#$%&*\s]+)(?:(?:\s+|,\s*)(?<param>[^/@#$%&*\s]+))*(?=\s|$))?", RegexOptions.Compiled);
        // public static Regex Annotation { get; private set; } = new Regex(@"(?<annotation>[@#$%&*])(?<text>[0-zA-Z]+)\b");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">The inputted text</param>
        /// <param name="matches">The Match collection gathered from the text.</param>
        /// <returns>Returns a list of <see cref="RegexSearchType"/> depending on how many / there are.</returns>
        public static List<RegexSearchType> GetSlashSearchType(string text, out MatchCollection matches)
        {
            matches = SlashCommands.Matches(text);
            List<RegexSearchType> regexSearchTypes = new();
            foreach (Match match in matches)
            {
                switch (match.Groups["command"].Value.ToLower())
                {
                    case "give":
                    case "giveto":
                    case "giving":
                    case "giv":
                        regexSearchTypes.Add(RegexSearchType.Give);
                        break;
                    case "copy":
                        regexSearchTypes.Add(RegexSearchType.Copy);
                        break;
                    case "w":
                    case "whisper":
                    case "whis":
                    case "wh":
                    case "whi":
                    case "whisp":
                    case "whispe":
                        regexSearchTypes.Add(RegexSearchType.Whisper);
                        break;
                    case "default":
                    case "defaulty":
                    case "defaul":
                    case "defaulting":
                    case "defau":
                    case "defa":
                    case "def":
                    case "de":
                    case "d":
                        regexSearchTypes.Add(RegexSearchType.Default);
                        break;
                    case "clear":
                    case "cl":
                    case "cle":
                    case "clea":
                        regexSearchTypes.Add(RegexSearchType.Clear);
                        break;
                    case "create":
                    case "cr":
                    case "cre":
                    case "crea":
                    case "creat":
                    case "new":
                        regexSearchTypes.Add(RegexSearchType.New);
                        break;
                    case "jump":
                    case "jum":
                        regexSearchTypes.Add(RegexSearchType.Jump);
                        break;
                    case "list":
                    case "lis":
                    case "li":
                        Debug.Log("Building list");
                        regexSearchTypes.Add(RegexSearchType.List);
                        break;
                    case "help":
                    case "hel":
                    case "h":
                    case "he":
                            regexSearchTypes.Add(RegexSearchType.Help);
                        break;
                    case "effect":
                    case "attribute":
                    case "attri":
                    case "eff":
                    case "effec":
                    case "efect":
                        regexSearchTypes.Add(RegexSearchType.AttributeTemplete);
                        break;
                    case "entity":
                    case "enemy":
                    case "killable":
                        regexSearchTypes.Add(RegexSearchType.Entities);
                        break;
                    case "it":
                    case "ite":
                    case "item":
                    case "weapon":
                    case "consumable":
                    case "inventoryitem":
                    case "inventory_item":
                    case "inventory":
                    case "armor":
                        regexSearchTypes.Add(RegexSearchType.InventoryItem);
                        break;
                    case "Die":
                        regexSearchTypes.Add(RegexSearchType.Die);
                        break;
                    default:
                        regexSearchTypes.Add(RegexSearchType.None);
                        break;
                }
            }
            return regexSearchTypes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        /// <param name="index">Which param string to look at in the <paramref name="match"/></param>
        /// <returns>Gets what type of object the libary should search for</returns>
        public static LibraryObjects GetSlashLibarySearch(Match match, int index, bool ReturnDefault = true)
        {
            CaptureCollection conditions = match.Groups["param"].Captures;
            string condition = conditions[index].Value.ToLower();
            switch (condition)
            {
                case "effect":
                case "attribute":
                case "attri":
                case "effec":
                case "efect":
                case "eff":
                case "ef":
                    return (LibraryObjects.AttributeTemplete);
                case "entity":
                case "enemy":
                case "enem":
                case "ene":
                case "en":
                    return (LibraryObjects.Entities);
                case "it":
                case "ite":
                case "item":
                case "weapon":
                case "consumable":
                case "inventoryitem":
                case "inventory_item":
                case "inventory":
                case "inv":
                case "armor":
                case "i":
                    return (LibraryObjects.InventoryItem);
                case "quest":
                case "mission":
                case "quests":
                case "missions":
                    return (LibraryObjects.Quests);
                default:
                    if (!ReturnDefault)
                        return (LibraryObjects.None);
                    return DefaultLibaryObject;
            }
        }
        /// <summary>
        /// Gets the rest of the params as strings from <paramref name="match"/>
        /// </summary>
        /// <param name="match"></param>
        /// <param name="startIndex">Where to start looking in <paramref name="match"/></param>
        /// <returns>A list of strings</returns>
        public static List<string> GetSlashParamStrings(Match match, int startIndex, int endIndex)
        {
            List<string> strings = new();
            CaptureCollection conditions = match.Groups["param"].Captures;
            for (int i = startIndex; i < endIndex; i++)
            {
                Debug.Log($"{conditions[i].Value.Replace('_', ' ')}");
                strings.Add(conditions[i].Value.Replace('_', ' '));
            }
            return strings;
        }
        /// <summary>
        /// Gets the rest of the params from the <paramref name="matches"/>
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="startIndex">Where to start looking in <paramref name="matches"/></param>
        /// <returns></returns>
        public static List<string> GetSlashParamStrings(MatchCollection matches, int startIndex, int endIndex)
        {
            List<string> strings = new();
            foreach (Match match in matches)
            {
                CaptureCollection conditions = match.Groups["param"].Captures;

                // Safety check
                if (conditions == null || conditions.Count == 0)
                    continue;

                // Ensure indices are within bounds for this match
                int localStart = Math.Max(0, startIndex);
                int localEnd = Math.Min(endIndex, conditions.Count);

                for (int i = localStart; i < localEnd; i++)
                {
                    strings.Add(conditions[i].Value);
                }
            }
            return strings;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="index">Which param string to look at in the <paramref name="matches"/></param>
        /// <returns>Gets what type of object the libary should search for</returns>
        [Obsolete("Is not updated to have all of the search things",false)]
        public static List<LibraryObjects> GetSlashLibarySearch(MatchCollection matches, int index)
        {
            List<LibraryObjects> librarySearch = new();
            foreach (Match match in matches)
            {
                CaptureCollection conditions = match.Groups["param"].Captures;
                string condition = conditions[index].Value.ToLower();
                switch (condition)
                {
                    case "effect":
                    case "attribute":
                    case "attri":
                        librarySearch.Add(LibraryObjects.AttributeTemplete);
                        break;
                    case "entity":
                    case "enemy":
                    case "killable":
                    case "copy":
                        librarySearch.Add(LibraryObjects.Entities);
                        break;
                    case "item":
                    case "weapon":
                    case "consumable":
                    case "inventoryItem":
                    case "inventory_item":
                    case "inventory":
                    case "armor":
                        librarySearch.Add(LibraryObjects.InventoryItem);
                        break;
                    default:
                        librarySearch.Add(LibraryObjects.None);
                        break;
                }
            }
            return librarySearch;
        }
        /// <summary>
        /// Get the final number in several slash commands if they have one.
        /// </summary>
        /// <param name="matches">All matches</param>
        /// <returns>List of intagers</returns>
        public static List<int> GetSlashFinalNumber(MatchCollection matches)
        {
            List<int> finalNum = new();
            foreach (Match match in matches)
            {
                finalNum.Add(GetSlashFinalNumber(match));
            }
            return finalNum;
        }
        /// <summary>
        /// Get the final number in a single slash command if it has one.
        /// </summary>
        /// <param name="match">A single match</param>
        /// <returns>int</returns>
        public static int GetSlashFinalNumber(Match match, out bool foundFinalNumber)
        {
            foundFinalNumber = false;
            try
            {
                var captures = match.Groups["param"].Captures;
                if (captures == null || captures.Count == 0)
                    return 0;

                Debug.Log($"Value: {captures[^1].Value}");

                if (int.TryParse(captures[^1].Value, out int i))
                {
                    foundFinalNumber = true;
                    return i;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Gets the final number in a parma from a <see cref="Match"/>
        /// </summary>
        /// <param name="match"></param>
        /// <returns>The final number of param if possible, otherwise return 0</returns>
        public static int GetSlashFinalNumber(Match match)
        {
            try
            {
                var captures = match.Groups["param"].Captures;
                if (captures == null || captures.Count == 0)
                    return 0;

                Debug.Log($"Value: {captures[^1].Value}");

                if (int.TryParse(captures[^1].Value, out int i))
                {
                    return i;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        public static string GetHelpText(RegexSearchType search, ref bool found)
        {
            string helpBlock = string.Empty;
            if (search == RegexSearchType.Default)
            {
                helpBlock += $"Command Output:\n" +
                    $"/default #int -int @target\n" +
                    $"Current defaults: #{DefaultInventoryItemAmount}\n" +
                    $"Desc:\n" +
                    $"@ = target. @m = me, @l = looking at, @d = distance, (i.e @ml)\nIn /give # will give the default amount of items to the target if no amount is specfied.\nIn /jump the # default will cause you to jump by this amount: {DefaultInventoryItemAmount * 10f + 35f}\n";
                found = true;
            }
            if (search == RegexSearchType.Jump)
            {
                helpBlock += $"Command Output:\n" +
                    $"/jump default({DefaultInventoryItemAmount * 10f + 35f})\n" +
                    $"/jump #\n" +
                    $"/jump -\n"+
                    $"Desc:\n"+
                    $"Applies a jump force. Uses #, -, and @ Annotations\n";
                found = true;
            }
            if (search == RegexSearchType.Clear)
            {
                helpBlock += $"Command Output:\n" +
                    $"/clear @target\n" +
                    $"Clears the inventory and/or TODO:Effects";
                found = true;
            }
            if (search == RegexSearchType.Help)
            {
                helpBlock += $"Command Output:\n" +
                    $"/help /command\n" +
                    $"To get a list of all commands, do /help\n" +
                    $"Current usable Annaotations:\n" +
                    $"# Positive number\n" +
                    $"- Negative number\n" +
                    $"@ Target: (m = Self, l = Looking at, d = distance, C = Closest";
                found = true;
            }
            if (search == RegexSearchType.Give)
            {
                helpBlock += $"Command Output:\n" +
                    $"/give LibaryObject Name Name etc... FinalNumber #Amount #Amount @target\n" +
                    $"LibaryObject:\n" +
                    $"Item, Effect, Entity, Quest\n" +
                    $"Example:\n" +
                    $"/give item Shotgun Shells #1 #12 @ml\n" +
                    $"/give item Rocket Fish 3\n" +
                    $"/give effect Fire_Damage_2 @c\n" +
                    $"Desc:\n" +
                    $"Gives the target a libary item/effect/etc... Uses # and @ Annotations";
                found = true;
            }
            if (search == RegexSearchType.List)
            {
                helpBlock += $"Command Output:\n" +
                    $"/list LibaryObject\n" +
                    $"libaryObject:\n" +
                    $"Item, Effect, Entity, Quest\n" +
                    $"Example:\n" +
                    $"/list items \n" +
                    $"Desc:\n" +
                    $"Lists all Libary objects on the screen";
                found = true;
            }
            if (search == RegexSearchType.AttributeTemplete)
            {
                helpBlock += $"Holds all of the attributes possible to summon via /commands and custom weapons.\n" +
                    $"Example: /give effect Fire_Damage_2";
            }
            if (search == RegexSearchType.InventoryItem)
            {
                helpBlock += $"Anything that can be held in your inventory is a Inventory Item.\n " +
                    $"Shotguns, Shells, Blocks, Armor, building blocks, boots, etc... are all InventoryItems";
            }
            if (search == RegexSearchType.Entities)
            {
                helpBlock += $"All entities and players are held here. You can use /give to summon them at a position.\n" +
                    $"Use annotations to determine where it spawns:\n" +
                    $"@l position.\n" +
                    $"@m around self\n" +
                    "@d30 random Location around you\n" +
                    $"Example: /give entity Zombie @l\n";
            }
            if (search == RegexSearchType.New)
            {
                //TODO: Finsish
            }
            return helpBlock;
        }
        /// <summary>
        /// Check for Attributes, items, and other appriopriate /commands preset in <see cref="SlashRegex"/> <br></br>
        /// Use <see cref="AllLibary.ItemLibary"/> to find items.
        /// </summary>
        /// <param name="text">The message</param>
        /// <param name="inventorySize">The size of the player's inventory (use to prevent errors)</param>
        /// <param name="attributes">Attributes collected(as a string)</param>
        /// <param name="items">Items collected (as a string)</param>
        public static void GetChatBoxRegex(string text, int inventorySize, out List<string> attributes, out List<AddItemRequest> items, out List<string> msgData, out bool clear, out float jump)
        {
            jump = 0;
            attributes = new List<string>();
            items = new List<AddItemRequest>();
            msgData = new List<string>();
            clear = false;
            List<RegexSearchType> regexSearch = SlashRegex.GetSlashSearchType(text: text, matches: out MatchCollection commands);
            for (int i = 0; i < regexSearch.Count; i++)
            {
                #region Help Command
                if (regexSearch[i] == RegexSearchType.Help)
                {
                    bool foundThing = false;
                    for (int j = i + 1; j < regexSearch.Count; j++)
                    {
                        msgData.Add(GetHelpText(regexSearch[j], ref foundThing));
                    }
                    if (!foundThing)
                    {
                        msgData.Add("/default => Set default values, targets, etc...\n/give => give things to target\n/help => Get help on specsific commands (hint: use /help /jump or /help /help)\n/jump => jump\n/new => Create new Attribute\n/list => List all libaryObjects\n/clear => Clear inventory and/or effects");
                    }
                    break;
                }
                #endregion
                if (regexSearch[i] == RegexSearchType.List)
                {
                    LibraryObjects which = SlashRegex.GetSlashLibarySearch(commands[i], 0);
                    if (which == LibraryObjects.AttributeTemplete)
                    {
                        msgData.AddRange(AllLibary.ItemLibary.GetEffectNames());
                    }
                    if (which == LibraryObjects.InventoryItem)
                    {
                        msgData.AddRange(AllLibary.ItemLibary.GetInventoryItemNames());
                    }
                    if (which == LibraryObjects.Entities)
                    {
                        msgData.AddRange(AllLibary.ItemLibary.GetInventoryItemNames());
                    }
                }
                #region Give Command
                if ((int)regexSearch[i] > 4 && (int)regexSearch[i] < 10)
                {
                    LibraryObjects which;
                    if ((int)regexSearch[i] == 5)
                    {
                        which = SlashRegex.GetSlashLibarySearch(commands[i], 0);
                    }
                    else
                    {
                        which = (LibraryObjects)(int)regexSearch[i];
                    }

                    List<string> parameters = GetSlashParamStrings(commands[i], 1, commands[i].Groups["param"].Captures.Count);
                    for (int j = 0; j < parameters.Count; j++)
                    {
                        Debug.Log(parameters[i]);
                    }
                    if (which == LibraryObjects.AttributeTemplete)
                    {
                        attributes.AddRange(parameters.ToList<string>());
                    }
                    if (which == LibraryObjects.InventoryItem)
                    {
                        var annotationCaptures = commands[i].Groups["annotation"].Captures;
                        var textCaptures = commands[i].Groups["text"].Captures;
                        int annoIndex = 0;
                        int lastNumValue = SlashRegex.GetSlashFinalNumber(commands[i], out bool found);
                        bool processedAnnotations = false;
                        if (found)
                        {
                            parameters.Remove(parameters[^1]);
                        }
                        for (int j = 0; j < annotationCaptures.Count && j < textCaptures.Count; j++)
                        {
                            if (annotationCaptures[j].Value == "#")
                            {
                                if (int.TryParse(textCaptures[j].Value, out int amount) && amount > 0)
                                {
                                    amount = Mathf.Abs(amount);
                                    items.Add(new AddItemRequest(parameters[annoIndex], amount));
                                    processedAnnotations = true;
                                }
                                annoIndex++;
                            }
                        }
                        if (!found && !processedAnnotations)
                        {
                            lastNumValue = DefaultInventoryItemAmount;
                        }
                        if (lastNumValue > 0)
                        {
                            for (int j = 0; j < parameters.Count; j++)
                            {
                                items.Add(new AddItemRequest(parameters[j], lastNumValue));
                            }
                        }
                    }
                    /*
                    if (which == LibraryObjects.InventoryItem)
                    {
                        Debug.Log("Searching for Inventory Item");
                        int lastNumValues = SlashRegex.GetSlashFinalNumber(commands[i]);
                        bool giveSingle = true;
                        if (lastNumValues <= 0)
                        {
                            bool allFails = true;
                            int annoIndex = 1;
                            for (int j = 0; i < commands[i].Groups["annotation"].Captures.Count - 1; j++)
                            {
                                if (commands[i].Groups["annotation"].Captures[j].Value == "#")
                                {
                                    try
                                    {
                                        int.TryParse(commands[i].Groups["text"].Captures[j].Value, out int amount);
                                        amount = Mathf.Abs(amount);
                                        for (int k = 0; k < amount; k++)
                                        {
                                            items.New(parameters[annoIndex]);
                                        }
                                        allFails = false;
                                        giveSingle = false;
                                    }
                                    catch
                                    {
                                        Debug.LogWarning("Charatures after # were not digits");
                                    }
                                    annoIndex++;
                                }
                            }
                            if (allFails)
                            {
                                lastNumValues = DefaultInventoryItemAmount;
                            }
                        }
                    }
                }
                */
                }
                #endregion
                #region New Command
                if (regexSearch[i] == RegexSearchType.New)
                {
                    LibraryObjects which = SlashRegex.GetSlashLibarySearch(commands[i], 0);
                    if (which == LibraryObjects.AttributeTemplete)
                    {
                        List<string> parameters = GetSlashParamStrings(commands[i], 1, commands[i].Groups["param"].Captures.Count);
                        try
                        {
                            bool idenSuccess = int.TryParse(parameters[1],out int result);
                            bool strSuccess = int.TryParse(parameters[2],out int str);
                            bool timeSuccess = int.TryParse(parameters[3], out int time);
                            bool optionSuc = int.TryParse(parameters[4], out int op);
                            if (!idenSuccess) 
                            {
                                result = DefaultInventoryItemAmount;
                            }
                            if (!strSuccess) 
                            {
                                str = 1;
                            }
                            if (!timeSuccess) 
                            {
                                time = 8;
                            }
                            if (!optionSuc)
                            {
                                op = 1;
                            }
                            if (parameters.Count > 5)
                            {
                                List<string> others = new();
                                for (int j = 5; j < parameters.Count; j++)
                                {
                                    others.Add(parameters[j]);
                                }
                                AllLibary.ItemLibary.AddAttribute(new AttributesTemplete(parameters[0], (Attributes)result, str, time, op, others.ToArray()));
                            }
                            else
                            {
                                AllLibary.ItemLibary.AddAttribute(new AttributesTemplete(parameters[0], (Attributes)result, str, time, op));
                            }
                            SaveData.AddAttributes(parameters[0], (Attributes)result, str, time, op,"");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }
                #endregion
                if (regexSearch[i] == RegexSearchType.Default)
                {
                    SetDefaultRegex(commands[i]);
                }
                if (regexSearch[i] == RegexSearchType.Clear)
                {
                    clear = true;
                }
                if (regexSearch[i] == RegexSearchType.Die)
                {

                }
                if (regexSearch[i] == RegexSearchType.Jump)
                {
                    var annotationCaptures = commands[i].Groups["annotation"].Captures;
                    var textCaptures = commands[i].Groups["text"].Captures;
                    int lastNumValue = SlashRegex.GetSlashFinalNumber(commands[i], out bool found);
                    lastNumValue = Mathf.Clamp(lastNumValue, 0, inventorySize);
                    if (found)
                    {
                        jump =- lastNumValue;
                    }
                    for (int j = 0; j < annotationCaptures.Count && j < textCaptures.Count; j++)
                    {
                        if (annotationCaptures[j].Value == "#")
                        {
                            if (int.TryParse(textCaptures[j].Value, out int amount) && amount > 0)
                            {
                                jump =+ Mathf.Abs(amount);
                            }
                        }
                        if (annotationCaptures[j].Value == "-")
                        {
                            if (int.TryParse(textCaptures[j].Value, out int amount) && amount > 0)
                            {
                                jump = -Mathf.Abs(amount);
                            }
                        }
                    }
                    if (jump == 0)
                    {
                        jump = DefaultInventoryItemAmount * 10f + 35f;
                    }
                }
            }
        }
        /// <summary>
        /// Input chat regex / commands, only works with /Default
        /// </summary>
        /// <param name="text"></param>
        public static void SetChatDefaultRegexLimited(string text)
        {
            List<RegexSearchType> regexSearch = SlashRegex.GetSlashSearchType(text: text, matches: out MatchCollection commands);
            for (int i = 0; i < regexSearch.Count; i++)
            {
                if (regexSearch[i] == RegexSearchType.Default)
                {
                    SetDefaultRegex(commands[i]);
                }
            }
        }
        /// <summary>
        /// Sets the defualt regex options
        /// </summary>
        /// <param name="settings">Regex</param>
        public static void SetDefaultRegex(Match settings)
        {
            for (int i = 0; i < settings.Groups["annotation"].Captures.Count; i++)
            {
                if (settings.Groups["annotation"].Captures[i].Value == "#")
                {
                    try
                    {
                        int.TryParse(settings.Groups["text"].Captures[i].Value, out int amount);
                        DefaultInventoryItemAmount = amount;
                        Debug.Log(amount);
                    }
                    catch
                    {
                        Debug.LogWarning("Charatures after # were not digits");
                    }
                }
                if (settings.Groups["annotation"].Captures[i].Value == "-")
                {
                    try
                    {
                        int.TryParse(settings.Groups["text"].Captures[i].Value, out int amount);
                        DefaultInventoryItemAmount = -amount;
                        Debug.Log(-amount);
                    }
                    catch
                    {
                        Debug.LogWarning("Charatures after - were not digits");
                    }
                }
                if (settings.Groups["annotation"].Captures[i].Value == "@")
                {
                    Debug.LogWarning("@ annotation not fully functional yet");
                }
            }
            for (int i = 0; i < settings.Groups["param"].Captures.Count; i++) 
            {
                try
                {
                    DefaultLibaryObject = SlashRegex.GetSlashLibarySearch(settings, 0);

                }
                catch
                {
                    Debug.LogWarning("");
                }
            }
        }
        /// <summary>
        /// Defualt amount
        /// </summary>
        public static int DefaultInventoryItemAmount { get; private set; } = 1;
        public static LibraryObjects DefaultLibaryObject { get; private set; } = LibraryObjects.InventoryItem;
        /// <summary>
        /// Default targets
        /// </summary>
        public static List<RegexTarget> DefaultTarget { get; private set; } = new List<RegexTarget>();
        public static void SetAllDefaultRegex(int size, List<int> targ, int libaryObject)
        {
            DefaultInventoryItemAmount = size;
            DefaultTarget.Clear();
            DefaultLibaryObject = (LibraryObjects)libaryObject;
            for (int i = 0; i < targ.Count; i++)
            {
                DefaultTarget.Add((RegexTarget)targ[i]);
            }
        }
    }

    namespace Structual
    {
        public struct BoolInt
        {
            public bool boolValue;
            public int intValue;
            public BoolInt(bool boolValue, int intValue)
            {
                this.boolValue = boolValue;
                this.intValue = intValue;
            }
        }
        public struct NameId
        {
            public int Id;
            public string Name;
            public NameId(string name, int id)
            {
                Name = name;
                Id = id;
            }
        }
        public struct WorldLocation
        {
            public Vector3 Location { get; private set; }
            public string Name { get; private set; }
            public WorldLocation(string name, Vector3 location)
            {
                Name = name;
                Location = location;
            }
        }
        public struct AddItemRequest
        {
            private string ItemToAdd { get; set; }
            private int AmountToAdd { get; set; }
            public AddItemRequest(string name, int add)
            {
                ItemToAdd = name;
                AmountToAdd = add;
            }
            public readonly InventoryItem GetItem()
            {
                return SetItemAmount(new InventoryItem(AllLibary.ItemLibary.SearchLibaryForTemplete(ItemToAdd)));
            }
            private readonly InventoryItem SetItemAmount(InventoryItem item)
            {
                item.Amount = AmountToAdd;
                return item;
            }
        }
        /// <summary>
        /// A class that will create a hold of data. Great if you want certain events to be held or have higher prioirty requests preformed first.
        /// </summary>
        public class QueueInfo
        {
            /// <summary>
            /// Data saved
            /// </summary>
            public readonly ushort[] DataUShort;
            /// <summary>
            /// Data saved
            /// </summary>
            public readonly bool[] DataBool;
            /// <summary>
            /// Data saved
            /// </summary>
            public readonly string[] DataString;
            /// <summary>
            /// Data saved
            /// </summary>
            public readonly float[] DataFloat;
            public readonly Effects[] effects;
            public readonly ForceKnockback Knockback;
            /// <summary>
            /// What type of data, (if required)
            /// </summary>
            public readonly QueueData DataType;
            /// <summary>
            /// What to do with the data.
            /// </summary>
            public readonly CommandRequest Request;

            private readonly int toReceive;
            private readonly int priority;
            private readonly float timeStart;
            private bool destory = false;
            public readonly string Name;
            /// <summary>
            /// Setup data to be sent when next avaliable to be sent.
            /// </summary>
            /// <param name="receive"></param>
            /// <param name="name"></param>
            /// <param name="priority"></param>
            /// <param name="request"></param>
            public QueueInfo(int receive, string name, int priority, CommandRequest request)
            {
                toReceive = receive;
                this.priority = priority;
                timeStart = Time.time;
                DataType = QueueData.Name;
                Name = name;
                Request = request;
            }
            /// <summary>
            /// Setup data to be sent when next avalibale to be sent.
            /// </summary>
            /// <param name="receive">Who is to get the data</param>
            /// <param name="priority">What is the priority. Losest priority goes first.</param>
            /// <param name="args">The data</param>
            public QueueInfo(int receive, string name, int priority, CommandRequest request, params ushort[] args)
            {
                toReceive = receive;
                this.priority = priority;
                timeStart = Time.time;
                DataUShort = args;
                DataType = QueueData.Int;
                Name = name;
                Request = request;
            }
            /// <summary>
            /// Setup data to be sent when next avalibale to be sent.
            /// </summary>
            /// <param name="receive">Who is to get the data</param>
            /// <param name="priority">What is the priority. Losest priority goes first.</param>
            /// <param name="args">The data</param>
            public QueueInfo(int receive, string name, int priority, CommandRequest request, params float[] args)
            {
                toReceive = receive;
                this.priority = priority;
                timeStart = Time.time;
                DataFloat = new float[args.Length];
                DataFloat = args;
                DataType = QueueData.Int;
                Name = name;
                Request = request;
            }
            /// <summary>
            /// Setup data to be sent when next avalibale to be sent.
            /// </summary>
            /// <param name="receive">Who is to get the data</param>
            /// <param name="priority">What is the priority. Losest priority goes first.</param>
            /// <param name="args">The data</param>
            public QueueInfo(int receive, string name, int priority, CommandRequest request, params bool[] args)
            {
                toReceive = receive;
                this.priority = priority;
                timeStart = Time.time;
                DataBool = args;
                DataType = QueueData.Bool;
                Name = name;
                Request = request;
            }
            /// <summary>
            /// Setup data to be sent when next avalibale to be sent.
            /// </summary>
            /// <param name="receive">Who is to get the data</param>
            /// <param name="priority">What is the priority. Losest priority goes first.</param>
            /// <param name="args">The data</param>
            public QueueInfo(int receive, string name, int priority, CommandRequest request, params string[] args)
            {
                toReceive = receive;
                this.priority = priority;
                timeStart = Time.time;
                DataString = args;
                DataType = QueueData.String;
                Name = name;
                Request = request;
            }
            /// <summary>
            /// Setup data to be sent when next avalibale to be sent. THIS CANNOT BE SENT OVER SERVERS.
            /// </summary>
            /// <param name="receive">Who is to get the data</param>
            /// <param name="priority">What is the priority. Losest priority goes first.</param>
            /// <param name="effect">The data</param>
            public QueueInfo(int receive, string name, int priority, Effects[] effect)
            {
                toReceive = receive;
                this.priority = priority;
                Request = CommandRequest.Attributes;
                timeStart = Time.time;
                effects = effect;
                Name = name;
            }
            /// <summary>
            /// Setup data to be sent when next avalibale to be sent. THIS CANNOT BE SENT OVER SERVERS.
            /// </summary>
            /// <param name="receive">Who is to get the data</param>
            /// <param name="priority">What is the priority. Lowest priority goes first.</param>
            /// <param name="effect">The data</param>
            public QueueInfo(int receive, string name, int priority, ForceKnockback knock)
            {
                toReceive = receive;
                this.priority = priority;
                Request = CommandRequest.Knockback;
                timeStart = Time.time;
                Knockback = knock;
                Name = name;
            }

            public int GetId()
            {
                return toReceive;
            }
            public void MarkToDestory()
            {
                destory = true;
            }
            public bool GetIsDestructable()
            {
                return destory;
            }
            public float GetStartTime()
            {
                return timeStart;
            }
            public float GetPriority()
            {
                return priority;
            }
        }
    }
    namespace Movement
    {
        /// <summary>
        /// Create UP movement, which is effected by pressing UP/DOWN. Can also be used to increase a speed value. 
        /// </summary>
        [Obsolete("Irrelivant Just use Boost in SimpleMvm class to replicate the methods this class provided", true)]
        public class Xmovement
        {
            /// <summary>
            /// Extra speed when returning a value requiring speed.
            /// </summary>
            private float FowardSpeedBoost { get; set; }
            /// <summary>
            /// The time when <see cref="XUPSetMaxTime()"/> was called
            /// </summary>
            private float TimeStartMax { get; set; }
            /// <summary>
            /// The time when <see cref="SetDownTime()"/>
            /// </summary>
            private float TimeStartMin { get; set; }
            /// <summary>
            /// The amount of time it takes to slow down
            /// </summary>
            private float TimeToSlowDown { get; set; }
            /// <summary>
            /// The amount of time set each time a request is made to achieve min (0) speed.
            /// </summary>
            private float BaseTimeTillMinned { get; set; }
            /// <summary>
            /// The amount of time set each time a request is made to achieve max speed.
            /// </summary>
            private float BaseTimeTillMaxed { get; set; }
            /// <summary>
            /// A value that stores a value between 0-1. Is often returned with <see cref="FowardSpeedBoost"/> Example: <code>return FowardForce * FowardSpeedBoost</code>
            /// </summary>
            private float FowardForce { get; set; } //Recording value
            /// <summary>
            /// A value that stores your backwords movespeed. This value does nothing in the object.
            /// </summary>
            private float BackForce { get; set; }
            /// <summary>
            /// A flag used to check if you have pressed "move backwords"
            /// </summary>
            private bool IsPressingDown { get; set; } = false;
            /// <summary>
            /// Minimum foward speed
            /// </summary>
            private float ClampMin { get; set; }
            /// <summary>
            /// Stores a value between 0-1 based on when you released your "move foward" key. I'll let you figure out how this works<br></br>
            /// <see cref="SetDownTime"/>
            /// <code> Poland = Mathf.Clamp((Time.time - TimeStartMax) / BaseTimeTillMaxed, 0, 1);</code>
            /// <see cref="RequestSpeedDown"/>
            /// <code>FowardForce = Poland * Mathf.Min(1, (Mathf.Max(0, TimeToSlowDown - (rate * elapsedTime))));</code>
            /// </summary>
            private float Poland { get; set; }

            /// <summary>
            /// Controls the Foward/Backword movement. Move backwords at a flat rate
            /// </summary>
            /// <param name="foward">Boost Foward Movement</param>
            /// <param name="back">Boost Foward Movement</param>
            /// <param name="timeUP">Time to speed up</param>
            /// <param name="timeDown">Time to speed down</param>
            /// <param name="clamp">Minimum speed on pressing the button. (value from 0.0-1.0)</param>
            public Xmovement(float foward, float back, float timeUP, float timeDown, float clamp)
            {
                FowardSpeedBoost = foward;
                BackForce = back;
                BaseTimeTillMaxed = timeUP;
                BaseTimeTillMinned = timeDown;
                ClampMin = clamp;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Get Foward Speed Boost</returns>
            public float GetFowardSpeedBoost()
            {
                return FowardSpeedBoost;
            }
            /// <summary>
            /// Sets the amount of time needed to speed up.
            /// Used on KeyPressDown.
            /// <code> TimeTillMaxed = Time.time + BaseTimeTillMaxed;</code>
            /// <code> TimeStartMax = Time.time;</code>
            /// </summary>
            public void XUPSetMaxTime()
            {
                TimeStartMax = Time.time;
            }
            /// <summary>
            /// Gets the (current time - TimeStartMax)/BaseTimeTillMaxed in order to figure out how fast you can go. 
            /// Used on held KeyPress
            /// <code>float temp = ((Time.time - TimeStartMax) / BaseTimeTillMaxed) * Speed;</code>
            /// </summary>
            /// <returns>A value between 0 and 1</returns>
            public float XUPRequestSpeedUp()
            {
                float temp = (Time.time - TimeStartMax) / BaseTimeTillMaxed;
                FowardForce = Mathf.Clamp(temp, ClampMin, 1);
                return FowardForce * FowardSpeedBoost;
            }

            public float XDownRequestSpeedUp()
            {
                float temp = (Time.time - TimeStartMax) / BaseTimeTillMaxed;
                FowardForce = Mathf.Clamp(temp, ClampMin, 1);
                return FowardForce;
            }

            /// <summary>
            /// Flags backwards movement. Usefull to stop Foward movement.
            /// </summary>
            /// <param name="isReleasingKey"></param>
            /*
            public void XDownSetMovement(bool isReleasingKey, bool isPressingKey)
            {
                if (isReleasingKey == true)
                {
                    IsPressingDown = false;
                }
                else if (isPressingKey)
                {

                }
                else
                {
                    IsPressingDown = true;
                }


            }
            */
            /// <summary>
            /// Returns BackForce value
            /// </summary>
            /// <returns>BackForce</returns>
            public float XDownGetBackForce()
            {
                return BackForce;

            }
            /// <summary>
            /// Sets the slowdown time.
            /// </summary>
            public void SetDownTime()
            {
                Poland = Mathf.Clamp((Time.time - TimeStartMax) / BaseTimeTillMaxed, 0, 1);
                TimeStartMin = Time.time;
                TimeToSlowDown = FowardForce;

            }
            /// <summary>
            /// In order to reduce lag (I think), This will run a check to ensure that <see cref="RequestSpeedDown"/> doesn't run when not needed to.
            /// </summary>
            /// <returns>True/False</returns>
            public bool CanRequestSpeedDown()
            {
                return ((Time.time - TimeStartMin) <= BaseTimeTillMinned);
            }
            /// <summary>
            /// Returns how fast you can remove on a released keypress.
            /// </summary>
            /// <returns>A value between 0 and 1</returns>
            public float RequestSpeedDown()
            {
                float elapsedTime = Time.time - (TimeStartMin);
                float rate = TimeToSlowDown / BaseTimeTillMinned;
                FowardForce = Poland * Mathf.Min(1, (Mathf.Max(0, TimeToSlowDown - (rate * elapsedTime))));
                return FowardForce * FowardSpeedBoost;
            }
            /// <summary>
            /// Sets the amount of time needed to slow down,
            /// Used on KeyPressUp.
            /// </summary>
            /// <summary>
            /// Set the speed boost for speed up and speed down.
            /// </summary>
            /// <param name="math">0 = Direct, 1 = New, 2 = Multiply</param>
            /// <param name="foward">Foward boost</param>
            /// <param name="back">Back boost</param>
            public void SetSpeedBoost(int math, float foward, float back)
            {
                switch (math)
                {
                    case 0:
                        FowardSpeedBoost = foward;
                        BackForce = back;
                        break;
                    case 1:
                        FowardSpeedBoost += foward;
                        BackForce += back;
                        break;
                    case 2:
                        FowardSpeedBoost *= foward;
                        BackForce *= back;
                        break;
                    default:
                        break;
                }
            }
            /// <summary>
            /// Copy constructor for Xmovement
            /// </summary>
            /// <param name="other">The Xmovement object to copy from</param>
            public Xmovement(Xmovement other)
            {
                // Copy all properties from the other instance
                this.FowardSpeedBoost = other.FowardSpeedBoost;
                this.TimeStartMax = other.TimeStartMax;
                this.TimeStartMin = other.TimeStartMin;
                this.TimeToSlowDown = other.TimeToSlowDown;
                this.BaseTimeTillMinned = other.BaseTimeTillMinned;
                this.BaseTimeTillMaxed = other.BaseTimeTillMaxed;
                this.FowardForce = other.FowardForce;
                this.BackForce = other.BackForce;
                this.IsPressingDown = other.IsPressingDown;
                this.ClampMin = other.ClampMin;
                this.Poland = other.Poland;
            }
        }
        /// <summary>
        /// Control ALL areal movement in 1 object.
        /// </summary>
        [Obsolete("Unusable using Unity's force system, ", false)]
        public class AirMove
        {
            /// <summary>
            /// Controlls your force moving fowards. Using positive and negative numbers.
            /// </summary>
            private float ForwardForce { get; set; }
            /// <summary>
            /// Multiplied into your DeltaZ value. This is a value that has a min of 1.
            /// </summary>
            private float StillForce { get; set; }
            /// <summary>
            /// Controlls how fast you fall. Weither it be looking down via <see cref="TransferFowardDown(float, float, float, float)"/> or Crouching.
            /// </summary>
            private float DownForce { get; set; }
            /// <summary>
            /// Crouching Downwards float.
            /// </summary>
            private float BoostDown { get; set; }
            /// <summary>
            /// The max amount of down force
            /// </summary>
            private float DownForceMax { get; set; }
            private int DownForceType { get; set; } = 0;
            /// <summary>
            /// The max amount of foward force.
            /// </summary>
            private float ForwardForceMax { get; set; }
            /// <summary>
            /// The amount of your foward speed <see cref="Xmovement.FowardForce"/> * <see cref="Player.GetSpeed"/> that gets added to areal movement
            /// </summary>
            private float Boost { get; set; }
            /// <summary>
            /// The boost given to LEFT/RIGHT movement.
            /// </summary>
            private float SideBoost { get; set; }
            /// <summary>
            /// Is your LEFT/RIGHT movement force.
            /// </summary>
            private float Side { get; set; }
            /// <summary>
            /// Is your Left/right movement force when switching keys.
            /// </summary>
            private float SideTimer { get; set; }
            private float BaseSideTimer { get; set; }
            /// <summary>
            /// Max side boost
            /// </summary>
            private float SideMax { get; set; }
            /// <summary>
            /// Ground Pound bonus value.
            /// </summary>
            private float PoundBonus { get; set; }
            /// <summary>
            /// Stores an INT version of your PoundBonus.
            /// </summary>
            private int PresetPound { get; set; }
            /// <summary>
            /// External Force playing against you that adds to your forward Force.
            /// </summary>
            private float ExternalForce { get; set; }
            private float DownForceResistance { get; set; } = 0;
            private int Direction { get; set; } = 0;
            private float LeftRightTransition { get; set; } = 0;
            private float DirPow { get; set; } = 0;
            private float DefaultDecayRate { get; set; } = 0.8f;

            public void DirectionChangePress(float power)
            {
                DirPow += power * Time.deltaTime;
                DirPow = Mathf.Clamp(DirPow, -1, 1);
            }
            public void DirectionalChangeNoPress(float power)
            {
                if (DirPow != 0)
                {
                    float math = Mathf.Abs(DirPow) * power * Time.deltaTime;
                    if (DirPow > 0)
                    {
                        DirPow = Mathf.Max(DirPow - math, 0);
                    }
                    if (DirPow < 0)
                    {
                        DirPow = Mathf.Min(math, 0);
                    }
                }
            }

            /// <summary>
            /// Get the initial Pound.
            /// </summary>
            /// <returns>PresetPound value.</returns>
            public int GetPresetPound()
            {
                return PresetPound;
            }
            public void SetLastDirectionPressed(int direction)
            {
                Direction = direction;
            }
            public int GetLestDirection()
            {
                return Direction;
            }
            /// <summary>
            /// Upgrades Ground Pound Speed
            /// </summary>
            /// <param name="amount">Increase Amount * 0.000f</param>
            public void SetAddPoundBonus(float amount)
            {
                PoundBonus += amount * 0.0001f;
            }
            /// <summary>
            /// Gets your foward force
            /// </summary>
            /// <returns>Foward force value</returns>
            public float GetFowardForce(bool isGrounded)
            {
                CheckGrounded(isGrounded);
                return ForwardForce;
            }
            /// <summary>
            /// Gets your foward force
            /// </summary>
            /// <returns>Foward force value</returns>
            public float GetStillForce(bool isGrounded)
            {
                CheckGrounded(isGrounded);
                return StillForce + 1;
            }
            /// <summary>
            /// Gets Down Force.
            /// </summary>
            /// <param name="isGrounded"></param>
            /// <returns>Down Force</returns>
            public float GetDownForce(bool isGrounded)
            {
                CheckGrounded(isGrounded);
                return DownForce;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Push Force (left/right additional force) </returns>
            public float GetPushForce(bool isGrounded)
            {
                CheckGrounded(isGrounded);
                return Side * DirPow;
            }
            public void ReleaseSideKey(bool isGrounded)
            {
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Apply Down Force.
            /// </summary>
            /// <param name="velocity"></param>
            /// <param name="isGrounded"></param>
            public void ApplyDown(float velocity, bool isGrounded)
            {
                DownForce = Mathf.Min(DownForce + Mathf.Abs(velocity) * BoostDown * Time.deltaTime, DownForceMax);
                DownForce -= DownForceResistance * Time.deltaTime;
                if (DownForce < 0)
                {
                    DownForce = 0;
                }
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Decrease speed at a steady rate.
            /// </summary>
            /// <param name="rate"></param>
            /// <param name="isGrounded"></param>
            public void UpdateForceDecay(bool isGrounded)
            {
                // Smooth decay for forward force
                ForwardForce = Mathf.Lerp(ForwardForce, 0, DefaultDecayRate * Time.deltaTime);

                //Smooth decay for forward force
                Side = Mathf.Lerp(Side, 0, DefaultDecayRate * Time.deltaTime);

                // Decay still force
                StillForce = Mathf.Lerp(StillForce, 0, DefaultDecayRate * Time.deltaTime);
                //Debug.Log($"FowardForce: {ForwardForce}, StillForce: {StillForce}, decrease rate: {rate}");
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Decrease speed at a steady rate.
            /// </summary>
            /// <param name="rate"></param>
            /// <param name="isGrounded"></param>
            public void UpdateForceDecay(bool isGrounded, float decayRate)
            {
                // Smooth decay for forward force
                ForwardForce = Mathf.Lerp(ForwardForce, 0, decayRate * Time.deltaTime);

                //Smooth decay for forward force
                Side = Mathf.Lerp(Side, 0, decayRate * Time.deltaTime);

                // Decay still force
                StillForce = Mathf.Lerp(StillForce, 0, decayRate * Time.deltaTime);
                //Debug.Log($"FowardForce: {ForwardForce}, StillForce: {StillForce}, decrease rate: {rate}");
                CheckGrounded(isGrounded);
            }
            public void SetDefaultDecayRate(float rate)
            {
                rate = Mathf.Max(rate, 0);
                DefaultDecayRate = rate;
            }
            /// <summary>
            /// Apply down force
            /// </summary>
            /// <param name="velocity">Speed</param>
            /// <param name="flat">Flat rate</param>
            /// <param name="isGrounded">is grounded</param>
            public void ApplyDown(float velocity, float flat, bool isGrounded)
            {
                DownForce = Mathf.Min(DownForce + Mathf.Abs(velocity) * BoostDown * Time.deltaTime + flat * Time.deltaTime, DownForceMax);
                if (DownForce > flat * 1.1f)
                {
                    DownForce -= DownForceResistance * Time.deltaTime;
                }
                if (DownForce < 0)
                {
                    DownForce = 0;
                }
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Apply downforce instantly
            /// </summary>
            /// <param name="isGrounded">Is grounded?</param>
            public void ApplyDownInstant(bool isGrounded)
            {
                DownForce = GetForceMax(false);
                DownForce = Mathf.Max(DownForce - DownForceResistance * 10, 0);
                CheckGrounded(isGrounded);
            }

            /// <summary>
            /// Apply velocity while in air.
            /// </summary>
            /// <param name="velocity">DeltaZ</param>
            /// <param name="isGrounded">Is grounded</param>
            public void ApplySpeed(float velocity, float speed, bool isGrounded)
            {
                float direction = Mathf.Sign(velocity);
                float absVelocity = Mathf.Abs(velocity);

                ForwardForce += (absVelocity * Boost * direction) * Time.deltaTime;
                ForwardForce = Mathf.Clamp(ForwardForce, -ForwardForceMax, ForwardForceMax);

                StillForce += (absVelocity * Boost / speed) * Time.deltaTime;
                StillForce = Mathf.Clamp(StillForce, 0, ForwardForceMax);

                ForwardForce += ExternalForce * Time.deltaTime;
                ForwardForce = Mathf.Clamp(ForwardForce, -ForwardForceMax, ForwardForceMax);

                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Apply velocity to left/right
            /// </summary>
            /// <param name="velocity">DeltaXv</param>
            /// <param name="isGrounded"></param>
            public void ApplySideBoost(float velocity, float speed, bool isGrounded)
            {
                Side += (velocity * SideBoost / speed) * Time.deltaTime;
                Side += (ExternalForce / 2) * Time.deltaTime;
                Side = Mathf.Clamp(Side, -SideMax, SideMax);
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Checks if your grounded
            /// </summary>
            /// <param name="isGrounded">check grounded</param>
            public void CheckGrounded(bool isGrounded)
            {
                if (isGrounded == true)
                {
                    ForwardForce = 0;
                    DownForce = 0;
                    Side = 0;
                    StillForce = 0;
                    DirPow = 0;
                }
            }
            public void ApplyDownResitance(bool isGrounded)
            {
                DownForce = Mathf.Max(DownForce - DownForceResistance * Time.deltaTime, 0);
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Use to reduce your FowardForce
            /// </summary>
            /// <param name="isGrounded">check grounded</param>
            /// <param name="amount">the amount your decreasing by per second</param>
            /// <param name="isPressing">Decrease a sudden amount via amount</param>
            /// <param name="maxBack">Max backwards movement.</param>
            public void PressDown(bool isGrounded, bool isPressing, float amount, float maxBack)
            {
                if (isPressing)
                {
                    ForwardForce = (ForwardForce * amount - 1);
                }
                else
                {
                    ForwardForce -= amount * Time.deltaTime;
                    ForwardForce = Mathf.Max(ForwardForce, maxBack);
                }
                CheckGrounded(isGrounded);
            }
            /// <summary>
            /// Creates a object to store info on a player's movement in air
            /// </summary>
            /// <param name="boost">Foward mommentum boost. Use a non-negative float</param>
            /// <param name="boostDown">Down mommentum boost. Use a non-negative float</param>
            /// <param name="sideBoost">Side boost increase</param>
            /// <param name="poundBonus">Bonus for ground pounding</param>
            /// <param name="maxDown">Max speed moving down. (default 170)</param>
            public AirMove(float boost, float boostDown, float sideBoost, float poundBonus, float maxDown, float maxFForce)
            {
                ForwardForce = 0;
                DownForce = 0;
                Side = 0;
                BoostDown = boostDown;
                SideBoost = sideBoost;
                Boost = boost;
                PoundBonus = poundBonus * 0.0001f;
                SideMax = maxFForce * 0.65f;
                PresetPound = (int)PoundBonus;
                DownForceMax = maxDown;
                ForwardForceMax = maxFForce;
            }

            /// <summary>
            /// Increase Downspeed
            /// </summary>
            /// <param name="Direction">Your charatures looking angle</param>
            /// <param name="intencity">The higher the number, the less impactfull looking down with the mouse is.</param>
            /// <param name="limit">The point at which the mouse will no longer impact DownSpeed.</param>
            /// <param name="playerSpeed">Your current speed</param>
            public void TransferFowardDown(float Direction, float intencity, float limit, float playerSpeed)
            {
                float temp = (Direction) / (intencity);
                float temp2 = (Direction) / (intencity * 0.50f);

                if (Direction > limit && Direction < 120f)
                {
                    DownForce = Mathf.Max(DownForce, 0);
                    float moveValue = Mathf.Abs(ForwardForce) * temp * Time.deltaTime;
                    ForwardForce -= (moveValue / playerSpeed) * (intencity * 0.01f);
                    DownForce += moveValue;
                }
                if (Direction < (limit + 250) && Direction > 180f)
                {
                    float temp3 = DownForce;
                    float moveValue = Mathf.Abs(ForwardForce) * temp2 * Time.deltaTime * -1;
                    DownForce += moveValue;
                    temp3 = Mathf.Max(temp3 - DownForce, 0);
                    if (DownForce > 0)
                    {
                        ForwardForce += temp3 * 0.1f;
                    }
                }
            }
            /// <summary>
            /// Returns the value of the GroundPound
            /// </summary>
            /// <param name="bonus">BONUS</param>
            /// <returns>PoundBonus</returns>
            public float GetGroundPound(float bonus)
            {
                return PoundBonus * bonus;
            }
            /// <summary>
            /// Use for making the UI version of GroundPound looks nicer
            /// </summary>
            /// <returns>PoundBonus</returns>
            public int GetVisualGroundPound(float sub)
            {
                return (int)((PoundBonus * 1000f) - sub);
            }
            /// <summary>
            /// Change BoostDown Value
            /// </summary>
            /// <param name="bonus">Amount</param>
            public void SetDownBonus(float bonus)
            {
                BoostDown = bonus;
            }
            /// <summary>
            /// Multiply downforce by an amount
            /// </summary>
            /// <param name="amount">A value above 0.0</param>
            public void SetReducedDownForce(float amount)
            {
                DownForce *= amount;
            }
            /// <summary>
            /// Returns a A float of FowardForce or DownForce
            /// </summary>
            /// <param name="type">Choose between Foward Force Max (true) or Doown Force Max (false)</param>
            /// <returns>A float of FowardForce or DownForce</returns>
            public float GetForceMax(bool type)
            {
                if (type == true)
                {
                    return ForwardForceMax;
                }
                return DownForceMax;
            }
            /// <summary>
            /// Get Force Boost
            /// </summary>
            /// <returns><see cref="Boost"/></returns>
            public float GetForceBoost()
            {
                return Boost;
            }
            /// <summary>
            /// New Additional 
            /// </summary>
            /// <param name="velocity"></param>
            public void AddAdditionalForce(float velocity)
            {
                ExternalForce = velocity;
            }
            /// <summary>
            /// Decrease External force at a rate
            /// </summary>
            /// <param name="decreaseRate">Decrease Rate<code>Time.deltaTime * decreaseRate</code></param>
            public void DecreaseExternalForce(float decreaseRate)
            {
                if (ExternalForce > 0)
                {
                    ExternalForce -= (Time.deltaTime * decreaseRate);
                }
                else
                {
                    ExternalForce = 0;
                }
            }
            /// <summary>
            /// Set the max forward force.
            /// </summary>
            /// <param name="amount"></param>
            public void SetMaxFForce(float amount)
            {
                ForwardForceMax = amount;
            }
            /// <summary>
            /// Set Down Force Type
            /// </summary>
            /// <param name="amount">-1 = Instant</param>
            public void SetDForceType(int amount)
            {
                if (amount == -1)
                {
                    DownForceType = 1;
                }
                else
                {
                    PoundBonus = amount * 0.0001f;
                }

            }
            /// <summary>
            /// Set Bonus speed while the player holds foward
            /// </summary>
            /// <param name="amount">1</param>
            public void SetBonusFspeed(float amount)
            {
                Boost = amount;
                SideBoost = amount * 0.7f;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Get Down Force</returns>
            public int GetDForceType()
            {
                return DownForceType;
            }
            /// <summary>
            /// Set the max Down Force
            /// </summary>
            /// <param name="amount">Down Force Amount</param>
            public void SetMaxDForce(float amount)
            {
                DownForceMax = amount;
            }
            /// <summary>
            /// A value that will constalnly increase/decrease down force.<br></br>Unless set to 0. Which then this does nothing
            /// </summary>
            /// <param name="amount">The amount to adjust by</param>
            public void SetDownForceResistance(float amount)
            {
                DownForceResistance = amount;
            }
            /// <summary>
            /// Reduce down force by Multiplying by <paramref name="amount"/>
            /// </summary>
            /// <param name="amount">A value between 0.0 and 1.0</param>
            public void ReduceDownForce(float amount)
            {
                Math.Clamp(amount, 0, 1);
                if (DownForce > 0)
                {
                    DownForce *= amount;
                }
            }
            public AirMove(AirMove other)
            {
                // Copy all properties from the other instance
                this.ForwardForce = other.ForwardForce;
                this.StillForce = other.StillForce;
                this.DownForce = other.DownForce;
                this.BoostDown = other.BoostDown;
                this.DownForceMax = other.DownForceMax;
                this.DownForceType = other.DownForceType;
                this.ForwardForceMax = other.ForwardForceMax;
                this.Boost = other.Boost;
                this.SideBoost = other.SideBoost;
                this.Side = other.Side;
                this.SideMax = other.SideMax;
                this.PoundBonus = other.PoundBonus;
                this.PresetPound = other.PresetPound;
                this.ExternalForce = other.ExternalForce;
                this.DownForceResistance = other.DownForceResistance;
            }

        }
        /// <summary>
        /// Creates a stamninia bar.
        /// </summary>
        public class Stamania
        {
            /// <summary>
            /// A gauge value. Stores your current stamnina
            /// </summary>
            private float Stanama { get; set; }
            /// <summary>
            /// Your Max Stamnana
            /// </summary>
            private float StanamaMax { get; set; }
            /// <summary>
            /// The Original max stamnana
            /// </summary>
            private float StanamaBaseMax { get; set; }
            /// <summary>
            /// The Rate at which your stamnana refills
            /// </summary>
            private float RefillRate { get; set; }
            /// <summary>
            /// The preset delay before you gain sprint back
            /// </summary>
            private float RefillDelayBase { get; set; }
            /// <summary>
            /// The time untill it refills the Stanama
            /// </summary>
            private float RefillDelay { get; set; }
            /// <summary>
            /// Main Speed Boost. Used for when Stanama is greater than <see cref="HighDiffCut"/>. This value should be smaller than <see cref="HighDiffEff"/>
            /// </summary>
            private float SpeedEffectivity { get; set; }
            /// <summary>
            /// Low Stanama Speed Boost. Grants a slightly higher speed boost stanama is less than <see cref="HighDiffCut"/>. This value should be greater than <see cref="SpeedEffectivity"/>
            /// </summary>
            private float HighDiffEff { get; set; }
            /// <summary>
            /// The percent (a float value from 0-1) from when the speed increase is switched from <see cref="SpeedEffectivity"/> to <see cref="HighDiffEff"/>
            /// </summary>
            private float HighDiffCut { get; set; }
            /// <summary>
            /// Setup a object for storing stamnia and data related to it.
            /// </summary>
            /// <param name="stanMax">Max amount. (usually 100)</param>
            /// <param name="refillRate">Refill rate of stamina</param>
            /// <param name="refillDelay">Delay after pressing Sprint</param>
            /// <param name="Speedeffective">Bonus for Speed if got an upgrade of sorts</param>
            public Stamania(float stanMax, float refillRate, float refillDelay, float Speedeffective, float highDiffEff, float highDiffCut)
            {
                StanamaMax = stanMax;
                StanamaBaseMax = stanMax;
                Stanama = stanMax;
                RefillRate = refillRate;
                RefillDelayBase = refillDelay;
                SpeedEffectivity = Speedeffective;
                HighDiffEff = highDiffEff;
                HighDiffCut = highDiffCut;
            }
            public Stamania(Stamania other)
            {
                // Copy all properties from the other instance
                this.Stanama = other.Stanama;
                this.StanamaMax = other.StanamaMax;
                this.StanamaBaseMax = other.StanamaBaseMax;
                this.RefillRate = other.RefillRate;
                this.RefillDelayBase = other.RefillDelayBase;
                this.RefillDelay = other.RefillDelay;
                this.SpeedEffectivity = other.SpeedEffectivity;
                this.HighDiffEff = other.HighDiffEff;
                this.HighDiffCut = other.HighDiffCut;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Float value of (False)SpeedEffectivity or (True)HighDiffEff</returns>
            public float GetSpeedType(bool type)
            {
                if (type == true)
                {
                    return HighDiffEff;
                }
                return SpeedEffectivity;
            }
            /// <summary>
            /// <code> new float[] { Stanama, StanamaMax, RefillRate, RefillDelay, SpeedEffectivity };</code>
            /// </summary>
            /// <returns>Returns an float array with the data mentioned above.</returns>
            public float[] GetStaminaData()
            {
                return new float[] { Stanama, StanamaMax, RefillRate, RefillDelay, SpeedEffectivity };
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Stanama, a float value</returns>
            public float GetStamina()
            {
                return Stanama;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Max Stamina, a float value</returns>
            public float GetStaminaMax()
            {
                return StanamaMax;
            }
            /// <summary>
            /// Get Speed Adjustments. You can use a flat rate or a gradual decrease.
            /// </summary>
            /// <param name="amount">Decrease amount. Is not time based. Instead it uses <code> Stanama -= amount * Time.deltaTime;</code></param>
            /// <param name="type">0 = Gradual decrease, 1 = Flat Rate.</param>
            /// <returns></returns>
            public float GetAdjustment(float amount, int type)
            {
                if (type == 0)
                {

                    if (SetStamniaRequest(1) == true)
                    {
                        RefillDelay = Time.time + RefillDelayBase;
                        Stanama -= amount * Time.deltaTime;
                        float temp = Mathf.Abs(Stanama) / StanamaMax;
                        if (temp > HighDiffCut)
                        {
                            temp *= HighDiffEff;
                        }
                        else
                        {
                            temp *= SpeedEffectivity;
                        }
                        temp *= Mathf.Clamp((StanamaMax / StanamaBaseMax) * 0.75f, 1f, 1.5f);

                        return temp + 1;

                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (type == 1)
                {
                    if (SetStamniaRequest(1) == true)
                    {
                        RefillDelay = Time.time + RefillDelayBase;
                        Stanama -= amount * Time.deltaTime;
                        return SpeedEffectivity;
                    }
                    else
                    {
                        return 1;
                    }
                }
                return 1;
            }
            /// <summary>
            /// Appllies refill at a rate
            /// </summary>
            /// <param name="adjust">Default 1: Use to multiply the amount to refill</param>
            public void ApplyRefill(float adjust)
            {
                if (Time.time > RefillDelay && Stanama < StanamaMax)
                {
                    Stanama += RefillRate * adjust * Time.deltaTime;
                }
                else if (Stanama > StanamaMax)
                {
                    Stanama = StanamaMax;
                }
            }
            /// <summary>
            /// Set the refill rate
            /// </summary>
            /// <param name="rate">A value in seconds</param>
            public void SetRefillRate(float rate)
            {
                RefillRate = rate;
            }
            /// <summary>
            /// See's if you can get stamna
            /// </summary>
            /// <param name="amount"></param>
            /// <returns></returns>
            public bool SetStamniaRequest(float amount)
            {
                if (amount > Stanama)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            /// <summary>
            /// New Max Stamnina
            /// </summary>
            /// <param name="amount"></param>
            public void SetAddMaxStaminia(float amount)
            {
                StanamaMax += amount;
            }


        }
        /// <summary>
        /// Simple Movement with slowdown + speedup.
        /// </summary>
        public class SimpleMvm
        {
            /// <summary>
            /// The amount of time progressed to reach max speed
            /// </summary>
            private float BaseTimeTillMaxed { get; set; }
            /// <summary>
            /// The amount of time set each time a request is made to achieve max speed.
            /// </summary>
            private float BaseTimeTillMaxedBase { get; set; }
            /// <summary>
            /// The amount of time progressed till minimum (0) speed.
            /// </summary>
            private float BaseTimeTillMinned { get; set; }
            /// <summary>
            /// The amount of time set each time a request is made to achieve min (0) speed.
            /// </summary>
            private float BaseTimeTillMinnedBase { get; set; }
            /// <summary>
            /// Can be used to speed up a direction, Is not fully implemented, do not use.
            /// </summary>
            private float Speed { get; set; } = 1; // How fast you move. use for effects.
            private float TimeTillMaxed { get; set; } = 0;
            /// <summary>
            /// The Time when <see cref="SetMaxTime()"/> was called
            /// </summary>
            private float TimeStartMax { get; set; } = 0;
            /// <summary>
            /// The Time when <see cref="SetDownTime()"/> was called.
            /// </summary>
            private float TimeStartMin { get; set; } = 0;
            /// <summary>
            /// The Time when you fully slowdown.
            /// </summary>
            private float TimeTillMinned { get; set; } = 0;
            /// <summary>
            /// This Value is used so if a player was to release a movement key early while not at max speed, it would not set them to max speed. Used in <see cref="RequestSpeedDown"/>
            /// </summary>
            private float OldPercent { get; set; }
            private float Rotation { get; set; }
            /// <summary>
            /// Minimum speed when button pressed.
            /// </summary>
            private float ClampMin { get; set; }
            public float Boost { get; private set; }
            private MoveStates LastMoveState { get; set; }

            /// <summary>
            /// Copy constructor
            /// </summary>
            /// <param name="other"></param>
            public SimpleMvm(SimpleMvm other)
            {
                // Time progression and speed control
                this.BaseTimeTillMaxed = other.BaseTimeTillMaxed;
                this.BaseTimeTillMaxedBase = other.BaseTimeTillMaxedBase;
                this.BaseTimeTillMinned = other.BaseTimeTillMinned;
                this.BaseTimeTillMinnedBase = other.BaseTimeTillMinnedBase;

                // Speed modifiers and timing
                this.Speed = other.Speed;
                this.TimeTillMaxed = other.TimeTillMaxed;
                this.TimeStartMax = other.TimeStartMax;
                this.TimeStartMin = other.TimeStartMin;
                this.TimeTillMinned = other.TimeTillMinned;

                // Movement state tracking
                this.OldPercent = other.OldPercent;
                this.Rotation = other.Rotation;
                this.ClampMin = other.ClampMin;
            }
            /// <summary>
            /// Create a single direction movement for a single button.
            /// </summary>
            /// <param name="timeMax"> Time it takes to fully accelerate</param>
            /// <param name="timeDown">Time it takes to fully slow down</param>
            /// <param name="clamp">Minimum speed on pressing the button. (value from 0.0-1.0)</param>
            /// <param name="boost">Individual Direction Boost. (Default is 1)</param>
            public SimpleMvm(float timeMax, float timeDown, float clamp = 0.36f, float boost = 1)
            {
                BaseTimeTillMaxed = timeMax;
                BaseTimeTillMinned = timeDown;
                BaseTimeTillMaxedBase = BaseTimeTillMaxed;
                BaseTimeTillMinnedBase = BaseTimeTillMinned;
                ClampMin = clamp;
                Boost = boost;
            }
            /// <summary>
            /// Sets the amount of time needed to speed up.
            /// Used on KeyPressDown.
            /// <code> TimeTillMaxed = Time.time + BaseTimeTillMaxed;</code>
            /// <code> TimeStartMax = Time.time;</code>
            /// </summary>
            public void SetMaxTime()
            {
                TimeTillMaxed = Time.time + BaseTimeTillMaxed;
                TimeStartMax = Time.time;
            }
            /// <summary>
            /// Gets the (current time - TimeStartMax)/BaseTimeTillMaxed in order to figure out how fast you can go. 
            /// Used on held KeyPress
            /// <code>float temp = ((Time.time - TimeStartMax) / BaseTimeTillMaxed) * Speed;</code>
            /// </summary>
            /// <returns>A value between 0 and 1</returns>
            public float GetSpeedUp()
            {
                float temp = ((Time.time - TimeStartMax) / BaseTimeTillMaxed);
                Rotation = temp;

                return Mathf.Clamp(ClampMin + temp * (1 - ClampMin), ClampMin, 1);
            }
            /// <summary>
            /// Sets the amount of time needed to slow down,
            /// Used on KeyPressUp.
            /// </summary>
            public void SetMinTime()
            {
                float temp = ((Time.time - TimeStartMax) / BaseTimeTillMaxed);
                temp = Mathf.Clamp(ClampMin + temp * (1 - ClampMin), ClampMin, 1);
                OldPercent = temp;
                TimeTillMinned = Time.time + (BaseTimeTillMinned * temp);
                TimeStartMin = Time.time;
            }
            /// <summary>
            /// This will run a check to ensure that <see cref="RequestSpeedDown"/> doesn't run when not needed to.
            /// </summary>
            /// <returns>True/False</returns>
            public bool GetCanRequestSpeedDown()
            {
                return Time.time <= TimeTillMinned;
            }
            /// <summary>
            /// Gets the (current time - TimeStartMin)/BaseTimeTillMaxed in order to figure out how fast you can go. 
            /// Used on held KeyPress
            /// <code>float temp = ((Time.time - TimeStartMin) / BaseTimeTillMinned) * Speed;</code>
            /// </summary>
            /// <returns>A value between 0 and 1</returns>
            public float GetSpeedDown()
            {
                float temp = ((Time.time - TimeStartMin) / BaseTimeTillMinned) * Speed;
                temp = Mathf.Clamp(temp, ClampMin, 1);
                return Mathf.Max(OldPercent - temp, 0);
            }
            /// <summary>
            /// Changes how long it takes to accelerate
            /// </summary>
            /// <param name="reset">True = Resets to base value</param>
            /// <param name="mathType">0 = Multiply, 1 = New, 2 = Directly Make BaseTimeTillMaxed = value</param>
            /// <param name="value">Change the value by this much</param>
            public void SetBaseTimeMaxed(bool reset, int mathType, float value)
            {
                if (reset)
                {
                    BaseTimeTillMaxed = BaseTimeTillMaxedBase;
                }
                if (mathType == 0)
                {
                    BaseTimeTillMaxed *= value;
                }
                if (mathType == 1)
                {
                    BaseTimeTillMaxed += value;
                }
                else
                {
                    BaseTimeTillMaxed = value;
                }
            }
            /// <summary>
            /// Changes how long it takes to decaccelerate
            /// </summary>
            /// <param name="reset">True = Resets to base value</param>
            /// <param name="mathType">0 = Multiply, 1 = New, 2 = Directly Make BaseTimeTillMaxed = value</param>
            /// <param name="value">Change the value by this much</param>
            public void SetbaseTimeMinned(bool reset, int mathType, float value)
            {
                if (reset)
                {
                    BaseTimeTillMinned = BaseTimeTillMinnedBase;
                }
                if (mathType == 0)
                {
                    BaseTimeTillMinned *= value;
                }
                if (mathType == 1)
                {
                    BaseTimeTillMinned += value;
                }
                else
                {
                    BaseTimeTillMinned = value;
                }
            }
            public void AutoMoveSystem(MoveStates state)
            {
                if (state == MoveStates.OnPress)
                {
                    SetMaxTime();
                    LastMoveState = state;
                }
                if (state == MoveStates.OnRelease)
                {
                    SetMinTime();
                    LastMoveState = state;
                }
                if (state == MoveStates.None && LastMoveState == MoveStates.OnPress)
                {
                    SetMinTime();
                    LastMoveState = MoveStates.OnRelease;
                }
                if (state == MoveStates.OnHold && LastMoveState == MoveStates.OnRelease)
                {
                    SetMaxTime();
                    LastMoveState = MoveStates.OnHold;
                }
            }
            public float GetDelta()
            {
                if (LastMoveState == MoveStates.OnPress)
                {
                    return GetSpeedUp();
                }
                if (LastMoveState == MoveStates.OnRelease)
                {
                    return GetSpeedDown() * -1;
                }
                return 0f;
            }
            public float GetDelta(AirMovement move)
            {
                if (LastMoveState == MoveStates.OnPress)
                {
                    return GetSpeedUp() * move.GetDirection(1f);
                }
                if (LastMoveState == MoveStates.OnRelease)
                {
                    return GetSpeedDown() * move.GetDirection(1f);
                }
                return 0f;
            }
        }
        public struct JumpSystem
        {
            public float ChargeToJump { get; private set; }
            private readonly float ChargeToJumpBase;
            private readonly float ChargeBonus;
            private readonly bool instantJump;
            private readonly int murderJumpsBase;
            public int Jumps { get; private set; }
            public int JumpsBase { get; private set; }
            public int MurderJumps { get; private set; }
            private readonly float MurderJumpsPer;

            public void SetJumpAmount(int jumps)
            {
                Jumps = jumps;
                JumpsBase = jumps;
            }
            /// <summary>
            /// Instant jumps
            /// </summary>
            /// <param name="jumps">Amount of jumps allowed</param>
            public JumpSystem(int jumps, int superMurderJumps, float murderJumps = 1f)
            {
                Jumps = jumps;
                instantJump = true;
                ChargeToJump = 0;
                ChargeToJumpBase = 0;
                ChargeBonus = 0;
                JumpsBase = jumps;
                MurderJumpsPer = murderJumps;
                murderJumpsBase = superMurderJumps;
                MurderJumps = superMurderJumps;
            }
            public float Jump(bool isGrounded, MoveStates state)
            {
                if (JumpsBase > 1)
                {
                    float strength = 1;
                    if (instantJump && state == MoveStates.OnPress)
                    {
                        if (isGrounded)
                        {
                            Jumps = JumpsBase;
                            MurderJumps = murderJumpsBase;
                            return strength;
                        }
                        else
                        {
                            Jumps--;
                            if (Jumps > 0)
                            {
                                return strength;
                            }
                            if (murderJumpsBase > 0)
                            {    
                                if (MurderJumps > 0)
                                {
                                    MurderJumps--;
                                    return strength * MurderJumpsPer;
                                }
                                return 0;
                            }
                            
                        }
                    }
                    if (!instantJump && state == MoveStates.OnPress)
                    {
                        Jumps = JumpsBase;
                        ChargeToJump = Time.time + ChargeToJumpBase;
                    }
                    else if (!instantJump && state == MoveStates.OnRelease)
                    {
                        if (isGrounded)
                        {
                            return Mathf.Clamp((Time.time - ChargeToJump) / (ChargeToJumpBase) * ChargeBonus, 0, ChargeBonus);
                        }
                        else
                        {
                            Jumps--;
                            if (Jumps > 0)
                            {
                                return Mathf.Clamp((Time.time - ChargeToJump) / (ChargeToJumpBase) * ChargeBonus, 0, ChargeBonus);
                            }
                            return 0;
                        }
                    }
                    return 0;
                }
                Debug.Log("Jump failed due to JumpsBase being less than 1.");
                return 0;

            }
        }
        /// <summary>
        /// Knockback calcultaion to make weight feel powerfull. 
        /// </summary>
        public struct ForceKnockback
        {
            /// <summary>
            /// Knockback direction
            /// </summary>
            private Vector3 Knockback { get; set; }
            private float Weight { get; set; }
            /// <summary>
            /// Setup a knockback force.
            /// </summary>
            /// <param name="knockback">The force</param>
            /// <param name="weight"></param>
            public ForceKnockback(Vector3 knockback, float weight)
            {
                weight = Mathf.Max(weight, 1);
                Weight = weight;
                Knockback = knockback;
            }
            /// <summary>
            /// Gets the knockback to apply to the charature
            /// </summary>
            /// <param name="entitiyWeight">Your/an entities <see cref="Player.Weight"/></param>
            /// <param name="destroy">Allow the object to be destoryed.</param>
            /// <returns>Vector3 = <c>Mathf.Pow(<see cref="Weight"/>/<paramref name="entitiyWeight"/>,2) * <see cref="Knockback"/></c></returns>
            public readonly Vector3 GetKnockback(float entitiyWeight)
            {
                Vector3 tempKnockback = Mathf.Pow(Weight / entitiyWeight, 2) * Knockback;
                return tempKnockback;
            }
        }
        /// <summary>
        /// Movement System for WASD or whatever you use.
        /// </summary>
        public struct MovingSystemKeyboard
        {
            private readonly Dictionary<MovingDirection, SimpleMvm> movement;
            private float InAirF;
            private float InAirS;
            /// <summary>
            /// Create a varibable direction movement system. The size of the keycodes determines the amount of SimpleMvM vars created.
            /// </summary>
            /// <param name="timeMax">Time it takes to slow down</param>
            /// <param name="timeDown">Time it takes to speed up</param>
            /// <param name="clamp">Minimum move speed</param>
            /// <param name="boost">How much boost does holding FOWARD give you.</param>
            public MovingSystemKeyboard(float timeMax, float timeDown, float clamp = 0.8f, float boost = 1, float inAirF = 0.45f, float inAirS = 0.15f)
            {
                movement = new Dictionary<MovingDirection, SimpleMvm>()
                {
                    { MovingDirection.Up, new SimpleMvm(timeMax, timeDown, clamp, boost) },
                    { MovingDirection.Down, new SimpleMvm(timeMax, timeDown, clamp, boost) },
                    { MovingDirection.Right, new SimpleMvm(timeMax, timeDown, clamp,1) },
                    { MovingDirection.Left, new SimpleMvm(timeMax, timeDown, clamp,1) }
                };
                InAirF = inAirF;
                InAirS = inAirS;
            }
            public void HandleKeyInput(MoveStates move, MovingDirection direct)
            {
                movement[direct].AutoMoveSystem(move);
            }
            /// <summary>
            /// Gets your movement deltas
            /// 
            /// </summary>
            /// <returns>Vector3(foward,0,right)/returns>
            public readonly Vector3 GetSimpleMvmDeltas(bool isGrounded)
            {
                Vector3 delta = new Vector3(movement[MovingDirection.Down].GetDelta() - movement[MovingDirection.Up].GetDelta(), 0, movement[MovingDirection.Right].GetDelta() - movement[MovingDirection.Left].GetDelta());
                if (!isGrounded)
                {
                    delta = new Vector3(delta.x * InAirF, 0, delta.z * InAirS);
                }
                return delta;
            }
        }
        /// <summary>
        /// Ground pound
        /// </summary>
        public struct GRDPound
        {
            public float Usages { get; private set; }
            public float BaseUsages { get; private set; }

            public GRDPound(float usages)
            {
                Usages = usages;
                BaseUsages = usages;
            }
            public void Reset()
            {
                Usages = BaseUsages;
            }
            public bool CanPound()
            {
                if (Usages > 0)
                {
                    Usages--;
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Create areal acceleartion
        /// </summary>
        public struct AirMovement
        {
            private float DirPow;
            private readonly float MaxSpeed;
            public float GroundDrag { get; private set; }
            public float AirDrag { get; private set; }
            public AirMovement(float maxSpeed, float groundDrag, float airDrag)
            {
                DirPow = 0;
                MaxSpeed = maxSpeed;
                GroundDrag = groundDrag;
                AirDrag = airDrag;
            }
            public void DirectionalChangeNoPress(float power)
            {
                if (DirPow != 0)
                {
                    float math = Mathf.Abs(DirPow) * power * Time.fixedDeltaTime;
                    if (DirPow > 0)
                    {
                        DirPow = Mathf.Max(DirPow - math, 0);
                    }
                    if (DirPow < 0)
                    {
                        DirPow = Mathf.Min(math, 0);
                    }
                }
            }
            public void DirectionChangePress(float power)
            {
                DirPow += power * Time.fixedDeltaTime;
                DirPow = Mathf.Clamp(DirPow, -MaxSpeed, MaxSpeed);
            }
            public readonly float GetDirection(float speed)
            {
                return DirPow * speed;
            }
        }
        [Obsolete("A failed version of Stamania, May fix in the future", false)]
        class SpeedBoost
        {
            private float Boost { get; set; }
            private float TimeOfBoost { get; set; }
            private float TimerReset { get; set; }
            private float TimeSaved { get; set; }
            private float Clamp { get; set; }

            /// <summary>
            /// Create a object to store info for boosting
            /// </summary>
            /// <param name="boost">Efficiancy</param>
            /// <param name="timer">Delay between usages</param>
            /// <param name="timeOfBoost">time of boost</param>
            /// <param name="clamp">Minimum amount</param>
            public SpeedBoost(float boost, float timer, float timeOfBoost, float clamp)
            {
                Boost = boost;
                TimeOfBoost = timeOfBoost;
                TimerReset = timer;
                TimeSaved = 0;
                Clamp = clamp;
            }
            /// <summary>
            /// Determines if you can Use another boost
            /// </summary>
            /// <returns>True/False</returns>
            public bool GetCanBoost()
            {
                if (Time.time - TimeSaved > TimerReset)
                {
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Set the TimeSaved value to Time.time
            /// </summary>
            public void ApplyBoost()
            {
                TimeSaved = Time.time;
            }
            /// <summary>
            /// Gets the current boost value
            /// </summary>
            /// <returns>A value from 0.0 to 1.0 multiplied by Boost</returns>
            public float GetBoost()
            {
                float temp = (Time.time - TimeSaved) / TimeOfBoost;
                temp = Mathf.Clamp(temp, Clamp, 1);
                temp = Mathf.Abs(temp - 1);
                if (temp <= 0)
                {
                    return 0;
                }
                return temp * Boost;
            }

        }
    }
    namespace Effect
    {
        /// <summary>
        /// Stores Effects.
        /// </summary>
        public struct Effects
        {
            public string Name { get; private set; }
            public Attributes Attributes { get; private set; }
            public float Strength { get; private set; }
            public float Time { get; private set; }
            public float Option { get; private set; }
            private string[] Others { get; set; }

            /// <summary>
            /// Data held in a Effect
            /// </summary>
            /// <param name="name"></param>
            /// <param name="attributes"></param>
            /// <param name="strength"></param>
            /// <param name="time"></param>
            /// <param name="option"></param>
            public Effects(string name, Attributes attributes, float strength, float time, float option)
            {
                Name = name;
                Attributes = attributes;
                Strength = strength;
                Time = time;
                Option = option;
                Others = null;
            }
            public Effects(string name, Attributes attributes, float strength, float time, float option, params string[] others)
            {
                Name = name;
                Attributes = attributes;
                Strength = strength;
                Time = time;
                Option = option;
                Others = others;
            }
            /// <summary>
            /// Get names of other effects to be called via the <see cref="AllLibary.ItemLibary"/>
            /// </summary>
            /// <returns></returns>
            public readonly string[] GetOtherEffects()
            {
                return Others;
            }
        }
        /// <summary>
        /// Create fire damage. Fire damage will stack
        /// </summary>
        public class FireDamage
        {
            /// <summary>
            /// Damage per tick
            /// </summary>
            public float Damage { get; private set; }

            /// <summary>
            /// Time when the fire effect expires
            /// </summary>
            private float ExpireTime { get; set; }

            /// <summary>
            /// Time between damage ticks
            /// </summary>
            private float TickInterval { get; set; }

            /// <summary>
            /// Time when the next damage tick should occur
            /// </summary>
            private float NextTickTime { get; set; }

            /// <summary>
            /// Setup fire damage.
            /// </summary>
            /// <param name="damage">How much damage per tick</param>
            /// <param name="duration">How long the fire lasts in seconds</param>
            /// <param name="tickInterval">Time between damage ticks in seconds</param>
            public FireDamage(float damage, float duration, float tickInterval)
            {
                Damage = damage;
                ExpireTime = Time.time + duration;
                TickInterval = tickInterval;
                NextTickTime = Time.time + tickInterval;
            }

            /// <summary>
            /// Checks if the fire effect has expired
            /// </summary>
            /// <returns>true = Delete, false = Keep</returns>
            public bool HasExpired()
            {
                return Time.time >= ExpireTime;
            }

            /// <summary>
            /// Checks if it's time for the next damage tick and prepares for the next one
            /// </summary>
            /// <returns>true if damage should be applied, false otherwise</returns>
            public bool ShouldApplyDamage()
            {
                if (Time.time >= NextTickTime)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Advances to the next tick time after damage has been applied
            /// </summary>
            public void AdvanceToNextTick()
            {
                NextTickTime = Time.time + TickInterval;
            }

            /// <summary>
            /// Gets the remaining duration of the fire effect
            /// </summary>
            /// <returns>Remaining time in seconds</returns>
            public float GetRemainingTime()
            {
                return Mathf.Max(0f, ExpireTime - Time.time);
            }

            /// <summary>
            /// Gets the progress of the fire effect (0 to 1)
            /// </summary>
            /// <returns>Progress from start (0) to expiration (1)</returns>
            public float GetProgress()
            {
                float totalDuration = ExpireTime - (Time.time - GetRemainingTime());
                return 1f - (GetRemainingTime() / totalDuration);
            }
        }
        /// <summary>
        /// Apply healing over time.
        /// </summary>
        public class Regeneration
        {
            /// <summary>
            /// Damage per tick
            /// </summary>
            public float HealthPerTick { get; private set; }

            /// <summary>
            /// Time when the fire effect expires
            /// </summary>
            private float ExpireTime { get; set; }

            /// <summary>
            /// Time between damage ticks
            /// </summary>
            private float TickInterval { get; set; }

            /// <summary>
            /// Time when the next damage tick should occur
            /// </summary>
            private float NextTickTime { get; set; }

            /// <summary>
            /// Setup fire damage.
            /// </summary>
            /// <param name="damage">How much damage per tick</param>
            /// <param name="duration">How long the fire lasts in seconds</param>
            /// <param name="tickInterval">Time between damage ticks in seconds</param>
            public Regeneration(float health, float duration, float tickInterval)
            {
                HealthPerTick = health;
                ExpireTime = Time.time + duration;
                TickInterval = tickInterval;
                NextTickTime = Time.time + tickInterval;
            }

            /// <summary>
            /// Checks if the fire effect has expired
            /// </summary>
            /// <returns>true = Delete, false = Keep</returns>
            public bool HasExpired()
            {
                return Time.time >= ExpireTime;
            }

            /// <summary>
            /// Checks if it's time for the next damage tick and prepares for the next one
            /// </summary>
            /// <returns>true if damage should be applied, false otherwise</returns>
            public bool ShouldApplyDamage()
            {
                if (Time.time >= NextTickTime)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Advances to the next tick time after damage has been applied
            /// </summary>
            public void AdvanceToNextTick()
            {
                NextTickTime = Time.time + TickInterval;
            }

            /// <summary>
            /// Gets the remaining duration of the fire effect
            /// </summary>
            /// <returns>Remaining time in seconds</returns>
            public float GetRemainingTime()
            {
                return Mathf.Max(0f, ExpireTime - Time.time);
            }

            /// <summary>
            /// Gets the progress of the fire effect (0 to 1)
            /// </summary>
            /// <returns>Progress from start (0) to expiration (1)</returns>
            public float GetProgress()
            {
                float totalDuration = ExpireTime - (Time.time - GetRemainingTime());
                return 1f - (GetRemainingTime() / totalDuration);
            }
        }
        /// <summary>
        /// Reverse Gravity + Remove all downforce.
        /// </summary>
        public class Floatation
        {
            /// <summary>
            /// How strong the floating effect is
            /// </summary>
            private float Strength { get; set; }
            /// <summary>
            /// How much time remains on the effect
            /// </summary>
            private float TimeRemain { get; set; }
            /// <summary>
            /// How much lift there is on the effect
            /// </summary>
            public float Lift { get; private set; }
            /// <summary>
            /// Your Gravity
            /// </summary>
            public float Gravity { get; private set; }
            /// <summary>
            /// Are you floating?
            /// </summary>
            public bool IsFloating { get; private set; }
            /// <summary>
            /// Is this effect Infinite?
            /// </summary>
            private bool IsInf { get; set; }
            /// <summary>
            /// Setup Floatation to mess with gravity when the Floating attribute is applied
            /// </summary>
            /// <param name="gravity">Your base gravity</param>
            public Floatation(float gravity)
            {
                IsInf = false;
                IsFloating = false;
                Strength = 0;
                Gravity = 0;
                TimeRemain = 0;
                Lift = 0;
                Gravity = gravity;
            }
            /// <summary>
            /// Applies Floating effect & automaticly Applies the strongest value, longest time, and strongest lit. 
            /// </summary>
            /// <param name="strength">STrength of 0 = 9.8 * -1, Strenght  of 1 = 9.8 *-2 gravity, etc...</param>
            /// <param name="time">How long the effect lasts</param>
            /// <param name="lift">Resistance to down force</param>
            public void SetupFloatation(float strength, float time, float lift)
            {
                if (Strength < strength)
                {
                    Strength = strength;
                }
                if (time < 0)
                {
                    IsInf = true;
                }
                if (TimeRemain < time + Time.time)
                {
                    TimeRemain = time + Time.time;
                }
                if (Lift < lift)
                {
                    Lift = lift;
                }
                SetGravity(Gravity);
            }
            /// <summary>
            /// Sets gravity.
            /// </summary>
            /// <param name="grav"></param>
            public void SetGravity(float grav)
            {
                Gravity = -1 * Mathf.Abs(grav + (Strength - 0.999f));
                IsFloating = true;
            }
            /// <summary>
            /// Reset the gravity back to normal once the timer has passed
            /// </summary>
            /// <param name="grav">BaseGravity</param>
            /// <returns></returns>
            public float ResetGravity(float grav)
            {
                if (Time.time > TimeRemain && IsInf == false)
                {
                    Gravity = grav;
                    Lift = 0;
                }
                return Gravity;
            }
        }
        /// <summary>
        /// Decrease Accuracy but increase Damage.
        /// </summary>
        public class Crying
        {
            private float Inaccuracy { get; set; }
            private float DamageInc { get; set; }
            private float TimeQuicken { get; set; }
            private float TimeRemain { get; set; }
            public Crying(float inaccuracyMult, float time, float damageDecreaseMult)
            {
                Inaccuracy = inaccuracyMult;
                TimeRemain = time + Time.time;
                DamageInc = damageDecreaseMult;
                TimeQuicken = Mathf.Clamp(1 + ((inaccuracyMult / 10) - damageDecreaseMult * 0.25f) / 4, 1, 4f);
            }
            /// <summary>
            /// Checks if time.time > timeRemain
            /// </summary>
            /// <returns>true = Delete, false = Keep</returns>
            public bool GetExistTime()
            {
                if (Time.time > TimeRemain)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Get the inaccuracy
            /// </summary>
            /// <returns></returns>
            public float GetInaccuracy()
            {
                return Inaccuracy;
            }
            /// <summary>
            /// Gets the shot increase rate. 
            /// </summary>
            /// <param name="value">The amount of these attributes being applied.</param>
            /// <returns><see cref="TimeQuicken"/><c> * (1 / Mathf.Pow(</c><paramref name="value"/><c>,2))</c></returns>
            public float GetShotIncrease(float value)
            {
                return TimeQuicken * (1 / Mathf.Pow(value, 3));
            }
            /// <summary>
            /// Gets the Damage decrease
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public float GetDamageAdj()
            {
                return DamageInc;
            }
        }
        public class Wounded
        {
            /// <summary>
            /// Health gained per tick
            /// </summary>
            private float Resistance { get; set; }
            /// <summary>
            /// How much time remains on the effect
            /// </summary>
            private float TimeRemain { get; set; }
            /// <summary>
            /// The rate at which your healed
            /// </summary>
            public float Absorption { get; set; }
            private bool IsGetAbsorption { get; set; }

            /// <summary>
            /// Reset the Regeneration class back to 0.
            /// </summary>
            public void ClearWound()
            {
                Resistance = 1;
                TimeRemain = 0;
                Absorption = 0;
                IsGetAbsorption = false;
            }
            /// <summary>
            /// Setup the Regen class. Will automactly choose the better stats individually.
            /// </summary>
            /// <param name="resistances">Health gained</param>
            /// <param name="timer">Time efffect lasts</param>
            /// <param name="absorp">Tick of activations</param>
            public void SetupResistance(float resistances, float timer, float absorp)
            {
                if (Resistance < resistances)
                {
                    Resistance = resistances;
                }
                if (TimeRemain < timer + Time.time)
                {
                    TimeRemain = timer + Time.time;
                }
                if (Absorption > absorp)
                {
                    Absorption = absorp;
                }
                IsGetAbsorption = true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="Resistance"/></returns>
            public float GetResistances()
            {
                return Resistance;
            }
            public float? GetAbsorption(float AbsorptionHP)
            {
                if (IsGetAbsorption == true)
                {
                    if (AbsorptionHP < Absorption)
                    {
                        IsGetAbsorption = false;
                        return AbsorptionHP;
                    }
                }
                return null;
            }
            public float GetTimeRemain()
            {
                return TimeRemain;
            }
        }
    }
    namespace Items
    {
        /// <summary>
        /// Ensures a class is compatable with the <see cref="IInventorySystem{T}"/>
        /// </summary>
        public interface IInvetorySystemCompability
        {
            /// <summary>
            /// Checks if the item is a empty/to be replaced slot.
            /// </summary>
            /// <returns>true/false</returns>
            public bool GetIsEmptyItem();
            /// <summary>
            /// Gets the ID where the item is being stored.
            /// </summary>
            /// <returns>SlotID</returns>
            public int GetSlotID();
            /// <summary>
            /// Gets the SlotID and moves it to a new location
            /// </summary>
            /// <param name="NewLocation">ID of where the item is to go</param>
            /// <returns>SlotID</returns>
            public int GetSlotID(int NewLocation);
            /// <summary>
            /// Move an item
            /// </summary>
            /// <param name="location">The ID of where to go</param>
            public void MoveItem(int location);
            public Texture GetTheTexture();
        }
        /// <summary>
        /// Create a projectile pathing, explosive sizes, damage falloff, yeeting, etc... to be used in <see cref="Item"/>
        /// </summary>
        public class Projectile : INameDesc, IDisposable
        {
            private bool _disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        // Dispose managed resources
                        Attributes?.Clear();
                        SphereicalObject = null;
                    }
                    _disposed = true;
                }
            }

            ~Projectile()
            {
                Dispose(false);
            }
            /// <summary>
            /// Starting location
            /// </summary>
            private Vector3 StartLocation { get; set; }
            /// <summary>
            /// The gravity of the weapon.
            /// </summary>
            private float Gravity { get; set; }
            /// <summary>
            /// How fast the proj moves.
            /// </summary>
            private float Speed { get; set; }
            /// <summary>
            /// How long the proj lives for before triggering or disapearing.
            /// </summary>
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
            public float ExplosiveDamageMin { get; private set; } = 0.5f;
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
            public Vector3 KnockBack { get; private set; }
            /// <summary>
            /// Weight applied by the shooer.
            /// </summary>
            public float AdditionalWeight { get; private set; }
            /// <summary>
            /// Knockback Weight. Weight = Weight * Damage
            /// </summary>
            public float Weight { get; private set; }
            /// <summary>
            /// Minimum percent fall off. A value from 0.0 to 1.0;
            /// </summary>
            public float MinPercentFalloff { get; private set; }
            /// <summary>
            /// How high will the proj go on launch. Use to create lobs/lift shots.
            /// </summary>
            private float Yeet { get; set; }
            public GameObject SphereicalObject { get; private set; }
            public float Damage { get; private set; }
            public List<string> Attributes { get; private set; } = new List<string>();

            public string GetName()
            {
                return SphereicalObject.ToString();
            }
            public string GetDesc()
            {
                return $"Attributes: {Attributes}, Weight {Weight}, Gravity {Gravity}";
            }
            public bool GetName(string name)
            {
                return GetName() == name;
            }
            public bool GetDesc(string desc)
            {
                return GetDesc() == desc;
            }
            /*
            /// <summary>
            /// Create a projectile
            /// </summary>
            /// <param name="size">The size of the object</param>
            /// <param name="gravity">How much does the bullet move downward</param>
            /// <param name="speed">Speed of the bullet</param>
            /// <param name="liveTime">The time you live</param>
            /// <param name="size">The size of the bullet</param>
            /// <param name="minDist">The furthest the bullet can travel before reaching minimum damage</param>
            /// <param name="maxDist">The furthest hte bullet can travel before damage fall-off is applied</param>
            /// <param name="minPer">a value from 0.0 to 1.0, which will be multiplied into damage</param>
            /// <param name="percing">Can the bullet travel through objects, Players/enemies = -1, Anything else = 100</param>
            public void SetupProjectile(float weight, float minDist, float maxDist, float minPer, float yeet, float gravity, float speed, float liveTime, int percing, Vector3 size, Vector3 startLocation, params Vector3[] locations)
            {
                Gravity = gravity;
                Speed = speed;
                LiveTime = liveTime;
                Size = size;
                Yeet = yeet;
                MinDist = minDist;
                MaxDist = maxDist;
                MinPercentFalloff = minPer;
                Piercing = percing;
                StartLocation = startLocation;
                Weight = weight;
                TargetLocation = locations;
            }
            */
            /// <summary>
            /// Copy constructor for Projectile - performs deep copy of all properties
            /// </summary>
            /// <param name="other">Projectile to copy</param>
            public Projectile(Projectile other)
            {
                if (other == null)
                    throw new ArgumentNullException(nameof(other), "Source projectile to copy cannot be null");

                // Copy value types and structs (these are copied by value)
                StartLocation = other.StartLocation;
                Gravity = other.Gravity;
                Speed = other.Speed;
                LiveTime = other.LiveTime;
                ExplosiveSize = other.ExplosiveSize;
                ExplosiveTime = other.ExplosiveTime;
                SmallExplosiveSize = other.SmallExplosiveSize;
                ExplosiveDamageMin = other.ExplosiveDamageMin;
                Piercing = other.Piercing;
                MinDist = other.MinDist;
                MaxDist = other.MaxDist;
                Size = other.Size;
                KnockBack = other.KnockBack;
                Weight = other.Weight;
                MinPercentFalloff = other.MinPercentFalloff;
                Yeet = other.Yeet;
                Damage = other.Damage;

                // Deep copy list of attributes
                Attributes = new List<string>(other.Attributes);

                // Copy GameObject reference (note: this is a reference copy, not a deep copy of the GameObject)
                SphereicalObject = other.SphereicalObject;
            }
            public Projectile(float gravity, float yeet, float speed, float liveTime, int piercing, float size, float weight, float damage, Vector3 knockback, GameObject sphereicalObject, params string[] attributes)
            {
                Gravity = gravity;
                Speed = speed;
                Yeet = yeet;
                LiveTime = liveTime;
                Piercing = piercing;
                Size = size;
                Weight = weight;
                KnockBack = knockback;
                SphereicalObject = sphereicalObject;
                Damage = damage;
                Attributes.AddRange(attributes);
            }
            public Projectile(float gravity, float yeet, float speed, float liveTime, int piercing, float size, float weight, float damage, float minDist, float maxDist, float minPercent, Vector3 knockback, GameObject sphereicalObject, params string[] attributes) : this(gravity, yeet, speed, liveTime, piercing, size, weight, damage, knockback, sphereicalObject, attributes)
            {
                SetFallOff(minDist, maxDist, minPercent);
            }
            /// <summary>
            /// Create a projectile in the projectile templete
            /// </summary>
            /// <param name="gravity"></param>
            /// <param name="yeet"></param>
            /// <param name="speed"></param>
            /// <param name="liveTime"></param>
            /// <param name="piercing"></param>
            /// <param name="size"></param>
            /// <param name="weight"></param>
            /// <param name="damage"></param>
            /// <param name="minDist"></param>
            /// <param name="maxDist"></param>
            /// <param name="minPercent"></param>
            /// <param name="explosiveSize"></param>
            /// <param name="explosiveTime"></param>
            /// <param name="smallestExplosiveSize"></param>
            /// <param name="explosiveDamageMin"></param>
            /// <param name="knockback"></param>
            /// <param name="sphereicalObject"></param>
            /// <param name="attributes"></param>
            public Projectile(float gravity, float yeet, float speed, float liveTime, int piercing, float size, float weight, float damage, float minDist, float maxDist, float minPercent, float explosiveSize, float explosiveTime, float smallestExplosiveSize, float explosiveDamageMin, Vector3 knockback, GameObject sphereicalObject, params string[] attributes) : this(gravity, yeet, speed, liveTime, piercing, size, weight, damage, knockback, sphereicalObject, attributes)
            {
                SetFallOff(minDist, maxDist, minPercent);
                SetExplosiveSize(explosiveSize, explosiveTime, smallestExplosiveSize, explosiveDamageMin);
            }
            public Projectile()
            {

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
            /// How long the projectile lasts
            /// </summary>
            /// <param name="add">New aditional time</param>
            /// <returns><see cref="LiveTime"/> + add</returns>
            public float GetLiveTime(float add)
            {
                return LiveTime + add;
            }
            /// <summary>
            /// Sets the size of the explosive
            /// </summary>
            /// <param name="smallSize">Smallest size of the explosvie</param>
            /// <param name="size">Largest size</param>
            /// <param name="explosiveTime">How long it takes to fully explode</param>
            public void SetExplosiveSize(float explosiveSize, float explosiveTime, float smallestExplosiveSize, float explosiveDamageMin)
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
            /// 
            /// </summary>
            /// <returns></returns>
            public Vector3 ReturnToStart()
            {
                return StartLocation;
            }
            /// <summary>
            /// How fast the object travels
            /// </summary>
            /// <returns><see cref="Speed"/></returns>
            public float GetSpeed()
            {
                return Speed;
            }
            /// <summary>
            /// The gravity of the object
            /// </summary>
            /// <returns><see cref="Gravity"/></returns>
            public float GetGravity()
            {
                return Gravity;
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
            /// Initial UPForce;
            /// </summary>
            /// <returns><see cref="Yeet"/></returns>
            public float GetYeet()
            {
                return Yeet;
            }
            /// <summary>
            /// Get knockback
            /// </summary>
            /// <param name="direct"></param>
            /// <returns></returns>
            public ForceKnockback GetKnockback()
            {
                return new ForceKnockback(KnockBack, Weight);
            }
            public void SetupProjectile(float damage, int peirce)
            {
                Damage += damage;
                Piercing += peirce;
            }
        }
        /// <summary>
        /// The base item. Used in <see cref="InventoryItem"/>. You'll mainly use an instantiation such as <see cref="Weapon"/>, <see cref="Consumable"/>, <see cref="Armor"/> 
        /// <br></br> All items contain: 
        /// <list type="bullet">
        /// <item>Names, Desc, Inventory size and <see cref="Projectile"/> stats.</item>
        /// <item><see cref="ItemType"/> to indicate to the <see cref="InventoryItem"/> on which instantiation to grab.</item>
        /// <item><see cref="AnimationSys"/>, <see cref="Texture"/>, names of attributes which are located in the <see cref="AllLibary.GetEffectNames()"/>, </item>
        /// </list>
        /// </summary>
        public class Item : INameDesc
        {
            /// <summary>
            /// Your name
            /// </summary>
            protected string Name { get; set; } = "Null";
            /// <summary>
            /// What Instantitation of the weapon is it.
            /// </summary>
            protected ItemType itemType;
            /// <summary>
            /// ID, I suppose if you wanted to create an item system in a libary based on ID you could use this, otherwise its just pointless. But i'll leave it in for any nerdy ppl.
            /// </summary>
            protected int ID { get; set; }
            /// <summary>
            /// Description
            /// </summary>
            protected string Desc { get; set; } = string.Empty;
            /// <summary>
            /// Contains Animations for the weapon.
            /// </summary>
            protected AnimationSys? Anim { get; set; }
            /// <summary>
            /// A list of every projectile usable by the item
            /// </summary>
            protected List<Projectile> Project { get; set; } = new List<Projectile>();
            public List<string> Effects { get; protected set; } = new List<string>();
            protected bool IsOn { get; set; } = true;
            /// <summary>
            /// Commands
            /// </summary>
            protected CommandRequest[] CommandRequests { get; set; }
            /// <summary>
            /// The extra data buttons.
            /// </summary>
            public ExtraDataType[] ExtraDataButton { get; protected set; } = new ExtraDataType[Enum.GetValues(typeof(ExtraDataType)).Length];
            /// <summary>
            /// A item with a name and type
            /// </summary>
            /// <param name="name"></param>
            public Item(string name, ItemType type)
            {
                Name = name;
                itemType = type;
            }
            public virtual int GetPassiveData()
            {
                return 0;
                //No data goes here cause the base Item class doesn't have anything to update.
            }
            /// <summary>
            /// Gets the name of the weapon
            /// </summary>
            /// <returns>string</returns>
            public string GetName()
            {
                return Name;
            }
            /// <summary>
            /// Names of effects that this item can afflict
            /// </summary>
            /// <param name="effects"></param>
            public void SetEffect(params string[] effects)
            {
                Effects = effects.ToList<string>();
            }
            /// <summary>
            /// Checks if 2 names are the same.
            /// </summary>
            /// <param name="name">Name of the item</param>
            /// <returns>True/False</returns>
            public bool GetName(string name)
            {
                return name == GetName();
            }
            public int GetID()
            {
                return ID;
            }
            public ItemType GetItemType()
            {
                return itemType;
            }
            /// <summary>
            /// Gets the description of the item
            /// </summary>
            /// <returns>String</returns>
            public string GetDesc()
            {
                return Desc;
            }
            public bool GetDesc(string desc)
            {
                return Desc == desc;
            }
            /// <summary>
            /// Set a barebones Description
            /// </summary>
            /// <param name="text">text</param>
            public void SetBasicDesc(string text)
            {
                Desc = text;
            }
            /// <summary>
            /// Create an empty item. Often used with Creating a weapon to have no other attributes applied at : base();
            /// </summary>
            public Item()
            {
                //Ensure nothing is here
            }
            /// <summary>
            /// Use to setup ammo
            /// </summary>
            /// <param name="name"></param>
            /// <param name="desc"></param>
            /// <param name="attributes"></param>
            /// <param name="proj"></param>
            public Item(string name, string desc, List<Projectile> proj)
            {
                Name = name;
                Desc = desc;
                Project.AddRange(proj);
                itemType = ItemType.Ammo;
            }
            /// <summary>
            /// Setup a new animation
            /// </summary>
            /// <param name="textures">Images</param>
            /// <param name="cuts">Cuts, where does the new animaiton start</param>
            /// <param name="type">What type of animation is it</param>
            public void SetAnimations(Texture[] textures, int[] cuts, AnimationType[] type, AudioClip[] clip)
            {
                Anim = new AnimationSys(textures, cuts, type, clip);
            }
            /// <summary>
            /// Get the animations
            /// </summary>
            /// <returns><see cref="Anim"/></returns>
            public AnimationSys? GetAnimationClass()
            {
                return Anim;
            }
            /// <summary>
            /// Adds a new projectile to the list
            /// </summary>
            /// <param name="project">WOW a Projectile who woulda guessed???</param>
           // public void SetupProjectile(Projectile project)
            //{
              //  Project.Add(project);
            //}
            public void SetupProjectile(params Projectile[] projects)
            {
#if UNITY_EDITOR
                foreach (Projectile projectile in projects)
                {
                    Debug.Log($"Projectile added : {projectile.GetName()}");
                }
#endif
                Project.AddRange(projects);
            }
            public void ClearProjectiles()
            {
                Project.Clear();
            }
            /// <summary>
            /// Get Projectile based on index
            /// </summary>
            /// <param name="id">index</param>
            /// <returns>Projectile</returns>
            public Projectile GetProjectile(int id, bool newProj = false)
            {
                if (newProj)
                {
                    try
                    {
                        return new Projectile(Project[id]);
                    }
                    catch
                    {
                        try
                        {
                            return new Projectile(Project[0]);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                try
                {
                    Debug.Log($"Trying to get projectile {id}");
                    return (Project[id]);
                }
                catch (IndexOutOfRangeException ex) 
                {
                    Debug.LogWarning(ex.Message);
                    try
                    {
                        Debug.Log($"Trying to get Default projectile (the first one)");
                        return (Project[0]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Unable to find proj {e}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogAssertion(ex);
                    try
                    {
                        return (Project[0]);
                    }
                    catch (Exception ex2)
                    {
                        Debug.LogAssertion(ex2);
                    }
                }
                Debug.LogAssertion("Error finding projectile");
                return null;
            }
            /// <summary>
            /// Get All Projectiles
            /// </summary>
            /// <param name="id">index</param>
            /// <returns>Projectile</returns>
            public List<Projectile> GetProjectile()
            {
                return Project;
            }
            /// <summary>
            /// Get 
            /// </summary>
            /// <returns></returns>
            public List<string> GetEffects()
            {
                return Effects;
            }
            public CommandRequest GetCommandRequest(int id)
            {
                return CommandRequests[id];
            }
            public CommandRequest[] GetCommandRequests()
            {
                return CommandRequests;
            }
            public void SetOn(bool on)
            {
                IsOn = on;
            }
            public bool GetOn()
            {
                return IsOn;
            }
            public void SetDescription(string type)
            {
                Desc = type;
            }
            /// <summary>
            /// Copy constructor for Item - performs a deep copy where needed
            /// </summary>
            /// <param name="other">Item to copy</param>
            public Item(Item other)
            {
                if (other == null)
                    throw new ArgumentNullException(nameof(other), "Source item to copy cannot be null.");

                // Copy primitive types and simple properties
                Name = other.Name;
                itemType = other.itemType;
                ID = other.ID;
                Desc = other.Desc;
                IsOn = other.IsOn;

                // Deep copy AnimationSys
                if (other.Anim != null)
                {
                    Anim = new AnimationSys?((AnimationSys)other.Anim);
                }
                else
                {
                    Anim = null;
                }

                // Deep copy Projectile list
                Project = other.Project?.Select(p => new Projectile(p)).ToList() ?? new List<Projectile>();

                // Deep copy Effects list
                Effects = other.Effects?.ToList() ?? new List<string>();

                // Copy CommandRequests array
                CommandRequests = other.CommandRequests?.ToArray();

                // Copy ExtraDataButton array
                ExtraDataButton = other.ExtraDataButton?.ToArray() ?? new ExtraDataType[Enum.GetValues(typeof(ExtraDataType)).Length];
            }

        }
        /// <summary>
        /// A item to be consumed that only applies an attribute or heals.
        /// </summary>
        public class Consumable : Item
        {
            protected ConsumableClass ConsumableClass { get; set; }
            protected bool IsHealingItem { get; set; } = true;
            protected float AppliedAmount { get; set; }
            protected float AppliedTick { get; set; }
            public float HealTickTimer { get; protected set; }
            protected float TotalTime { get; set; }
            protected float ConsumableDelay { get; set; }
            protected float PassiveReloadDelay { get; set; }
            /// <summary>
            /// Create a instant heal consumable
            /// </summary>
            /// <param name="name"></param>
            /// <param name="consumeClass"></param>
            /// <param name="consumeDelay"></param>
            /// <param name="healAmount"></param>
            /// <param name="healTick"></param>
            public Consumable(string name, ConsumableClass consumeClass, float consumeDelay, float healAmount) : base()
            {
                Name = name;
                ConsumableClass = consumeClass;
                ConsumableDelay = consumeDelay;
                AppliedAmount = healAmount;
                itemType = ItemType.Consumable;
                if (healAmount <= 0)
                {
                    IsHealingItem = false;
                }
            }
            public Consumable(Consumable other) : base(other) // Call base (Item) copy constructor
            {
                ConsumableClass = other.ConsumableClass;
                IsHealingItem = other.IsHealingItem;
                AppliedAmount = other.AppliedAmount;
                AppliedTick = other.AppliedTick;
                HealTickTimer = other.HealTickTimer;
                TotalTime = other.TotalTime;
                ConsumableDelay = other.ConsumableDelay;
                PassiveReloadDelay = other.PassiveReloadDelay;
            }
        }
        public class Armor : Item
        {
            public Armor(Armor other) : base(other)
            {

            }
        }
        /// <summary>
        /// A item Instation for a weapon.<br></br>Weapons are the most info heavy objects.
        /// </summary>
        public class Weapon : Item
        {
            /// <summary>
            /// What "class" is the weapon. Used to determine resistances.
            /// </summary>
            protected WeaponClass WeaponClass { get; }
            /// <summary>
            /// Use to give apply simple weapon mechanics in the main script
            /// </summary>
            public WeaponDesign WeaponDesign { get; }
            public AmmoHoldDesign AmmoHold { get; }
            /// <summary>
            /// Does the weapon deal damage?
            /// </summary>
            protected bool HasDamage { get; set; }
            /// <summary>
            /// How much damage does it deal
            /// </summary>
            protected float Damage { get; set; }
            /// <summary>
            /// How long it takes to charge the weapon
            /// </summary>
            protected float? ChargeTime { get; set; }
            protected float Mincharge { get; set; }
            protected float Weight { get; set; }
            /// <summary>
            /// How much knockback is applied during a hit
            /// </summary>
            protected ForceKnockback KnockBack { get; set; }
            /// <summary>
            /// Does the weapon have Reloading?
            /// </summary>
            protected bool HasReload { get; set; }
            /// <summary>
            /// How fast does the weapon reload
            /// </summary>
            protected float ReloadSpeed { get; set; }
            /// <summary>
            /// Is the weapon reloading?
            /// </summary>
            protected bool IsReloading { get; set; } = false;
            /// <summary>
            /// The delay between each attack
            /// </summary>
            protected float AttackDelay { get; set; }
            /// <summary>
            /// The time it takes to attack. Mainly used for animiations and Melee attacks where hitboxes appear for so many frames.
            /// </summary>
            protected float AttackTime { get; set; }
            /// <summary>
            /// A value from 0.001 to 1;
            /// </summary>
            protected float ChargeScale { get; set; }
            /// <summary>
            /// How many shots does the weapon do if charged
            /// </summary>
            protected int ExtraShots { get; set; } = 1;
            /// <summary>
            /// Minimum damage done while fully charged
            /// </summary>
            protected float MinChargeDamage { get; set; }
            /// <summary>
            /// How much ammo do you currenlty have
            /// </summary>
            protected int Ammo { get; set; }
            /// <summary>
            /// What is the maximum ammo the weapon can hold
            /// </summary>
            public int MaxAmmo { get; protected set; }
            /// <summary>
            /// True = 1 bullet gets reloaded at a time. False = All at once
            /// </summary>
            protected bool ReloadType { get; set; }
            /// <summary>
            /// The current time between shots.
            /// </summary>
            public float TimerAttackDelay { get; protected set; }
            /// <summary>
            /// The current time during a shot.
            /// </summary>
            public float TimerAttackTime { get; protected set; }
            /// <summary>
            /// the current time reloading
            /// </summary>
            public float TimerReload { get; protected set; }
            /// <summary>
            /// A timer currenlty being stored on how long it takes to charge the weapon
            /// </summary>
            public float TimerCharge { get; protected set; }
            /// <summary>
            /// A patterean for weapons like shotguns or tri-shot rifles
            /// </summary>
            protected Vector2[] Pattern { get; set; }
            protected Vector3 Size { get; set; }
            /// <summary>
            /// Does the weapon even have a pattern
            /// </summary>
            public bool UsingPattern { get; protected set; }
            public bool IsCharging { get; protected set; }
            protected float? SphereAccuracy { get; set; }
            protected float InitialDelay { get; set; }
            protected float TotalPatternShots { get; set; }
            protected float MaxCharge { get; set; }
            protected float ShotAIMDecrease { get; set; }
            protected float StopShootingDecrease { get; set; }
            protected List<string> AcceptedAmmo { get; set; } = new List<string>();
            public int AdditinoalPiercing { get; set; }
            /// <summary>
            /// Create a Single fire weapon with no extra details.
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">Animation time during attack</param>
            /// <param name="damage">Maximum Damage</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage) : base()
            {
                Name = name;
                WeaponClass = weaponClass;
                Damage = damage;
                AttackDelay = attackDelay;
                AttackTime = attackTime;
                HasReload = false;
                HasDamage = damage != 0;
                itemType = ItemType.Weapon;
                AmmoHold = AmmoHoldDesign.Endless;
                UsingPattern = false;
            }
            /// <summary>
            /// Create an Melee Item with a Weapon Class
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="weaponDesign">The type of desing the weapon is based on.</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">How long the hitbox lasts</param>
            /// <param name="damage">Maximum Damage</param>
            /// <param name="size">How big the hitbox is</param>
            /// <param name="intialDelay">How long it takes to swing</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage, float intialDelay, Vector3 size) : this(name, weaponClass, attackDelay, attackTime, damage)
            {
                HasReload = false;
                HasDamage = damage != 0;
                itemType = ItemType.Melee;
                UsingPattern = false;
                Size = size;
                InitialDelay = intialDelay;
                AmmoHold = AmmoHoldDesign.Endless;
            }

            /// <summary>
            /// Create a Shot pattern weapon with no Ammo. Includes a spread if wanted.
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="weaponDesign">The type of desing the weapon is based on.</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">Animation time during attack</param>
            /// <param name="damage">Maximum Damage</param>
            /// <param name="sphereAccuracy">Accuracy of the shot. The bigger the number, the less accurate your shot is.</param>
            /// <param name="row">How many rows of bullets</param>
            /// <param name="colm">How many colm's of bullets</param>
            /// <param name="distanceApart">How far appart should each bullet be</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage, float sphereAccuracy, int row, int colm, float distanceApart) : this(name, weaponClass, attackDelay, attackTime, damage)
            {
                HasReload = false;
                HasDamage = damage != 0;
                SphereAccuracy = sphereAccuracy;
                itemType = ItemType.Weapon;
                SetBulletPattern(row, colm, distanceApart);
                UsingPattern = true;
                TotalPatternShots = row * colm;
                AmmoHold = AmmoHoldDesign.Endless;
            }
            /// <summary>
            /// Create a Single fire weapon with no ammo. Includes a spread if wanted.
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="weaponDesign">The type of desing the weapon is based on.</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">Animation time during attack</param>
            /// <param name="damage">Maximum Damage</param>
            /// <param name="sphereAccuracy">Accuracy of the shot. The bigger the number, the less accurate your shot is.</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage, float sphereAccuracy) : this(name, weaponClass, attackDelay, attackTime, damage)
            {
                HasReload = false;
                HasDamage = damage != 0;
                itemType = ItemType.Weapon;
                UsingPattern = false;
                SphereAccuracy = sphereAccuracy;
                AmmoHold = AmmoHoldDesign.Endless;
                WeaponDesign = WeaponDesign.Standered;
            }
            /// <summary>
            /// Create a charged weapon. Includes a spread if wanted.
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="weaponDesign">The type of desing the weapon is based on.</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">Animation time during attack</param>
            /// <param name="damage">Maximum Damage</param>
            /// <param name="sphereAccuracy">Accuracy of the shot. The bigger the number, the less accurate your shot is.</param>
            /// <param name="chargeTime">how long it takes to charge the attack.</param>
            /// <param name="scaleBonus">A value between 0.01+ and 1.0, If the value is a 0.5, the amount of extra "charges" would be 2. If the value is 0.33, the extra charges would be 3. If the value is 0.01, then there would be 100 charges.</param>
            /// <param name="damageMin">The minimum damage</param>
            /// <param name="extraShotsBonus">DOES NOT WORK. TODO: Make multiple shots fire when filled above an amount</param>
            /// <param name="minCharge">The minimum charge required to fire.</param>
            /// <param name="maxCharge">The max allowed charge.</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage, float sphereAccuracy, float chargeTime, float scaleBonus, int extraShotsBonus, float damageMin, float minCharge, float maxCharge) : this(name, weaponClass, attackDelay, attackTime, damage, sphereAccuracy)
            {
                HasReload = false;
                HasDamage = damage != 0;
                itemType = ItemType.Weapon;
                UsingPattern = false;
                MinChargeDamage = damageMin;
                ChargeScale = scaleBonus;
                ExtraShots = extraShotsBonus;
                Mincharge = minCharge;
                MaxCharge = maxCharge;
                if (minCharge < 0 || minCharge > 1)
                    throw new ArgumentException("minCharge must be between 0 and 1");

                if (scaleBonus < 0.0001 || scaleBonus > 1)
                    throw new ArgumentException("scaleBonus must be between 0.0001 and 1");
                AmmoHold = AmmoHoldDesign.Endless;
                WeaponDesign = WeaponDesign.Charged;
            }
            /// <summary>
            /// Create a single fire weapon with ammo. Includes a spread if wanted.
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="weaponDesign">The type of desing the weapon is based on.</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">Animation time during attack</param>
            /// <param name="damage">Maximum Damage</param>
            /// <param name="sphereAccuracy">Accuracy of the shot. The bigger the number, the less accurate your shot is.</param>
            /// <param name="oneAtATime">Do you reload the entire magazine at once or 1 at a time.</param>
            /// <param name="reloadTime">How long it takes to reload each ammo slot/all at once.</param>
            /// <param name="ammo">How much ammo does the weapon have</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage, float sphereAccuracy, bool oneAtATime, float reloadTime, int ammo) : base()
            {
                Name = name;
                WeaponClass = weaponClass;
                Damage = damage;
                AttackDelay = attackDelay;
                AttackTime = attackTime;
                HasReload = true;
                ReloadSpeed = reloadTime;
                Ammo = 0;
                MaxAmmo = ammo;
                HasDamage = damage != 0;
                ReloadType = oneAtATime;
                itemType = ItemType.Weapon;
                UsingPattern = false;
                SphereAccuracy = sphereAccuracy;
                AmmoHold = AmmoHoldDesign.SingleBullet;
                WeaponDesign = WeaponDesign.Standered;
            }
            /// <summary>
            /// Create a weapon with ammo and a shot pattern. Includes a spread if wanted.
            /// </summary>
            /// <param name="name">The name of the weapon</param>
            /// <param name="weaponClass">The class it is, used with resistances and damage bonuses</param>
            /// <param name="weaponDesign">The type of desing the weapon is based on.</param>
            /// <param name="attackDelay">Time between attacks</param>
            /// <param name="attackTime">Animation time during attack</param>
            /// <param name="damage">Maximum Damage</param>
            /// <param name="sphereAccuracy">Accuracy of the shot. The bigger the number, the less accurate your shot is.</param>
            /// <param name="oneAtATime">Do you reload the entire magazine at once or 1 at a time.</param>
            /// <param name="reloadTime">How long it takes to reload each ammo slot/all at once.</param>
            /// <param name="ammo">How much ammo</param>
            /// <param name="colm">Colm of bullets</param>
            /// <param name="row">Row of bullets</param>
            /// <param name="distanceApart">How far apart are the bullets</param>
            public Weapon(string name, WeaponClass weaponClass, float attackDelay, float attackTime, float damage, float sphereAccuracy, bool oneAtATime, float reloadTime, int ammo, int row, int colm, float distanceApart) : this(name, weaponClass, attackDelay, attackTime, damage, sphereAccuracy)
            {
                HasReload = true;
                ReloadSpeed = reloadTime;
                Ammo = 0;
                MaxAmmo = ammo;
                HasDamage = damage != 0;
                ReloadType = oneAtATime;
                itemType = ItemType.Weapon;
                SetBulletPattern(row, colm, distanceApart);
                UsingPattern = true;
                AmmoHold = AmmoHoldDesign.Magazine;
            }
            /// <summary>
            /// Copy constructor for Weapon - performs deep copy of weapon-specific properties
            /// </summary>
            /// <param name="other">Weapon to copy</param>
            public Weapon(Weapon other) : base(other) // First copy base Item properties
            {
                if (other == null)
                    throw new ArgumentNullException(nameof(other), "Source weapon to copy cannot be null");

                // Copy weapon-specific properties
                WeaponClass = other.WeaponClass;
                HasDamage = other.HasDamage;
                Damage = other.Damage;
                ChargeTime = other.ChargeTime;
                Mincharge = other.Mincharge;
                Weight = other.Weight;
                KnockBack = other.KnockBack;
                HasReload = other.HasReload;
                ReloadSpeed = other.ReloadSpeed;
                IsReloading = other.IsReloading;
                AttackDelay = other.AttackDelay;
                AttackTime = other.AttackTime;
                ChargeScale = other.ChargeScale;
                ExtraShots = other.ExtraShots;
                MinChargeDamage = other.MinChargeDamage;
                Ammo = other.Ammo;
                MaxAmmo = other.MaxAmmo;
                ReloadType = other.ReloadType;
                TimerAttackDelay = other.TimerAttackDelay;
                TimerAttackTime = other.TimerAttackTime;
                TimerReload = other.TimerReload;
                TimerCharge = other.TimerCharge;
                IsCharging = other.IsCharging;
                InitialDelay = other.InitialDelay;
                TotalPatternShots = other.TotalPatternShots;
                MaxCharge = other.MaxCharge;
                ShotAIMDecrease = other.ShotAIMDecrease;
                StopShootingDecrease = other.StopShootingDecrease;
                AmmoHold = other.AmmoHold;
                AdditinoalPiercing = other.AdditinoalPiercing;
                // Deep copy AcceptedAmmo list
                AcceptedAmmo = new List<string>(other.AcceptedAmmo);

                // Copy sphere accuracy (nullable float)
                SphereAccuracy = other.SphereAccuracy;

                // Copy UsingPattern flag
                UsingPattern = other.UsingPattern;
                // Deep copy pattern array (Vector2 is a struct, so shallow copy is fine)
                Pattern = other.Pattern?.ToArray();

                // Copy size (Vector3 is a struct)
                Size = other.Size;

                // Copy weapon designs (assuming WeaponDesign is an enum or struct)
                WeaponDesign = other.WeaponDesign;

            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="Damage"/></returns>
            public float GetDamage()
            {
                return Damage;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>The weapon class of the weapon. Used to calculate resistances.</returns>
            public WeaponClass GetWeaponClass()
            {
                return WeaponClass;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>AttackDelay</returns>
            public float GetAtackDelay()
            {
                return AttackDelay;
            }
            /// <summary>
            /// Apply an attack delay used to determine when you can shoot again.
            /// </summary>
            public void ApplyAttackDelay()
            {
                TimerAttackDelay = Time.time + AttackDelay;
                TimerAttackTime = Time.time + AttackTime;
            }
            /// <summary>
            /// Use on mouse down to initiate charging weapons.
            /// </summary>
            /// <param name="timer">The current time</param>
            /// <returns>True/False, True if <paramref name="timer"/> > <see cref="TimerAttackDelay"/></returns>
            public bool SetCharge(float timer)
            {
                if (timer > TimerAttackDelay)
                {
                    TimerCharge = Time.time;
                    IsCharging = true;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Reload delay. Time it takes to reload
            /// </summary>
            public void ApplyReloadDelay()
            {
                TimerReload = Time.time + ReloadSpeed;
            }
            /// <summary>
            /// Used mainly with melee systems but can be used on projectiles/explosives to control animation speed.
            /// </summary>
            /// <returns>Gets the time it takes to attack</returns>
            public float GetAtackTime()
            {
                return AttackTime;
            }
            /// <summary>
            /// Time it takes to shoot a weapon while Shooting
            /// </summary>
            /// <returns></returns>
            public float GetChargeTime()
            {
                return (float)ChargeTime;
            }
            /// <summary>
            /// Get Time it takes to reload
            /// </summary>
            /// <returns></returns>
            public float GetReloadTime()
            {
                return ReloadSpeed;
            }
            /// <summary>
            /// Get a preset stat outupt
            /// </summary>
            /// <returns>Int[3] [Damage, Charge Time, Attack Delay]</returns>
            public float[] GetPresetStats()
            {
                float[] temp = new float[3];
                temp[0] = Damage;
                if (itemType == ItemType.Melee)
                {
                    temp[1] = Size.z;
                }
                else
                {
                    temp[1] = (float)SphereAccuracy;
                }
                if (ChargeTime == null)
                {
                    temp[2] = AttackDelay;
                }
                else
                {
                    temp[2] = (float)ChargeTime;
                }

                return temp;
            }
            public void SetDescription(int type)
            {
                if (type == 0)
                {
                    Desc = $"{Name}\nClass: {WeaponClass}, {WeaponDesign} Design\n{GetPresetStatsDesc()}:{GetPresetStats()}";
                }
            }
            /// <summary>
            /// Get a prset stat Desc output
            /// </summary>
            /// <returns>String[3] ["Damage", "Charge Time", "Attack Delay"]</returns>
            public string[] GetPresetStatsDesc()
            {
                string[] temp = new string[3];
                temp[0] = "Damage";
                if (itemType == ItemType.Melee)
                {
                    temp[1] = "Size:";
                }
                else
                {
                    temp[1] = "Accuracy:";
                }
                if (ChargeTime == null)
                {
                    temp[2] = "Attack Delay:";
                }
                else
                {
                    temp[2] = "Charge Time:";
                }
                return temp;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>How much ammo you have</returns>
            public int GetAmmoCount()
            {
                return Ammo;
            }
            /// <summary>
            /// Determiens if you have any ammo or even if the weapon your using uses ammo. If you don't have ammo and you can reload, this will return true.
            /// </summary>
            /// <returns>true/false</returns>
            public bool GetIsAmmoEmpty()
            {
                if (Ammo == 0 && HasReload == true)
                {
                    return true;
                }
                return false;
            }
            public bool GetIsAmmoFull()
            {
                if (Ammo == MaxAmmo && HasReload == true)
                {
                    return true;
                }
                return false;
            }
            /// <summary>
            /// While Autoreloading, best used to see if you have Maxammo or more than max ammo.
            /// </summary>
            /// <returns></returns>
            public bool GetNeedToReload()
            {
                if (HasReload == false)
                {
                    return false;
                }
                else if (Ammo >= MaxAmmo)
                {
                    Ammo = MaxAmmo;
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="MaxAmmo"/> - <see cref="Ammo"/></returns>
            public int GetAmmoNeededToRefill()
            {
                return MaxAmmo - Ammo;
            }
            /// <summary>
            /// Adds ammo to the weapon.
            /// </summary>
            public void AddAmmo(int amount)
            {
                Ammo = Mathf.Clamp(Ammo + amount, 0, MaxAmmo);
                Debug.Log($"Total {Ammo}");
            }
            /// <summary>
            /// Adds ammo directly from player's inventory
            /// </summary>
            public void AddAmmo(Player player, int amount)
            {
                if (player == null || amount <= 0 || Ammo >= MaxAmmo)
                {
                    return;
                }

                Debug.Log($"Adding {amount} ammo from player inventory");

                for (int i = 0; i < player.GetInventory().Count; i++)
                {
                    InventoryItem inventoryItem = player.GetInventoryItem(i);

                    if (inventoryItem.MarkedForDeletion)
                    {
                        player.DeleteItem(i);
                        i--; // Adjust index after deletion
                        continue;
                    }

                    if (inventoryItem.GetItemType() != ItemType.Ammo)
                    {
                        continue;
                    }

                    string itemName = inventoryItem.GetName();
                    foreach (string acceptedAmmoName in AcceptedAmmo)
                    {
                        if (Ammo >= MaxAmmo) break;

                        if (itemName.Contains(acceptedAmmoName))
                        {
                            int availableAmmo = inventoryItem.GetHeldAmount();
                            if (availableAmmo >= int.MaxValue)
                            {
                                availableAmmo = int.MaxValue - 1;
                            }
                            int neededAmmo = MaxAmmo - Ammo;
                            int ammoToAdd = Mathf.Min((int)availableAmmo, neededAmmo, amount);

                            if (ammoToAdd <= 0) continue;

                            Item ammoItem = inventoryItem.GetItem();
                            if (ammoItem != null && ammoItem.GetProjectile() != null &&
                                (Project.Count == 0 || !Project[0].Equals(ammoItem.GetProjectile()[0])))
                            {
                                UpdateProjectilesFromAmmo(ammoItem.GetProjectile());
                            }

                            // Remove ammo from inventory
                            inventoryItem.DecreaseHeld((int)ammoToAdd);
                            AddAmmo(ammoToAdd);

                            player.CheckForDeletion();

                            if (Ammo >= MaxAmmo) return;
                            break; // Move to next inventory item
                        }
                    }
                }
            }
            /// <summary>
            /// Adds ammo directly from your inventory. Will update the projectiles to match the ammo's projectiles.
            /// </summary>
            /// <param name="inven">A List of Inventory Items</param>
            public void AddAmmo(List<InventoryItem> inven, int amount)
            {
                if (inven == null || amount <= 0 || Ammo >= MaxAmmo)
                {
                    return;
                }

                Debug.Log($"Adding {amount} ammo from item list");

                foreach (InventoryItem inventoryItem in inven)
                {
                    if (inventoryItem == null || inventoryItem.MarkedForDeletion || ItemType.Ammo != inventoryItem.GetItemType())
                    {
                        continue;
                    }

                    string itemName = inventoryItem.GetName();
                    foreach (string acceptedAmmoName in AcceptedAmmo)
                    {
                        if (Ammo >= MaxAmmo) break;

                        if (itemName.Contains(acceptedAmmoName))
                        {
                            int availableAmmo = inventoryItem.GetHeldAmount();
                            int neededAmmo = MaxAmmo - Ammo;
                            int ammoToAdd = Mathf.Min((int)availableAmmo, neededAmmo, amount);
                            Debug.Log($"Ammo to add: {ammoToAdd}");


                            if (ammoToAdd <= 0) continue;

                            // Update projectiles if different
                            Item ammoItem = inventoryItem.GetItem();
                            if (ammoItem != null && ammoItem.GetProjectile() != null)
                            {
                                UpdateProjectilesFromAmmo(ammoItem.GetProjectile());
                            }

                            inventoryItem.DecreaseHeld((int)ammoToAdd);
                            AddAmmo(ammoToAdd);

                            if (Ammo >= MaxAmmo) return;
                            break;
                        }
                    }
                }
            }
            /// <summary>
            /// Adds ammo to the weapon dependent on <see cref="ReloadType"/>
            /// </summary>
            public void AddAmmo()
            {
                if (ReloadType)
                {
                    Ammo++;
                }
                else
                {
                    Ammo = MaxAmmo;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="MaxAmmo"/></returns>
            public int GetMaxAmmo()
            {
                return MaxAmmo;
            }
            /// <summary>
            /// Consume ammo
            /// </summary>
            /// <param name="amount">the amount</param>
            public void ConsumeAmmo(int amount)
            {
                Ammo -= amount;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="HasReload"/></returns>
            public bool GetUsesAmmo()
            {
                return HasReload;
            }
            /// <summary>
            /// Set ammo which is acecptable by the weapon.
            /// </summary>
            /// <param name="names"></param>
            public void SetAcceptableAmmo(int additionalPiercing, params string[] names)
            {
                Debug.Log($"Names length: {names.Length}");
                AcceptedAmmo.AddRange(names);
                AdditinoalPiercing += additionalPiercing;
            }
            /// <summary>
            /// Optimized method to update projectiles from ammo
            /// </summary>
            private void UpdateProjectilesFromAmmo(List<Projectile> newProjectiles)
            {
                if (newProjectiles == null || newProjectiles.Count == 0) return;

                // Only update if projectiles are different
                if (Project.Count != newProjectiles.Count ||
                    (Project.Count > 0 && !Project[0].Equals(newProjectiles[0])))
                {
                    Project.Clear();
                    foreach (Projectile newProjectile in newProjectiles)
                    {
                        Projectile copy = new Projectile(newProjectile);
                        copy.SetupProjectile(Damage, AdditinoalPiercing);
                        Project.Add(copy);
                    }
                }
            }
            /// <summary>
            /// A pattern for bullets
            /// </summary>
            /// <param name="row"></param>
            /// <param name="colm"></param>
            public void SetBulletPattern(int row, int colm, float distance)
            {
                if (row <= 0 || colm <= 0)
                {
                    row = 1;
                    colm = 1;
                }

                Pattern = new Vector2[row * colm];
                int index = 0;

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < colm; j++)
                    {
                        // Center the pattern around (0,0)
                        float x = (j - (colm - 1) / 2f) * distance;
                        float y = (i - (row - 1) / 2f) * distance;
                        Pattern[index++] = new Vector2(x, y);
                    }
                }
                UsingPattern = true;
            }
            /// <summary>
            /// Bullet pattern
            /// </summary>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public Vector2[] GetBulletPattern()
            {
                if (UsingPattern)
                {
                    return Pattern;
                }
                throw new Exception("This weapon does not have a bullet Pattern");
            }
            /// <summary>
            /// Reload in the background
            /// </summary>
            /// <returns></returns>
            public bool ActivatePassiveReload()
            {
                if (!IsReloading && HasReload && Ammo < MaxAmmo)
                {
                    TimerReload = Time.time + ReloadSpeed;
                    IsReloading = true;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Stop the passive reloading.
            /// </summary>
            public void EndPassiveLoad()
            {
                if (IsReloading)
                {
                    IsReloading = false;
                }
            }
            /// <summary>
            /// Get the current ammo reloaded.
            /// </summary>
            public override int GetPassiveData()
            {
                int ammoReturn = 0;
                if (IsReloading && Time.time >= TimerReload)
                {
                    if (ReloadType)
                    {
                        float extra = Time.time - TimerReload;
                        ammoReturn = (Mathf.Max(Mathf.RoundToInt(extra/TimerReload),1)); //one bullet at a time system.
                        if (Ammo < MaxAmmo)
                        {
                            TimerReload = Time.time + ReloadSpeed; // Continue reloading
                        }
                        else
                        {
                            IsReloading = false;
                        }
                    }
                    else
                    {
                        if (Ammo < MaxAmmo)
                        {
                            ammoReturn = MaxAmmo;
                        }
                        else
                        {
                            ammoReturn = 0;
                        }
                        IsReloading = false;
                    }
                }
                return ammoReturn;
            }
            /// <summary>
            /// Get shoot gun
            /// </summary>
            /// <param name="endPassiveLoading">If so, should it end the passive reloading.</param>
            /// <returns>True for yes, you may shoot, false o</returns>
            public bool GetCanFire(bool endPassiveLoading)
            {
                if (Time.time > TimerAttackDelay && !GetIsAmmoEmpty())
                {
                    if (endPassiveLoading)
                    {
                        EndPassiveLoad();
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Only use on weapons that reload their entire clip at once. Gets if your currenlty reloading.
            /// </summary>
            /// <returns>True/False</returns>
            public bool GetIsReloading()
            {
                return IsReloading;
            }
            /// <summary>
            /// Gets a list of data for charging info<br></br><br></br>
            /// 0. <see cref="ChargeScale"/>
            /// <list type="number">
            /// <item><see cref="TimerCharge"/></item>
            /// <item>Your damage value between the minimum and maximum</item>
            /// <item>How many shots you fire, (int value)</item>
            /// <item>How much you charged it / its max possible charge value</item>
            /// <item><c>(Time.time - TimerCharge)/(ChargeTime);</c></item>
            /// <item>Previous option but without clamping</item>
            /// </list>
            /// </summary>
            /// <returns>float[6]</returns>
            public float[] GetChargeData() //TODO: Make a struct that holds this data rather than a overthetopcomplicated array.
            {
                float[] returnData = new float[7];

                // Store basic charge data
                returnData[0] = ChargeScale;
                returnData[1] = TimerCharge;

                // Handle case where charge time is zero to avoid division by zero
                if ((float)ChargeTime <= 0)
                {
                    returnData[2] = Damage;
                    returnData[3] = 1;
                    returnData[4] = 1;
                    returnData[5] = 0;
                    return returnData;
                }

                // Calculate charge progress (0 to 1)
                float chargeDuration = Mathf.Max(0, Time.time - TimerCharge);
                returnData[6] = (float)((float)chargeDuration / (float)ChargeTime);
                float chargeProgress = Mathf.Clamp01((float)((float)chargeDuration / (float)ChargeTime));
                returnData[5] = chargeProgress;

                // Calculate charge segments
                if (ChargeScale <= 0)
                {
                    returnData[2] = Damage;
                    returnData[3] = 1;
                    returnData[4] = 1;
                }
                else
                {
                    int chargeLevels = Mathf.FloorToInt(chargeProgress / ChargeScale);
                    float chargeRatio = Mathf.Clamp01((chargeProgress % ChargeScale) / ChargeScale);

                    // Calculate damage and shots based on charge
                    returnData[2] = Mathf.Lerp(MinChargeDamage, Damage, chargeProgress);
                    returnData[3] = Mathf.Min(1 + chargeLevels, ExtraShots);
                    returnData[4] = chargeRatio;
                }
                //Debug.Log($"Charge data - Progress: {chargeProgress}, Damage: {returnData[2]}, Shots: {returnData[3]}, Ratio: {returnData[4]}");
                return returnData;
            }
            /// <summary>
            /// Sets the charging state of a weapon
            /// </summary>
            /// <param name="state">True/False</param>
            public void SetChargeState(bool state)
            {
                IsCharging = state;
            }
            /// <summary>
            /// Get if the charge progress is above the minimum value.
            /// </summary>
            /// <returns>True/False</returns>
            public bool GetAboveMinCharge()
            {
                if (ChargeTime <= 0) return true;

                float chargeDuration = Mathf.Max(0, Time.time - TimerCharge);
                float chargeProgress = (float)Mathf.Clamp01((float)((float)chargeDuration / (float)ChargeTime));
                return chargeProgress >= Mincharge;
            }
            /// <summary>
            /// Get if the charge progress is above the max value.
            /// </summary>
            /// <returns>true/false</returns>
            public bool GetAboveMaxCharge()
            {
                if (ChargeTime != null)
                {
                    float chargeDuration = Mathf.Max(0, Time.time - TimerCharge);
                    float chargeProgress = (float)((float)chargeDuration / (float)ChargeTime);
                    return chargeProgress >= MaxCharge;
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="MinChargeDamage"/></returns>
            public float GetMinimiumDamage()
            {
                return MinChargeDamage;
            }
            /// <summary>
            /// Get the accruacy of the weapon
            /// </summary>
            /// <param name="hasRandom">Are the bullets spawned in randomly or have preset accuracty</param>
            /// <returns><c>(float)SphereAccuracy * (UnityEngine.Random.value - 0.5f);</c></returns>
            public float GetSphereAccuracy(bool hasRandom)
            {
                if (SphereAccuracy <= 0) return 0;
                if (SphereAccuracy == null) return 0;
                if (hasRandom)
                {
                    return (float)SphereAccuracy * (UnityEngine.Random.value - 0.5f);
                }
                return (float)SphereAccuracy;
            }
            /// <summary>
            /// Get the accuracy of the weaopn
            /// </summary>
            /// <param name="hasRandom">Are the bullets spawned in randomly or have preset accuracty</param>
            /// <returns><c>(float)(Mathf.Max((float)SphereAccuracy + adj, 0)) * (UnityEngine.Random.value - 0.5f);</c></returns>
            /// <param name="adj">New some adjustments to the Sphereacale accuracy</param>
            public float GetSphereAccuracy(bool hasRandom, float adj)
            {
                if (SphereAccuracy <= 0) return 0;
                if (SphereAccuracy == null) return 0;
                if (hasRandom)
                {
                    return (float)(Mathf.Max((float)SphereAccuracy + adj, 0) * (UnityEngine.Random.value - 0.5f));
                }
                return (float)SphereAccuracy;
            }
            /// <summary>
            /// Get the size of hitbox.x cord.
            /// </summary>
            /// <param name="offset">The offset from your player.</param>
            /// <returns></returns>
            public float GetBoxSize(float offset)
            {
                return Size.z + offset;
            }
            /// <summary>
            /// Gets the box size for melee weapons
            /// </summary>
            /// <returns></returns>
            public Vector3 GetBoxSize()
            {
                return Size;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns><see cref="InitialDelay"/></returns>
            public float GetInititalDelay()
            {
                return InitialDelay;
            }

        }
        /// <summary>
        /// Used in floating blcoks and used in <see cref="InventorySystem"/> as a list to create an inventory system. Stores the following data:
        /// <list type="bullet"></list>
        /// <item>Stores <see cref="Items.Item"/> and its instantations (i.e: <see cref="Weapon"/>, <see cref="Armor"/></item>
        /// <item>SlotID, Name, Desc, Price, Size, Texture</item>
        /// <item>Can be declared Empty (replacable) in <see cref="InventorySystem"/></item>
        /// </summary>
        public class InventoryItem : INameDesc, IInvetorySystemCompability
        {
            /// <summary>
            /// Item's Slot ID. Used when swapping items.
            /// </summary>
            protected int SlotID { get; set; }
            /// <summary>
            /// The amount of said item in inventory
            /// </summary>
            protected int amount = 1;
            /// <summary>
            /// Can you hold multiple of said item.
            /// </summary>
            public HoldingType HoldingType { get; protected set; } = HoldingType.UnlmintedStackable;
            public int Amount
            {
                get
                {
                    if (HoldingType == HoldingType.Single)
                    {
                        return 1;
                    }
                    return amount;
                }
                set
                {
                    if (HoldingType == HoldingType.Single)
                    {
                        amount = 1;
                    }
                    else if (HoldingType == HoldingType.LimitedStackable)
                    {
                        amount = Mathf.Clamp(value, 0, maxAmount);
                    }
                    else if (HoldingType == HoldingType.UnlmintedStackable)
                    {
                        amount = Mathf.Max(0, value);
                    }

                    // Mark for deletion if amount reaches 0 for stackable items
                    if (amount <= 0 && HoldingType != HoldingType.Single)
                    {
                        MarkedForDeletion = true;
                    }
                }
            }
            protected int maxAmount;
            /// <summary>
            /// The name of the item
            /// </summary>
            protected string Name { get; set; }
            /// <summary>
            /// The description of the item
            /// </summary>
            protected string Description { get; set; }
            /// <summary>
            /// How many inventory slots does it take to hold the item
            /// </summary>
            protected int SizeOfObject { get; set; }
            public int Price { get; protected set; }
            protected Item Item { get; set; }
            /// <summary>
            /// The Item Type.
            /// </summary>
            protected ItemType ItemType { get; set; }
            /// <summary>
            /// The UIIcon of the item
            /// </summary>
            protected Texture UiIcon { get; set; } = null;
            protected Material Material { get; set; } = null;
            /// <summary>
            /// Is the item a "empty" item to be replaced/disposed of when a new item enters the inventory.
            /// </summary>
            protected bool EmptyItem { get; set; }
            /// <summary>
            /// Is the weapon Charging?
            /// </summary>
            protected bool IsCharging { get; set; }
            public bool MarkedForDeletion { get; protected set; } = false;
            private float weight;
            public float Weight { get { return weight * amount; } protected set { weight = value; } }
            /// <summary>
            /// Setup the InventoryItem with a weapon.
            /// </summary>
            /// <param name="item">A Weapon</param>
            /// <param name="size">Invenotry Slot sizes. Doesn't do anything right now. However this could be used to create a item weight limit if needed</param>
            /// <param name="uiIcon">The Image to display in the inventory</param>
            /// <param name="price">How much does the item cost.</param>
            /// <param name="hold">Can this weapon hold multiple of itself in one slot</param>
            /// <param name="maxAmount">Maximum amount in hand</param>
            public InventoryItem(Weapon item, HoldingType hold, int size, int price, float weight, Texture uiIcon, int maxAmount = 1)
            {
                Name = item.GetName();
                Description = item.GetDesc();
                SizeOfObject = size;
                Item = item;
                ItemType = item.GetItemType();
                UiIcon = uiIcon;
                Price = price;
                HoldingType = hold;
                Weight = weight;
                if (HoldingType == HoldingType.LimitedStackable)
                {
                    this.maxAmount = maxAmount;
                }
                if (HoldingType == HoldingType.Single)
                {
                    this.maxAmount = 1;
                }
                Debug.Log($"Holding type: {HoldingType}");
            }
            /// <summary>
            /// Create a consumable item
            /// </summary>
            /// <param name="item">A Consumable</param>
            /// <param name="size">How big is it in the inventory</param>
            /// <param name="hold">holding type</param>
            /// <param name="price">How expensive</param>
            /// <param name="uiIcon"></param>
            /// <param name="maxAmount"></param>
            public InventoryItem(Consumable item, int size, HoldingType hold, int price, float weight, Texture uiIcon, int maxAmount = 1)
            {
                Name = item.GetName();
                Description = item.GetDesc();
                SizeOfObject = size;
                Item = item;
                ItemType = item.GetItemType();
                UiIcon = uiIcon;
                Price = price;
                HoldingType = hold;
                Weight = weight;
                if (HoldingType == HoldingType.LimitedStackable)
                {
                    this.maxAmount = maxAmount;
                }
                if (HoldingType == HoldingType.Single)
                {
                    this.maxAmount = 1;
                }
                Debug.Log($"Holding type: {HoldingType}");
            }
            /// <summary>
            /// Setup the InventoryItem to have a Item object
            /// </summary>
            /// <param name="item"></param>
            /// <param name="hold">Holding type</param>
            /// <param name="size"></param>
            /// <param name="uiIcon"></param>
            public InventoryItem(Item item, int size, HoldingType hold, int price, float weight, Texture uiIcon, int maxAmount = 1)
            {
                HoldingType = hold;
                Name = item.GetName();
                Debug.Log(Name);
                Description = item.GetDesc();
                SizeOfObject = size;
                Item = item;
                ItemType = item.GetItemType();
                UiIcon = uiIcon;
                Price = price;
                Weight = weight;
                if (HoldingType == HoldingType.LimitedStackable)
                {
                    this.maxAmount = maxAmount;
                }
                if (HoldingType == HoldingType.Single)
                {
                    this.maxAmount = 1;
                }
                Debug.Log($"Holding type: {HoldingType}");
            }
            public DuplicateReturn GetIsDuplication(bool apply, InventoryItem item)
            {
                // Check if items are the same type
                bool match = Name == item.GetName() && Description == item.GetDesc();
                if (!match)
                    return DuplicateReturn.False;

                // If not applying, just return that it's a duplicate
                if (!apply)
                    return DuplicateReturn.True;

                // Handle stackable items
                if (HoldingType == HoldingType.LimitedStackable || HoldingType == HoldingType.UnlmintedStackable)
                {
                    int availableSpace = 0;

                    if (HoldingType == HoldingType.LimitedStackable)
                    {
                        availableSpace = maxAmount - Amount;
                    }
                    else if (HoldingType == HoldingType.UnlmintedStackable)
                    {
                        availableSpace = int.MaxValue - Amount;
                    }

                    if (availableSpace <= 0)
                        return DuplicateReturn.True; // No space, but still a duplicate

                    int amountToAdd = Math.Min(item.Amount, availableSpace);
                    Amount += amountToAdd;

                    // Reduce the source item's amount
                    int remaining = item.Amount - amountToAdd;
                    item.Amount = remaining;

                    if (remaining <= 0)
                    {
                        item.MarkedForDeletion = true;
                        return DuplicateReturn.Incrimented;
                    }

                    return DuplicateReturn.Incrimented;
                }

                return DuplicateReturn.True; // Non-stackable duplicate
            }
            /// <summary>
            /// Setup the InventoryItem to have Armor
            /// </summary>
            /// <param name="item"></param>
            /// <param name="id"></param>
            /// <param name="name"></param>
            /// <param name="description"></param>
            /// <param name="size"></param>
            /// <param name="uiIcon"></param>
            public InventoryItem(Armor item, int size, HoldingType hold, int price, float weight, Texture uiIcon, int maxAmount = 1)
            {
                Name = item.GetName();
                Description = item.GetDesc();
                SizeOfObject = size;
                Item = item;
                ItemType = ItemType.Armor;
                UiIcon = uiIcon;
                Price = price;
                HoldingType = hold;
                Weight = weight;
                if (HoldingType == HoldingType.LimitedStackable)
                {
                    this.maxAmount = maxAmount;
                }
                if (HoldingType == HoldingType.Single)
                {
                    this.maxAmount = 1;
                }
            }
            public T GetItem<T>() where T : Item
            {
                try { return Item as T; }
                catch { return null; }
            }
            /// <summary>
            /// Returns a <see cref="BaseCharacter.Items.Item"/> if it has one.
            /// </summary>
            /// <returns></returns>
            public Item GetItem()
            {
                try { return Item; }
                catch { return null; }
            }
            public void SetGenericMaterial(Material mat)
            {
                Material = mat;
            }
            public Material GetGenericMaterial()
            {
                return Material;
            }
            /// <summary>
            /// Setup a Empty item which is to be overwritten when nonEmpty inventory item is to join
            /// </summary>
            /// <param name="id">Set to the slot id</param>
            public InventoryItem(int id)
            {
                EmptyItem = true;
                Name = "";
                Item = new Item("Empty", ItemType.Empty);
                SlotID = id;
                Price = 0;
                Weight = 0;
            }
            public void SetSlotId(int id)
            {
                SlotID = id;
            }
            /// <summary>
            /// Attempts to get the texture. If it doesn't have a texture return null.
            /// </summary>
            /// <returns>the texture stored in this object</returns>
            public Texture GetTheTexture()
            {
                if (UiIcon != null)
                {
                    return UiIcon;
                }
                return null;
            }
            /// <summary>
            /// Set the texture of the hotbar item.
            /// </summary>
            /// <param name="texture">Texture</param>
            public void SetTheTexture(Texture texture)
            {
                UiIcon = texture;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Gets the name of the item</returns>
            public string GetName()
            {
                return Name;
            }
            /// <summary>
            /// Compares the name requested and returns True/False
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool GetName(string name)
            {
                if (name == null)
                {
                    return false;
                }
                return Name == name;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns>The Desc</returns>
            public string GetDesc()
            {
                return Description;
            }
            public bool GetDesc(string desc)
            {
                if (desc == null)
                {
                    return false;
                }
                return Description == desc;
            }
            /// <summary>
            /// Get the size of the object in your inventory.
            /// </summary>
            /// <returns></returns>
            public int GetSize()
            {
                return SizeOfObject;
            }

            //TODO: ADD AUTO STATS SYSTEM:
            /// <summary>
            /// Checks if the item is a empty/to be replaced slot.
            /// </summary>
            /// <returns>true/false</returns>
            public bool GetIsEmptyItem()
            {
                return EmptyItem;
            }
            /// <summary>
            /// Gets the ID where the item is being stored.
            /// </summary>
            /// <returns>SlotID</returns>
            public int GetSlotID()
            {
                return SlotID;
            }
            /// <summary>
            /// GetsSlotID and moves it immidiatly to a new location
            /// </summary>
            /// <param name="NewLocation">ID of where the item is to go</param>
            /// <returns>SlotID</returns>
            public int GetSlotID(int NewLocation)
            {
                MoveItem(NewLocation);
                return SlotID;
            }
            /// <summary>
            /// Move an item
            /// </summary>
            /// <param name="location">The ID of where to go</param>
            public void MoveItem(int location)
            {
                SlotID = location;
            }
            /// <summary>
            /// Get what instantiation the <see cref="BaseCharacter.Item"/> is via <see cref="Enums.ItemType"/>
            /// </summary>
            /// <returns></returns>
            public ItemType GetItemType()
            {
                return ItemType;
            }
            public bool GetCharging()
            {
                return IsCharging;
            }
            public void SetCharge(bool value)
            {
                IsCharging = value;
            }
            /// <summary>
            /// Decrease how much of item is held in this object
            /// </summary>
            /// <param name="decrease"></param>
            /// <returns>The amount to give back</returns>
            public int DecreaseHeld(int decrease)
            {
                Mathf.Abs(decrease);
                if (decrease >= amount)
                {
                    int value = amount;
                    amount = 0;
                    MarkedForDeletion = true;
                    return value;
                }
                amount -= decrease;
                return amount;
            }
            /// <summary>
            /// Preform an automatic action.<br></br>
            /// <list type="number">
            /// <item>Item: Nothing</item>
            /// <item>Wewapon: Reload</item>
            /// <item>Consumable: Conditional Effect</item>
            /// </list>
            /// </summary>
            /// <param name="items">Inventory</param>
            /// <returns></returns>
            public bool GetPassiveData(List<InventoryItem> items)
            {
                if (ItemType == ItemType.Weapon)
                {
                    GetItem<Weapon>().AddAmmo(items,GetItem<Weapon>().GetPassiveData());
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Preform an automatic action.<br></br>
            /// <list type="number">
            /// <item>Item: Nothing</item>
            /// <item>Wewapon: Reload</item>
            /// <item>Consumable: Conditional Effect</item>
            /// </list>
            /// </summary>
            /// <param name="items">Inventory</param>
            /// <returns></returns>
            public bool GetPassiveData(Player player)
            {
                if (ItemType == ItemType.Weapon)
                {
                    GetItem<Weapon>().AddAmmo(player, GetItem<Weapon>().GetPassiveData());
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Copy constructor
            /// </summary>
            /// <param name="other"></param>
            /// <exception cref="ArgumentNullException">Null</exception>
            public InventoryItem(InventoryItem other)
            {
                if (other == null)
                {
                    throw new ArgumentNullException(nameof(other));
                }

                // Copy primitive properties
                SlotID = other.SlotID;
                amount = other.amount;
                maxAmount = other.maxAmount;
                Name = other.Name;
                Description = other.Description;
                SizeOfObject = other.SizeOfObject;
                Price = other.Price;
                ItemType = other.ItemType;
                EmptyItem = other.EmptyItem;
                IsCharging = other.IsCharging;
                HoldingType = other.HoldingType;
                MarkedForDeletion = other.MarkedForDeletion;
                Weight = other.Weight;

                // Deep copy the Item based on its type
                if (other.Item != null)
                {
                    switch (other.Item.GetItemType())
                    {
                        case ItemType.Weapon:
                        case ItemType.Melee:
                            Item = new Weapon((Weapon)other.Item);
                            break;
                        case ItemType.Consumable:
                            Item = new Consumable((Consumable)other.Item);
                            break;
                        case ItemType.Armor:
                            Item = new Armor((Armor)other.Item);
                            break;
                        case ItemType.Empty:
                            Item = new Item(other.Item.GetName(), ItemType.Empty);
                            break;
                        default:
                            Item = new Item(other.Item); // Ammo, Itmes, etc... are all just an item.
                            break;
                    }
                }

                UiIcon = other.UiIcon; // Texture is typically managed by Unity
                Material = other.Material;
            }
            /// <summary>
            /// Get how many you can hold
            /// </summary>
            /// <returns></returns>
            public int GetHeldAmount()
            {
                if (HoldingType == HoldingType.Single)
                {
                    return 1;
                }
                else
                {
                    return Amount;
                }
            }
            public string GetHeldAmountString()
            {
                if (HoldingType.Single == HoldingType)
                {
                    return ItemType switch
                    {
                        ItemType.Weapon => $"{GetItem<Weapon>().GetAmmoCount()}/{GetItem<Weapon>().GetMaxAmmo()}",
                        _ => $"{Name}",
                    };
                }
                else
                {
                    return $"{Amount}";
                }
            }
        }
        /// <summary>
        /// Create a quest
        /// </summary>
        public class Quest : InventorySystem, INameDesc, IMoney, IInvetorySystemCompability
        {
            /// <summary>
            /// Money Eanred
            /// </summary>
            protected int MoneyReward { get; set; }
            protected bool HasItemsReward { get; set; }
            protected bool WillRegenHealthOnCompletion { get; set; }
            protected bool IsVisible { get; set; }
            protected bool IsClickable { get; set; }
            public int MaxHealthIncrease { get; protected set; }
            public float SpeedInc { get; protected set; }
            public float JumpInc { get; protected set; }
            public float VisionChange { get; protected set; }
            protected string QuestName { get; set; }
            protected string Desc { get; set; }
            protected string SceneName { get; set; }
            protected QuestStage Stage { get; set; }
            protected int Level { get; set; }
            public bool IsCompleted { get; protected set; }
            public bool IsFailed { get; protected set; }
            /// <summary>
            /// Quest stage
            /// </summary>
            protected bool IsActive { get; set; } = false;
            public int SlotID { get; protected set; } = 0;
            public int GetSlotID()
            {
                return SlotID;
            }
            public int GetSlotID(int newSlot)
            {
                int slot = SlotID;
                SlotID = newSlot;
                return slot;
            }
            protected bool IsEmptyQuest = false;
            public bool GetIsEmptyItem()
            {
                return IsEmptyQuest;
            }
            public void MoveItem(int id)
            {
                SlotID = id;
            }
            protected Texture TextureUI { get; set; }

            /// <summary>
            /// Setup a quest.
            /// </summary>
            /// <param name="name">Name of the quest</param>
            /// <param name="money"></param>
            /// <param name="hpInc"></param>
            /// <param name="speedInc"></param>
            /// <param name="jumpInc"></param>
            /// <param name="visionChange"></param>
            /// <param name="HealthReg"></param>
            public Quest(string name, string sceneName, int level, int money, int hpInc, float speedInc, float jumpInc, float visionChange, bool HealthReg, string desc)
            {
                QuestName = name;
                MoneyReward = money;
                MaxHealthIncrease = hpInc;
                SpeedInc = speedInc;
                JumpInc = jumpInc;
                VisionChange = visionChange;
                HasItemsReward = false;
                WillRegenHealthOnCompletion = HealthReg;
                SceneName = sceneName;
                Level = level;
                Desc = desc;
            }
            public Quest(string name, string sceneName, int level, int money, int hpInc, float speedInc, float jumpInc, float visionChange, bool HealthReg, string desc, InventoryItem item)
            {
                QuestName = name;
                MoneyReward = money;
                MaxHealthIncrease = hpInc;
                SpeedInc = speedInc;
                JumpInc = jumpInc;
                VisionChange = visionChange;
                HasItemsReward = false;
                WillRegenHealthOnCompletion = HealthReg;
                SceneName = sceneName;
                Level = level;
                Desc = desc;
                Inventory.Add(item);
            }
            public Quest(Quest quest)
            {
                MoneyReward = quest.MoneyReward;
                HasItemsReward = quest.HasItemsReward;
                Inventory = quest.Inventory != null ? new List<InventoryItem>(quest.Inventory) : null;
                WillRegenHealthOnCompletion = quest.WillRegenHealthOnCompletion;
                IsVisible = quest.IsVisible;
                IsClickable = quest.IsClickable;
                MaxHealthIncrease = quest.MaxHealthIncrease;
                SpeedInc = quest.SpeedInc;
                JumpInc = quest.JumpInc;
                VisionChange = quest.VisionChange;
                QuestName = quest.QuestName;
                Desc = quest.Desc;
                SceneName = quest.SceneName;
                Stage = quest.Stage;
                Level = quest.Level;
                IsActive = quest.IsActive;
            }
            public Quest(int i)
            {
                SlotID = i;
                IsEmptyQuest = true;
                Stage = QuestStage.Inactive;
            }
            public void SetQuestStage(QuestStage stage)
            {
                Stage = stage;
                IsActive = (stage == QuestStage.Active);
                IsCompleted = (stage == QuestStage.Completed || stage == QuestStage.Rewarded);
                IsFailed = (stage == QuestStage.Failed);

                // Update visibility based on stage
                IsVisible = (stage != QuestStage.Unavailable);
                IsClickable = (stage == QuestStage.Inactive || stage == QuestStage.Completed);
            }
            public QuestStage GetQuestStage()
            {
                return Stage;
            }
            public string GetSceneStage()
            {
                return SceneName;
            }
            public string GetQuestName()
            {
                return QuestName;
            }
            /// <summary>
            /// Set the description for the weapon to appear on the UI
            /// </summary>
            /// <param name="UIdesc"></param>
            /// <param name="type"></param>
            public void SetDesc(string UIdesc, int type)
            {
                if (type == 0)
                {
                    Desc = UIdesc;
                }
                else
                {
                    Desc = $"{UIdesc}\nRewards: HP{MaxHealthIncrease}, Speed{SpeedInc}, Jump{JumpInc}, Vision {VisionChange}";
                }

            }
            public string GetDesc()
            {
                return Desc;
            }
            public bool GetDesc(string desc)
            {
                return desc == Desc;
            }
            public string GetName()
            {
                return QuestName;
            }
            public bool GetName(string name)
            {
                return name == QuestName;
            }
            public Texture GetTheTexture()
            {
                return TextureUI;
            }
            public void SetIsClickable(bool clicky)
            {
                IsClickable = clicky;
            }
            public void SetIsVisible(bool isSeeable)
            {
                IsVisible = isSeeable;
            }
            #region GetRewards
            /// <summary>
            /// Get how much money you have
            /// </summary>
            /// <returns></returns>
            public int GetMoneyInt()
            {
                return MoneyReward;
            }
            /// <summary>
            /// Get how much money you have formatted as a string
            /// </summary>
            /// <returns>Dollars.Cents</returns>
            public string GetMoney()
            {
                return $"{MoneyReward / 100.0:C2}";
            }
            public bool HealPlayer()
            {
                return WillRegenHealthOnCompletion;
            }
            public int GetLevelActivation()
            {
                return Level;
            }
            public void ApplyRewards(Player player)
            {
                if (Stage != QuestStage.Completed) return;

                player.AddMoney(MoneyReward);
                if (Inventory != null)
                {
                    foreach (InventoryItem item in Inventory)
                    {
                        player.AddItem(item);
                    }
                }
                SetQuestStage(QuestStage.Rewarded);
            }
            #endregion
        }
    }
    namespace Enemy
    {
        public class PathFind
        {
               
        }
        public static class PresetPathing
        {
            
        }
    }
    /// <summary>
    /// Used by many storage systems such as <see cref="Player"/>, <see cref="Entities"/> To display inventory UI, you'll need to use <see cref="InvManager"/> to show textures and hotbars.
    /// </summary>
    public class InventorySystem : IInventorySystem<InventoryItem>
    {
        /// <summary>
        /// How many items can you hold
        /// </summary>
        protected int SizeOfInventory { get; set; }

        /// <summary>
        /// The base size of your inventory (without expansions)
        /// </summary>
        protected int BaseSizeOfInventory { get; set; }

        /// <summary>
        /// What you have in your inventory
        /// </summary>
        protected List<InventoryItem> Inventory { get; set; } = new List<InventoryItem>();

        /// <summary>
        /// The selected item used for swapping.
        /// </summary>
        protected int PendingItem { get; set; } = -1;

        /// <summary>
        /// Current Inventory Slot
        /// </summary>
        protected int CurrentHotBarSlot { get; set; } = 0;

        /// <summary>
        /// Hotbar Size.
        /// </summary>
        protected int HotbarSize { get; set; } = 6;
        /// <summary>
        /// Returns the total weight of all the items.
        /// </summary>
        protected float TotalWeight
        {
            get 
            {
                float weight = 0;
                for (int i = 0; i < Inventory.Count; i++)
                {
                    weight += Inventory[i].Weight;
                }
                return weight;
            }
        }
        #region Fill Null Inventory
            /// <summary>
            /// Clear an inventory and then fill it with empty items which can be moved or erased. 
            /// Used to clear an inventory or to fill a Null inventory.
            /// </summary>
        public void FillNullInventory()
        {
            Inventory.Clear();
            // Ensure SizeOfInventory is at least 1
            int actualSize = Mathf.Max(1, SizeOfInventory);
            for (int i = 0; i < actualSize; i++)
            {
                Inventory.Add(new InventoryItem(i));
            }
        }
        /// <summary>
        /// Will clear an inventory starting from <paramref name="start"/>
        /// </summary>
        /// <param name="start">Where to begin</param>
        public void FillNullInventory(int start)
        {
            // Ensure SizeOfInventory is at least 1
            int actualSize = Mathf.Max(1, SizeOfInventory);

            if (start >= actualSize)
            {
                return;
            }

            // Remove items from start position to end
            if (start < Inventory.Count)
            {
                Inventory.RemoveRange(start, Inventory.Count - start);
            }

            // New new empty items
            for (int i = start; i < actualSize; i++)
            {
                Inventory.Add(new InventoryItem(i));
            }
        }
        #endregion
        #region Add and Delete Items/Inventory spaces
        /// <summary>
        /// Allows you to delete an item and replace it with an empty item.
        /// </summary>
        /// <param name="id">The slot at where the Inventory item was removed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when id is out of range</exception>
        public void DeleteItem(int id)
        {
            if (id < 0 || id >= Inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID is out of inventory range");
            }

            Inventory[id] = new InventoryItem(id);
        }
        /// <summary>
        /// Checks your entire inventory as to remove items which are <see cref="InventoryItem.MarkedForDeletion"/>
        /// </summary>
        public void CheckForDeletion()
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i].MarkedForDeletion)
                {
                    DeleteItem(i);
                }
            }
        }
        /// <summary>
        /// Makes inventory bigger and fills the empty spots with empty items
        /// </summary>
        /// <param name="add">Number of slots to add</param>
        public void AddInventorySpaces(int add)
        {
            int oldSize = SizeOfInventory;
            add = Mathf.Max(1, Mathf.Abs(add));
            SizeOfInventory += add;
            FillNullInventory(oldSize);
        }
        /// <summary>
        /// Adds an <see cref="InventoryItem"/>to the inventory unless the inventory is full
        /// </summary>
        /// <param name="item">The <see cref="InventoryItem"/> to be added</param>
        /// <param name="start">Where to start the search. This is great when loading from a save file when you need to add spacing between items spawning in the inventory</param>
        public bool AddItem(InventoryItem item, int start = 0)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }

            start = Mathf.Clamp(start, 0, SizeOfInventory - 1);

            // First, try to stack with existing items of the same type
            for (int i = 0; i < Mathf.Min(SizeOfInventory, Inventory.Count); i++)
            {
                if (!Inventory[i].GetIsEmptyItem())
                {
                    DuplicateReturn output = Inventory[i].GetIsDuplication(true, item);

                    if (output == DuplicateReturn.Incrimented)
                    {
                        // Item was partially or fully stacked
                        if (item.Amount <= 0 || item.MarkedForDeletion)
                        {
                            return true; // Item fully consumed
                        }
                        // Continue to find space for remaining items
                        break;
                    }
                    else if (output == DuplicateReturn.True)
                    {
                        // Items are same type but couldn't stack (non-stackable or full)
                        // Continue searching for empty slot
                    }
                }
            }

            // Clean up any marked for deletion items
            CheckForDeletion();

            // If item still exists and has amount > 0, find empty slot
            if (item.Amount > 0 && !item.MarkedForDeletion)
            {
                return FindEmptySlotForItem(item, start);
            }

            return true; // Item was fully stacked
        }
        /// <summary>
        /// Find an empty slot for an item
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="start">Start location for search</param>
        /// <returns></returns>
        private bool FindEmptySlotForItem(InventoryItem item, int start)
        {
            // Try to find an empty slot from start position
            for (int i = start; i < SizeOfInventory; i++)
            {
                if (TryPlaceItemInSlot(item, i))
                    return true;
            }

            // If not found from start position, search from beginning
            if (start > 0)
            {
                for (int i = 0; i < start; i++)
                {
                    if (TryPlaceItemInSlot(item, i))
                        return true;
                }
            }

            Debug.Log("After searching for space in your inventory, we found NOTHING open.");
            return false;
        }
        /// <summary>
        /// Try to place an item in a slot.
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <param name="slotIndex">Start slot index</param>
        /// <returns>Was it successfull</returns>
        private bool TryPlaceItemInSlot(InventoryItem item, int slotIndex)
        {
            if (slotIndex < Inventory.Count)
            {
                if (Inventory[slotIndex].GetIsEmptyItem())
                {
                    Inventory[slotIndex] = item;
                    item.MoveItem(slotIndex);
                    CheckPhaseItem(slotIndex);
                    return true;
                }
            }
            else
            {
                // Expand the inventory list if needed
                item.MoveItem(slotIndex);
                Inventory.Add(item);
                CheckPhaseItem(slotIndex);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Adds multiple <see cref="InventoryItem"/> to the inventory
        /// </summary>
        /// <param name="items">Array of items to be added</param>
        /// <param name="start">Where to start the search</param>
        /// <returns>True if all items were added successfully</returns>
        public bool AddItem(InventoryItem[] items, int start = 0)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), "Items array cannot be null");
            }

            if (Inventory == null || SizeOfInventory <= 0)
            {
                Debug.LogWarning("Inventory is not properly initialized");
                return false;
            }

            start = Mathf.Clamp(start, 0, SizeOfInventory - 1);
            bool allItemsAdded = true;

            foreach (InventoryItem currentItem in items)
            {
                if (currentItem == null)
                {
                    Debug.LogWarning("Null item in array, skipping");
                    continue;
                }

                bool itemAdded = false;

                // First try to stack with existing items of the same type
                for (int i = 0; i < Mathf.Min(SizeOfInventory, Inventory.Count); i++)
                {
                    if (!Inventory[i].GetIsEmptyItem())
                    {
                        DuplicateReturn dReturn = Inventory[i].GetIsDuplication(true, currentItem);

                        if (dReturn == DuplicateReturn.Incrimented)
                        {
                            // Item was partially or fully stacked
                            if (currentItem.Amount <= 0 || currentItem.MarkedForDeletion)
                            {
                                itemAdded = true; // Item fully consumed
                                break;
                            }
                        }
                    }
                }

                // If item wasn't fully consumed by stacking and still exists, find empty slot
                if (!itemAdded && currentItem.Amount > 0 && !currentItem.MarkedForDeletion)
                {
                    itemAdded = FindEmptySlotForItem(currentItem, start);
                }
                else if (currentItem.MarkedForDeletion || currentItem.Amount <= 0)
                {
                    // Item was fully consumed by stacking
                    itemAdded = true;
                }

                if (!itemAdded)
                {
                    Debug.LogWarning($"Could not find space for item in inventory: {currentItem.GetName()}");
                    allItemsAdded = false;
                }

                // Clean up after each item addition to free up slots
                CheckForDeletion();
            }

            return allItemsAdded;
        }
        /// <summary>
        /// Run a check that identifiefies which items are usable during the PreGame and PostVoting
        /// </summary>
        /// <param name="index"></param>
        public void CheckPhaseItem(int index)
        {
            //TODO: Somthing that uses
            //ItemType type = Inventory[index].GetItemType();
        }
        #endregion
        #region Order By
        public void OrderItemsByName()
        {
            Inventory = Inventory.OrderBy(item => item.GetName()).ToList();
        }
        public void OrderItemsByItemType()
        {
            Inventory = Inventory.OrderBy(item => item.GetItemType()).ToList();
        }
        public void OrderItemsBySize()
        {
            Inventory = Inventory.OrderBy(item => item.GetSize()).ToList();
        }
        public void OrderItemsByPrice()
        {
            Inventory = Inventory.OrderBy(item => item.Price).ToList();
        }
        private void ReindexItems()
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                Inventory[i].MoveItem(i);
            }
        }
        #endregion
        #region Swap Items
        /// <summary>
        /// Lets you swap 2 inventory items.
        /// </summary>
        /// <param name="index1">Item a</param>
        /// <param name="index2">Item b</param>
        /// <exception cref="IndexOutOfRangeException"> You seletected an item out of range</exception>
        public void SwapItem(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0 || index1 >= Inventory.Count || index2 >= Inventory.Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index1 == index2)
            {
                return; // No need to swap same items
            }
            try
            {
                (Inventory[index1], Inventory[index2]) = (Inventory[index2], Inventory[index1]);
                // Update their slot IDs
                Inventory[index1].MoveItem(index2);
                Inventory[index2].MoveItem(index1);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Swap failed: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Set a item as Selected. Or <see cref="PendingItem"/>
        /// </summary>
        /// <param name="index">Which item</param>
        public void SelectItem(int index)
        {
            PendingItem = (index >= 0 && index < Inventory.Count) ? index : -1;
        }
        /// <summary>
        /// Get the selected item and then immidiatly clear it.
        /// </summary>
        /// <returns></returns>
        public int GetPendingItemAndClear()
        {
            if (PendingItem > -1)
            {
                int temp = PendingItem;
                PendingItem = -1;
                return temp;
            }
            return -1;
        }
        /// <summary>
        /// Get the item awating to be swapped.
        /// </summary>
        /// <returns></returns>
        public int GetPendingItem()
        {
            return PendingItem;
        }
        #endregion
        #region Hotbar
        /// <summary>
        /// Setup the hotbar size and default slot
        /// </summary>
        /// <param name="amount">Size of the hotbar</param>
        /// <param name="defaultHotbarSlot">Default slot</param>
        public void SetupHotbar(int amount, int defaultHotbarSlot = 0)
        {
            HotbarSize = (byte)Mathf.Clamp(amount, 1, SizeOfInventory);
            CurrentHotBarSlot = Mathf.Clamp(defaultHotbarSlot, 0, HotbarSize - 1);
        }
        /// <summary>
        /// Scroll up or Scroll down.
        /// </summary>
        /// <param name="amount">A value +1 or -1</param>
        public void ScrollItem(int amount)
        {
            if (amount == 0) return;
            CurrentHotBarSlot += amount;
            if (CurrentHotBarSlot < 0)
            {
                CurrentHotBarSlot = HotbarSize - 1;
            }
            if (CurrentHotBarSlot > HotbarSize - 1)
            {
                CurrentHotBarSlot = 0;
            }
        }
        /// <summary>
        /// Go to a slot. Does not throw an error if the slot is greater than the hotbar size, instead the method will do nothing.
        /// </summary>
        /// <param name="slot">slot ID to go to</param>
        public void SetHotbarSlot(int slot)
        {
            if (slot >= 0 && slot < HotbarSize)
            {
                CurrentHotBarSlot = slot;
            }
        }
        /// <summary>
        /// Gets the size of the hotbar
        /// </summary>
        /// <returns><code>HotbarSize</code></returns>
        public int GetHotbarSize()
        {
            return HotbarSize;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Gets the hotbar slot that is currently being highlighted</returns>
        public int GetHotbarSlot()
        {
            return CurrentHotBarSlot;
        }
        #endregion
        #region GetMescData
        /// <summary>
        /// Get the total inventory size. 
        /// </summary>
        public int GetInventorySize()
        {
            return SizeOfInventory;
        }

        /// <summary>
        /// Gets the texture of an item.
        /// </summary>
        /// <param name="id">What slot the item is in your inventory.</param>
        /// <returns>A Texture.</returns>
        public Texture GetTextureItem(int id)
        {
            if (id < 0 || id >= Inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID is out of inventory range");
            }
            return Inventory[id].GetTheTexture();
        }
        #endregion
        #region Get Inventory Items
        /// <summary>
        /// Get your ENTIRE INVENTORY
        /// </summary>
        /// <returns><see cref="Inventory"/></returns>
        public List<InventoryItem> GetInventory()
        {
            return Inventory;
        }
        /// <summary>
        /// Return only items based on the item type
        /// </summary>
        /// <param name="type">The item type</param>
        /// <returns>The Inventory based on your type</returns>
        public List<InventoryItem> GetInventory(ItemType type)
        {
            List<InventoryItem> typeTori = new();
            foreach (InventoryItem item in Inventory)
            {
                if (type == item.GetItemType())
                {
                    typeTori.Add(item);
                }
            }
            return typeTori;
        }
        public List<InventoryItem> GetInventory(HoldingType type)
        {
            List<InventoryItem> typeTori = new();
            foreach (InventoryItem item in Inventory)
            {
                if (type == item.HoldingType)
                {
                    typeTori.Add(item);
                }
            }
            return typeTori;
        }
        /// <summary>
        /// Get a single inventory item based on slot id.
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns>A <see cref="InventoryItem"/></returns>
        public InventoryItem GetInventoryItem(int id)
        {
            if (id < 0 || id >= Inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID is out of inventory range");
            }
            return Inventory[id];
        }
        /// <summary>
        /// Get the inventory item based on <see cref="CurrentHotBarSlot"/>
        /// </summary>
        /// <returns></returns>
        public InventoryItem GetInventoryItemCurrentHotbar()
        {
            return Inventory[CurrentHotBarSlot];
        }
        /// <summary>
        /// Find an inventory item by name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns><see cref="InventoryItem"/></returns>
        public InventoryItem GetInventoryItem(string name)
        {
            return Inventory.Find(item => item.GetName() == name);
        }
        #endregion
        #region Full Scan
        /// <summary>
        /// Check passive data of items.
        /// </summary>
        public void CheckPassive()
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                Inventory[i].GetPassiveData(Inventory);
            }
        }
        #endregion
    }
    /// <summary>
    /// The player class. Use for any real life persons playing the game.
    /// </summary>
    public class Player : InventorySystem, IWallet, INameDesc, IApplyEffects, IMoney
    {
        public uint ID { get; private set; }
        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { get; protected set; }
        public string Desc { get; protected set; }
        /// <summary>
        /// Money
        /// </summary>
        protected int Money { get; set; }
        /// <summary>
        /// Are you alive
        /// </summary>
        protected bool IsAlive { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        protected string Description { get; set; }
        public StatHealth Health { get; private set; }
        /// <summary>
        /// Adranaline time based systems. 
        /// </summary>
        public float Adranaline { get; protected set; }
        /// <summary>
        /// How far away does it activate
        /// </summary>
        public float AdranalineDistance { get; protected set; }
        /// <summary>
        /// Your base Speed on spawn
        /// </summary>
        public Stat SpeedBase { get; private set; }
        /// <summary>
        /// Your bonus speed from external items such as speed boosts or powerups
        /// </summary>
        protected StatAdjustment SpeedBonus { get; set; } = new StatAdjustment();
        /// <summary>
        /// Your current speed
        /// </summary>
        protected float Speed
        {
            get
            {
                return SpeedBase.Max * SpeedBonus.Strength;
            }
        }
        protected float RotationSpeed { get; set; }
        /// <summary>
        /// Your current Jump
        /// </summary>
        public float Jump
        {
            get
            {
                return JumpBase.Max * JumpBonus.Strength;
            }
        }
        public float GroundPound
        {
            get
            {
                return GroundPoundBase.Max * GroundPoundBonus.Strength;
            }
        }
        public Stat GroundPoundBase { get; private set; }
        protected StatAdjustment GroundPoundBonus { get; set; } = new StatAdjustment();
        /// <summary>
        /// Your Jump on game start
        /// </summary>
        public Stat JumpBase { get; protected set; }
        /// <summary>
        /// Your bonus jump from external items such as jump boots or powerups
        /// </summary>
        protected StatAdjustment JumpBonus { get; set; } = new StatAdjustment();
        /// <summary>
        /// Your current gravity
        /// </summary>
        protected float Gravity { get; set; }
        /// <summary>
        /// base Gravity
        /// </summary>
        public float GravityBase { get; protected set; }
        public float GravityProtectionTime { get; protected set; }
        /// <summary>
        /// Your Weight. 100 is the normal
        /// </summary>
        public float Weight
        {
            get
            {
                return WeightBase * WeightAdjustment.Strength + TotalWeight;
            }
        }
        /// <summary>
        /// Your Weight on spawn
        /// </summary>
        public float WeightBase { get; protected set; }
        /// <summary>
        /// Adjusting weight due to items, armor, effects, etc...
        /// </summary>
        protected StatAdjustment WeightAdjustment { get; set; } = new StatAdjustment();
        /// <summary>
        /// Interaction range from enemies and also effects how stealthy you are.
        /// </summary>
        public float Vision
        {
            get
            {
                return VisionBase.Max * VisionAdjustment.Strength;
            }
        }
        /// <summary>
        /// Your base vision at spawn
        /// </summary>
        public Stat VisionBase { get; protected set; }
        public StatAdjustment VisionAdjustment { get; protected set; } = new StatAdjustment();
        public float Aiming 
        { 
            get
            {
                return Aim.Max * AimAdjustment.Strength + CryingAim;
            } 
        }
        public Stat Aim { get; protected set; }
        private float CryingAim = 0;
        public StatAdjustment AimAdjustment{ get; protected set; } = new StatAdjustment();
        /// <summary>
        /// Interaction range.
        /// </summary>
        protected float Reach { get; set; }
        protected float[] Resistances { get; set; } = new float[Enum.GetValues(typeof(WeaponClass)).Length];
        /// <summary>
        /// Attribute Fire damage
        /// </summary>
        protected List<FireDamage> FireDamage { get; set; } = new List<FireDamage>();
        /// <summary>
        /// :^( effect
        /// </summary>
        protected List<Crying> Cryings { get; set; } = new List<Crying>();
        /// <summary>
        /// Flytation effect
        /// </summary>
        protected Floatation Floating { get; set; }
        /// <summary>
        /// Regenerate health over time
        /// </summary>
        protected List<Regeneration> ActiveRegenerations { get; set; } = new List<Regeneration>();
        protected Wounded Wound { get; set; } = new Wounded();
        protected QuestSystem QuestSystem { get; set; }
        public int GameLevel
        {
            get
            {
                return Mathf.Min(Health.Level, SpeedBase.Level, JumpBase.Level, VisionBase.Level, GroundPoundBase.Level, Aim.Level);
            }
            protected set
            {
                level = value;
                Health.Level = value;
                SpeedBase.Level = value;
                JumpBase.Level = value;
                VisionBase.Level = value;
                GroundPoundBase.Level = value;
                Aim.Level = value;
            }
        }
        private int level;
        public float BreakingSpeed { get; protected set; } = 1f;
        //private string HomeScene { get; set; } = "SampleScene";

        #region Initialization
        /// <summary>
        /// Setup player
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="healthBase"></param>
        /// <param name="hpStartPercent"></param>
        /// <param name="weight"></param>
        /// <param name="vision"></param>
        /// <param name="aim"></param>
        /// <param name="sizeOfInventory"></param>
        /// <param name="adranaline"></param>
        public Player(string name, string desc, int healthBase, float hpStartPercent, float weight, float vision, float aim, int sizeOfInventory, float adranaline) : this(name, healthBase, hpStartPercent, weight, vision, sizeOfInventory)
        {
            Desc = desc;
            Aim = new("Aim", aim, 2, 0);
            Adranaline = adranaline;
        }
        /// <summary>
        /// Lets you create a basic player with the basic attributes.<br></br>
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="healthBase">Your Starting Health</param>
        /// <param name="hpStartPercent">Percentage (A value from 0 to 1)</param>
        /// <param name="speedBase">Your base speed</param>
        /// <param name="jumpBase">Your base Jump</param>
        /// <param name="weightBase">Your Weight. 1000 = Standered</param>
        /// <param name="vision">Your interaction with enemies. Will decrease with stealth attributes</param>
        /// <param name="sizeOfInventory">Your Inventory ise</param>
        /// <param name="gravity">Your Gravity. Set to 1 for default (9.8)</param>
        public Player(string name, int healthBase, float hpStartPercent, float weightBase, float vision, int sizeOfInventory)
        {
            Name = name;
            Health = new StatHealth(name, healthBase, 2, 0, healthBase * hpStartPercent);
            WeightBase = weightBase;
            IsAlive = true;
            VisionBase = new Stat("Vision", vision, -1, 0);
            SizeOfInventory = sizeOfInventory;
            BaseSizeOfInventory = sizeOfInventory;
            Inventory = new List<InventoryItem>();
            for (int i = 0; i < Resistances.Length; i++)
            {
                Resistances[i] = 1;
            }
            FillNullInventory();
            PendingItem = -1;
        }

        /// <summary>
        /// Use when grabbing save data to fully fill out the player data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="healthBase"></param>
        /// <param name="health"></param>
        /// <param name="speedBase"></param>
        /// <param name="speed"></param>
        /// <param name="speedBonus"></param>
        /// <param name="jump"></param>
        /// <param name="jumpBase"></param>
        /// <param name="jumpNonGrounded"></param>
        /// <param name="jumpBonus"></param>
        /// <param name="gravity"></param>
        /// <param name="gravityBase"></param>
        /// <param name="weight"></param>
        /// <param name="weightBase"></param>
        /// <param name="weightBonus"></param>
        /// <param name="vision"></param>
        /// <param name="visionBase"></param>
        /// <param name="reach"></param>
        /// <param name="resistances"></param>
        /// <param name="sizeOfInventory"></param>
        /// <param name="baseSizeOfInventory"></param>
        /// <param name="CurrenthotbarSlot"></param>
        /// <param name="hotBarSize"></param>
        /// <param name="money"></param>
        /// <param name="adranalineDist">Distance of activation</param>
        /// <param name="adrenaline">Time Scale abilty. Value goes from 0-1000</param>
        /// <param name="aim">Accuracy</param>
        /// <param name="desc">Desc</param>
        /// <param name="groundPoundBase">Base groud pound</param>
        /// <param name="healthMax">Max health</param>
        /// <param name="Id">ID</param>
        /// <param name="isAlive">Is the game alive</param>
        /// <param name="level">Level</param>
        /// <param name="pendingItem">Pending item</param>
        /// <param name="rotationSpeedbase">Rotaion speed (2D games)</param>
        public Player(uint Id, string name, string desc, int money,
            bool isAlive, int healthMax, float health, int healthBase, float adrenaline, float adranalineDist,
            float speedBase, float groundPoundBase, float jumpBase, float gravityBase, float weightBase, float visionBase, float aim, float reach, float[] resistances, int level,
            int sizeOfInventory, int baseSizeOfInventory, int pendingItem, int CurrenthotbarSlot, int hotBarSize, float rotationSpeedbase)
        {
            //Basic
            ID = Id;
            Name = name;
            Desc = desc;
            Money = money;
            //Health
            IsAlive = isAlive;
            GameLevel = level;
            Health = new StatHealth("Health", healthBase, 2, level, health);
            Adranaline = adrenaline;
            AdranalineDistance = adranalineDist;
            //Stats
            SpeedBase = new Stat("Speed", speedBase, 2, GameLevel);
            JumpBase = new Stat("Jump", jumpBase, 0.5f, GameLevel);
            Aim = new Stat("Aim", aim, 1f, GameLevel);
            Reach = reach;
            GravityBase = gravityBase;
            Floating = new Floatation(gravityBase);
            WeightBase = weightBase;
            IsAlive = true;
            VisionBase = new Stat("Vision", visionBase, -0.9f, GameLevel);
            //Inventory
            SizeOfInventory = sizeOfInventory;
            BaseSizeOfInventory = baseSizeOfInventory;
            Inventory = new List<InventoryItem>();
            FillNullInventory();
            PendingItem = pendingItem;
            CurrentHotBarSlot = CurrenthotbarSlot;
            HotbarSize = (byte)hotBarSize;
            RotationSpeed = rotationSpeedbase;

            SetupHotbar(HotbarSize, 0);
            for (int i = 0; i < Resistances.Length; i++)
            {
                Resistances[i] = resistances[i];
            }
        }
        /// <summary>
        /// Setup a player for sleeper agent multiplayer mode
        /// </summary>
        /// <param name="name">Username</param>
        /// <param name="healthBase">Uses <see cref="Shealth"/></param>
        /// <param name="rotationSpeed">How fast you rotate with keyboard inputs</param>
        /// <param name="weightBase">Knockback system. 1000 = weight</param>
        /// <param name="sizeOfInventory">The size of your inventory. For sleeper agent, default to 2, as the game will automaticly add spaces when needed.</param>
        public Player(string name, int healthBase, float rotationSpeed, float weightBase, int sizeOfInventory)
        {
            Name = name;
            Health = new StatHealth(name, healthBase, 2, 0, healthBase);
            RotationSpeed = rotationSpeed;
            WeightBase = weightBase;
            IsAlive = true;
            SizeOfInventory = sizeOfInventory;
            BaseSizeOfInventory = sizeOfInventory;
            Inventory = new List<InventoryItem>();
            for (int i = 0; i < Resistances.Length; i++)
            {
                Resistances[i] = 1;
            }
            FillNullInventory();
            PendingItem = -1;
        }
        /// <summary>
        /// Setup a player for sleeper agent host mode.
        /// </summary>
        /// <param name="name">Username</param>
        /// <param name="healthBase">Uses <see cref="Shealth"/></param>
        /// <param name="sizeOfInventory">The size of your inventory. For sleeper agent, default to 2, as the game will automaticly add spaces when needed.</param>
        public Player(uint id, string name, int healthBase, int sizeOfInventory)
        {
            Name = name;
            Health = new StatHealth(name, healthBase, 2, 0, healthBase);
            WeightBase = 1000f;
            IsAlive = true;
            SizeOfInventory = sizeOfInventory;
            BaseSizeOfInventory = sizeOfInventory;
            Inventory = new List<InventoryItem>();
            for (int i = 0; i < Resistances.Length; i++)
            {
                Resistances[i] = 1;
            }
            FillNullInventory();
            PendingItem = -1;
            ID = id;

        }
        public void SetupMovement(float speed, float jump, float groundPound, float gravity, float rotationSpeed, float breakSpeed, float gravityProtectionTime)
        {
            SpeedBase = new Stat("Speed", speed, 5, 0);
            JumpBase = new Stat("Jump", jump, 0.5f, 0);
            GroundPoundBase = new Stat("GroundPound", groundPound, 30, 0);
            GravityBase = gravity;
            Floating = new Floatation(gravity);
            RotationSpeed = rotationSpeed;
            BreakingSpeed = breakSpeed;
            GravityProtectionTime = gravityProtectionTime;
        }
        #endregion
        #region Money
        /// <summary>
        /// Get how much money you have
        /// </summary>
        /// <returns></returns>
        public int GetMoneyInt()
        {
            return Money;
        }
        /// <summary>
        /// Get how much money you have formatted as a string
        /// </summary>
        /// <returns>Dollars.Cents</returns>
        public string GetMoney()
        {
            return $"{Money / 100.0:C2}";
        }
        public void AddMoney(int amount)
        {
            Money += amount;
        }
        public int SpendMoney(int amount)
        {
            Money -= amount;
            if (Money > 0)
            {
                return Money;
            }
            else
            {
                Money += amount;
                return 0;
            }
        }
        #endregion
        #region Resistances
        public void SetupResistance(WeaponClass wpnclass, float value)
        {
            Health.SetResistance(wpnclass, value);
        }
        public void SetupResistances(params (WeaponClass, float)[] resistances)
        {
            for (int i = 0; i < resistances.Length; i++)
            {
                Health.SetResistance(resistances[i].Item1, resistances[i].Item2);
            }
        }
        public float[] GetResistances()
        {
            return Resistances;
        }
        #endregion
        #region Vision

        #endregion
        #region Weight
        /// <summary>
        /// Set WeightAdjustment. You cannot effect WeightBase. 
        /// <code> Weight = WeightBase + WeightAdjustment;</code>
        /// </summary>
        /// <param name="amount">Set WeightAdjustment</param>
        public void SetWeight(int amount)
        {
            WeightBase = amount;
        }
        #endregion
        #region Speed, Rotation,
        /// <summary>
        /// Returns your speed value
        /// </summary>
        /// <returns>Speed</returns>
        public float GetSpeed()
        {
            return Speed;
        }
        public float GetSpeedBase()
        {
            return SpeedBase.Max;
        }
        public void SetRotationSpeed(float amount)
        {
            RotationSpeed = amount;
        }
        /// <summary>
        /// Get rotation speed with a modifier
        /// </summary>
        /// <param name="multi">Multiply <see cref="RotationSpeed"/>by</param>
        /// <returns><see cref="RotationSpeed"/> * <paramref name="multi"/></returns>
        public float GetRotationSpeed(float multi)
        {
            return RotationSpeed * multi;
        }
        #endregion
        #region Gravity, Jump
        public float GetGravity()
        {
            return Gravity;
        }
        public float GetGroundPound(bool baseValue)
        {
            if (baseValue)
            {
                return GroundPoundBase.Max;
            }
            return GroundPound;
        }
        #endregion
        #region Name and Description
        public void SetName(string name)
        {
            if (name.Length > 19 || name.Length < 4)
            {
                return;
            }
            else
            {
                Name = name;
            }
        }
        public bool GetName(string name)
        {
            return Name == name;
        }
        public string GetName()
        {
            return Name;
        }
        /// <summary>
        /// Set the desc of the player
        /// </summary>
        /// <param name="desc">Set the Desc using a string</param>
        public void SetDec(string desc)
        {
            Description = desc;
        }
        /// <summary>
        /// Get the desc of the player
        /// </summary>
        /// <returns><code>Description</code></returns>
        public string GetDesc()
        {
            return Description;
        }
        public bool GetDesc(string desc)
        {
            return Description == desc;
        }
        #endregion
        #region Reach
        /// <summary>
        /// The range you can interact with objects in a raidus
        /// </summary>
        /// <param name="range">new range</param>
        public void SetReachRange(float range)
        {
            Reach = range;
        }
        public float GetReach()
        {
            return Reach;
        }
        #endregion
        public AnimationSys? GetAnimation(int id)
        {
            if (Inventory[id].GetItemType() == ItemType.Weapon || Inventory[id].GetItemType() == ItemType.Melee)
            {
                AnimationSys? clssy = Inventory[id].GetItem<Weapon>().GetAnimationClass();
                return clssy;
            }
            else if (Inventory[id].GetItemType() == ItemType.Item)
            {
                AnimationSys? clssy = Inventory[id].GetItem().GetAnimationClass();
                return clssy;
            }
            return null;
        }
        #region Attributes
        /// <summary>
        /// The main Attribute applyer.
        /// </summary>
        /// <param name="attribute"></param>
        public virtual void ApplyAttribute(Effects attribute)
        {
            ApplyAttribute(attribute.Attributes, attribute.Strength, attribute.Time, attribute.Option);
            if (attribute.GetOtherEffects() != null)
            {
                foreach (string search in attribute.GetOtherEffects())
                {
                    ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(search));
                }
            }
        }
        public virtual void ApplyAttribute(List<Effects> attribute)
        {
            if (attribute.Count <= 0) return;
            for (int i = 0; i < attribute.Count; i++)
            {
                ApplyAttribute(attribute[i].Attributes, attribute[i].Strength, attribute[i].Time, attribute[i].Option);
                if (attribute[i].GetOtherEffects() != null)
                {
                    for (int j = 0; j < attribute[i].GetOtherEffects().Length; j++)
                    {
                        ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(attribute[i].GetOtherEffects()));
                    }
                }
            }
        }
        public virtual void ApplyAttribute(Effects[] attribute)
        {
            if (attribute.Length <= 0) return;
            for (int i = 0; i < attribute.Length; i++)
            {
                ApplyAttribute(attribute[i].Attributes, attribute[i].Strength, attribute[i].Time, attribute[i].Option);
                if (attribute[i].GetOtherEffects() != null)
                {
                    for (int j = 0; j < attribute[i].GetOtherEffects().Length; j++)
                    {
                        ApplyAttribute(AllLibary.ItemLibary.SearchLibaryForAttribute(attribute[i].GetOtherEffects()));
                    }
                }
            }
        }
        public virtual void ApplyAttribute(AttributesTemplete attribute)
        {
            ApplyAttribute(attribute.GetAttriStruct());
        }
        public virtual void ApplyAttribute(List<AttributesTemplete> attribute)
        {
            if (attribute.Count <= 0) return;
            for (int i = 0; i < attribute.Count; i++)
            {
                ApplyAttribute(attribute[i].GetAttriStruct());
            }
        }
        public virtual void ApplyAttribute(AttributesTemplete[] attribute)
        {
            if (attribute.Length <= 0) return;
            for (int i = 0; i < attribute.Length; i++)
            {
                ApplyAttribute(attribute[i].GetAttriStruct());
            }
        }
        /// <summary>
        /// Apply attributes
        /// </summary>
        /// <param name="attributes">the attribute</param>
        /// <param name="strength">How strong the attribute is</param>
        /// <param name="time">How long it lasts</param>
        /// <param name="options">Usually contains tick rate, but sometimes contains addiotnal option such as Lift value in Floataitons</param>
        public virtual void ApplyAttribute(List<Attributes> attributes, List<float> strength, List<float> time, List<float> options)
        {
            if (attributes.Count <= 0) return;
            for (int i = 0; i < attributes.Count; i++)
            {
                ApplyAttribute(attributes[i], strength[i], time[i], options[i]);
            }
        }
        public virtual void ApplyAttribute(Attributes attributes, float strength, float time, float options)
        {
            Debug.Log($"Attributes: {attributes}, Stength: {strength}, Time: {time}, Options: {options}");
            if (attributes == Attributes.Poison)
            {
                SetFireDamage(strength, time, options);
            }
            if (attributes == Attributes.Flytation)
            {
                SetFlyatation(strength, time, options);
            }
            if (attributes == Attributes.Regeneration)
            {
                SetRegeneration(strength, time, options);
            }
            if (attributes == Attributes.Crying)
            {
                SetCrying(strength, time, options);
            }
            if (attributes == Attributes.Wounded)
            {
                SetWounded(strength, time, options);
            }
            if (attributes == Attributes.Speed)
            {
                SpeedBonus.SetAdjustment(strength, time);
            }
            if (attributes == Attributes.Jump)
            {
                JumpBonus.SetAdjustment(strength, time);
            }
        }
        /// <summary>
        /// Progress attribtues to the next phase of the game.
        /// </summary>
        protected void SetFireDamage(float damage, float time, float tickRate)
        {
            FireDamage.Add(new FireDamage(damage, time, tickRate));
        }
        protected void SetFlyatation(float strength, float time, float lift)
        {
            Floating.SetupFloatation(strength, time, lift);
            Gravity = Floating.ResetGravity(GravityBase);
        }
        protected void SetRegeneration(float health, float time, float tickRate)
        {
            ActiveRegenerations.Add(new Regeneration(health, time, tickRate));
        }
        protected void SetCrying(float strength, float time, float speedInc)
        {
            Cryings.Add(new Crying(strength, time, speedInc));
        }
        protected void SetWounded(float resistance, float time, float absorption)
        {
            Wound.SetupResistance(resistance, time, absorption);
        }

        public float GetArialAdjustments()
        {
            return Floating.Lift;
        }
        public void ApplyFireDamage()
        {
            if (FireDamage.Count > 0)
            {
                //Debug.Log($"Fire instances: {FireDamage.Count}");
                float totalDamage = 0;

                for (int i = FireDamage.Count - 1; i >= 0; i--)
                {
                    if (FireDamage[i].HasExpired())
                    {
                        Debug.Log($"Removing expired fire (Time={Time.time})");
                        FireDamage.RemoveAt(i);
                        continue;
                    }
                    if (FireDamage[i].ShouldApplyDamage())
                    {
                        float instanceDamage = FireDamage[i].Damage;
                        FireDamage[i].AdvanceToNextTick();
                        totalDamage += instanceDamage;
                    }
                }

                if (totalDamage > 0)
                {
                    Health.DamagePlayer(totalDamage, WeaponClass.Magic, false);
                }
            }
        }
        // In your healing system
        public void ApplyRegeneration()
        {
            if (ActiveRegenerations.Count > 0)
            {
                //Debug.Log($"Regen instances: {FireDamage.Count}");
                float totalDamage = 0;

                for (int i = ActiveRegenerations.Count - 1; i >= 0; i--)
                {
                    if (ActiveRegenerations[i].HasExpired())
                    {
                        Debug.Log($"Removing expired regen (Time={Time.time})");
                        ActiveRegenerations.RemoveAt(i);
                        continue;
                    }
                    if (ActiveRegenerations[i].ShouldApplyDamage())
                    {
                        float instanceDamage = ActiveRegenerations[i].HealthPerTick;
                        ActiveRegenerations[i].AdvanceToNextTick();
                        totalDamage += instanceDamage;
                    }
                    Debug.Log($"Regen amount: {totalDamage})");
                }

                if (totalDamage > 0)
                {
                    Health.Heal(totalDamage);
                }
            }
        }
        public void ApplyFlytation()
        {
            Gravity = Floating.ResetGravity(GravityBase);
        }
        public void ApplyCrying()
        {
            CryingAim = 0;
            if (Cryings.Count > 0)
            {
                for (int i = 0; i < Cryings.Count; i++)
                {
                    if (Cryings[i].GetExistTime())
                    {
                        Cryings.RemoveAt(i);
                    }
                    else
                    {
                        CryingAim += Cryings[i].GetInaccuracy();
                    }
                }
            }
        }
        public void ApplyWounded()
        {

        }
        public bool GetIsRegeenrating()
        {
            return ActiveRegenerations.Count > 0;
        }
        /// <summary>
        /// Apply stats from adjustments
        /// </summary>
        public void ApplyStatAdjustments()
        {
            SpeedBonus.CheckTime();
            JumpBonus.CheckTime();
            GroundPoundBonus.CheckTime();
            VisionAdjustment.CheckTime();
            WeightAdjustment.CheckTime();
            AimAdjustment.CheckTime();

        }
        #endregion
        #region Leveling
        public void IncrimentLevel(Stat stat)
        {
            stat.Level = stat.Level + 1;
        }
        #endregion

    }
    public class LevelSys
    {

    }
    /// <summary>
    /// Create an animation class that contains <see cref="Texture[]"/>[], <see cref="AnimationType"/>[], <see cref="CutPoints"/>
    /// </summary>
    public struct AnimationSys
    {
        /// <summary>
        /// An array of Textures
        /// </summary>
        public Texture[] Animation { get; private set; }
        /// <summary>
        /// Points to "cut" the animation. Used as indexes to each animation
        /// </summary>
        public int[] CutPoints { get; private set; }
        /// <summary>
        /// What type of animation will play
        /// </summary>
        public AnimationType[] Type { get; private set; }
        /// <summary>
        /// What audio clip will play.
        /// </summary>
        public AudioClip[] Audio { get; private set; }
        /// <summary>
        /// Create a new Animation
        /// </summary>
        /// <param name="animation">Textures</param>
        /// <param name="cut">New animatoin</param>
        /// <param name="type">Animation type</param>
        public AnimationSys(Texture[] animation, int[] cut, AnimationType[] type)
        {
            Animation = animation;
            CutPoints = cut;
            Type = type;
            Audio = null;
        }
        public AnimationSys(Texture[] animation, int[] cut, AnimationType[] type, AudioClip[] clips)
        {
            Animation = animation;
            CutPoints = cut;
            Type = type;
            Audio = clips;
        }
        /// <summary>
        /// Copy constructor for AnimationSys
        /// </summary>
        /// <param name="other">AnimationSys to copy from</param>
        public AnimationSys(AnimationSys other)
        {
            // Create new arrays to ensure deep copy
            this.Animation = new Texture[other.Animation.Length];
            Array.Copy(other.Animation, this.Animation, other.Animation.Length);

            this.CutPoints = new int[other.CutPoints.Length];
            Array.Copy(other.CutPoints, this.CutPoints, other.CutPoints.Length);

            this.Type = new AnimationType[other.Type.Length];
            Array.Copy(other.Type, this.Type, other.Type.Length);

            this.Audio = new AudioClip[other.Audio.Length];
            Array.Copy(other.Audio, this.Audio, other.Audio.Length);
        }

    }
    public class QuestSystem : IInventorySystem<Quest>
    {
        public int Stage { get; protected set; }
        protected List<Quest> Quests { get; set; } = new List<Quest>();
        /// <summary>
        /// How many quests can be held
        /// </summary>
        protected int SizeOfQuests;
        /// <summary>
        /// The base size of your inventory (without expansions)
        /// </summary>
        protected int BaseSizeOfQuests { get; set; }
        /// <summary>
        /// The selected item used for swapping.
        /// </summary>
        protected int PendingItem { get; set; } = -1;

        /// <summary>
        /// Current Inventory Slot
        /// </summary>
        protected int CurrentHotBarSlot { get; set; } = 0;

        /// <summary>
        /// Hotbar Size.
        /// </summary>
        protected int HotbarSize { get; set; } = 6;

        #region Fill Null Inventory
        /// <summary>
        /// Clear an inventory and then fill it with empty items which can be moved or erased. 
        /// Used to clear an inventory or to fill a Null inventory.
        /// </summary>
        public void FillNullInventory()
        {
            Quests.Clear();
            // Ensure SizeOfInventory is at least 1
            int actualSize = Mathf.Max(1, SizeOfQuests);
            for (int i = 0; i < actualSize; i++)
            {
                Quests.Add(new Quest(i));
            }
        }
        /// <summary>
        /// Will clear an inventory starting from <paramref name="start"/>
        /// </summary>
        /// <param name="start">Where to begin</param>
        public void FillNullInventory(int start)
        {
            // Ensure SizeOfInventory is at least 1
            int actualSize = Mathf.Max(1, SizeOfQuests);

            if (start >= actualSize)
            {
                return;
            }

            // Remove items from start position to end
            if (start < Quests.Count)
            {
                Quests.RemoveRange(start, Quests.Count - start);
            }

            // New new empty items
            for (int i = start; i < actualSize; i++)
            {
                Quests.Add(new Quest(i));
            }
        }
        #endregion
        #region Add and Delete Items/Inventory spaces
        /// <summary>
        /// Allows you to delete an item and replace it with an empty item.
        /// </summary>
        /// <param name="id">The slot at where the Inventory item was removed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when id is out of range</exception>
        public void DeleteItem(int id)
        {
            if (id < 0 || id >= Quests.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID is out of inventory range");
            }

            Quests[id] = new Quest(id);
        }

        /// <summary>
        /// Makes inventory bigger and fills the empty spots with empty items
        /// </summary>
        /// <param name="add">Number of slots to add</param>
        public void AddInventorySpaces(int add)
        {
            int oldSize = SizeOfQuests;
            add = Mathf.Max(1, Mathf.Abs(add));
            SizeOfQuests += add;
            FillNullInventory(oldSize);
        }
        /// <summary>
        /// Adds an <see cref="InventoryItem"/>to the inventory unless the inventory is full
        /// </summary>
        /// <param name="item">The <see cref="InventoryItem"/> to be added</param>
        /// <param name="start">Where to start the search. This is great when loading from a save file when you need to add spacing between items spawning in the inventory</param>
        public bool AddItem(Quest item, int start = 0)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            start = Mathf.Clamp(start, 0, SizeOfQuests - 1);
            bool slotFound = false;

            // Try to find an empty slot
            for (int i = start; i < SizeOfQuests; i++)
            {
                if (i < Quests.Count)
                {
                    if (Quests[i].GetIsEmptyItem())
                    {
                        Quests[i] = item;
                        item.MoveItem(i);
                        CheckPhaseItem(i);
                        slotFound = true;
                        break;
                    }
                }
                else
                {
                    item.MoveItem(i);
                    Quests.Add(item);
                    slotFound = true;
                    break;
                }
            }

            if (!slotFound)
            {
                // Find first null item and replace
                for (int i = 0; i < Quests.Count; i++)
                {
                    if (Quests[i].GetIsEmptyItem())
                    {
                        Quests[i] = item;
                        item.MoveItem(i);
                        CheckPhaseItem(i);
                        slotFound = true;
                        break;
                    }
                }
            }
            if (!slotFound)
            {
                Debug.Log("After searching for space in your invenotry, we found NOTHING open.");
            }
            //Use this return command to determine if a object should respawn where it was or progress the code. So if you collect an item but your inven is full, this function returns "false" under the condition stated. Which then you could make the item reappear or not destroy it.
            return slotFound;
        }
        /// <summary>
        /// Adds an <see cref="InventoryItem"/>to the inventory unless the inventory is full
        /// </summary>
        /// <param name="item">The <see cref="InventoryItem"/> to be added</param>
        /// <param name="start">Where to start the search. This is great when loading from a save file when you need to add spacing between items spawning in the inventory</param>
        public bool AddItem(Quest[] item, int start = 0)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            start = Mathf.Clamp(start, 0, SizeOfQuests - 1);
            bool allItemsAdded = true;

            foreach (Quest currentItem in item)
            {
                if (currentItem == null)
                {
                    throw new ArgumentNullException(nameof(item), "Item in array cannot be null");
                }

                bool slotFound = false;

                // First search from start position to end
                for (int i = start; i < SizeOfQuests; i++)
                {
                    if (Quests[i].GetIsEmptyItem())
                    {
                        Quests[i] = currentItem;
                        currentItem.MoveItem(i);
                        CheckPhaseItem(i);
                        slotFound = true;
                        break;
                    }
                }

                // If not found and start > 0, search from beginning to start
                if (!slotFound && start > 0)
                {
                    for (int i = 0; i < start; i++)
                    {
                        if (Quests[i].GetIsEmptyItem())
                        {
                            Quests[i] = currentItem;
                            currentItem.MoveItem(i);
                            CheckPhaseItem(i);
                            slotFound = true;
                            break;
                        }
                    }
                }

                if (!slotFound)
                {
                    Debug.LogWarning($"Could not find space for item in inventory: {currentItem}");
                    allItemsAdded = false;
                }
            }

            //Use this return command to determine if a object should respawn where it was or progress the code. So if you collect an item but your inven is full, this function returns "false" under the condition stated. Which then you could make the item reappear or not destroy it.
            return allItemsAdded;
        }
        /// <summary>
        /// Run a check that identifiefies which items are usable during the PreGame and PostVoting
        /// </summary>
        /// <param name="index"></param>
        public void CheckPhaseItem(int index)
        {
            //TODO: Somthing that uses
            //ItemType type = Inventory[index].GetItemType();
        }
        #endregion
        #region Order By
        public void OrderItemsByName()
        {
            Quests = Quests.OrderBy(item => item.GetName()).ToList();
        }
        public void OrderItemsByPrice()
        {
            Quests = Quests.OrderBy(item => item.GetMoneyInt()).ToList();
        }
        private void ReindexItems()
        {
            for (int i = 0; i < Quests.Count; i++)
            {
                Quests[i].MoveItem(i);
            }
        }
        #endregion
        #region Swap Items
        /// <summary>
        /// Lets you swap 2 inventory items.
        /// </summary>
        /// <param name="index1">Item a</param>
        /// <param name="index2">Item b</param>
        /// <exception cref="IndexOutOfRangeException"> You seletected an item out of range</exception>
        public void SwapItem(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0 || index1 >= Quests.Count || index2 >= Quests.Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index1 == index2)
            {
                return; // No need to swap same items
            }
            try
            {
                (Quests[index1], Quests[index2]) = (Quests[index2], Quests[index1]);
                // Update their slot IDs
                Quests[index1].MoveItem(index2);
                Quests[index2].MoveItem(index1);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Swap failed: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Set a item as Selected. Or <see cref="PendingItem"/>
        /// </summary>
        /// <param name="index">Which item</param>
        public void SelectItem(int index)
        {
            PendingItem = (index >= 0 && index < Quests.Count) ? index : -1;
        }
        /// <summary>
        /// Get the selected item and then immidiatly clear it.
        /// </summary>
        /// <returns></returns>
        public int GetPendingItemAndClear()
        {
            if (PendingItem > -1)
            {
                int temp = PendingItem;
                PendingItem = -1;
                return temp;
            }
            return -1;
        }
        /// <summary>
        /// Get the item awating to be swapped.
        /// </summary>
        /// <returns></returns>
        public int GetPendingItem()
        {
            return PendingItem;
        }
        #endregion
        #region Hotbar
        /// <summary>
        /// Setup the hotbar size and default slot
        /// </summary>
        /// <param name="amount">Size of the hotbar</param>
        /// <param name="defaultHotbarSlot">Default slot</param>
        public void SetupHotbar(int amount, int defaultHotbarSlot)
        {
            HotbarSize = (byte)Mathf.Clamp(amount, 1, SizeOfQuests);
            CurrentHotBarSlot = Mathf.Clamp(defaultHotbarSlot, 0, HotbarSize - 1);
        }
        /// <summary>
        /// Scroll up or Scroll down.
        /// </summary>
        /// <param name="amount">A value +1 or -1</param>
        public void ScrollItem(int amount)
        {
            if (amount == 0) return;
            CurrentHotBarSlot += amount;
            if (CurrentHotBarSlot < 0)
            {
                CurrentHotBarSlot = HotbarSize - 1;
            }
            if (CurrentHotBarSlot > HotbarSize - 1)
            {
                CurrentHotBarSlot = 0;
            }
        }
        /// <summary>
        /// Go to a slot. Does not throw an error if the slot is greater than the hotbar size, instead the method will do nothing.
        /// </summary>
        /// <param name="slot">slot ID to go to</param>
        public void SetHotbarSlot(int slot)
        {
            if (slot >= 0 && slot < HotbarSize)
            {
                CurrentHotBarSlot = slot;
            }
        }
        /// <summary>
        /// Gets the size of the hotbar
        /// </summary>
        /// <returns><code>HotbarSize</code></returns>
        public int GetHotbarSize()
        {
            return HotbarSize;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Gets the hotbar slot that is currently being highlighted</returns>
        public int GetHotbarSlot()
        {
            return CurrentHotBarSlot;
        }
        #endregion
        #region GetMescData
        /// <summary>
        /// Get the total inventory size. 
        /// </summary>
        public int GetInventorySize()
        {
            return SizeOfQuests;
        }

        /// <summary>
        /// Gets the texture of an item.
        /// </summary>
        /// <param name="id">What slot the item is in your inventory.</param>
        /// <returns>A Texture.</returns>
        public Texture GetTextureItem(int id)
        {
            if (id < 0 || id >= Quests.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID is out of inventory range");
            }
            return Quests[id].GetTheTexture();
        }
        #endregion
        #region Get Inventory Items
        /// <summary>
        /// Get your ENTIRE INVENTORY
        /// </summary>
        /// <returns><see cref="Inventory"/></returns>
        public List<Quest> GetInventory()
        {
            return Quests;
        }
        public List<Quest> GetNewInventory()
        {
            return new List<Quest>(Quests);
        }
        /// <summary>
        /// Get a single inventory item based on slot id.
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns>A <see cref="InventoryItem"/></returns>
        public Quest GetInventoryItem(int id)
        {
            if (id < 0 || id >= Quests.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID is out of inventory range");
            }
            return Quests[id];
        }
        /// <summary>
        /// Get the inventory item based on <see cref="CurrentHotBarSlot"/>
        /// </summary>
        /// <returns></returns>
        public Quest GetInventoryItemCurrentHotbar()
        {
            return Quests[CurrentHotBarSlot];
        }
        /// <summary>
        /// Find an inventory item by name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns><see cref="InventoryItem"/></returns>
        public Quest GetInventoryItem(string name)
        {
            return Quests.Find(item => item.GetName() == name);
        }
        #endregion
        #region Quests
        public QuestStage GetOnQuest(int index)
        {
            return Quests[index].GetQuestStage();
        }
        public string GetQuestDesc(int index)
        {
            return Quests[index].GetDesc();
        }
        public string GetQuestSceneStage(int index)
        {
            return Quests[index].GetSceneStage();
        }
        public string GetQuestName(int index)
        {
            return Quests[index].GetQuestName();
        }
        public void SetQuestCompleted(int index)
        {
            Quests[index].SetQuestStage(QuestStage.Completed);
        }
        public void SetQuestStage(int index, QuestStage stage)
        {
            Quests[index].SetQuestStage(stage);
        }
        #endregion
    }
    /// <summary>
    /// Holds Several common methods.
    /// <list type="bullet">
    /// <item>Scene methods: <see cref="GetCurrentSceneName()"/> and <see cref="ReloadCurrentScene()"/></item>
    /// <item>Array methods: <see cref="ArrayRandomReadjustment{T}(T[])"/> <see cref="ArrayRandomReadjustment{T}(List{T})"/>, <see cref="CompareArray{T}(T[], T)"/> and <see cref="ResizeArray{T}(T[], int)"/></item>
    /// <item>Mesc methods: <see cref="GetAllGameObjects(GameObject)"/> and <see cref="RandomValue(int)"/></item>
    /// </list>
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// Gets the name of the current scene.
        /// </summary>
        /// <returns>a string of the name</returns>
        public static string GetCurrentSceneName()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            return currentScene.name;
        }
        /// <summary>
        /// Reloads current scene
        /// </summary>
        public static void ReloadCurrentScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
        /// <summary>
        /// Randomize an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalArray">The array</param>
        /// <returns>A randomized array</returns>
        public static T[] ArrayRandomReadjustment<T>(T[] originalArray)
        {
            T[] shuffledArray = originalArray.Clone() as T[];

            // Fisher-Yates shuffle algorithm for true randomization
            for (int i = shuffledArray.Length - 1; i > 0; i--)
            {
                // Get a random index between 0 and i (inclusive)
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                // Swap elements
                (shuffledArray[randomIndex], shuffledArray[i]) = (shuffledArray[i], shuffledArray[randomIndex]);
            }

            return shuffledArray;
        }
        public static List<T> ArrayRandomReadjustment<T>(List<T> originalArray)
        {
            List<T> shuffledArray = originalArray;

            // Fisher-Yates shuffle algorithm for true randomization
            for (int i = shuffledArray.Count - 1; i > 0; i--)
            {
                // Get a random index between 0 and i (inclusive)
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                // Swap elements
                (shuffledArray[randomIndex], shuffledArray[i]) = (shuffledArray[i], shuffledArray[randomIndex]);
            }

            return shuffledArray;
        }
        public static void ArrayRandomReadjustment<T>(ref T[] originalArray)
        {
            T[] shuffledArray = originalArray.Clone() as T[];

            // Fisher-Yates shuffle algorithm for true randomization
            for (int i = shuffledArray.Length - 1; i > 0; i--)
            {
                // Get a random index between 0 and i (inclusive)
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                // Swap elements
                (shuffledArray[randomIndex], shuffledArray[i]) = (shuffledArray[i], shuffledArray[randomIndex]);
            }
            originalArray = shuffledArray;
        }
        public static void ArrayRandomReadjustment<T>(ref List<T> originalArray)
        {
            List<T> shuffledArray = originalArray;

            // Fisher-Yates shuffle algorithm for true randomization
            for (int i = shuffledArray.Count - 1; i > 0; i--)
            {
                // Get a random index between 0 and i (inclusive)
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                // Swap elements
                (shuffledArray[randomIndex], shuffledArray[i]) = (shuffledArray[i], shuffledArray[randomIndex]);
            }
            originalArray = shuffledArray;
        }
        /// <summary>
        /// Use to compare if an array of <typeparamref name="T"/> contains the current value.
        /// </summary>
        /// <typeparam name="T">The type of elements to compare</typeparam>
        /// <param name="test">Array to test against</param>
        /// <param name="current">Current value to compare</param>
        /// <returns>True if the current value is found in the test array</returns>
        public static bool CompareArray<T>(T[] test, T current)
        {
            for (int i = 0; i < test.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(test[i], current))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns a value between 1 and <paramref name="amount"/>. If you input a value less than 1, then you will receive a return of 0.
        /// </summary>
        /// <param name="amount">A INT number.</param>
        /// <returns>A random INT</returns>
        public static int RandomValue(int amount)
        {
            if (amount < 1)
            {
                return 0;
            }
            return 1 + (int)(amount * UnityEngine.Random.value);
        }
        /// <summary>
        /// Lets you resise the array by creating a copy of the original array and making it the size of the older array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">Array to be changed</param>
        /// <param name="newSize">new size</param>
        /// <returns>int[]</returns>
        public static T[] ResizeArray<T>(T[] original, int newSize)
        {
            T[] newArray = new T[newSize];
            Array.Copy(original, newArray, Mathf.Min(original.Length, newSize));
            return newArray;
        }
        /// <summary>
        /// Get all of the hashsets of the gameobjects in you and your children. This should help avoid hitting yourself when you shoot a weapon.
        /// </summary>
        /// <param name="parent">Whatever summoned the object</param>
        /// <returns>A HashSet of Gameobjects</returns>
        public static HashSet<GameObject> GetAllGameObjects(GameObject parent)
        {
            HashSet<GameObject> gameObjects = new HashSet<GameObject>();

            // Get all transforms including parent and children
            Transform[] allTransforms = parent.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allTransforms)
            {
                gameObjects.Add(t.gameObject);
            }

            return gameObjects;
        }
    }
    namespace Stats
    {
        /// <summary>
        /// A stat which can have levels, name, desc, and a value.
        /// </summary>
        public class Stat : INameDesc
        {
            private readonly string name;
            private string desc = string.Empty;
            public float Max { get; protected set; }
            public int Level
            {
                get
                {
                    return Mathf.RoundToInt((Max / increasePerLevel) - Level1Stat);
                }
                set
                {
                    Max = value * increasePerLevel + Level1Stat;
                    desc = $"{name}: Value: {Max}, Level: {Level}";
                }
            }
            private float increasePerLevel;
            public float Level1Stat { get; private set; }

            public Stat()
            {

            }
            public Stat(string name, float level1Stat, int increasePerLevel)
            {
                this.increasePerLevel = increasePerLevel;
                this.name = name;
                this.Level1Stat = level1Stat;
                Level = 0;
                desc = $"{name}: Value: {Max}, Level: {Level}";
            }
            public Stat(string name, float level1Stat, float increasePerLevel, int currentLevel)
            {
                this.increasePerLevel = increasePerLevel;
                this.name = name;
                this.Level1Stat = level1Stat;
                Level = currentLevel;
                desc = $"{name}: Value: {Max}, Level: {Level}";
            }
            public string GetName()
            {
                return name;
            }
            public string GetDesc()
            {
                return desc;
            }

            public bool GetName(string name)
            {
                return name == this.name;
            }

            public bool GetDesc(string name)
            {
                return name == desc;
            }
        }
        /// <summary>
        /// Allows for a Current stat and a Max stat.
        /// </summary>
        public class StatHealth : Stat, IHealth
        {
            /// <summary>
            /// Are you alive?
            /// </summary>
            private bool isAlive = true;
            /// <summary>
            /// Your Current Health.
            /// <code>
            /// get
            /// {
            ///     return <see cref="current"/>
            /// }
            /// set
            /// {
            ///    current = Mathf.Min(value, Max);
            /// }
            /// </code>
            /// </summary>
            /// <remarks>
            /// If your health goes below or equil to 0, the value <see cref="isAlive"/> will be set to false.
            /// </remarks>
            public float Current
            {
                get
                {
                    return current;
                }
                set
                {
                    current = Mathf.Min(value, Max);
                    if (current <= 0)
                    {
                        isAlive = false;
                    }
                    else
                    {
                        isAlive = true;
                    }
                }
            }
            /// <summary>
            /// Your current health
            /// </summary>
            private float current;
            /// <summary>
            /// Resistances to damage
            /// </summary>
            protected float[] Resistances { get; set; } = new float[Enum.GetValues(typeof(WeaponClass)).Length];
            /// <summary>
            /// Setup Health Stat
            /// </summary>
            /// <param name="name">Name of stat</param>
            /// <param name="level1Stat">Value at level 1</param>
            /// <param name="increasePerLevel">Increase per level.</param>
            /// <param name="currentLevel">Current level</param>
            /// <param name="currentValue">Current value</param>
            public StatHealth(string name, int level1Stat, int increasePerLevel, int currentLevel, float currentValue) : base(name, level1Stat, increasePerLevel, currentLevel)
            {
                Current = currentValue;
            }
            /// <summary>
            /// Setup each invidual resistance
            /// </summary>
            /// <param name="wpn">Resistance</param>
            /// <param name="amount">Amount</param>
            public void SetResistance(WeaponClass wpn, float amount)
            {
                Resistances[(int)wpn] = amount;
            }
            /// <summary>
            /// Damage a player and bypass resistances.
            /// </summary>
            /// <param name="value">Value of damage</param>
            public void DamagePlayer(float value)
            {
                value = Mathf.Max(value, 0);
                Current -= value;
            }

            /// <summary>
            /// Damage the player with resistances applied. 
            /// </summary>
            /// <param name="value">Damage amount</param>
            /// <param name="weapon">Weapon type.</param>
            /// <param name="fake">Doesn't damage the player</param>
            /// <returns>The amount of health you would have after being damaged.</returns>
            public float DamagePlayer(float value, WeaponClass weapon, bool fake = false)
            {
                value = ((float)value * Resistances[(int)weapon]);
                if (fake)
                {
                    return Current - value;
                }
                Current -= value;
                return Current;
            }
            /// <summary>
            /// Damage the player with resistances applied. Along with the ability to make a minimum health Barriar
            /// </summary>
            /// <param name="value">Damage amount</param>
            /// <param name="weapon">Weapon type.</param>
            /// <param name="fake">Doesn't damage the player</param>
            /// <returns></returns>
            public float DamagePlayer(float value, WeaponClass weapon, bool fake, float lowestHealth)
            {
                if (Current > lowestHealth)
                {
                    value = ((float)value * Resistances[(int)weapon]);
                    if (fake)
                    {
                        return Current - value;
                    }
                    Current = Mathf.Max((Current - value), lowestHealth);
                }
                return Current;
            }
            /// <summary>
            /// Damage the player with a value from 0.0 to 1.0, this will turn into a percent. 
            /// You can choose between 3 different types of decreasing. 
            /// <list type="number">
            /// <item>Option: Remove Amount from MaxHealth<code> Health -= (int)((float)value * (float)HealthMax); </code></item>
            /// <item>Option: Remove Amount from current health<code> Health = (int)((float)Health * (float)value);</code></item>
            /// <item>Option: Remove Health by adding Health + Max Heatlh, then devide by 2. Afterwords multiply that by the desired percentage.<code> Health = (int)((float)(((float)Health + (float)HealthMax) / (float)(2)) * value);</code></item>
            /// </list>
            /// </summary>
            /// <param name="value">A value from 0.0 to 1.0</param>
            /// <param name="DecreaesType"></param>
            public void DamagePlayer(float value, HealthDamagePercentage DecreaesType)
            {
                if ((int)DecreaesType == 1)
                {
                    Current -= (int)((float)value * (float)Max);
                }
                if ((int)DecreaesType == 2)
                {
                    Current = (int)((float)Current * (float)value);
                }
                if ((int)DecreaesType == 3)
                {
                    Current = (int)((float)(((float)Current + (float)Max) / (float)(2)) * value);
                }
            }
            /// <summary>
            /// Returns if health <= 0
            /// </summary>
            /// <returns></returns>
            public bool GetIsAlive()
            {
                return isAlive;
            }
            /// <summary>
            /// Returns a list of ints with the following data:
            /// <list type="bullet">
            /// <item>Current Health</item>
            /// <item>Max Heatlh</item>
            /// <item>Max - Current</item>
            /// <item>Level</item>
            /// </list>
            /// </summary>
            /// <returns></returns>
            public List<int> GetHPInfo()
            {
                List<int> Hptemp = new()
            {
                (int)Current,
                (int)Max,
                (int)Max - (int)Current,
                Level,
            };
                return Hptemp;
            }
            /// <summary>
            /// Heals a player. Prevents them from going below 0.02f. Can be used to create a poison effect.
            /// </summary>
            /// <param name="amount">Amount</param>
            public void Heal(float amount)
            {
                Current = Mathf.Clamp(Current + amount, 0.02f, Max);
            }
            /// <summary>
            /// Heals a player, prevents HP from going under <paramref name="min"/>
            /// </summary>
            /// <param name="amount">Heal back amount</param>
            /// <param name="min">Minimum Health</param>
            public void Heal(float amount, float min)
            {
                Current = Mathf.Clamp(Current + amount, min, Max);
            }
        }
        /// <summary>
        /// Adjust a single stat
        /// </summary>
        public class StatAdjustment
        {
            /// <summary>
            /// Returns the <see cref="strength"/> value <u>unless</u> <see cref="hasStarted"/> is <see cref="false"/>.
            /// </summary>
            public float Strength
            {
                get
                {
                    if (!hasStarted)
                    {
                        return 1f;
                    }
                    return strength;
                }
            }
            /// <summary>
            /// The strength of the effect
            /// </summary>
            private float strength = 1f;
            /// <summary>
            /// When will the effect end?
            /// </summary>
            public float EndTime { get; private set; } = 0f;
            /// <summary>
            /// Has the effect started?
            /// </summary>
            private bool hasStarted;
            public void SetAdjustment(float strength, float time)
            {
                this.strength += strength;
                if (Time.time < EndTime)
                {
                    EndTime += time;
                }
                else
                {
                    EndTime = Time.time + time;
                }
                hasStarted = true;
            }
            /// <summary>
            /// Check the current time.
            /// </summary>
            public void CheckTime()
            {
                if (hasStarted && Time.time >= EndTime)
                {
                    Reset();
                }
            }
            /// <summary>
            /// Reset the value
            /// </summary>
            private void Reset()
            {
                strength = 1f;
                EndTime = 0f;
                hasStarted = false;
            }
        }
    }
}