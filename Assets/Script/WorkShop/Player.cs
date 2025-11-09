using System.Collections.Generic;
using UnityEngine;

// ต้องมั่นใจว่า Player Object มี CapsuleCollider
[RequireComponent(typeof(CapsuleCollider))]
public class Player : Character
{
    [Header("Hand setting")]
    public Transform RightHand;
    public Transform LeftHand;
    public List<Item> inventory = new List<Item>();

    Vector3 _inputDirection;
    bool _isAttacking = false;
    bool _isInteract = false;

    private AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip interactSound;
    public AudioClip healSound;
    public AudioClip damageSound;

   
    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    public bool IsCrouching { get; private set; }

    // --- ส่วนที่เพิ่มเข้ามาสำหรับ Collider ---
    [Header("Crouch Collider Settings")]
    [SerializeField] private float crouchHeight = 1.0f;
    private CapsuleCollider capsuleCollider;
    private float originalHeight;
    private Vector3 originalCenter;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = maxHealth;
        audioSource = GetComponent<AudioSource>();
        
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalHeight = capsuleCollider.height;
        originalCenter = capsuleCollider.center;
        
        IsCrouching = false; // เริ่มต้นด้วยการยืน
    }

    public void FixedUpdate()
    {
        Vector3 moveDirection = _inputDirection;

        if (IsCrouching)
        {
            moveDirection *= crouchSpeedMultiplier;
        }

        Move(moveDirection);
        Turn(_inputDirection);
        Attack(_isAttacking);
        Interact(_isInteract);
    }
    public void Update()
    {
        HandleInput();
    }
    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        _inputDirection = new Vector3(x, 0, y);
        if (Input.GetMouseButtonDown(0))
        {
            _isAttacking = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isInteract = true;
        }

        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            IsCrouching = !IsCrouching;
            animator.SetBool("IsCrouching", IsCrouching);

            UpdateColliderForCrouch();  
        }
    }

    private void UpdateColliderForCrouch()
    {
        if (IsCrouching)
        {
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = new Vector3(originalCenter.x, crouchHeight / 2, originalCenter.z);
        }
        else
        {
            capsuleCollider.height = originalHeight;
            capsuleCollider.center = originalCenter;
        }
    }
    // --------------------------------------

    public void Attack(bool isAttacking)
    {
        if (isAttacking)
        {
            animator.SetTrigger("Attack");
            var e = InFront as Idestoryable;
            if (e != null)
            {
                e.TakeDamage(Damage);
                Debug.Log($"{gameObject.name} attacks for {Damage} damage.");
            }
            _isAttacking = false;
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }
    private void Interact(bool interactable)
    {
        if (interactable)
        {
            IInteractable e = InFront as IInteractable;
            if (e != null)
            {
                e.Interact(this);
            }
            _isInteract = false;
            if (interactSound != null)
            {
                audioSource.PlayOneShot(interactSound);
            }
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        GameManager.instance.UpdateHealthBar(health, maxHealth);
        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }
    public override void Heal(int amount)
    {
        base.Heal(amount);
        GameManager.instance.UpdateHealthBar(health, maxHealth);
        if (healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }
    }
} 