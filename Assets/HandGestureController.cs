using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.InputSystem;

[RequireComponent(typeof(XRHandSkeletonDriver))]
public class HandGestureController : MonoBehaviour
{
    public InputActionAsset handGesturesActions; // Arrastra tu "HandGesturesActions" aquí.
    private InputAction thumbsUpAction;

    // Referencia al hueso del pulgar (asignado en el Inspector).
    public Transform thumbTip; 

    // Rotación para el gesto (ajusta estos valores según necesites).
    private Quaternion thumbsUpRotation = Quaternion.Euler(-90, 0, 0);
    private Quaternion thumbDefaultRotation;

    void Awake()
    {
        // Configura la acción "ThumbsUp" desde tu InputActionAsset.
        thumbsUpAction = handGesturesActions.FindActionMap("HandGestures").FindAction("ThumbsUp");
        thumbDefaultRotation = thumbTip.localRotation;
    }

    void OnEnable()
    {
        thumbsUpAction.performed += OnThumbsUp;
        thumbsUpAction.canceled += OnThumbsDown;
        thumbsUpAction.Enable();
    }

    void OnDisable()
    {
        thumbsUpAction.performed -= OnThumbsUp;
        thumbsUpAction.canceled -= OnThumbsDown;
        thumbsUpAction.Disable();
    }

    private void OnThumbsUp(InputAction.CallbackContext context)
    {
        thumbTip.localRotation = thumbsUpRotation;
    }

    private void OnThumbsDown(InputAction.CallbackContext context)
    {
        thumbTip.localRotation = thumbDefaultRotation;
    }
}