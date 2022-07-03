using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    private static GameMaster instance;
    public Vector2 lastCheckPointPos;
    public bool FacingDirection;  //direction au niveau du respawn

    private void Start() {
        lastCheckPointPos = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }else
        {
            Destroy(gameObject);
        }
    }


}
