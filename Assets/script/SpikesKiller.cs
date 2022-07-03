using UnityEngine;

public class SpikesKiller : MonoBehaviour
{
    
    public void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Et paf t'es mort !");
        GameObject.FindGameObjectWithTag("Player").GetComponent<Playerpos>().KillPlayer();
    }

}
