using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerControls", menuName = "Game/Player Controls")]
public class ControlsScheme : ScriptableObject
{
    [Header("Movement")]
    public KeyCode moveUp = KeyCode.R;
    public KeyCode moveDown = KeyCode.F;
    public KeyCode moveLeft = KeyCode.D;
    public KeyCode moveRight = KeyCode.G;
    public KeyCode crouch = KeyCode.UpArrow;
    public KeyCode sprint = KeyCode.Mouse4;
    public KeyCode jump = KeyCode.Space;
    public KeyCode breaking = KeyCode.LeftShift;
    public KeyCode breaking2 = KeyCode.Z;
    [Header("Action")]
    public KeyCode primaryFire = KeyCode.Mouse0;
    public KeyCode secondaryFire = KeyCode.Mouse1;
    public KeyCode utilityKey = KeyCode.V;
    public KeyCode reload = KeyCode.A;
    public KeyCode interact = KeyCode.T;
    public KeyCode Inventory = KeyCode.H;
    [Header("HotBar Slots")]
    public KeyCode moveKey = KeyCode.E;
    public KeyCode slot1 = KeyCode.Alpha1;
    public KeyCode slot2 = KeyCode.Alpha2;
    public KeyCode slot3 = KeyCode.Alpha3;
    public KeyCode slot4 = KeyCode.Alpha4;
    public KeyCode slot5 = KeyCode.Alpha5;
    public KeyCode slot6 = KeyCode.Alpha6;
    public KeyCode slot7 = KeyCode.Alpha7;
    public KeyCode slot8 = KeyCode.Alpha8;
    public KeyCode slot9 = KeyCode.Alpha9;
    public KeyCode slot10 = KeyCode.Alpha0;
    public KeyCode slot11 = KeyCode.Minus;
    public KeyCode slot12 = KeyCode.Plus;
    [Header("Options")]
    public KeyCode PauseGame = KeyCode.Escape;
    public KeyCode HelpMenu = KeyCode.F1;
    public KeyCode ResetGame = KeyCode.F9;
    public KeyCode EndGame = KeyCode.F10;
    [Header("Chatting")]
    public KeyCode OpenChat = KeyCode.O;
    public KeyCode CommandChat = KeyCode.Slash;
    /// <summary>
    /// 0. moveUp
    /// <list type="number">
    /// <item>moveDown</item>
    /// <item>moveLeft</item>
    /// <item>moveRight</item>
    /// <item>Crouch</item>
    /// <item>Sprint</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public KeyCode[] GetExtraKeys()
    {
        return new KeyCode[] { crouch, sprint };
    }
    public KeyCode[] GetMoveKeys()
    {
        return new KeyCode[] { moveUp, moveDown, moveLeft, moveRight };
    }
    /// <summary>
    /// 0. primary Fire
    /// <list type="number">
    /// <item>SecondaryFire</item>
    /// <item>Utility key</item>
    /// <item>Reload</item>
    /// <item>Inventory</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public KeyCode[] GetActionKeys()
    {
        return new KeyCode[] { primaryFire, secondaryFire, utilityKey, reload, Inventory,interact };
    }
    /// <summary>
    /// 0.  move Items Key
    /// <list type="number">
    /// <item>slot 1</item>
    /// <item>slot 2</item>
    /// <item>slot 3...</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public KeyCode[] GetHotbarKeys()
    {
        return new KeyCode[] { moveKey, slot1, slot2, slot3, slot4, slot5, slot6 };
    }
    /// <summary>
    /// 0. Pause Game
    /// <list type="number">
    /// <item>Help Menu</item>
    /// <item>Reset Game</item>
    /// <item>End Game</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public KeyCode[] GetOptions()
    {
        return new KeyCode[] { PauseGame, HelpMenu, ResetGame, EndGame };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if you are pressing UP/Down/Left/Right</returns>
    public bool GetIsAMovementKeyPressed()
    {
        if (Input.GetKey(moveUp) ||  Input.GetKey(moveDown) || Input.GetKey(moveLeft)  || Input.GetKey(moveRight))
        {
            return true;
        }
        return false;
    }
}
