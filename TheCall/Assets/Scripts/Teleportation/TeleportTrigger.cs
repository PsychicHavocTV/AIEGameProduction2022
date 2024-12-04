using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

public class TeleportTrigger : MonoBehaviour
{
    public Teleportation teleportWendigo;
    public GameObject teleportLocation1;
    public GameObject teleportLocation2;
    public TeleportTrigger duoTrigger;
    public TeleportTrigger[] teleportTriggers;
    public GameObject activeWendigo;
    public GameObject[] teleportTriggersGO;
    public GameObject[] wendigos;
    public NavMeshAgent[] wendigoNMAs;
    public WendigoStateManager[] wendigoControllers;
    public bool triggered = false;
    public bool hasDuo = false;
    public int triggerIndex;
    public int previousIndex;
    private bool changedFromDefault = false;
    private bool readyToTP = true;
    private bool isRunning = false;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public IEnumerator WendigoChasingCheck()
    {
        isRunning = true;
        if (GameManager.Instance.wendigoChasing == false)
        {
            Debug.Log("Wendigo isn't chasing. Teleporting Wendigo.");
            TeleportWendigo();
            isRunning = false;
            StopCoroutine(WendigoChasingCheck());
        }
        else if (GameManager.Instance.wendigoChasing == true)
        {
            if (GameManager.Instance.outOfPlayerView == true)
            if (readyToTP == true)
                readyToTP = false;
            Debug.Log("Wendigo is chasing. Waiting to check again.");
        }
        yield return new WaitForSeconds(0.01f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "player" || other.tag == "Player")
        {
            if (isRunning == false)
            {
                StartCoroutine(WendigoChasingCheck());
            }
            if (triggered == false)
            {
                triggered = true;
            }
            else if (triggered == true)
            {
                triggered = false;
            }
        }
    }

    public void TeleportWendigo()
    {
        readyToTP = true;
        WendigoStateManager activeController = activeWendigo.GetComponent<WendigoStateManager>();
        if (triggered == true)
        {
            Vector3 tpLocation = new Vector3(teleportLocation2.transform.position.x, teleportLocation2.transform.position.y, teleportLocation2.transform.position.z);
            teleportWendigo.TeleportToNext(activeController, tpLocation);
        }
        else if (triggered == false)
        {
            Vector3 tpLocation = new Vector3(teleportLocation1.transform.position.x, teleportLocation1.transform.position.y, teleportLocation1.transform.position.z);
            teleportWendigo.TeleportToNext(activeController, tpLocation);
        }
        readyToTP = false;
        return;
    }

    public void ActivateWendigo()
    {
        
        //if (GameManager.Instance.wendigoChasing == false)
        //{
            //teleportWendigo.TeleportToLocation(activeWendigo.GetComponent<WendigoStateManager>());
            //for (int i = 0; i <= wendigos.Length - 1; i++)
            //{
            //    wendigos[i].SetActive(false);
            //}
            //
            //if (triggered == false || duoTrigger.triggered == false)
            //{
            //    wendigos[previousIndex].SetActive(true);
            //    activeWendigo = wendigos[previousIndex];
            //    GameManager.Instance.currentWendigo = previousIndex;
            //}
            //else if (triggered == true)
            //{
            //    wendigos[triggerIndex].SetActive(true);
            //    activeWendigo = wendigos[triggerIndex];
            //    GameManager.Instance.currentWendigo = triggerIndex;
            //}
            //
            //for (int i = 0; i <= teleportTriggers.Length - 1; i++)
            //{
            //    if (teleportTriggers[i].activeWendigo != activeWendigo)
            //    {
            //        teleportTriggers[i].activeWendigo = activeWendigo;
            //    }
            //}

            //switch(GameManager.Instance.currentWendigo)
            //{
            //    case 0:
            //        {
            //            // Deactivate Wendigo[0] and activate Wendigo[1].
            //            if (wendigos[0].activeSelf == true)
            //            {
            //                wendigos[0].SetActive(false);
            //                GameManager.Instance.currentWendigo = 1;
            //                wendigos[1].SetActive(true);
            //                activeWendigo = wendigos[1];
            //                teleportWendigo = activeWendigo.GetComponent<Teleportation>();
            //                wendigoControllers[1].StartRoaming();
            //            }
            //            triggered = true;
            //            break;
            //        }
            //    case 1:
            //        {
            //            if (triggered == false)
            //            {
            //                // Deactivate Wendigo[1] and activate Wendigo[2].
            //            }
            //            else if (triggered == true)
            //            {
            //                // Deactivate Wendigo[1] and activate Wendigo[0].
            //                if (wendigos[1].activeSelf == true)
            //                {
            //                    teleportWendigo.TeleportToLocation(activeWendigo.GetComponent<WendigoStateManager>());
            //                    wendigos[1].SetActive(false);
            //                    GameManager.Instance.currentWendigo = 0;
            //                    wendigos[0].SetActive(true);
            //                    activeWendigo = wendigos[0];
            //                    teleportWendigo = activeWendigo.GetComponent<Teleportation>();
            //                    wendigoControllers[0].StartRoaming();
            //                }
            //                triggered = false;
            //            }
            //            break;
            //        }
            //}
        //a}
    }


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.GameOver = false;
        GameManager.Instance.GamePaused = false;
        GameManager.Instance.wendigoCreatures = new GameObject[4];
        GameManager.Instance.wendigoNMAs = new NavMeshAgent[4];
        GameManager.Instance.wendigoLoadedX = new float[4];
        GameManager.Instance.wendigoLoadedY = new float[4];
        GameManager.Instance.wendigoLoadedZ = new float[4];
        GameManager.Instance.activeWendigo = activeWendigo;
        GameManager.Instance.wendigo = activeWendigo;
        activeWendigo = wendigos[0];
        for (int i = 0; i <= wendigos.Length - 1; i++)
        {
            GameManager.Instance.wendigoCreatures[i] = wendigos[i];
        }
        for (int i = 0; i <= wendigos.Length - 1; i++)
        {
            wendigoNMAs[i] = wendigos[i].GetComponent<NavMeshAgent>();
            wendigoControllers[i] = wendigos[i].GetComponent<WendigoStateManager>();
            Debug.Log("NAV MESH AGENT #" + i + " STATUS: SET");
            Debug.Log("STATE MACHINE CONTROLLER #" + i + " STATUS: SET");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GamePaused == false)
        {
            if (GameManager.Instance.activeWendigo != activeWendigo)
            {
                GameManager.Instance.activeWendigo = activeWendigo;
            }
            if (GameManager.Instance.finishedChasing == true && teleportWendigo.hasTeleported == false && readyToTP == true)
            {
                
            }
        }
    }
}
