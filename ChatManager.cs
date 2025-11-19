using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// A chat manager which you can enter data.
/// </summary>
public class ChatManager : MonoBehaviour
{
    [SerializeField] MsgBox Output;
    [SerializeField] TMP_InputField EnterMessage;
    [SerializeField] GOActive InputBoxes;
    [SerializeField] bool DefaultsOn = false;
    private bool SheerMessagesState;
    private int LastState;
    private readonly List<string> previousMsgs = new List<string>();

    private void Start()
    {
        InputBoxes.SetState(DefaultsOn);
        SheerMessagesState = DefaultsOn;
    }
    public void OpenBox(string startText = "")
    {
        InputBoxes.SetState(true);
        SheerMessagesState = true;
        EnterMessage.text = startText;
    }
    public void CloseBox(float switchTime = 0f)
    {
        InputBoxes.SetState(false);
        if (switchTime > 0)
        {
            SheerMessagesState = false;
        }
        else
        {
            SheerMessagesState = false;
        }
    }
    public bool ToggleBox(float switchTime = 0f)
    {
        if (!SheerMessagesState)
        {
            OpenBox();
            LastState = -1;
            return true;
        }
        CloseBox(switchTime);
        LastState = -1;
        return false;
    }
    /// <summary>
    /// Add text and calculate based on how long the text is to keep the box up longer.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="minTime"></param>
    /// <param name="maxTime"></param>
    /// <param name="switchTime"></param>
    public void AddText(string text, float startAlpha, float minTime = 5f, float switchTime = 0.02f)
    {
        float time = minTime + switchTime * text.Length;
        Output.SetText(text, false, time * 2, startAlpha);
    }
    /// <summary>
    /// Get current chat
    /// </summary>
    /// <returns><see cref="EnterMessage.text"/></returns>
    public string GetInputText()
    {
        return EnterMessage.text;
    }
    /// <summary>
    /// Adds message to a list (so you can scroll) and Chatbox
    /// </summary>
    public void ClearText()
    {
        previousMsgs.Add(EnterMessage.text);
        EnterMessage.text = "";
        LastState = -1;
    }
    public void ClearAndCloseTextBox(float switchTime = 0f)
    {
        ClearText();
        CloseBox(switchTime);
    }
    public void SelectInputField()
    {
        if (EnterMessage != null)
        {
            EnterMessage.Select();
            EnterMessage.ActivateInputField();
        }
    }
    /// <summary>
    /// Scroll between previous messages
    /// </summary>
    /// <param name="Direction">Scroll amount</param>
    public void ScrollMsgs(int Direction)
    {
        if (LastState >= previousMsgs.Count - 1 && Direction > 0) 
        {
            LastState = -1;
            EnterMessage.text = "";
            return;
        }
        else if (LastState <= -1 && Direction < 0)
        {
            LastState = previousMsgs.Count - 1;
        }
        else if (Direction > 0)
        {
            LastState = LastState + 1;
        }
        else if (Direction < 0)
        {
            LastState = LastState - 1;
        }

        EnterMessage.text = previousMsgs[LastState];
    }
    
}
