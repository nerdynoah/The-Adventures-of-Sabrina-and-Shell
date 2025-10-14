using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsSlider : MonoBehaviour
{
    [SerializeField] MsgBox msg;
    private int Bots { get; set; }
    public void OnSlider(float slider)
    {
        Bots = (int)slider;
        msg.SetMsgStat("Bots: ", Bots, true);
    }
    public void OnEnable()
    {
        msg.SetMsgStat("Bots: ", Bots, true);
    }
    public void OnStart()
    {
        msg.SetMsgStat("Bots: ", Bots, true);
    }
}
