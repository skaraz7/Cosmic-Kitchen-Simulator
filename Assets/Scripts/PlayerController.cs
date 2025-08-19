using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Cámara")]
    public float mouseSensitivity = 4f;

    CharacterController controller;
    Transform cam;
    float xRot = 0f;
    float yVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        cam = GetComponentInChildren<Camera>()?.transform;
        if (cam == null)
        {
            var camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.transform.SetParent(transform);
            camObj.AddComponent<Camera>();
            camObj.transform.localPosition = new Vector3(0, 1.6f, 0);
            cam = camObj.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Mouse look (eje X/Y) ---
        float rawMouseX = Input.GetAxis("Mouse X");
        float rawMouseY = Input.GetAxis("Mouse Y");
        
        // Clamp input to prevent overflow
        rawMouseX = Mathf.Clamp(rawMouseX, -10f, 10f);
        rawMouseY = Mathf.Clamp(rawMouseY, -10f, 10f);
        
        float mouseX = rawMouseX * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = rawMouseY * mouseSensitivity * 100f * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -85f, 85f);
        
        if (cam != null)
        {
            cam.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        }
        
        transform.Rotate(Vector3.up * mouseX);

        // --- Movimiento WASD ---
        float horizontal = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        float vertical = Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 1f);
        Vector3 move = (transform.right * horizontal + transform.forward * vertical) * moveSpeed;

        // --- Gravedad / salto ---
        if (controller.isGrounded)
        {
            yVelocity = -2f;
            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        yVelocity += gravity * Time.deltaTime;

        controller.Move((move + Vector3.up * yVelocity) * Time.deltaTime);
    }
}
