using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using static BaseCharacter.Items.BoxColors;
using static Enums;
public class UiDisplay : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    /// <summary>
    /// Where the hotbar slot is on the UI.
    /// </summary>
    private int ID { get; set; }
    /// <summary>
    /// Use when the inventory item and the UIDisplay do not align.
    /// </summary>
    public int? ItemInvSlot { get; private set; }
    /// <summary>
    /// Item being set to "pending" in order to be moved to another slot.
    /// </summary>
    private bool PendingItem { get; set; }
    private bool IsClicked { get; set; }
    /// <summary>
    /// Is the item in the hotbar.
    /// </summary>
    private bool IsHotbar { get; set; }
    /// <summary>
    /// Is open in the inventory
    /// </summary>
    private bool IsOpen { get; set; }
    private Texture Item { get; set; }
    /// <summary>
    /// The red outline around items. Used to indicate what item your selecting
    /// </summary>
    private RawImage slotbar;
    /// <summary>
    /// The item itself that is being stored.
    /// </summary>
    [SerializeField] private RawImage backgroundItem;
    [SerializeField] private ExtraMenu extraMenus;
    [SerializeField] private MsgBox count;
    private bool HasExtraMenu;
    /*
    [SerializeField] private Color Idle;
    [SerializeField] private Color Hover;
    [SerializeField] private Color IdleSelected;
    [SerializeField] private Color HoverSelected;
    */
    public bool IsClickable { get; private set; }
    public bool HasLoaded { get; private set; }

    private bool shouldSwap = false;


    private void Start()
    {
        slotbar = GetComponent<RawImage>();
        SetNotSelected();
    }

    // Implement the IPointerDownHandler interface
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsClickable)
        {
            IsClicked = !IsClicked;
            UpdateSelectionVisual();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsClickable && Input.GetMouseButton(0))
        {
            IsClicked = true;
            shouldSwap = true;
            UpdateSelectionVisual();
        }
        else if (IsClickable)
        {
            DarkenBackground();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsClickable)
        {
            UpdateSelectionVisual();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void UpdateOnLoad()
    {
        if (!HasLoaded)
        {
            try
            {
                backgroundItem.gameObject.SetActive(false);
                slotbar.color = Color.clear;
                HideNonHotbar();
                if (IsHotbar)
                {
                    backgroundItem.gameObject.SetActive(true);
                    slotbar.color = idle;
                }
                HasLoaded = true;
            }
            catch
            {
                Debug.LogWarning("Slotbar has not loaded fully");
            }
        }
    }
    public void UpdateSelectionVisual()
    {
        if (IsClicked)
        {
            slotbar.color = idleSelected;
            backgroundItem.color = new Color(1, 1, 1, 0.8f);
        }
        else
        {
            SetNotSelected();
        }
    }
    public void DarkenBackground()
    {
        slotbar.color *= new Color(0.5f, 0.5f, 0.5f);
    }

    /// <summary>
    /// Place the Hotbar slot where it belongs + give the hotbar the ID being used.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void SetupHotbarSlot(int id, Vector3 position)
    {
        transform.position = position;
        ID = id;
        IsHotbar = true;
    }
    /// <summary>
    /// Place the Hotbar slot where it belongs + give the hotbar the ID being used.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void SetupHotbarSlot(int id, Vector3 position, float scale)
    {
        transform.position = position;
        transform.localScale = new Vector3(scale,scale,scale);
        ID = id;
        IsHotbar = true;
    }

    /// <summary>
    /// Setup the Invenotry when the Player presses Inventory.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void SetupInvenotrySlot(int id, Vector3 position)
    {
        transform.position = position;
        ID = id;
        IsHotbar = false;
    }
    /// <summary>
    /// Setup the Invenotry when the Player presses Inventory.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void SetupInvenotrySlot(int id, Vector3 position, float scale)
    {
        transform.position = position;
        transform.localScale = new Vector3(scale, scale, scale);
        ID = id;
        IsHotbar = false;
        Debug.Log(ID);
    }
    /// <summary>
    /// Hide all items not in your hotbar
    /// </summary>
    public void HideNonHotbar()
    {
        try
        {
            if (!IsHotbar)
            {
                backgroundItem.gameObject.SetActive(false);
                slotbar.color = Color.clear;
                PendingItem = false;
                IsOpen = false;
                count.SetText("", true);
            }
        }
        catch
        {
            Debug.LogWarning("HideNonHotbar(): Slotbar has not loaded in yet");
        }
    }
    public void HideEverything()
    {
        try
        {
            backgroundItem.gameObject.SetActive(false);
            slotbar.color = Color.clear;
            PendingItem = false;
            IsOpen = false;
            count.SetText("", true);
        }
        catch
        {
            Debug.LogWarning("HideHotbar(): Slotbar has not loaded in yet");
        }
    }
    /// <summary>
    /// Show the rest of your inventory
    /// </summary>
    public void ShowNonHotbar()
    {
        if (!IsHotbar)
        {
            try
            {
                backgroundItem.gameObject.SetActive(true);
                slotbar.color = idle;
                IsOpen = true;
                SetNotSelected();
            }
            catch
            {
                Debug.LogWarning("ShowNonHotbar:Extra Inventory Closed");
            }
        }
    }

    /// <summary>
    /// Show the item currently being held in the hotbar
    /// </summary>
    /// <param name="item"></param>
    public void SetTexture(Texture item)
    {
        if (backgroundItem == null)
        {
            Debug.LogError("UiDisplay: backgroundItem is not assigned");
            return;
        }
        Item = item;
        backgroundItem.texture = item;
        backgroundItem.color = Color.white;
        ItemInvSlot = null;
    }
    /// <summary>
    /// Show the item currently being held in the hotbar
    /// </summary>
    /// <param name="item"></param>
    /// <param name="invSlot"></param>
    public void SetTexture(Texture item, int invSlot)
    {
        if (backgroundItem == null)
        {
            Debug.LogError("UiDisplay: backgroundItem is not assigned");
            return;
        }
        Item = item;
        backgroundItem.texture = item;
        backgroundItem.color = Color.white;
        ItemInvSlot = invSlot;
    }

    /// <summary>
    /// Clear image and color from the hotbar.
    /// </summary>
    public void ClearTexture()
    {
        backgroundItem.texture = null;
        backgroundItem.color = Color.clear;
        ItemInvSlot = null;
    }

    /// <summary>
    /// Set the item's <see cref="PendingItem"></see> to true and then run <see cref="SetSelected"/>
    /// </summary>
    public void SetSelectedPendingItem()
    {
        PendingItem = true;
        SetSelected();
    }

    /// <summary>
    /// Set <see cref="PendingItem"/> to false, then see if the item is being selected. If true, run <see cref="SetSelected"/>. Otherwise run <see cref="SetNotSelected"/>
    /// </summary>
    /// <param name="hotbar"></param>
    public void RemovePendingItem(int hotbar)
    {
        PendingItem = false;
        if (ID == hotbar)
        {
            SetSelected();
        }
        else
        {
            SetNotSelected();
        }
    }
    /// <summary>
    /// Removes the black color from clicking on an item.
    /// </summary>
    /// <param name="inv">ID</param>
    public void RemoveClickItem(int inv)
    {
        IsClicked = false;
        if (ID == inv)
        {
            SetSelected();
        }
        else
        {
            SetNotSelected();
        }
    }
    /// <summary>
    /// Where the hotbar is on your UI.
    /// </summary>
    /// <returns>ID</returns>
    public int GetId()
    {
        return ID;
    }
    public int? GetPlayerLocation()
    {
        if (ItemInvSlot == null)
        {
            return ID;
        }
        return ItemInvSlot;
    }

    /// <summary>
    /// Set the color of the slotbar to Black if Pending or Red if not pending.
    /// </summary>
    public void SetNotSelected()
    {
        try
        {
            extraMenus.SetNotSelected();
        }
        catch
        {
            Debug.Log("ExtraMenus is either null or has not loaded in yet.");
        }
        try
        {
            
            if (PendingItem)
            {
                slotbar.color = idleSelected;
            }
            else if (IsHotbar)
            {
                slotbar.color = idle;
            }
            else if (IsOpen)
            {
                slotbar.color = idle;
            }
        }
        catch
        {
            Debug.Log("Slotbar has not loaded in yet.");
        }
    }

    /// <summary>
    /// Set the slotbar color to a Blue if pending, or a pink if not pending.
    /// </summary>
    public void SetSelected()
    {
        try
        {
            extraMenus.SetSelected(IsHotbar);
        }
        catch
        {
            Debug.LogWarning("ExtraMenu has not loaded in yet");
        }
        
        try
        {
           
            if (PendingItem)
            {
                slotbar.color = hoverSelected;
            }
            else
            {
                slotbar.color = hover;
            }
        }
        catch
        {
            Debug.LogWarning("Slotbar has not loaded in yet.");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns><see cref="IsClicked"/></returns>
    public bool GetClicked()
    {
        return IsClicked;
    }
    public void SetupExtraMenu(params ExtraDataType[] extraMenuDataType)
    {
        extraMenus.SetupExtraMenu(extraMenuDataType);
    }
    public int[] GetExtraMenuSliders()
    {
        return extraMenus.sliders;
    }
    public int GetExtraMenuRoleSlider()
    {
        return extraMenus.GetSliderRoleValue();
    }
    public bool[] GetExtraMenuButtons()
    {
        return extraMenus.presses;
    }
    /// <summary>
    /// Set your inventory, hotbar, etc... to be Clickable
    /// </summary>
    /// <param name="type">True = Clickable on the UI.</param>
    public void SetClickable(bool type)
    {
        IsClickable = type;
    }
    /// <summary>
    /// Used to only make the hotbar clickable.
    /// </summary>
    public void SetClickableHotbaronly()
    {
        if (IsHotbar)
        {
            IsClickable = true;
        }
        else
        {
            IsClickable = false;
        }
    }
    void Update()
    {
        UpdateOnLoad();
    }
    public void SetSliderSelfRole(int role)
    {
        try
        {
            extraMenus.SetSliderSelfRole(role);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"ExtraMenu has either not been spawned in or there was in error in SetSliderSelfRole(PlayerRole role). {e}");
        }
    }
    public bool GetSelfButton()
    {
        return extraMenus.GetSelfButton();
    }
    public void SetCount(string amount)
    {
        count.SetText($"{amount}", true);
    }
    public void ClearCount()
    {
        count.ClearMsg();
    }
}