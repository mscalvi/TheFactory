using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingUI : MonoBehaviour
{
    [SerializeField] GameObject NewGameBtn;

    public void Initialize(){
            NewGameBtn.SetActive(true);
    }

    public void NewGameBtnFunction()
    {
        SceneManager.LoadScene("ExpeditionScene");
    }
}
