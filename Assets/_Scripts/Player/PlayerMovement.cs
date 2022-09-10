using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    //this is the camera trasform. It's used to determine where 'forward' is, checking the player's rotation will not work since the game object itself is not rotating.
    //That's because it caused some collision issues like: the player starts spinning uncontrollably after a collision with rigidbodies.
    public Transform forward;

    public float speed = 12f;
    public float jumpForce = 3f;

    //sprint
    public float sprintMultiplier;
    private bool isRunning;
    //Stamina
    public float maxStamina, stamina;
    public float staminaDelay, staminaDelayCounter;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    //Stops movement when set to false
    public bool movementLock;

    //player GameObject singleton
    public static PlayerMovement instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            print("Multiple player instances occured. Deleting the old one...");
            Destroy(instance.gameObject);
            instance = this;
        }
    }
    private void Update()
    {
        if (!movementLock)
            Crouching();
        WalkingAndJumping();
    }

    void WalkingAndJumping()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = forward.right * x + forward.forward * z;
        move.y = 0;


        if (isGrounded)
        {
            rb.velocity = movementLock ? rb.velocity : move * speed + new Vector3(0, rb.velocity.y, 0);

            if(Input.GetButtonDown("Jump") && !movementLock)
                rb.AddForce(new Vector3(0, jumpForce));
        }

        //Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && x + z != 0 && staminaDelayCounter <= 0 && !isRunning)
        {
            speed *= sprintMultiplier;
            isRunning = true;
        }
        if (isRunning && x + z == 0)
        {
            speed /= sprintMultiplier;
            isRunning = false;
        }
        if (isRunning && stamina <= 0)
        {
            speed /= sprintMultiplier;
            isRunning = false;
            staminaDelayCounter = staminaDelay;
        }

        //Stamina
        if (isRunning)
            stamina -= Time.deltaTime;
        if (stamina < maxStamina && !isRunning)
            stamina += Time.deltaTime / 2;
        if (staminaDelayCounter > 0)
            staminaDelayCounter -= Time.deltaTime;
    }

    void Crouching()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Crouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StandUp();
    }

    private void Crouch()
    {
        print("crouching");
    }

    private void StandUp()
    {
        print("standing up");
    }
}