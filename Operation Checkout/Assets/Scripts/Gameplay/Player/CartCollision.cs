using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CartCollision : MonoBehaviour
{
    public UnityEvent OnCartCollision;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag($"Player1") || other.collider.CompareTag($"Player2"))
        {
            OnCartCollision?.Invoke();
        }
    }
}
