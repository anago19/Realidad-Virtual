using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.currentState == GameState.Chase){
            return;
        }
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetState(GameState.Chase);
        }
    }
}
