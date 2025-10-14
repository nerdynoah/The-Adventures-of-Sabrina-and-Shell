using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class SliderDatacollect : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private ExtraDataType extraDataType;
    [SerializeField] private bool SkipOwnRole = true;
    [SerializeField] private MsgBox MsgBox;
    private int ownRole;
    /// <summary>
    /// Is the slider usable.
    /// </summary>
    private bool isUsable = true;
    public int GetSliderValue()
    {
        int extra = 0;
        if (SkipOwnRole)
        {
            if (slider.value >= ownRole)
            {
                extra = 1;
            }
        }
        return (int)slider.value + extra;
    }
    public ExtraDataType GetDataType()
    {
        return extraDataType;
    }
    public bool GetIsUsable()
    {
        return isUsable;
    }
    public void SetUsable(bool isUsable)
    {
        this.isUsable = isUsable;
        slider.enabled = isUsable;
    }
    private void Start()
    {
        slider.navigation = new Navigation() { mode = Navigation.Mode.None };
        if (extraDataType == ExtraDataType.SliderRoles)
        {

        }
        OnSliderChange();
    }
    public void OnSliderChange()
    {
        int extra = 0;
        if (SkipOwnRole)
        {
            if (slider.value >= ownRole)
            {
                extra = 1;
            }
        }
        MsgBox.SetRoleText("Guess", $"{slider.value + extra}");
    }
    public void SetPlayerRole(int role)
    {
        ownRole = role;
    }

}
