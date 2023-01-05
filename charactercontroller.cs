using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    public float smoothTime = 0.1f;
    public float turnSmoothTime = 0.1f;

    // Item variables
    public GameObject[] items;
    public int currentItem = 0;
    public int maxItems = 5;
    private bool hasItem = false;

    // Skill variables
    public GameObject[] skills;
    public int currentSkill = 0;
    public int maxSkills = 4;
    private bool hasSkill = false;

    // Weapon switching variables
    public int currentWeapon = 0;
    public GameObject[] weapons;

    // Attack variables
    public float attackRange = 5f;
    public float attackRate = 1f;
    private float nextAttack = 0f;

    // Blocking variables
    public bool isBlocking = false;
    public float blockDuration = 2f;
    private float blockTime = 0f;

    // Character components
    private CharacterController controller;
    private Animator animator;
    private Transform cameraTransform;

    // Movement variables
    private Vector3 moveDirection;
    private Vector3 smoothMoveVelocity;
    private Vector3 smoothRotationVelocity;
    private float smoothRotation;
    private bool isJumping = false;
    private bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get character components
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        // Deactivate all weapons except for the first one
        for (int i = 1; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if character is on the ground
        isGrounded = controller.isGrounded;

        // Get movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Set running animation
        animator.SetBool("isRunning", (horizontal != 0 || vertical != 0));

        // Set movement direction
        moveDirection = new Vector3(horizontal, 0f, vertical);
        moveDirection = transform.TransformDirection(moveDirection);

        // Set movement speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= runSpeed;
            animator.SetBool("isRunning", true);
        }
        else
        {
            moveDirection *= moveSpeed;
            animator.SetBool("isRunning", false);
        }

        // Apply gravity
        if (!isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            animator.SetTrigger("jump");
        }

        // Attack
        if (Input.GetMouseButton(0) && Time.time > nextAttack)
        {
            nextAttack = Time.time + 1f / attackRate;

            // Check if an enemy is in range
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    animator.SetTrigger("attack");
                    hit.collider.GetComponent<Enemy>().TakeDamage(10);
                }
            }
        }

        // Heavy attack
        if (Input.GetMouseButton(1) && Time.time > nextAttack)
        {
            nextAttack = Time.time + 1f / attackRate;

            // Check if an enemy is in range
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    animator.SetTrigger("heavyAttack");
                    hit.collider.GetComponent<Enemy>().TakeDamage(20);
                }
            }
        }

        // Block
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isBlocking = true;
            blockTime += Time.deltaTime;
            animator.SetBool("isBlocking", true);

            // End blocking after a certain duration
            if (blockTime > blockDuration)
            {
                isBlocking = false;
                blockTime = 0f;
                animator.SetBool("isBlocking", false);
            }
        }
        else
        {
            isBlocking = false;
            blockTime = 0f;
            animator.SetBool("isBlocking", false);
        }

        // Use item
        if (Input.GetKeyDown(KeyCode.Q) && hasItem)
        {
            animator.SetTrigger("useItem");
            items[currentItem].GetComponent<Item>().Use();
            hasItem = false;
            currentItem = 0;
        }

        // Use skill
        if (Input.GetKeyDown(KeyCode.E) && hasSkill)
        {
            animator.SetTrigger("useSkill");
            skills[currentSkill].GetComponent<Skill>().Use();
            hasSkill = false;
            currentSkill = 0;
        }

        // Switch weapons
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon++;
            if (currentWeapon >= weapons.Length)
            {
                currentWeapon = 0
            }

            // Deactivate all weapons
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].SetActive(false);
            }

            // Activate current weapon
            weapons[currentWeapon].SetActive(true);
        }

        // Collect item
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Check if an item is in range
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
            {
                if (hit.collider.CompareTag("Item"))
                {
                    if (currentItem < maxItems)
                    {
                        animator.SetTrigger("collectItem");
                        items[currentItem] = hit.collider.gameObject;
                        currentItem++;
                        hasItem = true;
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }

        // Collect skill
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Check if a skill is in range
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
            {
                if (hit.collider.CompareTag("Skill"))
                {
                    if (currentSkill < maxSkills)
                    {
                        animator.SetTrigger("collectSkill");
                        skills[currentSkill] = hit.collider.gameObject;
                        currentSkill++;
                        hasSkill = true;
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }

        // Use item 1
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasItem)
        {
            animator.SetTrigger("useItem");
            items[0].GetComponent<Item>().Use();
            hasItem = false;
            currentItem = 0;
        }

        // Use item 2
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasItem)
        {
            animator.SetTrigger("useItem");
            items[1].GetComponent<Item>().Use();
            hasItem = false;
            currentItem = 1;
        }

        // Use item 3
        if (Input.GetKeyDown(KeyCode.Alpha3) && hasItem)
        {
            animator.SetTrigger("useItem");
            items[2].GetComponent<Item>().Use();
            hasItem = false;
            currentItem = 2;
        }

        // Use item 4
        if (Input.GetKeyDown(KeyCode.Alpha4) && hasItem)
        {
            animator.SetTrigger("useItem");
            items[3].GetComponent<Item>().Use();
            hasItem = false;
            currentItem = 3;
        }

        // Use skill 1
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasSkill)
        {
            animator.SetTrigger("useSkill");
            skills[0].GetComponent<Skill>().Use();
            hasSkill = false;
            currentSkill = 0;
        }

        // Use skill 2
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasSkill)
        {
            animator.SetTrigger("useSkill");
            skills[1].GetComponent<Skill>().Use();
            hasSkill = false;
            currentSkill = 1;
        }

        // Use skill 3
        if (Input.GetKeyDown(KeyCode.Alpha3) && hasSkill)
        {
            animator.SetTrigger("useSkill");
            skills[2].GetComponent<Skill>().Use();
            hasSkill = false;
            currentSkill = 2;
        }

        // Use skill 4
        if (Input.GetKeyDown(KeyCode.Alpha4) && hasSkill)
        {
            animator.SetTrigger("useSkill");
            skills[3].GetComponent<Skill>().Use();
            hasSkill = false;
            currentSkill = 3;
        }

        // Smooth movement
        controller.Move(moveDirection * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, controller.transform.position, ref smoothMoveVelocity, smoothTime);

        // Smooth rotation
        float targetRotation = cameraTransform.eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothRotationVelocity, turnSmoothTime);
    }
}
