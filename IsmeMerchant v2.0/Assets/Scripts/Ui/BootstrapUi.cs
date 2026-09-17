using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapUi : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("LandingScene");
    }
}
