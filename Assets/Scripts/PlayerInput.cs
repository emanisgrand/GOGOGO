using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    // store horizontal input
     
    public float H { get; private set;} // Defaults to 0f
    // store vertical input
    public float V { get; private set; } // Defaults to 0f
    
    
    
    // global flag for enabling and disabling user input
    public bool InputEnabled { get; set; } // defaults to false
    
    // get keyboard input
    public void GetKeyInput() {
        H = (InputEnabled) ? Input.GetAxisRaw("Horizontal") : 0f;
        V = (InputEnabled) ? Input.GetAxisRaw("Vertical") : 0f;
    }
}
