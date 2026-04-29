using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompanyDesigner : MonoBehaviour
{
    public void ReturnButton()
    {
        SceneManager.LoadScene("LandingScene");
    }
}
