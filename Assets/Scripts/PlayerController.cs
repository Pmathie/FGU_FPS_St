using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public Transform playerCamera;
    public float mouseSensitivity =  1.5f;
    private float xRotation;
   
    [Header("Movement")]
    public float standardSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float crouchHeight = 1f;

    public GameObject playerModel;

    private float normalHeight;
    private bool isGrounded;
    private bool isCrouching = false;
    private bool isSprinting = false;


    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>(); //Henter CharacterController komponenten fra GameObjectet som dette script er sat på.
        playerCamera = Camera.main.transform;
        normalHeight = controller.height;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        isGrounded = controller.isGrounded; //controller.isGrounded tjekker om CharacterControlleren er grounded. Denne opdateres hver gang, vi bruger controller.Move() funktionen. Vi gemmer en lokal variabel, så vi kun opdaterer den én gang hver frame 
        Sprint();
        Crouch();
        Movement();
        MouseLook();
        Jump();
    }

    void Movement()
    {
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; //Hvis vi står på jorden, og vores velocity (hastighed) er mindre end 0, så ændrer vi vores velocity til -2f. Dette gør vi så tyngdekraften ikke bare bliver stærkere og stærkere. Samtidig tilføjer vi en lille negativ værdi, så vi altid er "presset" mod jorden.
        }
        float x = Input.GetAxis("Horizontal"); // Henter input fra A/D, venstre/højre piletaster eller venstre/højre på det venstre joystick på en controller
        float z = Input.GetAxis("Vertical");  // Henter  input fra W/S, op/ned piletaster eller op/ned på det venstre joystick på en controller

        Vector3 moveDirection = transform.right * x + transform.forward * z;
        controller.Move(moveDirection * CurrentSpeed() * Time.deltaTime);
        
        // Denne del håndterer tyngdekraften og jumping
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void Sprint() 
    {
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
                isSprinting = true;
            else
                isSprinting = false;
        }
    }
    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            isCrouching = true;
            controller.height = crouchHeight; // Sætter CharacterControllerens højde til vores crouchHeight når vi croucher
        }
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            if (isCrouching && CanStandUp())
            {
                isCrouching = false;
                controller.height = normalHeight; // Sætter CharacterControllerens højde tilbage til normalhøjde når vi stopper med at crouche
            }

        }

        // Opdaterer spillerens model til at matche crouch tilstanden
        float crouchScale = crouchHeight / normalHeight;
       
        if (isCrouching)
        {
            playerModel.transform.localScale = new Vector3(1f, crouchScale, 1f);     
        }
        else
        {
            playerModel.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Begrænser hvor meget vi kan kigge op og ned, så vi ikke kan lave en fuld rotation
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0); // Roterer kameraet op og ned baseret på musens Y bevægelse
        transform.Rotate(Vector3.up * mouseX); // Roterer spilleren venstre og højre baseret på musens X bevægelse
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    float CurrentSpeed()
    {       
        if (isCrouching)
        {
            return crouchSpeed;
        }
        if (isSprinting)
        {
            return sprintSpeed;
        }
        else
        {
            return standardSpeed;
        }
    }
    bool CanStandUp()
    {
        // Hvor meget ekstra højde vi har brug for at stå op
        float extraHeightNeeded = normalHeight - controller.height;

        // Vores nuværende collider bounds - altså størrelsen og positionen på vores collider på vores CharacterController
        Bounds bounds = controller.bounds;

        // Vi laver et origin punkt for raycastet lige over vores hoved
        Vector3 origin = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

        // Vi laver et raycast, hvor vi sender en stråle opad for at tjekke om der er noget over vores hoved
        return !Physics.Raycast(origin, Vector3.up, extraHeightNeeded + 0.05f);
    }
    

}

