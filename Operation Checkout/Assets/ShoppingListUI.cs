using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingListUI : MonoBehaviour
{
    public ShoppingListGenerator shoppingListGenerator;
    public GameObject shoppingListItemPrefab;
    public Transform shoppingListContainer;

    private List<ShoppingListItem> shoppingList;
    public List<ItemSlot> itemSlots; // define this list in the Inspector and add the desired slots

    private void Start()
    {
        shoppingList = shoppingListGenerator.GenerateShoppingList();
        PopulateShoppingListUI();
    }

    private void PopulateShoppingListUI()
    {
        // Calculate the size and spacing of the shopping list items based on the size of the shopping list container
        float containerWidth = shoppingListContainer.GetComponent<RectTransform>().rect.width;
        float slotWidth = itemSlots[0].GetComponent<RectTransform>().rect.width;
        float itemWidth = Mathf.Min(containerWidth / shoppingList.Count, slotWidth);
        float itemSpacing = itemWidth * 0.2f;

        // Randomize the order of the shopping list
        shoppingList = shoppingList.OrderBy(x => Random.value).ToList();

        // Loop through each item slot and assign the corresponding shopping list item prefab
        for (int i = 0; i < itemSlots.Count; i++)
        {
            // If we've already assigned a shopping list item prefab to this slot, skip it
            if (itemSlots[i].IsOccupied()) continue;

            // Instantiate the shopping list item prefab for the current item
            GameObject shoppingListItemObject = Instantiate(shoppingListItemPrefab, itemSlots[i].transform);

            // Set the position of the shopping list item prefab based on the selected slot position
            RectTransform shoppingListItemRect = shoppingListItemObject.GetComponent<RectTransform>();
            shoppingListItemRect.anchoredPosition = Vector2.zero;

            // Set the size, icon, and name of the shopping list item prefab based on the shopping list item data
            shoppingListItemRect.sizeDelta = new Vector2(itemWidth - itemSpacing, itemWidth - itemSpacing);
            shoppingListItemObject.GetComponent<Image>().sprite = shoppingList[i].itemIcon;
            shoppingListItemObject.GetComponentInChildren<Text>().text = shoppingList[i].itemName;

            // Assign the shopping list item prefab to the selected slot
            itemSlots[i].AssignItem(shoppingListItemObject);
        }
    }





}
