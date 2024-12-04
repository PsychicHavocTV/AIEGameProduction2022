using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class WendigoChaseState : BaseState
{
    public NavMeshAgent nma;
    float playerDistance;
    Vector3 destinationPosition;

    public override void EnterState(WendigoStateManager wendigo)
    {
        GameManager.Instance.wendigoChasing = true;
        Debug.Log("Chasing...");
        wendigo.stateAnimator.ResetTrigger(wendigo.walkingParam);
        wendigo.stateAnimator.SetTrigger(wendigo.chaseParam);
        nma.speed = 18.5f;
        nma.acceleration = 18.5f;
    }

    public override void UpdateState(WendigoStateManager wendigo)
    {
        if (GameManager.Instance.GamePaused == true)
        {
            nma.acceleration = 0;
            nma.speed = 0;
            nma.enabled = false;
        }
        else if (GameManager.Instance.GameOver == false)
        {
            if (nma.enabled == false)
            {
                nma.enabled = true;
            }
            if (nma.acceleration == 0)
            {
                nma.acceleration = 18.5f;
            }
            if (nma.speed != 18.5f)
            {
                nma.speed = 18.5f;
            }
            if (wendigo.hidingController.isHidden == true)
            {
                if (wendigo.timerRunning == false)
                {
                    wendigo.ControlTimer();
                }
            }

            wendigo.transform.LookAt(wendigo.playerRef.transform);

            if (wendigo.hidingController.isHidden == false)
            {
                RaycastHit hit;
                playerDistance = Vector3.Distance(wendigo.playerRef.transform.position, wendigo.transform.position);
                Vector3 rayDirection = wendigo.playerRef.transform.position - wendigo.transform.position;
                // Check if the player is in front of the Wendigo & within its Field Of View.
                if ((Vector3.Angle(rayDirection, wendigo.transform.forward)) < 45) 
                {
                    if ((Vector3.Angle(rayDirection, wendigo.transform.forward)) < 45) // Is player within field of view
                    {
                       if (Physics.Raycast(wendigo.transform.position, rayDirection, out hit, 110))
                       {
                           if (hit.collider.gameObject.tag == "Player")
                           {
                               wendigo.StartChasing();
                           }
                           else
                           {
                               if (wendigo.timerRunning == false)
                               {
                                   wendigo.ControlTimer();
                               }
                           }
                       }
                       else
                       {
                           if (wendigo.timerRunning == false)
                           {
                               wendigo.ControlTimer();
                           }
                       }
                    }
                    else
                    {
                        if (wendigo.timerRunning == false)
                        {
                            wendigo.ControlTimer();
                        }
                    }
                }
                else
                {
                    if (wendigo.timerRunning == false)
                    {
                        wendigo.ControlTimer();
                    }
                }
            }

            nma.ResetPath();
            var path = new NavMeshPath();
            destinationPosition = wendigo.playerRef.transform.position;
            nma.CalculatePath(destinationPosition, path);
            nma.SetDestination(destinationPosition);
            nma.SetPath(path);
            //else if (path.status == NavMeshPathStatus.PathInvalid)
            //{
            //  nma.SetDestination(destinationPosition);
            //}
        }
    }
}
