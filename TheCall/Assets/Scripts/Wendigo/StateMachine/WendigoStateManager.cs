using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using UnityEngine.Animations;
using System.Runtime.InteropServices.WindowsRuntime;

public class WendigoStateManager : MonoBehaviour
{
    // Private Variables & References
    [SerializeField]
    NavMeshAgent nma; // Reference to the Nav Mesh Agent.
    [SerializeField]
    GameObject Wendigo; // Reference to the itself.
    WendigoRoamingState roamingState = new WendigoRoamingState(); // Wendigo's ROAMING state script.
    WendigoChaseState chaseState = new WendigoChaseState(); // Wendigo's CHASING state script.
    WendigoJumpscareState jumpscareState = new WendigoJumpscareState(); // Wendigo's JUMPSCARE event handling script.
    BaseState currentState; // The Wendigo's current state.
    bool inView = false; // If the target is within view.

    // Public Variables & References
    public HideController hidingController;
    public Teleportation teleportHandler; // Reference to the 'Teleportation' script.
    public PlayerController pController; // Reference to the PlayerController script.
    public GameObject playerRef; // Reference to the player.
    public GameObject playerCamera; // Reference to the players camera.
    public GameObject behindPlayerSpawn; // Reference to the position behind the player the wendigo can spawn at.
    public GameObject RaycastOrigin;
    public bool teleportTimerRunning = false;
    public bool teleportTimerFinished = false;
    public bool timerRunning = false; // If a timer is running.
    public bool timerFinished = false; // If a timer is finished.
    public bool goBehindPlayer = true; // If the Wendigo can move behind the player.
    public bool isBehindPlayer = false; // If the Wendigo is currently behind the player.
    public bool teleportAway = false; // If the Wendigo is ready to teleport away.
    public float roamSpeed = 4.5f;

    // Animation Parameters & Controllers
    public Animator stateAnimator;
    public string idleParam = "NULL";
    public string walkingParam = "NULL";
    public string chaseParam = "NULL";
    public string killParam = "NULL";
    public string howlParam = "NULL";


    private void Start()
    {
        nma.enabled = true;
        // Setup
        {
            // Set NavMeshAgent references
            teleportHandler.nma = nma;
            roamingState.nma = nma;
            chaseState.nma = nma;
            jumpscareState.nma = nma;

            roamingState.Wendigo = Wendigo; // Set local Wendigo reference in the ROAMING state.
            pController = playerRef.GetComponent<PlayerController>(); // Set PlayerController Reference.
            hidingController = playerRef.GetComponent<HideController>();
        }

        currentState = roamingState; // Set the starting state for the Wendigo.
        currentState.EnterState(this); // Begin running behaviour for the current state.
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameOver == false)
        {
            CheckPLAYERView();
            currentState.UpdateState(this); // Update behaviour for the current state each frame.
        }
        else
        {
        }    
    }

    public void StartChasing() // Start the Wendigo's CHASING state.
    {
        if (GameManager.Instance.GameOver == false)
        {
            if (teleportHandler.hasTeleported == true)
            {
                teleportHandler.hasTeleported = false;
            }
            GameManager.Instance.finishedChasing = false;
            GameManager.Instance.wendigoRoaming = false;
            GameManager.Instance.wendigoChasing = true;
            currentState = chaseState; // Update currentState to chaseState.
            
            currentState.EnterState(this); // Start behaviour for the current state.
        }
    }

    public void StartRoaming() // Start the Wendigo's ROAMING state.
    {
        if (GameManager.Instance.GameOver == false)
        {
            GameManager.Instance.finishedChasing = true;
            if (GameManager.Instance.wendigoChasing == true)
            {
                GameManager.Instance.wendigoChasing = false;
            }
            currentState = roamingState; // Update currentState to roamingState.
            stateAnimator.ResetTrigger(chaseParam);
            stateAnimator.SetTrigger(walkingParam);
            currentState.EnterState(this); // Start behaviour for the current state.
        }
    }


    public void ControlTimer() // Function to control the activation of the 'ChaseTimer' Coroutine.
    {
        if (GameManager.Instance.GameOver == false)
        {
            if (timerRunning == false) // If the ChaseTimer is not currently running
            {
                StartCoroutine(ChaseTimer()); // Start the ChaseTimer.
                return;
            }
            else // Otherwise, if the ChaseTimer is already running
            {
                return; // Do nothing.
            }
        }
    }

    public void TryTeleport(WendigoStateManager wsm)
    {
        if (teleportTimerRunning == false)
        {
            int tpChance = 0;
            tpChance = Random.Range(0, 7);
            Debug.Log("Result: " + tpChance);
            if (tpChance <= 2)
            {
                StartCoroutine(TeleportTimer());
                if (teleportTimerFinished == true)
                {
                }
            }
        }
    }

    public IEnumerator WatchTimer() // Timer for how long the player can look at the Wendigo before it disappears when it has spawned behind them.
    {
        timerRunning = true; // Set 'timerRunning' to true.
        timerFinished = false; // Set 'timerFinished' to false.
        yield return new WaitForSeconds(0.5f); // Wait for half a second.
        timerRunning = false; // Set 'timerRunning' to false.
        timerFinished = true; // Set 'timerFinished' to true.
        teleportAway = true; // Set 'teleportAway' to true.
        StopCoroutine(WatchTimer());
    }

    public IEnumerator TeleportTimer()
    {
        teleportTimerRunning = true;
        teleportTimerFinished = false;
        yield return new WaitForSeconds(5f);
        Debug.Log("Timer is finished");
        teleportTimerRunning = false;
        teleportTimerFinished = true;
        teleportHandler.TeleportToLocation(this);
        StopCoroutine(TeleportTimer());
        
    }

    public IEnumerator ChaseTimer() // Timer to check whether the player has gotten far enough away from the Wendigo CHASING them for it to go back to ROAMING.
    {
        if (GameManager.Instance.GameOver == false && GameManager.Instance.GamePaused == false)
        {
            timerRunning = true; // Set 'timerRunning' to true.
            timerFinished = false; // Set 'timerFinished' to false.
            if (hidingController.isHidden == true) // If the player is not within the Wendigo's view
            {
                timerFinished = true; // Set 'timerFinished' to true.
                timerRunning = false; // Set 'timerRunning' to false.
                StartRoaming(); // Start ROAMING.
                StopCoroutine(ChaseTimer());
            }
            yield return new WaitForSecondsRealtime(3f); // Wait for three(3) seconds.
            CheckView();
            //Vector3 rayDirection = playerRef.transform.position - transform.position;
            if (inView == false)
            {
                StartRoaming();
                //StopCoroutine(ChaseTimer());
            }
            if (inView == true && hidingController.isHidden == false)
            {
                GameManager.Instance.finishedChasing = false;
            }
            timerFinished = true; // Set 'timerFinished' to true.
            timerRunning = false; // Set 'timerRunning' to false.
            StopCoroutine(ChaseTimer());
        }
    }

    public void CheckView() // Function to check if the player is currently within the Wendigo's view.
    {
        if (GameManager.Instance.GameOver == false && GameManager.Instance.GamePaused == false)
        {
            RaycastHit hit;

            Vector3 rayDirection = playerRef.transform.position - transform.position; // Calculate the direction the player is in.
            if ((Vector3.Angle(rayDirection, transform.forward)) < 50)
            {
                if ((Vector3.Angle(rayDirection, transform.forward)) < 50) // Is player within field of view
                {
                    if (Physics.Raycast(transform.position, rayDirection, out hit, 90)) // If the player is close enough to the wendigo
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            inView = true; // Set 'inView' to true.
                            return;
                        }
                        else
                        {
                            inView = false;
                            return;
                        }
                    }
                }
            }
            return;
        }
    }

    public void CheckPLAYERView()
    {
        if (GameManager.Instance.GameOver == false && GameManager.Instance.GamePaused == false)
        {
            RaycastHit hit;

            Vector3 rayDirection = transform.position - playerRef.transform.position; // Calculate the direction the player is in.
            if ((Vector3.Angle(rayDirection, playerCamera.transform.forward)) < 20)
            {
                if ((Vector3.Angle(rayDirection, playerCamera.transform.forward)) < 20) // Is player within field of view
                {
                    if (Physics.Raycast(playerRef.transform.position, rayDirection, out hit, 30)) // If the player is close enough to the wendigo
                    {
                        if (hit.collider.gameObject.tag == "Wendigo" || hit.collider.gameObject.tag == "wendigo")
                        {
                            GameManager.Instance.outOfPlayerView = false;
                        }
                    }
                    else // Otherwise
                    {
                        GameManager.Instance.outOfPlayerView = true;
                    }

                }
            }
            return;
        }
    }

    // Game Over Checking.
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GameOver == false && GameManager.Instance.GamePaused == false)
        {
            if (currentState == roamingState || currentState == chaseState) // If the Wendigo is currently ROAMING or CHASING
            {
                if (hidingController.isHiding == false && hidingController.isHidden == false)
                {
                    if (other.tag == "Player") // If the triggering object is the player
                    {
                        currentState = roamingState; // Update currentState to roamingState.
                        currentState.EnterState(this); // Start behaviour for the current state.
                        pController.enabled = false; // Disable the PlayerController.
                        GameManager.Instance.DoGameOver(Wendigo, playerRef); // Run GameOver behavour from the GameManager.
                    }

                    if (currentState == roamingState) // If the Wendigo is currently ROAMING
                    {
                        if (other.tag != "Player" && other.tag != "Ground") // If the triggering object is not the player or the ground
                        {
                            if (roamingState.moving == true) // Uf the 'moving' bool in the ROAMING state is currently true
                            {
                                roamingState.moving = false; // Set the 'moving' bool in the ROAMING state to false.
                                roamingState.moveAmount = 0; // Set the 'moveAmount' int in the ROAMING state to 0.
                            }
                        }
                    }
                }

            }
        }
    }
}
