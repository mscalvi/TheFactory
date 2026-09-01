using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public (string,string) SetText(GameHelper.Tutorial Type)
    {
        string Title = "";
        string Info = "";

        switch (Type)
        {
            // Inicio do Jogo, na Landing
            case GameHelper.Tutorial.StartTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Mercador de Isme";
                    Info = "Olá! Bem-vindo à sua nova Companhia! O Mercador de Isme está pronto para se aventurar e " +
                        "conhecer novos mares, trazendo ingredientes para confeccionar produtos exclusivos. Que venha a fortuna!";
                }
                GameState.ProgressState.StartTut = true;
                return (Title, Info);

            // Tomar Dano, na Expedition
            case GameHelper.Tutorial.ShipTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Navio";
                    Info = "Quando a Vida do Navio chega a 0, a Expedição termina. Clicando na Barra de Vida, no canto " +
                        "superior direito, é possível ver outras informações do Navio, como Resistência e Reparos. " +
                        "\n" +
                        "Além disso, ao longo do jogo, é possível trocar de Navio, conseguindo mais espaços para Armas " +
                        "e valores de atributos maiores.";
                }
                GameState.ProgressState.ShipTut = true;
                return (Title, Info);

            // Inicio da Expedition, na Expedition
            case GameHelper.Tutorial.ExpeditionTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Expedição";
                    Info = "A Expedição representa uma empreitada de coleta de dinheiro e recursos, através da " +
                        "exploração dos mares e rios de Furma. Ela segue por incontáveis destinos e dias, até que o Navio " +
                        "não tenha mais condições de continuar.";
                }
                GameState.ProgressState.ExpeditionTut = true;
                return (Title, Info);

            // Comprar Upgrade, na Expedition
            case GameHelper.Tutorial.UpgradesTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Melhorias";
                    Info = "As melhorias compradas durante uma Expedição só são válidas até o final dela. Servem para " +
                        "aumentar as capacidades do Navio, das Armas e dos ganhos financeiros. " +
                        "\n" +
                        "Melhorias compradas fora da Expedição são permanentes.";
                }
                GameState.ProgressState.UpgradesTut = true;
                return (Title, Info);

            // Inimigo Morto, na Expedition
            case GameHelper.Tutorial.ClickTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Marcando Alvos";
                    Info = "Durante uma Expedição, você pode clicar em um inimigo para marcá-lo. Um alvo marcado " +
                        "tem chances de deixar cair Ingredientes e outros recursos, que alimentam outras funcionalidades " +
                        "do jogo. Além disso, é possível causar danos maiores a inimigos marcados. " +
                        "\n" +
                        "Inicialmente, só é possível marcar um inimigo por vez.";
                }
                GameState.ProgressState.ClickTut = true;
                return (Title, Info);

            // Clicar em Buildings, na Landing
            case GameHelper.Tutorial.BuildingsTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Salas de Melhorias";
                    Info = "As Melhorias permanentes são divididas em Salas, cada uma com sua categoria de melhorias, " +
                        "como Armas e Munições, qualidade do Navio, ganho de recursos e avanço da Companhia.";
                }
                GameState.ProgressState.BuildingsTut = true;
                return (Title, Info);
            
            // Clicar em Alchemy, na Landing
            case GameHelper.Tutorial.AlchemyTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Alquimia";
                    Info = "A Alquimia é feita com os Ingredientes coletados durante uma Expedição, e serve para gerar " +
                        "Marcos de forma constante. Após liberado um novo Produto, ele irá, a cada ciclo, gerar uma quantidade " +
                        "determinada de Marcos, podendo ser recomprado para acumular valores.";
                }
                GameState.ProgressState.AlchemyTut = true;
                return (Title, Info);

            // Clicar em Bestiary, na Landing
            case GameHelper.Tutorial.BestiaryTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Bestiário";
                    Info = "O Bestiário mostra informações dos inimigos conhecidos, divididos em categorias. " +
                        "Conforme mais inimigos de determinado tipo são mortos, mais informações ficam " +
                        "disponíveis no Bestiário.";
                }
                GameState.ProgressState.BestiaryTut = true;
                return (Title, Info);

            // Início do Produto, na Landing
            case GameHelper.Tutorial.MarcosTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Marcos";
                    Info = "Os Marcos são a moeda do império, e utilizada para grandes negócios. É possível conseguir com inimigos especiais, " +
                        "ou através da venda de produtos alquímicos.";
                }
                GameState.ProgressState.MarcosTut = true;
                return (Title, Info);
            
            // Final da Noite 1, na Expedition
            case GameHelper.Tutorial.ExperienceTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Experiência";
                    Info = "A Experiência é a moeda interna de uma Expedição. Ela reinicia a cada nova Expedição, não " +
                        "podendo ser acumulada, e serve para comprar Melhorias internas. Pode ser obtida através de abates " +
                        "de inimigos, ao final de uma Noite e no começo de uma Expedição.";
                }
                GameState.ProgressState.ExperienceTut = true;
                return (Title, Info);

            // Chegar na Destination 1, na Expedition
            case GameHelper.Tutorial.DestinationsTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Rotas";
                    Info = "O contador no canto superior esquerdo indica o dia atual e o dia final para sair de uma Rota, " +
                        "sendo que essa define os tipos de inimigos que podem aparecer, além de seus modificadores. " +
                        "Terminar uma Rota implica no ganho de Prestígio.";
                }
                GameState.ProgressState.DestinationsTut = true;
                return (Title, Info);

            // Primeiro Reload, na Expedition
            case GameHelper.Tutorial.WeaponsTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Armas e Munições";
                    Info = "O Navio é carregado com Armas, e cada Arma possui sua Munição. Para ver as características " +
                        "das Armas e Muniçõe equipadas, utilize o quarto menu inferior." +
                        "\n" +
                        "Todo conjunto de Arma e Munição precisa ser recarregado. O progresso da recarga é automático, " +
                        "e pode ser visto no mesmo menu.";
                }
                GameState.ProgressState.WeaponsTut = true;
                return (Title, Info);

            // Segundo Inimigo Visto, na Expedition
            case GameHelper.Tutorial.KnowledgeTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Conhecimento";
                    Info = "Sempre que um novo tipo de inimgo for avistado, você ganhará pontos de Conhecimento. ";
                }
                GameState.ProgressState.KnowledgeTut = true;
                return (Title, Info);

            // Final do Dia 1, na Expedition
            case GameHelper.Tutorial.FumeTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Fume";
                    Info = "A Moeda local em Lapa Liandra, ainda utilizada em pequenos comércios e empreitadas locais. " +
                    "São a principal moeda fora de uma Expedição. São utilizados para comprar Melhorias " +
                    "e outros recursos do jogo. Podem ser obtidos ao final de um Dia ou ao eliminar alvos " +
                    "especiais.";
                }
                GameState.ProgressState.FumeTut = true;
                return (Title, Info);

            // Clicar em Shop, na Landing
            case GameHelper.Tutorial.ShopTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "Compras";
                    Info = "Aqui você pode gastar os Marcos acumulados, trocando por produtos permanentes, como novos Navios " +
                        "e Armas, ou por melhorias contínuas, os Contratos.";
                }
                GameState.ProgressState.ShopTut = true;
                return (Title, Info);

        }

        return (Title, Info);
    }
}
