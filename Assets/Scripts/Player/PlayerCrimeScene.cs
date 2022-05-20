using UnityEngine;
using Inputs;

public class PlayerCrimeScene : MonoBehaviour {
    [Header("Interact")]
    [SerializeField] private Transform inspecInteractable;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float maxInteractableDistance = 8f;
    [SerializeField] private string currentInteractableName;
    [SerializeField] private Interactable currentInteraction;

    [SerializeField] private Camera cam;
    private bool interacting;
    Controls actions;

    private void Start() {
        if(actions == null) actions = new Controls();
        actions.Enable();

        actions.Player.Interact.performed += ctx => GetInteractionInput();
    }

    private void GetInteractionInput() => UseInteraction();

    private void FixedUpdate() {
        CheckForInteractable();
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
}