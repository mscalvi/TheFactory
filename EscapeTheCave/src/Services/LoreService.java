package Services;

public class LoreService {

    private String currentLore;

    private boolean firstGreenElixirCollected;

    public LoreService() {

        currentLore =
                "Cada passo conta no caminho para a segurança.";

        firstGreenElixirCollected = false;
    }

    public String getCurrentLore() {
        return currentLore;
    }

    public void startLore() {

        currentLore =
                "Você cai para as profundezas da enorme caverna, mal conseguindo se segurar em uma plataforma. Algo ecoa nas profundezas... Mas sua cabeça" +
                        "está confusa demais para compreender a situação.";
    }

    public void floorLore(int floor) {

        if (floor == 40) {

            currentLore =
                    "A caverna parece não ter fim. "
                            + "Você precisa continuar subindo.";

        } else if (floor == 70) {

            currentLore =
                    "Os morcegos estão cada vez mais próximos. "
                            + "Talvez exista uma saída acima.";

        } else if (floor == 130) {

            currentLore =
                    "Os estalagmites cortam pela sua pele, e qualquer deslize pode ser fatal.";
        }
    }

    public void pinkElixirLore() {

        currentLore =
                "O elixir te ajuda a melhorar, talvez se puder encontrar mais deles...";
    }

    public void greenElixirLore() {

        if (!firstGreenElixirCollected) {

            currentLore =
                    "Seus itens se espalharam na queda, dezenas de elixires e tônicos revigorantes... Eles devem ajudar!";

            firstGreenElixirCollected = true;
        }
    }
}