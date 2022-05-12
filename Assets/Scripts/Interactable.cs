using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Interactable : MonoBehaviour {

    [SerializeField] private InteractableType interactableType;
    [SerializeField] private bool interacted;
    [SerializeField] private GameObject photoMaker;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start() {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Interact(Transform _transform) {
        InspectInteraction(_transform);
    }

    private void InspectInteraction(Transform _transform) {
        interacted = !interacted;

        if(interacted) {
            transform.SetParent(_transform);
            transform.localPosition = Vector3.zero;
        } else {
            transform.SetParent(null);
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }

    public void Interact(Vector3 _point, Vector3 _pos) {
        CallPhotoInteraction(_point, _pos);
    }

    public InteractableType GetInteractableType() {
        return interactableType;
    }

    private void CallPhotoInteraction(Vector3 _point, Vector3 _pos) {
        if(interacted) { return; }
        interacted = true;

        Quaternion _rot = Quaternion.LookRotation(_pos - transform.position);
        _rot.eulerAngles = new Vector3(-90, _rot.y + 90, 0);
        Vector3 _markerPos = _point + (_pos - transform.position) * .25f;
        _markerPos.y = transform.position.y;
        GameObject _marker = Instantiate(photoMaker, _markerPos, _rot);

        if(CrimeScene.Instance) {
            int _markerNum = CrimeScene.Instance.GetPhotoMakerValue();

            _marker.GetComponentInChildren<TextMeshPro>().text = _markerNum.ToString("00");

            CrimeScene.Instance.AddPhotoMaker(_marker.transform);
        }
    }
}


public enum InteractableType {
    inspect,
    callPhoto
}