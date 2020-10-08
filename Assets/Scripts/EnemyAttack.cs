using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    PlayerManager m_player;

    private void Awake() {
        m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

   public void Attack() {
        m_player?.InvokeDeathEvent();
    }    
}
