using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    private GameMaster gm;
    public bool directionRight = true; //direction au niveau du respawn

    private void Start() {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
    }
     
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player")) {
            gm.lastCheckPointPos = transform.position;
            gm.FacingDirection = directionRight;
        }

    }
    
}
