using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {
   // ref to AnimController
   public Animator playerAnimController;
   // string id for the PlayerDeath animation trigger parameter
   private static readonly int IsDead = Animator.StringToHash("isDead");

   // trigger the death animation
   public void TriggerDeathAnim() {
      playerAnimController?.SetTrigger(IsDead);
   }
}
