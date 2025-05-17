using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    private CharacterController characterController;
    private Vector3 movementDirection;
    private InputSystem_Actions playerInputSystem;
    [SerializeField] private float speed = 5;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookDirection;
    private float verticalVelocity;

    private Vector2 moveInput;
    private Vector2 aimInput;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInputSystem = new InputSystem_Actions();

        //MOVEMENT
        playerInputSystem.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        playerInputSystem.Player.Move.canceled += context => moveInput = Vector2.zero;

        //AIM
        playerInputSystem.Player.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        playerInputSystem.Player.Aim.canceled += context => aimInput = Vector2.zero;
    }

    private void OnEnable()
    {
        playerInputSystem.Enable();
    }

    private void OnDisable()
    {
        playerInputSystem.Disable();
    }


    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * speed * Time.deltaTime);
        }
    }

    private void ApplyLookingAround()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            Debug.Log(hitInfo.point);
            lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0f;
            lookDirection.Normalize();

            transform.forward = lookDirection;
        }
        else
        {
            verticalVelocity = 0f;
        }

    }

    private void ApplyGravity()
    {

        if (characterController.isGrounded == false)
        {
            verticalVelocity = verticalVelocity - 9.81f * Time.deltaTime;

            movementDirection.y = verticalVelocity;
        }
    }

    void Update()
    {
        ApplyMovement();
        ApplyLookingAround();
    }
}
