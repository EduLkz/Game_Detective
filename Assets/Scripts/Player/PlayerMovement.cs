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

    [Header("Step info")]
    [SerializeField] private float stepHeight = .15f;
    [SerializeField] private float stepDistance = .1f;
    private Transform camParent;
    private float lastStep;
    private float defaultHeight;
    private float stepDuration = 1.75f;

    private void Step() {

        if(movementInput == Vector2.zero) {
            lastStep = 0;
            camParent.position = Vector3.up * defaultHeight;

            return;
        }

        float _sd = stepDistance;
        if(running) {
            _sd = stepDistance / stepDuration;
        }
        lastStep = Time.time + _sd;



        if((lastStep + _sd) / 2f < Time.time) {
            camParent.localPosition = Vector3.up * (defaultHeight + stepHeight);
        } else {
            camParent.localPosition = Vector3.up * defaultHeight;
        }
    }

    

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
        camParent = cam.transform.parent;
        defaultHeight = camParent.localPosition.y;


        if(actions == null) actions = new Controls();
        actions.Enable();

        actions.Player.Movement.performed += ctx => GetMovementInput(ctx.ReadValue<Vector2>());
        actions.Player.Movement.canceled += ctx => GetMovementInput(Vector2.zero);

        actions.Player.Look.performed += ctx => GetMouseInput(ctx.ReadValue<Vector2>());
        actions.Player.Look.canceled += ctx => GetMouseInput(Vector2.zero);

    }

    #region Input Actions

    private void GetMovementInput(Vector2 value) => movementInput = value;

    private void GetMouseInput(Vector2 value) => mouseInput = value;

    
    #endregion

    #region Updates
    private void Update() {
        Look();
        Step();
    }

    private void FixedUpdate() {
        Movement();
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

    
    #endregion
}