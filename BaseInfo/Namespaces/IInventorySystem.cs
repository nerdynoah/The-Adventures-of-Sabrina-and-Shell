using System.Collections.Generic;
using UnityEngine;
namespace BaseCharacter
{
    /// <summary>
    /// Create a inventory system to hold <typeparamref name="T"/>
    /// <list type="bullet">
    /// <item>Used by: <see cref="Items.InventoryItem"/></item>
    /// <item>Used by: <see cref="Items.Quest"/></item>
    /// </list>
    /// <list type="bullet">
    /// <item>Used in: <see cref="InventorySystem"/></item>
    /// <item>Used in: <see cref="QuestSystem"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">Object to be held</typeparam>
    public interface IInventorySystem<T>
    {
        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start">wHere to start search</param>
        /// <returns>If the item was added or not</returns>
        bool AddItem(T item, int start);
        /// <summary>
        /// Delete item at index <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        void DeleteItem(int id);
        void FillNullInventory();
        void FillNullInventory(int start);
        int GetHotbarSlot();
        List<T> GetInventory();
        T GetInventoryItem(int id);
        T GetInventoryItem(string name);
        T GetInventoryItemCurrentHotbar();
        int GetInventorySize();
        int GetPendingItem();
        int GetPendingItemAndClear();
        Texture GetTextureItem(int id);
        void OrderItemsByName();
        void ScrollItem(int amount);
        void SelectItem(int index);
        void SetHotbarSlot(int slot);
        void SetupHotbar(int amount, int defaultHotbarSlot);
        void SwapItem(int index1, int index2);
    }
}