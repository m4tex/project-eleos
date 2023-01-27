using System.Collections;
using System.Collections.Generic;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour
{
    private NavMeshAgent _navAgent;
    // Start is called before the first frame update
    private void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.destination = PlayerMovement.Main.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
