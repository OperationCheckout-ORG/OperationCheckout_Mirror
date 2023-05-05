using System;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public CartController cartController;
    public Animator cartPlayerAnimator;
    public Animator playerAnimator;

    private void Awake()
    {
        cartPlayerAnimator = GetComponent<Animator>();
        playerAnimator = GetComponentInChildren<Animator>();
    }

    public void EnterCart(CartController cart)
    {
        cartController.enabled = true;
        cartController.transform.position = cart.transform.position;
        cartController.transform.rotation = cart.transform.rotation;
        playerAnimator.enabled = false;
        cartPlayerAnimator.enabled = true;
        enabled = false;
        transform.SetParent(cart.transform);
    }

    public void LeaveCart()
    {
        transform.SetParent(null);
        cartController.enabled = false;
        cartController.rigidBody.velocity = Vector3.zero;
        cartPlayerAnimator.enabled = false;
        playerAnimator.enabled = true;
        enabled = true;
    }
}