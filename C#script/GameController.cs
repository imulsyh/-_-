using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static bool isGameAlive = true;

    public static CameraShake camShake;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && isGameAlive == true)
        {
            SceneManager.LoadScene(0);
        }
        if(Time.timeScale == 0f)
        {
            SceneManager.LoadScene(2);
            isGameAlive = true;
            Time.timeScale = 1f;
        }
    }
}
