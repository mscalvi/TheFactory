package Game;

import Ui.GamePanel;
import Ui.LorePanel;

public class GameLoop implements Runnable {

    private GamePanel gamePanel;
    private LorePanel lorePanel;

    public GameLoop(GamePanel gamePanel, LorePanel lorePanel)
    {
        this.gamePanel = gamePanel;
        this.lorePanel = lorePanel;
    }

    @Override
    public void run() {

        while (true) {

            gamePanel.update();
            lorePanel.update();

            try {
                Thread.sleep(16);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}