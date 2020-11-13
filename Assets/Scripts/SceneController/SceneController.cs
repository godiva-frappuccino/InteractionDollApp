using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int sceneFPS = 15;
    void Start()
    {
      Application.targetFrameRate = sceneFPS;
    }
    public void toMainManu(string sceneName){
      SceneManager.LoadScene(sceneName);
    }
}
