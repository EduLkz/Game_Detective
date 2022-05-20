using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Inputs;

public class OfficeScreenManager : MonoBehaviour {


    [SerializeField] private Camera screenCamera;
    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private Canvas pcCanvas;

    private Controls actions;
    Vector3 mousePos;
    Vector2 _cursorPos;

    private void Start() {
        if(actions == null) actions = new Controls();
        actions.Enable();

        actions.Computer.Cursor.performed += ctx => GetMousePos(ctx.ReadValue<Vector2>());
    }

    void GetMousePos(Vector3 _value) => mousePos = _value;//= new Vector3(_value.x, _value.y, 0);

    private void LateUpdate() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(pcCanvas.transform as RectTransform, mousePos, pcCanvas.worldCamera, out _cursorPos);
        cursorImage.transform.localPosition = _cursorPos * 1.15f;
    }
}