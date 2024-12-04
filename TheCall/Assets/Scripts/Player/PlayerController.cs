using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Enable/Disable player input.")]
    public bool canMove = true;

    public bool takingPhoto = false;

    [SerializeField]
    private GameObject cameraFlash;

    [Tooltip("Reference to the player's camera.")]
    public Transform playerCamera;

    [Tooltip("Reference to the player's objectives.")]
    public PlayerObjectives playerObjectives;

    [SerializeField, Tooltip("How fast the player can walk.")]
    private float walkSpeed = 8.0f;
    [SerializeField, Tooltip("How fast the player can run.")]
    private float runSpeed = 16.0f;

    [SerializeField, Tooltip("How long till the player can take another photo. (In seconds)")]
    private float cameraFlashCooldown = 2.0f;

    //public float CurrentMovingSpeed
    //{
    //    get => m_currentMovingSpeed;
    //}
    //private float m_currentMovingSpeed = 0.0f;

    private float m_animMovingSpeed = 0.0f;

    private CharacterController m_controller; // Character Controller component.

    private Vector2 m_input; // Moving input.
    private bool m_isRunning = false; // Run button input.
    private bool m_photoInput = false; // Taking photo input.

    private Vector3 m_velocity = Vector3.zero; // Velocity (Gravity)
    private float m_moveSpeed = 0.0f; // Move speed.

    private float m_cooldown = 0.0f; // The cooldown clock to stop players from spamming photos.

    private void Start()
    {
        //GameManager.Instance.GamePaused = true;
        m_controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (canMove == true)
        {
            DoPlayerMovement();
            CalculateAnimatorParameters();
            DoGravity();
        }

        // Rotate player body towards camera direction.
        transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, playerCamera.localEulerAngles.y, transform.localEulerAngles.z);

        // Take photo.
        if (m_photoInput)
        {
            m_photoInput = false;
            if (m_cooldown <= 0.0f)
            {
                TakePhoto();
                m_cooldown = cameraFlashCooldown;
            }
        }
        if (m_cooldown > 0.0f) // Do cooldown.
        {
            m_cooldown -= Time.deltaTime;
        }
        else if (m_cooldown <= 0.0f) // Clamp cooldown.
        {
            m_cooldown = 0.0f;
        }
    }

    private void DoPlayerMovement()
    {
        // Change move speed whether running or not.
        if (m_isRunning)
            m_moveSpeed = runSpeed;
        else
            m_moveSpeed = walkSpeed;

        // Player movement.
        Vector3 move = Vector3.zero;
        if (canMove) // Move player only if player input is enabled.
        {
            move = m_input.x * playerCamera.right + m_input.y * playerCamera.forward; // Get movement direction relative to camera direction.
            move.y = 0;
        }
        m_controller.Move(move.normalized * m_moveSpeed * Time.deltaTime); // Apply player movement.
    }

    private void DoGravity()
    {
        if (m_controller.isGrounded) // Is player touching ground.
        {
            m_velocity.y = -1.0f; // Push player out of ground.
        }
        else
        {
            m_velocity.y += Physics.gravity.y * Time.deltaTime; // Gravity.
        }
        m_controller.Move(m_velocity * Time.deltaTime); // Apply player velocity.
    }

    const float bobTransitionSpeed = 8.0f;
    float maxAnimSpeed = 1.0f;
    private void CalculateAnimatorParameters()
    {
        float currentSpeed = m_controller.velocity.normalized.magnitude;
        if (currentSpeed > 0.0f)
        {
            if (m_isRunning)
            {
                maxAnimSpeed = 2.0f;
            }
            else
            {
                maxAnimSpeed = 1.0f;
            }

            m_animMovingSpeed += Time.deltaTime * bobTransitionSpeed;
            m_animMovingSpeed = Mathf.Clamp(m_animMovingSpeed, 0.0f, maxAnimSpeed);
        }
        else if (currentSpeed < 1f)
        {
            m_animMovingSpeed -= Time.deltaTime * bobTransitionSpeed;
            m_animMovingSpeed = Mathf.Clamp(m_animMovingSpeed, 0.0f, maxAnimSpeed);
        }

        if (m_animMovingSpeed <= 0.0f)
        {
            maxAnimSpeed = 1.0f;
        }

        playerCamera.GetComponent<Animator>().SetFloat("BobBlend", m_animMovingSpeed);
    }

    // Input System messages.
    private void OnMove(InputValue value)
    {
        m_input = value.Get<Vector2>(); // Get movement input.
    }

    private void OnRun(InputValue value)
    {
        m_isRunning = value.isPressed; // Is running button pressed.
    }

    private void OnPhoto(InputValue value)
    {
        m_photoInput = value.Get<float>() >= 0.5f; // Is photo button pressed.
    }

    private void OnInteract(InputValue value)
    {
        // Interactables
        var interactables = UnityEngine.Object.FindObjectsOfType<Interaction>();
        for (int i = 0; i < interactables.Length; i++)
        {
            var interactable = interactables[i];
            interactable.Input = true;
            if (interactable.Interactable == true)
                interactable.Interacted = true;
            else
                interactable.Interacted = false;
        }
    }

    private void OnPause(InputValue value)
    {
        if (GameManager.Instance.GamePaused == false)
        {
            GameManager.Instance.GamePaused = true;
        }
    }

    // WENDIGO TESTING ONLY
    private void TakePhoto()
    {
        StartCoroutine(CameraTakePhoto());
    }

    private IEnumerator CameraTakePhoto()
    {
        takingPhoto = true;
        Debug.Log("Taking Photo!!");
        cameraFlash.GetComponent<Animator>().SetTrigger("Flash");
        yield return new WaitForNextFrameUnit();
        takingPhoto = false;
        StopCoroutine(CameraTakePhoto());
    }
}
