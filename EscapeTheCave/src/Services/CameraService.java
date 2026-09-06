package Services;

import Entities.Player;

public class CameraService {

    private Player player;

    private int cameraY;

    public CameraService(Player player) {
        this.player = player;
        this.cameraY = 0;
    }

    public void update() {

        int playerY = player.getBounds().y;

        if (playerY < 200) {
            cameraY = playerY - 200;
        }
    }

    public int getCameraY() {
        return cameraY;
    }
}