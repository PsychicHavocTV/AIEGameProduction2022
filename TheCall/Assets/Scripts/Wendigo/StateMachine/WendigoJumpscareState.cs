using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WendigoJumpscareState : BaseState
{
    public NavMeshAgent nma;

    public override void EnterState(WendigoStateManager wendigo)
    {
        Debug.Log("waiting behind player...");
    }

    public override void UpdateState(WendigoStateManager wendigo)
    {
        wendigo.transform.LookAt(wendigo.playerRef.transform.position);
        BehindPlayerScare(wendigo);
        wendigo.TryTeleport(wendigo);
    }

    public void BehindPlayerScare(WendigoStateManager wendigo)
    {
        if (wendigo.goBehindPlayer == true)
        {
            nma.enabled = false;
            Debug.Log("NavMeshAgent Disabled.");
            wendigo.behindPlayerSpawn.transform.forward = wendigo.playerRef.transform.forward;
            wendigo.transform.position = new Vector3(wendigo.behindPlayerSpawn.transform.position.x, wendigo.playerRef.transform.position.y + 0.5f,
                                                     wendigo.behindPlayerSpawn.transform.position.z);
            Debug.Log("Teleported Behind Player.");
            wendigo.transform.forward = wendigo.behindPlayerSpawn.transform.forward;
            Debug.Log("Rotated To Face Player.");
            wendigo.goBehindPlayer = false;
            wendigo.isBehindPlayer = true;
            nma.enabled = true;
            Debug.Log("NavMeshAgent Enabled.");
        }
        if (wendigo.teleportAway == true)
        {
            wendigo.teleportAway = false;
            Debug.Log("Teleport Away.");
            TeleportAway(wendigo);
        }
    }

    public void TeleportAway(WendigoStateManager wendigo)
    {
        //if (wendigo.isBehindPlayer == false)
        //{
        wendigo.isBehindPlayer = false;
        wendigo.teleportHandler.TeleportToLocation(wendigo);
        //}    
    }
}
