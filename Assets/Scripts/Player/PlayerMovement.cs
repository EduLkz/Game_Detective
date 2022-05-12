using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inputs;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float runSpeed = 7.5f;

    [Header("Mouse Look")]
    [SerializeField] private float lookSensitivity = 1;
    [SerializeField] private float lookSmooth = .25f;
    [SerializeField] private float minLookAngle = -60;
    [SerializeField] private float maxLookAngle = 75;
    [SerializeField] private bool invertY = false;

    [Header("Interact")]
    [SerializeField] private Transform inspecInteractable;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float maxInteractableDistance = 8f;
    [SerializeField] private string currentInteractableName;
    [SerializeField] private Interactable currentInteraction;
    
    private bool interacting;

    Vector2 mouseInput;
    Vector2 movementInput;

    Vector2 mouseLook;
    Vector2 smoothLook;

    bool running;

    Camera cam;
    Rigidbody body;
    Controls actions;    

    private void OnValidate() {
        if(minLookAngle == maxLookAngle) {
            minLookAngle = maxLookAngle - 10;
        }
    }

    private void Start() {
        body = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();


        if(actions == null) actions = new Controls();
        actions.Enable();

        actions.Player.Movement.performed += ctx => GetMovementInput(ctx.ReadValue<Vector2>());
        actions.Player.Movement.canceled += ctx => GetMovementInput(Vector2.zero);

        actions.Player.Look.performed += ctx => GetMouseInput(ctx.ReadValue<Vector2>());
        actions.Player.Look.canceled += ctx => GetMouseInput(Vector2.zero);

        actions.Player.Interact.performed += ctx => GetInteractionInput();
    }

    #region Input Actions

    private void GetMovementInput(Vector2 value) => movementInput = value;

    private void GetMouseInput(Vector2 value) => mouseInput = value;

    private void GetInteractionInput() => UseInteraction();
    #endregion

    #region Updates
    private void Update() {
        Look();
    }

    private void FixedUpdate() {
        Movement();
        CheckForInteractable();
    }
    #endregion

    #region Private methods

    private void Movement() {
        Vector3 _move = movementInput.y * transform.forward + movementInput.x * transform.right;
        _move *= (running) ? runSpeed : movementSpeed;

        _move.y = body.velocity.y;

        body.velocity = _move;
    }

    private void Look() {
        Vector2 _mlS = mouseInput * lookSensitivity;

        if(lookSmooth > 0.05) {
            smoothLook.x = _mlS.x * lookSmooth;
            smoothLook.y = _mlS.y * lookSmooth;
        } else {
            smoothLook = _mlS;
        }

        mouseLook += smoothLook;

        mouseLook.y = Mathf.Clamp(mouseLook.y, minLookAngle, maxLookAngle);

        cam.transform.localRotation = Quaternion.AngleAxis(mouseLook.y * ((invertY)? 1 : -1), Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
    }

    private void CheckForInteractable() {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxInteractableDistance, interactableMask)) {
            currentInteractableName = hit.transform.name;
        } else {
            if(!currentInteractableName.Equals(string.Empty)) {
                currentInteractableName = string.Empty;
            }
        }
    }

    private void UseInteraction() {
        if(interacting) {
            if(currentInteraction != null) {
                currentInteraction.Interact(inspecInteractable);
                currentInteraction = null;
                
            }

            interacting = false;
            return;
        }
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxInteractableDistance, interactableMask)) {
            currentInteraction = hit.transform.GetComponent<Interactable>();
            if(currentInteraction != null) {
                interacting = (currentInteraction.GetInteractableType() == InteractableType.inspect);

                switch(currentInteraction.GetInteractableType()) {
                    case InteractableType.inspect:
                        currentInteraction.Interact(inspecInteractable);
                        break;
                    case InteractableType.callPhoto:
                        currentInteraction.Interact(hit.point, transform.position);
                        break;
                }
            }
        }
    }
    #endregion
}