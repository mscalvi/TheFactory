package Game;

import Ui.GamePanel;

public class GameLoop implements Runnable {

    private GamePanel gamePanel;

    public GameLoop(GamePanel gamePanel) {
        this.gamePanel = gamePanel;
    }

    @Override
    public void run() {

        while (true) {

            gamePanel.update();

            try {
                Thread.sleep(16);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}