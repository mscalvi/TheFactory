using UnityEngine;
using UnityEngine.SceneManagement;

public class EndExpeditionService : MonoBehaviour
{
    private ExpeditionState ExpeditionState;
    private GameState GameState;

    private DecisionsService DecisionsService;
    public void Initialize(ExpeditionState expedition, GameState gameState, DecisionsService decisions)
    {
        ExpeditionState = expedition;
        GameState = gameState;

        DecisionsService = decisions;
    }

    public void TransferCurrencies()
    {
        foreach (var currency in ExpeditionState.ExpeditionCurrency)
        {
            if(currency.Key != CurrencyHelper.CurrencyType.Experience)
            {
                GameState.CompanyState.CompanyCurrency[currency.Key] = currency.Value;
            }
        }
    }

    public void CallFinalPopUp()
    {
        if (ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.GameOver)
        {
            DecisionsService.LastDecision(false);
        } else if (ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.Finished)
        {
            DecisionsService.LastDecision(true);
        }
    }

    public void LoadLandingPage()
    {
        SceneManager.LoadScene("LandingScene");
    }

    void OnEnable()
    {
        ExpeditionEvents.OnExpeditionEnd += EndExpedition;
        ExpeditionEvents.OnFinalPopUpClose += EndScene;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnExpeditionEnd -= EndExpedition;
        ExpeditionEvents.OnFinalPopUpClose -= EndScene;
    }

    void EndExpedition()
    {
        TransferCurrencies();
        CallFinalPopUp();
    }

    void EndScene()
    {
        LoadLandingPage();
    }
}