using UnityEngine;

[CreateAssetMenu(fileName = "New Shopping List Item", menuName = "Shopping List Item")]
public class ShoppingListItem : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public float itemPrice;
    public string itemCategory;
}