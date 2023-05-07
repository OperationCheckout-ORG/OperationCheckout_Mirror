using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingListGenerator : MonoBehaviour
{
    public ShoppingListItem[] itemDatabase;
    public int numItemsInList;

    public List<ShoppingListItem> GenerateShoppingList()
    {
        List<ShoppingListItem> shoppingList = new List<ShoppingListItem>();

        for (int i = 0; i < numItemsInList; i++)
        {
            int randomIndex = Random.Range(0, itemDatabase.Length);
            ShoppingListItem randomItem = itemDatabase[randomIndex];
            shoppingList.Add(randomItem);
        }

        return shoppingList;
    }
}
