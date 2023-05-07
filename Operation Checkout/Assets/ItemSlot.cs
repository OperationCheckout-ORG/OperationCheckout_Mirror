using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int index;
    public Image assignedImage;
    private GameObject item;

    public void AssignItem(GameObject shoppingListItemObject)
    {
        // Set the assigned image to the image component on the shopping list item prefab
        assignedImage.sprite = shoppingListItemObject.GetComponent<Image>().sprite;
    }
    
    
    public bool IsOccupied()
    {
        return item != null;
    }
}