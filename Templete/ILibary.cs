using BaseCharacter;
using BaseCharacter.Items;
using System.Collections.Generic;
using UnityEngine;

public interface ILibary
{
    void AddInventoryItem(InventoryItem item);
    void AddInventoryItem(params InventoryItem[] items);
    void AddQuest(Quest quest);
    InventoryItem GetInventoryItem(string name);
    Quest GetQuest(string name);
    List<Quest> GetQuestList();
    Texture GetTextureHotbar(string name);
    InventoryItem GetInventoryItem(int index);
    List<InventoryItem> GetInventory();
    void SortByName();
}