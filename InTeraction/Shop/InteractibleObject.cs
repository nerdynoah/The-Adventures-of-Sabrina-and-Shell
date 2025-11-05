using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using BaseCharacter.Items;

public class BuyableObject : MonoBehaviour
{
    public string weaponName;
    private int stock = 5000;
    [SerializeField] TMP_Text desc;
    [SerializeField] Material material;
    [SerializeField] MeshRenderer meshRenderer;
    private InventoryItem Temp = new(0);

    public void Start()
    {
        Temp = AllLibary.ItemLibary.SearchLibaryForTemplete(weaponName);
        ShowIcon();
        ShowText();
        
    }
    /// <summary>
    /// Display the products desc via taking data from the <see cref="Weapon"/> itself.
    /// </summary>
    private void ShowText()
    {
        //TODO: Remake this method
    }
    private void ShowIcon()
    {
        meshRenderer.material.mainTexture = Temp.GetTheTexture();
    }
    /// <summary>
    /// Attempts to buy the product at sale. If fails, returns Null.
    /// </summary>
    /// <param name="money">Money spent on product</param>
    /// <returns><see cref="WeaponTemplete"/> or <see cref="null"/></returns>
    public InventoryItem BuyProduct(int money)
    {
        if (money >= Temp.Price)
        {
            if (stock > 0)
            {
                UnlistItem();
                return Temp;
            }
            UnlistItem();
        }
        return null;
    }
    /// <summary>
    /// Unlists the buyable object.
    /// </summary>
    public void UnlistItem()
    {
        if (stock < 0)
        {
            Temp = null;
            desc.text = $"Out of Stock";
        }

    }
}
