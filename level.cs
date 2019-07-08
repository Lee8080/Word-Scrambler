using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level : MonoBehaviour
{
    public static float percentage = 0;

    public void easylevel()
    {
        percentage = 1.5f;
        SceneManager.LoadScene(2);
    } 

    public void normallevel()
    {
        percentage = 1;
        SceneManager.LoadScene(2);
    }

    public void hardlevel()
    {
        percentage = 0.6f;
        SceneManager.LoadScene(2);
    }
}
