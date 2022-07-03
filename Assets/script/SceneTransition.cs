using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnim;

    public void Update() {
        /*if (Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(LoadScene());
        }*/
    }

    IEnumerator LoadScene(){
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(0.35f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReloadScene(){
        StartCoroutine(LoadScene());
    }
}
