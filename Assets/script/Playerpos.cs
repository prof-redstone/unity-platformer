using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playerpos : MonoBehaviour
{
    private GameMaster gm;
    private SceneTransition sm; 

    void Start() {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        sm = GameObject.FindGameObjectWithTag("SM").GetComponent<SceneTransition>();
        transform.position = gm.lastCheckPointPos;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().direction = gm.FacingDirection; //direction au niveau du respawn
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            KillPlayer();
        }
    }

    public void KillPlayer(){
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload current scene
        sm.ReloadScene();
    }
}
