using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingPageDesigner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Ui do Barco Ativo?
        // Ui das Buildings Ativas?
    }

    public void ExpeditionButton()
    {
        SceneManager.LoadScene("ExpeditionScene");
    }

    public void ShipConfigButton()
    {
        SceneManager.LoadScene("ShipConfigScene");
    }
}
