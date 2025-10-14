using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BaseCharacter.Structual;
public class Bpress : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private MsgBox MsgBox;
    [SerializeField] private bool HideOnLoad;

    private bool isPressed = false;
    private bool isUsable = true;
    private int ID;
    private int PlayerID;
    private bool HasLoaded = false;
    private string namePlayer;

    /// <summary>
    /// Get is the button pressed
    /// </summary>
    /// <param name="setPressed">Reset button state on secuces, by defualt will unpress</param>
    /// <returns>Gets if it was pressed</returns>
    public bool GetIsPressed(bool setPressed = false)
    {
        bool pressed = isPressed;
        if (pressed)
        {
            isPressed = setPressed;
        }
        return pressed;
    }
    public BoolInt GetIsPressedPlayer(bool setPressed = false)
    {
        return new BoolInt(GetIsPressed(setPressed), PlayerID);
    }
    public bool GetIsUable()
    {
        return isUsable;
    }
    public void SetUsable(bool isUsable)
    {
        this.isUsable = isUsable;
        Button.enabled = isUsable;
    }
    /// <summary>
    /// Used when button is pressed in the unity inspection thing.
    /// </summary>
    public void SetPressed()
    {
        if (isUsable)
        {
            isPressed = true;
        }
    }
    /// <summary>
    /// Setup the Invenotry when the player presses Inventory.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void SetupButtonSlot(int id, Vector3 position, NameId playerID)
    {
        transform.position = position;
        ID = id;
        PlayerID = playerID.Id;
        namePlayer = playerID.Name;
        MsgBox.AddText(namePlayer, true);
    }
    /// <summary>
    /// Ensure 
    /// </summary>
    /// <param name="HideOnLoad"></param>
    public void UpdateOnLoad(bool HideOnLoad)
    {
        if (!HasLoaded)
        {
            try
            {
                Button.navigation = new Navigation() { mode = Navigation.Mode.None };
                HasLoaded = true;
                gameObject.SetActive(HideOnLoad);
            }
            catch
            {
                Debug.LogWarning("BPress has not loaded fully");
            }
        }
    }
    public void Update()
    {
        UpdateOnLoad(HideOnLoad);
    }
    public void HideButtons()
    {
        SetUsable(false);
        Button.gameObject.SetActive(false);
    }
    public void ShowButtons()
    {
        SetUsable(true);
        Button.gameObject.SetActive(true);
    }
    /// <summary>
    /// New text to a textbox without clearing the previous text.
    /// </summary>
    /// <param name="text">The text to be added</param>
    /// <param name="instant">instant or Delayed</param>
    public void AddText(string text, bool instant)
    {
        MsgBox.AddText(text, true);
    }

}
