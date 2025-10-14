using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveData;

public class MenuSaveUI : MonoBehaviour
{
    [SerializeField] MsgBox msg;
    
    void Start()
    {
        msg.SetText("Click new game to start from scratch. If you have a save file, hit Load game.", true);
    }
    /// <summary>
    /// Start the game by loading a save or having a new save.
    /// </summary>
    /// <param name="mode">0 = Load json file from <see cref="DeleteSave"/>.<br></br> 1 = deletes currently saved json file via <see cref="DeleteSave"/> and create a new save.</param>
    public void StartMode(int mode)
    {
        if (mode == 0)
        {
            if (TryLoadGame(out PlayerSaveData get))
            {
                
            }
            else
            {
                msg.SetText("Could not find save data. Start a new game instead.", true);
            }
        }
        else
        {
            DeleteSave();
        }
    }
}
