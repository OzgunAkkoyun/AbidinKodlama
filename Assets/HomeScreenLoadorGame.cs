using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenLoadorGame : MonoBehaviour
{
    public void SetLoadOrGame(int index)
    {
        PlayerPrefs.SetInt("isGameOrLoad",index);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ 1);
    }
}
