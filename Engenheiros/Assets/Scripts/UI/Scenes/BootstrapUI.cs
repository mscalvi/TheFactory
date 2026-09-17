using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapUI : MonoBehaviour
{
    public void Initialize()
    {
        SceneManager.LoadScene("LandingScene");
    }
}
