using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public GameObject virtualCam;

    public PlayerMovement PlayerMovement;
    public bool FreezPlayerOnExit = true;

    void Start() {
        
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !other.isTrigger){
            virtualCam.SetActive(true);
            
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && !other.isTrigger){
            virtualCam.SetActive(false);
            if(FreezPlayerOnExit == true){
                PlayerMovement.FreezPlayer(0.5f);
            }
        }
    }
}
