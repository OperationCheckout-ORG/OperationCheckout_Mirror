using System.Collections.Generic;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    [SerializeField] private List<GameObject> _items = new List<GameObject>();
    public Transform[] pickupParents;
    public int currentParentIndex = 0;
    public Vector3[] itemPositions;

    private void Start()
    {
        // Initialize the item positions array for each pickup parent
        itemPositions = new Vector3[pickupParents.Length * pickupParents[0].childCount];
        int index = 0;
        foreach (Transform parent in pickupParents)
        {
            foreach (Transform slot in parent)
            {
                itemPositions[index] = slot.position;
                index++;
            }
        }

    }

    public void AddItem(GameObject item)
    {
        _items.Add(item);

        item.transform.parent = pickupParents[currentParentIndex].transform;
        item.transform.position = pickupParents[currentParentIndex].position;
        item.transform.localRotation = Quaternion.identity;

        Rigidbody pickupRigidbody = item.GetComponent<Rigidbody>();
        if (pickupRigidbody != null)
        {
            Destroy(pickupRigidbody);
        }
        Collider[] pickupColliders = item.GetComponents<Collider>();
        foreach (Collider collider in pickupColliders)
        {
            Destroy(collider);
        }

        // Increment the current parent index
        currentParentIndex++;
        if (currentParentIndex >= pickupParents.Length)
        {
            currentParentIndex = 0;
        }
    }




    public void Checkout()
    {
        ScoreManager.Instance.AddScore(_items.Count);
        foreach (GameObject item in _items)
        {
            Destroy(item);
        }
        _items.Clear();
        currentParentIndex = 0;
    }
}