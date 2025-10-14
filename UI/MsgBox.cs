using BaseCharacter.Items;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private TMP_Text Text;
    [Tooltip("the delay in incrementing messages")]
    [SerializeField] private float baseDelay = 0.025f;
    [Tooltip("Use the text already set in the TMPro text")]
    [SerializeField] private bool UseDefaultText = false;
    [SerializeField] private bool UseDefaultColor = false;
    [Header("Optional")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RawImage RawImage;
    /// <summary>
    /// Does this textbox have a raw image.
    /// </summary>
    private bool hasRawImage = false;
    /// <summary>
    /// Text msg
    /// </summary>
    private string TextMsg { get; set; } = "";
    /// <summary>
    /// Incremented message
    /// </summary>
    private string NextMsg { get; set; } = "";
    /// <summary>
    /// Index of <see cref="NextMsg"/>
    /// </summary>
    private int index = 0;
    
    /// <summary>
    /// Time.time + baseDelay
    /// </summary>
    private float delay;
    /// <summary>
    /// Do we even need to add <see cref="NextMsg"/> text;
    /// </summary>
    private bool hasNextMsg = false;

    /// <summary>
    /// Collect text between {}
    /// </summary>
    private readonly Regex bracketRegex = new Regex(@"\{([^{}]*)\}");
    private float MaxHealth;
    private float Health;
    private float Warn;
    private float Warn2;
    private Color defaultColorRawImage;
    private float alphaTimer;
    private float alphaTimerBase;
    private float alphaStart = 1;
    private float endAlpha = 0;
    private bool runCrossAlpha = false;
    private string Credits { get; set; } = "All art was created by BirdyEnforcement, Phycho, Noodlesama and Xic. All Sound effects were created by BirdyEnforcement, Phsycho, and Noodlesama. Concept created by Psycho. Game coded by BirdyEnforcement using Mirror for multiplayer, TMPro for text. This game was coding in Unity version 2019.42.somthing";
    public void Start()
    {
        //Choose to keep the textbox the same from the unity editor or remove it.
        if (!UseDefaultText)
        {
            ClearMsg();
            if (UseDefaultColor)
            {
                Text.color = Text.color;
            }
        }
        else
        {
            ClearAllButDefaultText();
        }

        if (RawImage != null)
        {
            hasRawImage = true; //Tell the code that the rawImage is null.
            RawImage.color = Color.clear;
            defaultColorRawImage = RawImage.color;
        }
        delay = Time.time; //Ensure non-instant text doesn't break.
        hasNextMsg = false;
    }
    public void Update()
    {
        RunAlpha();
        RunNextMsg();
    }
    private void RunNextMsg()
    {
        if (!hasNextMsg) return;
        float currentTime = Time.time;
        float timeSinceLastUpdate = currentTime - delay;

        // Calculate how many characters we should add based on elapsed time.
        //To anyone who wants to complain I spent over 3 weeks trying to get this to work (my problems were very bad). Thank you DEEPSEEK AI for fixing this.
        if (timeSinceLastUpdate >= baseDelay)
        {
            // Calculate how many frames worth of delay we've accumulated
            int framesToProcess = Mathf.FloorToInt(timeSinceLastUpdate / baseDelay);

            // Ensure we process at least one character per frame
            int charsToAdd = Mathf.Max(1, framesToProcess);

            // Update the delay time, carrying over any remainder
            delay = currentTime + baseDelay - (timeSinceLastUpdate % baseDelay);
            UpdateText(charsToAdd);
        }
        if (hasRawImage && Text.color.a <= 0)
        {
            RawImage.color = Color.clear;
        }
    }
    private void RunAlpha()
    {
        if (!runCrossAlpha) return;
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Mathf.Clamp01(Mathf.Max(endAlpha, (alphaTimer - Time.time)/alphaTimerBase * alphaStart)));
        if (Time.time > alphaTimer)
        {
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b,endAlpha);
            runCrossAlpha = false;
        }
    }
    #region Regex
    /// <summary>
    /// Processes text containing {brackets} and returns the content within them
    /// </summary>
    /// <param name="input">Text containing bracketed content</param>
    /// <returns>List of strings found within brackets</returns>
    public List<string> GetBracketedContent(string input)
    {
        List<string> results = new List<string>();
        MatchCollection matches = bracketRegex.Matches(input);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count > 1)
            {
                results.Add(match.Groups[1].Value);
            }
        }

        return results;
    }

    /// <summary>
    /// Replaces content within brackets with specified values
    /// </summary>
    /// <param name="input">Original text with brackets</param>
    /// <param name="replacements">Dictionary of replacements (key = bracket content without {}, value = replacement)</param>
    /// <returns>Processed string with replacements</returns>
    public string ReplaceBracketedContent(string input, Dictionary<string, string> replacements)
    {
        return bracketRegex.Replace(input, match =>
        {
            string key = match.Groups[1].Value;
            return replacements.ContainsKey(key) ? replacements[key] : match.Value;
        });
    }
    #endregion
    #region Special Texts
    /// <summary>
    /// Set slider data;
    /// </summary>
    /// <param name="preText"></param>
    public void SetSliderValue(string preText)
    {
        if (slider != null)
        {
            TextMsg = $"{preText}{slider.value}";
            ApplyText();
        }
    }
    /// <summary>
    /// Text to be set by <see cref="BaseCharacter.InventoryItem"/>
    /// </summary>
    /// <param name="item"></param>
    public void SetItemText(BaseCharacter.Items.InventoryItem item)
    {
        TextMsg = $"{item.GetName()},\n{item.GetItemType()}";
        ApplyText();
    }
    /// <summary>
    /// Setup a text to Line 1/n Line 2
    /// </summary>
    /// <param name="intitial"></param>
    /// <param name="role"></param>
    public void SetRoleText(string intitial, string role)
    {
        TextMsg = $"{intitial}:\n{role}";
        ApplyText();
    }
    /// <summary>
    /// Set the text box up based on a Ammo/Max ammo system
    /// </summary>
    /// <param name="ammo">Current ammo</param>
    /// <param name="maxAmmo">Max ammo</param>
    /// <param name="icon">Use '/' or whatever you like to seporate the current/max</param>
    /// <param name="direction">True = Ammo/maxAmmo, False = maxAmmo/ammo</param>
    public void SetAmmo(int ammo, int maxAmmo, string icon, bool direction)
    {
        if (direction)
        {
            TextMsg = $"{ammo}{icon}{maxAmmo}";
        }
        else
        {
            TextMsg = $"{maxAmmo}{icon}{ammo}";
        }
        ApplyText();
    }
    /// <summary>
    /// Set the ammo dialog box based on what type of item your holding.
    /// </summary>
    /// <param name="item"></param>
    public void SetAmmo(InventoryItem item)
    {
        if (item.GetItemType() == Enums.ItemType.Weapon)
        {
            Weapon rockTMP = item.GetItem<Weapon>();
            if(rockTMP.GetUsesAmmo()){
                TextMsg = $"{rockTMP.GetAmmoCount()}/{rockTMP.GetMaxAmmo()}";
            }
            else 
            {
                TextMsg = $"Can Fire: {rockTMP.GetCanFire(false)}";
            }
        }
        if (item.GetItemType() == Enums.ItemType.Item)
        {
            TextMsg = $"{item.GetItem().GetDesc()}";
        }

        ApplyText();
    }
    /// <summary>
    /// Set a string and int value as a message.
    /// </summary>
    /// <param name="msg">Usually a stat name</param>
    /// <param name="stat">The Number.</param>
    /// <param name="direction">True = Msg/Stat, Flase = Stat/Msg</param>
    public void SetMsgStat(string msg, int stat, bool direction)
    {
        if (direction)
        {
            TextMsg = $"{msg}{stat}";
        }
        else
        {
            TextMsg = $"{stat}{msg}";
        }
        ApplyText();
    }
    
    /// <summary>
    /// You won the game
    /// </summary>
    /// <param name="time"></param>
    public void WonGame(float time, float score)
    {
        TextMsg = $"You have won the game!\nYour time is {time}s\nYour score is {score}";
        ApplyText();
    }
    /// <summary>
    /// Clear text
    /// </summary>
    public void ClearMsg()
    {
        TextMsg = "";
        NextMsg = "";
        index = 0;
        ApplyText();
    }
    /// <summary>
    /// Clear all text but the default.
    /// </summary>
    public void ClearAllButDefaultText()
    {
        NextMsg = "";
        TextMsg = Text.text;
        index = 0;
        ApplyText();
    }
    /// <summary>
    /// Clear the <see cref="NextMsg"/> text
    /// </summary>
    public void ClearNextMsg()
    {
        NextMsg = "";
        index = 0;
    }
    
    /// <summary>
    /// Display Credits.
    /// </summary>
    public void ShowCredits()
    {
        Text.text = $"{Credits}";
    }
    /// <summary>
    /// Pause the Menu.
    /// </summary>
    public void PauseMenu()
    {
        TextMsg = $"Game Paused\nPress ESC to unpause. Click Quit game to quit the game.";
        ApplyText();
    }
    /// <summary>
    /// Create a textbox with a title and desc.
    /// </summary>
    /// <param name="type">What type of ability</param>
    /// <param name="title">Title</param>
    /// <param name="desc">The description of the item</param>
    public void SetTitleDesc(string type, string title, string desc)
    {
        TextMsg = $"{type}: {title}\nFunction: {desc}";
        ApplyText();
    }
    #endregion
    #region Add, Update, Apply, Set Text
    /// <summary>
    /// Update the textbox
    /// </summary>
    private void ApplyText()
    {
        Text.text = $"{TextMsg}";
    }
    /// <summary>
    /// New text to a textbox without clearing the previous text.
    /// </summary>
    /// <param name="text">The text to be added</param>
    /// <param name="instant">instant or Delayed</param>
    public void AddText(string text, bool instant)
    {
        runCrossAlpha = false;
        if (instant)
        {
            // Complete any in-progress message first
            CompleteCurrentMessage();
            TextMsg += string.IsNullOrEmpty(TextMsg) ? text : $"\n{text}";
            ApplyText();
        }
        else
        {
            // Complete any in-progress message first
            CompleteCurrentMessage();

            // Initialize NextMsg if it's empty, otherwise add a new line
            if (string.IsNullOrEmpty(NextMsg))
            {
                NextMsg = text;
            }
            else
            {
                NextMsg += $"\n{text}";
            }

            delay = Time.time + baseDelay;
            hasNextMsg = true;
        }
    }
    /// <summary>
    /// Set the message and delete previous text. Slowly fade the text.
    /// </summary>
    /// <param name="text">The text to be set</param>
    /// <param name="instant">instant or Delayed</param>
    public void AddText(string text, bool instant, float time, float startAlpha = 1f, float endAlpha = 0f)
    {
        runCrossAlpha = false;
        if (hasRawImage)
        {
            RawImage.color = defaultColorRawImage;
        }
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Mathf.Clamp01(startAlpha));
        if (instant)
        {
            // Complete any in-progress message first
            CompleteCurrentMessage();
            TextMsg += string.IsNullOrEmpty(TextMsg) ? text : $"\n{text}";
            ApplyText();
        }
        else
        {
            // Complete any in-progress message first
            CompleteCurrentMessage();

            // Initialize NextMsg if it's empty, otherwise add a new line
            if (string.IsNullOrEmpty(NextMsg))
            {
                NextMsg = text;
            }
            else
            {
                NextMsg += $"\n{text}";
            }

            delay = Time.time + baseDelay;
            hasNextMsg = true;
        }
        CrossFadeAlpha(time, startAlpha, endAlpha);

    }
    /// <summary>
    /// Set the message and delete previous text
    /// </summary>
    /// <param name="text">The text to be set</param>
    /// <param name="instant">instant or Delayed</param>
    public void SetText(string text, bool instant)
    {
        runCrossAlpha = false;
        if (instant)
        {
            TextMsg = $"{text}";
            ApplyText();
        }
        else
        {
            ClearMsg();
            NextMsg = text;
            delay = Time.time + baseDelay;
            hasNextMsg = true;
        }
    }
    /// <summary>
    /// Set the message and delete previous text. Slowly fade the text.
    /// </summary>
    /// <param name="text">The text to be set</param>
    /// <param name="instant">instant or Delayed</param>
    public void SetText(string text, bool instant, float time, float startAlpha = 1f, float endAlpha = 0f)
    {
        runCrossAlpha = false;
        if (hasRawImage)
        {
            RawImage.color = defaultColorRawImage;
        }
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Mathf.Clamp01(startAlpha));
        if (instant)
        {
            TextMsg = $"{text}";
            ApplyText();
        }
        else
        {
            ClearMsg();
            NextMsg = text;
            delay = Time.time + baseDelay;
            hasNextMsg = true;
        }
        CrossFadeAlpha(time, startAlpha, endAlpha);

    }

    /// <summary>
    /// Update the text by so many letters at a time.
    /// </summary>
    /// <param name="inc">How many charatures to incriment by</param>
    public void UpdateText(int inc)
    {
        //Check for issues
        if (inc <= 0 || !hasNextMsg || string.IsNullOrEmpty(NextMsg))
            return;
        //Get charatures
        int remainingChars = NextMsg.Length - index;
        int actualCharsToAdd = Mathf.Min(inc, remainingChars);

        //Update text
        if (actualCharsToAdd > 0)
        {
            TextMsg += NextMsg.Substring(index, actualCharsToAdd);
            index += actualCharsToAdd;
        }
        if (index >= NextMsg.Length)
        {
            //Ensure all text is finished
            CompleteCurrentMessage();
        }
        else
        {
            //Update text;
            ApplyText();
        }
    }
    #endregion
    #region Background
    /// <summary>
    /// Set the color of the image in the background IF the code has one
    /// </summary>
    /// <param name="color"></param>
    public void SetImageColor(Color color)
    {
        if (hasRawImage)
        {
            RawImage.color = color;
        }
        else
        {
            Debug.LogWarning("A msgbox has attempted to interact with a rawImage. But no raw image exsist.");
        }
    }
    #endregion
    #region Color
    /// <summary>
    /// Sets the color of the text
    /// </summary>
    /// <param name="color"></param>
    public void SetTextColor(Color color)
    {
        Text.color = color;
    }
    public void CrossFadeAlpha(float time, float startAlpha, float endAlpha)
    {
        alphaStart = startAlpha;
        alphaTimer = Time.time + time;
        alphaTimerBase = time;
        this.endAlpha = endAlpha;
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, startAlpha);
        runCrossAlpha = true;
    }
    #endregion
    /// <summary>
    /// Set the deley between msgs when writing a NON-INSTANT message. Default is set in the inspector. (usually 0.025f)
    /// </summary>
    /// <param name="time">Delay between letters</param>
    public void SetDelay(float time)
    {
        baseDelay = time;
    }
    /// <summary>
    /// Finish the message;
    /// </summary>
    public void CompleteCurrentMessage()
    {
        if (hasNextMsg)
        {
            index = 0;
            hasNextMsg = false;
            ApplyText();
        }
    }
    #region Health
    /// <summary>
    /// Setup a health UI System
    /// </summary>
    /// <param name="maxHealth"></param>
    /// <param name="hp"></param>
    /// <param name="warningThresh"></param>
    /// <param name="warnThresh2"></param>
    public void SetupHealthUI(float maxHealth, float hp, float warningThresh, float warnThresh2)
    {
        MaxHealth = maxHealth;
        Health = hp;
        Warn = warningThresh;
        Warn2 = warnThresh2;
        TextMsg = $"{Health}";
        RawImage.color = new Color(0.4f, 0, 0.5f, 0.86f);
        SetHP(hp);
        ApplyText();
    }
    public void SetHP(float hp)
    {
        Health = hp;
        TextMsg = $"{Health}";
        if (Health <= Warn2)
        {
            if (Health >= Warn)
            {
                RawImage.color = new Color(0.69f, 0.1f, 0.74f, 0.95f);
            }
            else
            {
                RawImage.color = new Color(0.9f, 0.2f, 0.9f, 1f);
            }
        }
        else
        {
            RawImage.color = new Color(0.4f, 0, 0.5f, 0.86f);
        }
        Text.color = new Color(1, 1, 1);
        ApplyText();
    }
    #endregion
}
