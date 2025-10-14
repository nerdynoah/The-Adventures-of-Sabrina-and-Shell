using BaseCharacter;
using BaseCharacter.Items;
using BaseCharacter.Structual;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
/// <summary>
/// Invenotry Manager for the UI
/// </summary>
public class InvManager : MonoBehaviour
{
    /// <summary>
    /// The UI image
    /// </summary>
    [SerializeField] private UiDisplay Slot;

    /// <summary>
    /// Desc text.
    /// </summary>
    [SerializeField] private MsgBox TextBox;
    /// <summary>
    /// Use buttons
    /// </summary>
    [SerializeField] private Bpress ButtonSelection;
    [SerializeField] private bool UseTextBox = false;
    /// <summary>
    /// Size of inventory
    /// </summary>
    protected int InventorySize { get; set; }
    /// <summary>
    /// Size of hotbar
    /// </summary>
    protected int HotbarSize { get; set; }
    protected int ButtonSize { get; set; }
    /// <summary>
    /// Selected item
    /// </summary>
    protected int SelectedItem { get; set; }
    /// <summary>
    /// Seleteced UI width of the Hotbar
    /// </summary>
    protected int Width { get; set; }
    /// <summary>
    /// UI slots
    /// </summary>
    private UiDisplay[] activeSlots;
    private Bpress[] activeButtons;
    /// <summary>
    /// Animations
    /// </summary>
    private bool IsInventoryOpen { get; set; } = false;

    #region Clear Slots
    // New this to ensure proper cleanup
    private void OnDestroy()
    {
        ClearAllSlots();
    }

    /// <summary>
    /// Clear all slots by destorying them via <see cref="Object.Destroy(Object)"/>
    /// </summary>
    private void ClearAllSlots()
    {
        if (activeSlots != null)
        {
            foreach (var slot in activeSlots)
            {
                if (slot != null && slot.gameObject != null)
                {
                    Destroy(slot.gameObject);
                }
            }
            activeSlots = null;
        }
    }
    private void ClearAllButtons()
    {
        if (activeButtons != null)
        {
            foreach(var button in activeButtons)
            {
                if (button != null && button.gameObject != null)
                {
                    Destroy(button.gameObject);
                }
            }
        }
    }
    #endregion
    /// <summary>
    /// Select the item that is in use on the hotbar
    /// </summary>
    /// <param name="id">The ID of the hotbar item</param>
    public void SetSelectedItem(int id)
    {
        try
        {
            SelectedItem = id;
            foreach (UiDisplay display in activeSlots)
            {
                if (display.GetId() == id)
                {
                    display.SetSelected();
                }
                else
                {
                    display.SetNotSelected();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"UiDisplay has not loaded in yet. {ex}");
        }
    }
    /// <summary>
    /// Sets the current item and itembox to be seleted and highlighted. May display text and/or do other stuff.
    /// </summary>
    /// <param name="id">Where</param>
    /// <param name="name">Name of item</param>
    public void SetSelectedItem(int id, string name)
    {
        try
        {
            SelectedItem = id;
            foreach (UiDisplay display in activeSlots)
            {
                if (display.GetId() == id)
                {
                    display.SetSelected();
                    if (UseTextBox && TextBox != null)
                    {
                        TextBox.SetText(name, true, 2.6f,2,0);
                    }
                }
                else
                {
                    display.SetNotSelected();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"UiDisplay has not loaded in yet. {ex}");
        }
    }
    #region Clickable and Buttons
    public void SetClickable(bool clickable)
    {
        foreach (var slot in activeSlots)
        {
            slot.SetClickable(clickable);
        }
    }
    /// <summary>
    /// Set the button showable in <see cref="activeButtons"/>
    /// </summary>
    public void SetButtonShowable()
    {
        foreach (var button in activeButtons)
        {
            button.SetUsable(true);
        }
    }
    /// <summary>
    /// Hides all the buttons in <see cref="activeButtons"/>
    /// </summary>
    public void HideButtons()
    {
        if (activeButtons != null)
        {
            foreach (var button in activeButtons)
            {
                button.HideButtons();
            }
        }
        
    }
    /// <summary>
    /// Shows all of the buttons in <see cref="activeButtons"/>
    /// </summary>
    public void ShowButtons()
    {
        foreach(var button in activeButtons)
        {
            button.ShowButtons();
        }
    }
    #endregion
    #region Setup and Intiliazation
    /// <summary>
    /// Set the size of the hotbar slots.
    /// </summary>
    /// <param name="defaultSlot">The default slot. Set to 0</param>
    /// <param name="hotSize">Hotbar size</param>
    /// <param name="totalSize">Total size of inventory</param>
    public void SetupInventorySize(int totalSize, int hotSize, int defaultSlot = 0)
    {
        ClearAllSlots();
        // Validate input parameters
        hotSize = Mathf.Max(1, hotSize);
        defaultSlot = Mathf.Clamp(defaultSlot, 0, hotSize - 1);
        totalSize = Mathf.Max(hotSize, totalSize);

        // Initialize properties
        HotbarSize = hotSize;
        InventorySize = totalSize;
        SelectedItem = defaultSlot;
        Width = (int)(Screen.width * 0.70f);
        activeSlots = new UiDisplay[totalSize];

        // Create new slots
        for (int i = 0; i < hotSize; i++)
        {
            Vector3 tempLocation = new Vector3(Width + (i * -200), Screen.height * 0.02f + 60f, 0);
            UiDisplay slot = Instantiate(Slot, transform);

            if (slot != null)
            {
                slot.SetupHotbarSlot(i, tempLocation);
                activeSlots[i] = slot;
            }
        }
        
        for (int i = hotSize; i < totalSize; i++)
        {
            int cols = Mathf.Clamp(hotSize + 2, totalSize/4, 10);
            float startX = Screen.width / 3 - (cols * 100) / 2;
            int row = (i - hotSize) / cols;
            int col = (i - hotSize) % cols;

            Vector3 tempLocation = new Vector3(
                startX + (col * 200),
                Screen.height * 0.88f - (row * 150),
                0);

            UiDisplay slot = Instantiate(Slot, transform);
            if (slot != null)
            {
                slot.SetupInvenotrySlot(i, tempLocation);
                slot.UpdateSelectionVisual();
                activeSlots[i] = slot;
                
            }
        }
    }
    
    /// <summary>
    /// Set the size of the hotbar slots.
    /// </summary>
    /// <param name="defaultSlot">The default slot. Set to 0</param>
    /// <param name="distance">The distance between each button. </param>
    /// <param name="hotSize">How big the hotbar is</param>
    /// <param name="scale">Rescale the object</param>
    /// <param name="totalSize">The total size of your inventory</param>
    public void SetupInventorySize(int totalSize, int hotSize, int defaultSlot, float scale, float distance, int col, float StartScreenWidth = 0.2f, float height = 60f, float fullInvoHeight = 0.5f, float fullInvoHGap = 80f)
    {
        distance = Mathf.Abs(distance);
        distance = distance * -1;
        ClearAllSlots();
        // Validate input parameters
        hotSize = Mathf.Max(1, hotSize);
        defaultSlot = Mathf.Clamp(defaultSlot, 0, hotSize - 1);
        totalSize = Mathf.Max(hotSize, totalSize);

        // Initialize properties
        HotbarSize = hotSize;
        InventorySize = totalSize;
        SelectedItem = defaultSlot;
        Width = (int)(Screen.width * StartScreenWidth);
        activeSlots = new UiDisplay[totalSize];


        // Create new slots
        for (int i = 0; i < hotSize; i++)
        {
            Vector3 tempLocation = new Vector3(Width + (i * -distance), height, 0) * (scale + scale);
            UiDisplay slot = Instantiate(Slot, transform);

            if (slot != null)
            {
                Debug.Log($"{i}");
                slot.SetupHotbarSlot(i, tempLocation, scale);
                activeSlots[i] = slot;
            }
        }
        int inventorySlots = totalSize - hotSize;
        if (inventorySlots > 0 && col > 0)
        {
            int rows = Mathf.CeilToInt((float)inventorySlots / col);
            float startY = Screen.height * Mathf.Clamp01(fullInvoHeight);

            int slotIndex = hotSize;

            for (int row = 0; row < rows; row++)
            {
                for (int colm = 0; colm < col; colm++)
                {
                    if (slotIndex >= totalSize)
                        break;
                    Vector3 tempLocation = new Vector3(
                    Width + (row * -distance),
                    startY - (colm * fullInvoHGap),
                        0) * (scale + scale);

                    UiDisplay slot = Instantiate(Slot, transform);
                    if (slot != null)
                    {
                        slot.SetupInvenotrySlot(slotIndex, tempLocation, scale);
                        slot.UpdateSelectionVisual();
                        activeSlots[slotIndex] = slot; // Fixed: Use slotIndex, not colm
                    }
                    else
                    {
                        Debug.LogError(nameof(slot) + $"Slot was NULL");
                        throw new NullReferenceException(nameof(slot));
                    }
                        slotIndex++;
                }
            }
        }
    }
    public void CheckForNull()
    {
        foreach (UiDisplay display in activeSlots)
        {
            if (display == null)
            {
                Debug.LogAssertion(nameof(display) + " Is null");
            }
        }
    }
    /// <summary>
    /// Set the size of the hotbar slots.
    /// </summary>
    /// <param name="defaultSlot">The default slot. Set to 0</param>
    /// <param name="distance">The distance between each button. </param>
    /// <param name="hotSize">How big the hotbar is</param>
    /// <param name="scale">Rescale the object</param>
    /// <param name="totalSize">The total size of your inventory</param>
    public void SetupInventorySize(int totalSize, int hotSize, int defaultSlot, float scale, float distance, int row, int col)
    {
        distance = Mathf.Abs(distance);
        distance = distance * -1;
        ClearAllSlots();
        // Validate input parameters
        hotSize = Mathf.Max(1, hotSize);
        defaultSlot = Mathf.Clamp(defaultSlot, 0, hotSize - 1);
        totalSize = Mathf.Max(hotSize, totalSize);

        // Initialize properties
        HotbarSize = hotSize;
        InventorySize = totalSize;
        SelectedItem = defaultSlot;
        Width = (int)(Screen.width * 0.7f);
        activeSlots = new UiDisplay[totalSize];

        // Create new slots
        for (int i = 0; i < hotSize; i++)
        {
            Vector3 tempLocation = new Vector3(Width + (i * -distance), Screen.height * 0.02f + 60f, 0) * (scale + scale);
            UiDisplay slot = Instantiate(Slot, transform);

            if (slot != null)
            {
                slot.SetupHotbarSlot(i, tempLocation, scale);
                activeSlots[i] = slot;
            }
        }

        for (int i = hotSize; i < totalSize; i++)
        {
            int cols = Mathf.Clamp(hotSize + 2, totalSize / 4, 10);
            float startX = Screen.width / 3 - (cols * 100) / 2;

            Vector3 tempLocation = new Vector3(
                startX + (col * 200),
                Screen.height * 0.88f - (row * 150),
                0) * (scale + scale);

            UiDisplay slot = Instantiate(Slot, transform);
            if (slot != null)
            {
                slot.SetupInvenotrySlot(i, tempLocation, scale);
                slot.UpdateSelectionVisual();
                activeSlots[i] = slot;

            }
        }
    }
    /// <summary>
    /// Setup buttons to display.
    /// </summary>
    /// <param name="nameid">Name and there ID</param>
    public void SetupMultiplayerMenuButtons(List<NameId> nameid)
    {
        if (nameid.Count < 1)
        {
            Debug.LogAssertion("NameID struct array is empty");
            return;
        }
        ClearAllButtons();
        ButtonSize = nameid.Count;
        Width = (int)(Screen.width * 0.50f);
        activeButtons = new Bpress[ButtonSize];
        for (int i = 0; i < ButtonSize; i++)
        {
            Vector3 tempLocation = new Vector3(Width + Screen.width * 0.1f, Screen.height - 50 - i * 300f, 0);
            Bpress button = Instantiate(ButtonSelection, transform.parent);
            if (button != null)
            {
                button.SetupButtonSlot(i, tempLocation, nameid[i]);
                Debug.Log($"Among us {tempLocation}");
                activeButtons[i] = button;
            }
        }
    }
    /// <summary>
    /// Show if an item has an aditional button menu.
    /// </summary>
    /// <param name="extraText"></param>
    public void ShowMultiplayerMenuButtons(string extraText)
    {
        foreach(var button in activeButtons)
        {
            button.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(extraText))
            {
                button.AddText(extraText, true);
            }
        }
    }
    #endregion

    #region Getters
    /// <summary>
    /// Gets the item which is waiting to be swapped.
    /// </summary>
    /// <returns>Int?[2]</returns>
    public int?[] GetPending()
    {
        int?[] Swap = new int?[2];
        Swap[0] = null;
        Swap[1] = null;
        int i = 0;
        foreach (var slot in activeSlots)
        {
            if (slot.GetClicked() == true)
            {
                SelectItem(slot.GetId());
                Swap[i] = slot.GetId();
                i++;
            }
            if (i > 1)
            {
                break;
            }
        }
        return Swap;
    }
    /// <summary>
    /// Gets the size of the hotbar
    /// </summary>
    /// <returns>The size of the hotbar</returns>
    public int GetInventorySize()
    {
        return InventorySize;
    }
    /// <summary>
    /// Get hotbar size
    /// </summary>
    /// <returns></returns>
    public int GetHotbarSize()
    {
        return HotbarSize;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hotbarSlot"></param>
    /// <returns></returns>
    public int GetPlayerLocation(int hotbarSlot)
    {
        if (activeSlots[hotbarSlot].ItemInvSlot == null)
        {
            return hotbarSlot;
        }
        return (int)activeSlots[hotbarSlot].ItemInvSlot;
    }
    #endregion
    #region Texture Selection
    /// <summary>
    /// Apply a texture to the inventory boxes
    /// </summary>
    /// <param name="texture">The UIcon</param>
    /// <param name="id">The id in which the item goes</param>
    public void ApplyInventoryBoxes(Texture texture, int id)
    {
        if (texture == null)
        {
            Debug.LogWarning("ApplyInventoryBoxes: Texture is null");
            return;
        }

        if (id < 0 || id >= activeSlots.Length || activeSlots[id] == null)
        {
            Debug.LogError($"ApplyInventoryBoxes: Invalid slot ID {id}");
            return;
        }

        activeSlots[id].SetTexture(texture);
    }
    /// <summary>
    /// Clear the texture at a slot.
    /// </summary>
    /// <param name="id">Where</param>
    public void ClearTexture(int id)
    {
        activeSlots[id].ClearTexture();
        activeSlots[id].ClearCount();
    }
    /// <summary>
    /// Selected items
    /// </summary>
    /// <param name="id">id</param>
    public void SelectItem(int id)
    {
        activeSlots[id].SetSelectedPendingItem();
    }
    /// <summary>
    /// Sets the texture based on the current hotbar slot being selected.
    /// </summary>
    /// <param name="id">UI hotbar slot</param>
    /// <param name="texture">The Texture of the item</param>
    public void SetTexture(int id, Texture texture)
    {
        activeSlots[id].SetTexture(texture);
    }
    public void SetTexture(int id, Texture texture, string amount)
    {
        activeSlots[id].SetTexture(texture);
        activeSlots[id].SetCount(amount);
    }
    public void SetCount(int id, string amount)
    {
        activeSlots[id].SetCount(amount);
    }
    /// <summary>
    /// Remove Pending/Click Item Selection Color
    /// </summary>
    public void RemoveSelectItem(int hotbar)
    {
        foreach (UiDisplay display in activeSlots)
        {
            display.RemovePendingItem(hotbar);
            display.RemoveClickItem(hotbar);
        }
    }
    #endregion
    #region Refresh
    /// <summary>
    /// Refersh all of the UI icon slots
    /// </summary>
    /// <param name="player"></param>
    public void RefreshHotbarOnly(Player player)
    {
        for (int i = 0; i < HotbarSize; i++)
        {
            InventoryItem item = player.GetInventoryItem(i);
            if (item.MarkedForDeletion == true)
            {
                player.DeleteItem(i);
                ClearTexture(i);
                continue;
            }
            if (item != null && !item.GetIsEmptyItem())
            {
                SetTexture(i, item.GetTheTexture(),item.GetHeldAmountString());
            }
            else
            {
                ClearTexture(i);
            }
        }
    }
    /// <summary>
    /// Refresh the entire inventory
    /// </summary>
    /// <param name="player"></param>
    public void RefreshFullInventory(Player player)
    {
        for (int i = 0; i < HotbarSize; i++)
        {
            InventoryItem item = player.GetInventoryItem(i);
            if (item.MarkedForDeletion == true)
            {
                player.DeleteItem(i);
                ClearTexture(i);
                continue;
            }
            if (item != null && !item.GetIsEmptyItem())
            {
                Debug.Log($"Held amount: {item.GetHeldAmountString()}");
                SetTexture(i, item.GetTheTexture(),item.GetHeldAmountString());
            }
            else
            {
                ClearTexture(i);
            }
        }
        for (int i = HotbarSize; i < InventorySize; i++)
        {
            InventoryItem item = player.GetInventoryItem(i);
            if (item.MarkedForDeletion == true)
            {
                player.DeleteItem(i);
                ClearTexture(i);
                continue;
            }
            if (item != null && !item.GetIsEmptyItem())
            {
                SetTexture(i, item.GetTheTexture(),item.GetHeldAmountString());
            }
            else
            {
                ClearTexture(i);
            }
        }
        //SetExtraButtons(player.GetInventory());
    }
    #endregion
    #region Full Inventory
    /// <summary>
    /// Toggle the Inventory in 1 method.
    /// </summary>
    public void InventoryToggle()
    {
        Debug.Log($"InventoryToggle called. Current state: {IsInventoryOpen}");
        IsInventoryOpen = !IsInventoryOpen;
        if (IsInventoryOpen)
        {
            Debug.Log("Opening inventory");
            OpenEverthing();
        }
        else
        {
            Debug.Log("Closing inventory");
            CloseInventoryOpenHotbar();
        }
    }
    /// <summary>
    /// Open inventory window.
    /// </summary>
    public void OpenEverthing()
    {
        foreach (var slot in activeSlots)
        {
            slot.SetClickable(true);
            slot.ShowNonHotbar();
        }
    }
    /// <summary>
    /// Close inventory window.
    /// </summary>
    public void CloseInventoryOpenHotbar()
    {
        foreach (var slot in activeSlots)
        {
            slot.SetClickableHotbaronly();
            slot.HideNonHotbar();
        }
    }
    /// <summary>
    /// Gets if your entire inventoryis open
    /// </summary>
    /// <returns><see cref="IsInventoryOpen"/></returns>
    public bool GetFullInventoryOpen()
    {
        return IsInventoryOpen;
    }
    /// <summary>
    /// Close everything.
    /// </summary>
    public void CloseEverything()
    {
        foreach (var slot in activeSlots)
        {
            slot.SetClickable(false);
            slot.HideEverything();
        }
    }
    #endregion
    #region Show Certain Items
    #endregion
    
    #region Extra Menus
    /// <summary>
    /// Searches your entire inventory to determine which items require extra menus based apon <see cref="Abilities"/>
    /// </summary>
    /// <param name="inventory">Your entire Inventory</param>
    public void SetExtraButtons(List<InventoryItem> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            activeSlots[i].SetupExtraMenu(inventory[i].GetItem<Item>().ExtraDataButton);
        }
    }
    /// <summary>
    /// Set the extra buttons if an <see cref="Abilities"/> requires it. Searches for only 1 slot in your inventory
    /// </summary>
    /// <param name="singleInvItem">the Item</param>
    /// <param name="slot">The slot in your invenotry</param>
    public void SetExtraButtons(InventoryItem singleInvItem, int slot)
    {
        activeSlots[slot].SetupExtraMenu(singleInvItem.GetItem<Item>().ExtraDataButton);
    }
    public bool GetSelfButton(int slot)
    {
        return activeSlots[slot].GetSelfButton();
    }
    public int[] GetExtraDataSlider(int hotbarSlot)
    {
        return activeSlots[hotbarSlot].GetExtraMenuSliders();
    }
    /// <summary>
    /// Get what the slider is set to in the extra menu if required.
    /// </summary>
    /// <param name="hotbarSlot">Your current hotbar slot.</param>
    /// <returns>What Int the slider is currently set to.</returns>
    public int GetExtraDataRoleSlider(int hotbarSlot)
    {
        return activeSlots[hotbarSlot].GetExtraMenuRoleSlider();
    }
    public bool[] GetExtraDataButton(int hotbarSlot)
    {
        return activeSlots[hotbarSlot].GetExtraMenuButtons();
    }
    /// <summary>
    /// Exclude your role from the <see cref="ExtraDataType.SliderRoles"/> when selecting through a list of roles.
    /// </summary>
    /// <param name="role"></param>
    public void SetSliderSelfRole(int role)
    {
        foreach (UiDisplay display in activeSlots)
        {
            display.SetSliderSelfRole(role);
        }
    }
    #endregion
}