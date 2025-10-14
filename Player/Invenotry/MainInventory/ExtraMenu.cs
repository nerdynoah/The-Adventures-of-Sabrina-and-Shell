using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class ExtraMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] Buttons;
    [SerializeField] private GameObject[] Sliders;
    [SerializeField] private MsgBox[] Messages;
    [SerializeField] private ExtraDataType[] Type;
    /// <summary>
    /// Does the ability require the object to be summoned.
    /// </summary>
    private bool[] AllowedIndex = new bool[4];

    [SerializeField] private Color backGround = new Color(0.22f, 0.22f, 0.96f, 0.8f);
    [SerializeField] private RawImage spriteRenderer;
    [SerializeField] private RectTransform RectTransform;
    /// <summary>
    /// Is the <see cref="BaseCharacter.InventoryItem"/> selected in an inventory.
    /// </summary>
    private bool IsSelected;
    /// <summary>
    /// Is the button pressed.
    /// </summary>
    public bool[] presses { get; private set; } = new bool[1];
    /// <summary>
    /// What is the slider set too.
    /// </summary>
    public int[] sliders { get; private set; } = new int[1];
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Running Start() in ExtraMenu");
        spriteRenderer.color = backGround;
        RectTransform.localScale = new Vector3(1, 0f, 1);
    }
    private void Update()
    {
        if (IsSelected)
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                Bpress press = Buttons[i].GetComponent<Bpress>();
                if (press.GetIsPressed())
                {
                    presses[i] = true;
                }
            }
            for (int i = 0; i < Sliders.Length; i++)
            {
                SliderDatacollect slider = Sliders[i].GetComponent<SliderDatacollect>();
                sliders[i] = slider.GetSliderValue();
            }
        }
    }
    public bool GetSelfButton()
    {
        return presses[0];
    }
    public int GetSliderRoleValue()
    {
        return Sliders[0].GetComponent<SliderDatacollect>().GetSliderValue();
    }
    /// <summary>
    /// Show the buttons and the background image if selected.
    /// </summary>
    /// <param name="isHotbar">Use <see cref="UiDisplay.IsHotbar"/> to determine if the extra button should appear dependent on location</param>
    public void SetSelected(bool isHotbar)
    {
        if (isHotbar)
        {
            int usableThing = 0;
            spriteRenderer.enabled = true;
            for (int i = 0; i < Buttons.Length; i++)
            {
                if (i < Type.Length && AllowedIndex[i] && Type[i] == ExtraDataType.IsSelf)
                {
                    Buttons[i].SetActive(true);
                    Buttons[i].transform.localPosition = new Vector3(0, (usableThing * 40f), -4.9f);
                    usableThing++;
                }
                else
                {
                    Buttons[i].SetActive(false);
                }
            }

            // Then process sliders (offset index by buttons length)
            for (int i = 0; i < Sliders.Length; i++)
            {
                int typeIndex = i + Buttons.Length;
                if (typeIndex < Type.Length && AllowedIndex[typeIndex] && Type[typeIndex] == ExtraDataType.SliderRoles)
                {
                    Sliders[i].SetActive(true);
                    Sliders[i].transform.localPosition = new Vector3(0, (usableThing + 1) * 100f, -4.9f);
                    usableThing++;
                }
                else
                {
                    Sliders[i].SetActive(false);
                }
            }

            if (usableThing > 0)
            {
                IsSelected = true;
            }
        }
    }
    /// <summary>
    /// What Extra menu features are required. 
    /// </summary>
    /// <param name="extraMenuDataType">The extra menu type that should be added to the UI</param>
    public void SetupExtraMenu(params ExtraDataType[] extraMenuDataType)
    {
        if (extraMenuDataType != null)
        {
            AllowedIndex = new bool[Buttons.Length + Sliders.Length];
            presses = new bool[Buttons.Length];
            sliders = new int[sliders.Length];
            for (int i = 0; i < Buttons.Length; i++)
            {
                AllowedIndex[i] = false;
                presses[i] = false;
                sliders[i] = 0;
            }
            int usableThings = 0;
            for (int i = 0; i < extraMenuDataType.Length; i++)
            {
                for (int j = 0; j < Type.Length; j++)
                {
                    if (Type[j] == extraMenuDataType[i] && Type[j] == ExtraDataType.IsSelf)
                    {
                        AllowedIndex[j] = true;
                        usableThings++;
                    }
                    if (Type[j] == extraMenuDataType[i] && Type[j] == ExtraDataType.SliderRoles)
                    {
                        AllowedIndex[j] = true;
                        usableThings++;
                    }
                }
            }
            RectTransform.localScale = new Vector3(1, usableThings, 1);
        }
        else
        {
            Debug.Log("No extra menu required for item");
        }
    }
    
    public void SetNotSelected()
    {
        spriteRenderer.enabled = false;
        IsSelected = false;
        foreach (var button in Buttons)
        {
            button.SetActive(false);
        }
        foreach (var slider in Sliders)
        {
            slider.SetActive(false);
        }
    }
    public void SetSliderSelfRole(int role)
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            try
            {
                Sliders[i].GetComponent<SliderDatacollect>().SetPlayerRole(role);
            }
            catch 
            {
                Debug.LogWarning("Slider does not have SliderDataCollect object"); 
                continue;
            }
        }
    }
}
