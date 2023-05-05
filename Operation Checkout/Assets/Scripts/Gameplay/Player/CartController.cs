using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CartController : MonoBehaviour
{
    [Header("CORE")]
    public PlayerController _playerController;
    public int playerNumber;
    public Rigidbody rigidBody;
    public bool InCheckoutZone { get; private set; }
    public GameObject noCheckoutMessage;
    [SerializeField] private Animator _animator;
    [SerializeField] private RagdollController _ragdollController;
    private IAbilityController _abilityController;
   
    

    [Header("MOVEMENT & SPEED VARIABLES")] 
    [Space(20)]
    public float moveSpeed = 20f;

    [SerializeField] private float _maxSpeed = 25f;
    [SerializeField] private float _rotationSpeed = 20f;
    public float VelocityMagnitude => rigidBody.velocity.magnitude;
    public Vector3 p1Movement;
    public Vector3 p2Movement;
    private Quaternion _prevRotation;
    [SerializeField] private float _turnAngleThreshold = 60f;
    [SerializeField] private float _turningThreshold = 10f;


    [Header("CART VARIABLES")] 
    [Space(20)] [SerializeField]
    private Transform _frontOfCart;
    [SerializeField] private Vector3 _extents;

    [Header("TRAILS/PARTICLE EFFECTS")] 
    [Space(20)] [SerializeField]
    private TrailRenderer[] _skids;

    [SerializeField] private AudioSource _skidSource;
    [SerializeField] private float _skidThreshold = 1;

    [Header("ABILITY VARIABLES")] 
    [Space(20)]
    public float maxRamMagnitude = 20;
    public float ramCooldown = 5;
    public float ramDuration = 1;
    public float ramSpeedMultiplier = 2;
    public float lastRam = Single.MinValue;
    public Vector3 ramDirection;
    public bool isRamming;
    public bool canRam;

    [Space(10)] 
    public float blastCooldown;
    public float blastRadius;
    public float blastPower;
    public float blastDamage;
    public ParticleSystem blastParticles;
    public float maxBlastDistance;
    public bool canBlast;
    public float lastBlast = Single.MinValue;
    
    [Space(10)]
    public ParticleSystem dashParticles;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    public float dashSpeed = 50f;
    public bool isDashing = false;
    private float dashTime = 0f;
    private float lastDashTime = 0f;


    [Header("PICKUP/CHECKOUT MECHANICS")] 
    [SerializeField] private CartInventory _inventory;


    [Header("LAYER MASKS")] 
    [Space(20)] 
    public LayerMask enemyMask;

    [Header("EVENTS")] 
    [Space(20)] 
    public UnityEvent OnMoveSmoke;
    public UnityEvent OnCartCollision;
    public UnityEvent OnDash;
    public UnityEvent OnPickup;
    public UnityEvent OnCheckout;
    
    [Space(20)]
    
    [Header("INPUT")]
    // Public variables
    public P1InputControls P1InputControls;
    public P2InputControls P2InputControls;
    private Vector2 _p1Input;
    private Vector2 _p2Input;
    public bool isControlledByPlayer = true;
    

    #region Initialization

    public void SetAbilityController(IAbilityController abilityController)
    {
        _abilityController = abilityController;
    }

    private void InitializeInputs()
    {
        P1InputControls = new P1InputControls();
        P2InputControls = new P2InputControls();
    }

    private void Awake()
    {
        InitializeInputs();

        rigidBody = GetComponent<Rigidbody>();
        _inventory = GetComponent<CartInventory>();
        _animator = GetComponentInChildren<Animator>();
        _ragdollController = GetComponentInChildren<RagdollController>();


        SetupVisuals();
    }

   

    private void OnEnable()
    {
        P1InputControls.Enable();
        P2InputControls.Enable();
    }

    #endregion

    #region Physics

    private void FixedUpdate()
    {
        if (isControlledByPlayer)
        {
            if (playerNumber == 1)
            {
                AddRelativeForce(p1Movement);
                if (!isRamming) rigidBody.AddTorque(Vector3.up * _p1Input.x * _rotationSpeed * Time.fixedDeltaTime);
            
                /*if (Mathf.Abs(rigidBody.angularVelocity.y) > _turningThreshold)
                {
                    _ragdollController.ActivateRagdoll();
                }
                else
                {
                    _ragdollController.DeactivateRagdoll();
                }*/
            }

            if (playerNumber == 2)
            {
                AddRelativeForce(p2Movement);
                if (!isRamming) rigidBody.AddTorque(Vector3.up * _p2Input.x * _rotationSpeed * Time.fixedDeltaTime);
            
            
                /*if (Mathf.Abs(rigidBody.angularVelocity.y) > _turningThreshold)
                {
                    _ragdollController.ActivateRagdoll();
                }
                else
                {
                    _ragdollController.DeactivateRagdoll();
                }*/
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag($"CheckoutZone"))
        {
            InCheckoutZone = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (P1InputControls.ShoppingCart.PickUp.WasPerformedThisFrame())
        {
            if (other.CompareTag("Pickup"))
            {
                GameObject pickupObject = other.gameObject;
                pickupObject.transform.parent = transform;
                pickupObject.transform.localPosition = Vector3.up;
                pickupObject.transform.localRotation = Quaternion.identity;
                Rigidbody pickupRigidbody = pickupObject.GetComponent<Rigidbody>();
                if (pickupRigidbody != null)
                {
                    Destroy(pickupRigidbody);
                }

                Collider[] pickupColliders = pickupObject.GetComponents<Collider>();
                foreach (Collider collider in pickupColliders)
                {
                    Destroy(collider);
                }

                _inventory.AddItem(pickupObject);
                OnPickup?.Invoke();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag($"CheckoutZone"))
        {
            InCheckoutZone = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag($"Player1") || other.collider.CompareTag($"Player2") ||
            other.collider.CompareTag($"Shelves") || other.collider.CompareTag($"Walls"))
        {
            OnCartCollision?.Invoke();
        }
    }


    #endregion

    #region Game Logic

    private void Update()
    {
        HandleInput();
        Move();
        HandleSkids();
        HandleVisuals();
        HandleAbilities();
        Checkout();
        HandleGameStateChanged();
        
    }
    

    public void HandleAbilities()
    {
        if (_abilityController != null)
        {
            _abilityController.HandleRam();
            _abilityController.HandleBlast();
        }
    }
    
    public void Checkout()
    {
        if (InCheckoutZone && P1InputControls.ShoppingCart.Checkout.WasPressedThisFrame())
        {
            _inventory.Checkout();
            OnCheckout?.Invoke();
        }
        else if (!InCheckoutZone && P1InputControls.ShoppingCart.Checkout.WasPressedThisFrame())
        {
            StartCoroutine(NoCheckout());
        }
    }

    #endregion
    

    #region Input

    public void HandleInput()
    {
        _p1Input = P1InputControls.ShoppingCart.Movement.ReadValue<Vector2>();
        p1Movement = new Vector3(0, 0, _p1Input.y);

        // Detect if the player wants to dash
        if (playerNumber == 1 && P1InputControls.ShoppingCart.Dash.WasPressedThisFrame() &&
            Time.time - lastDashTime > dashCooldown)
        {
            lastDashTime = Time.time;
            isDashing = true;
            dashTime = 0f;
            rigidBody.velocity += transform.forward * dashSpeed;
            dashParticles.Play();
            OnDash?.Invoke();
        }
        else if (playerNumber == 2 && P2InputControls.ShoppingCart.Dash.WasPressedThisFrame() &&
                 Time.time - lastDashTime > dashCooldown)
        {
            lastDashTime = Time.time;
            isDashing = true;
            dashTime = 0f;
            rigidBody.velocity += transform.forward * dashSpeed;
            dashParticles.Play();

        }

        // Handle dash duration
        if (isDashing)
        {
            dashTime += Time.deltaTime;
            if (dashTime >= dashDuration)
            {
                isDashing = false;
            }
        }

        _p2Input = P2InputControls.ShoppingCart.Movement.ReadValue<Vector2>();
        p2Movement = new Vector3(0, 0, _p2Input.y);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerController.LeaveCart();
        }
    }

    public void HandleGameStateChanged()
    {
        if (P1InputControls.ShoppingCart.Pause.WasPressedThisFrame())
        {
            if (GameManager.Instance.GameState == GameManager.GameStates.Paused)
            {
                GameManager.Instance.GameState = GameManager.GameStates.Playing;
            }
            else if (GameManager.Instance.GameState == GameManager.GameStates.Playing)
            {
                GameManager.Instance.GameState = GameManager.GameStates.Paused;
            }

            GameManager.Instance.CheckStates();
        }

        if (P2InputControls.ShoppingCart.Pause.WasPressedThisFrame())
        {
            if (GameManager.Instance.GameState == GameManager.GameStates.Paused)
            {
                GameManager.Instance.GameState = GameManager.GameStates.Playing;
            }
            else if (GameManager.Instance.GameState == GameManager.GameStates.Playing)
            {
                GameManager.Instance.GameState = GameManager.GameStates.Paused;
            }

            GameManager.Instance.CheckStates();
        }
    }

    #endregion

    #region Movement

    public void Move()
    {
        HandleMaxSpeed();

        // Set the "Speed" parameter of the animator based on the cart's velocity
        float speed = rigidBody.velocity.magnitude;
        _animator.SetFloat("Speed", speed);

        // Set the "isMovingBackward" parameter of the animator based on the direction of movement
        float direction = Mathf.Sign(Vector3.Dot(rigidBody.velocity, transform.forward));
        _animator.SetBool("isMovingBackward", direction == -1);

        // Clamp the "Speed" parameter between 0 and _maxSpeed
        _animator.SetFloat("Speed", Mathf.Clamp(speed, 0f, _maxSpeed));
    }


    private void AddRelativeForce(Vector3 dir)
    {
        rigidBody.AddRelativeForce(dir * moveSpeed * Time.fixedDeltaTime);
        OnMoveSmoke?.Invoke();
    }

    public void HandleMaxSpeed()
    {
        if (rigidBody.velocity.magnitude > _maxSpeed)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, _maxSpeed);
        }
    }

    #endregion

    #region Visuals

    //[Header("VISUALS")] private ChromaticAberration _chrome;

    void SetupVisuals()
    {
        //var volume = FindObjectOfType<Volume>();
        //volume.profile.TryGet(out _chrome);
    }

    void HandleVisuals()
    {
        //_chrome.intensity.value = Mathf.InverseLerp(10, maxRamMagnitude, rigidBody.velocity.magnitude);
    }

    void HandleSkids()
    {
        var angular = Mathf.Abs(rigidBody.angularVelocity.y);
        var point = Mathf.InverseLerp(0, 6, angular);
        _skidSource.volume = point * 0.3f;
        // If angular velocity is greater than the skid treshold then emit the skid.
        var emit = angular > _skidThreshold;
        for (var i = 0; i < 2; i++) _skids[i].emitting = emit;
    }

    public void ToggleSkids(bool PauseSkids)
    {
        if (!_skidSource.isActiveAndEnabled)
        {
            return;
        }

        if (PauseSkids)
        {
            _skidSource.Pause();
        }
        else
        {
            _skidSource.Play();
        }
    }

    #endregion

    #region Gizmo Rendering

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_frontOfCart.position, _extents);
    }

    #endregion

    #region Disable/Enable

    private void OnDisable()
    {
        P1InputControls.Disable();
        P2InputControls.Disable();
    }

    #endregion


    private IEnumerator NoCheckout()
    {
        noCheckoutMessage.SetActive(true);

        yield return new WaitForSeconds(2f);
        
        noCheckoutMessage.SetActive(false);
    }
}