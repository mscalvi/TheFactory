package Services;

import Entities.Bats;
import Entities.Player;
import Entities.Floor;

import java.util.List;

public class GameService {

    private CameraService cameraService;

    private Player player;
    private Bats bats;
    private List<Floor> floors;

    private boolean gameOver;

    public GameService(Player player, Bats bats, List<Floor> floors, CameraService cameraService) {
        this.player = player;
        this.bats = bats;
        this.floors = floors;
        this.cameraService = cameraService;
        this.gameOver = false;
    }

    public void update() {

        if (gameOver) {
            return;
        }

        int playerBottom =
                player.getBounds().y
                        + player.getBounds().height
                        - (int) cameraService.getCameraY();

        if (bats.getY() <= playerBottom) {
            gameOver = true;
            System.out.println("GameOver MORCEGO");
        }

        Floor currentFloor = floors.get(player.getFloor() - 1);

        if (currentFloor.hasStalagmite(player.getPosition())) {
            gameOver = true;
            System.out.println("GameOver ESPINHO ");
        }
    }

    public boolean isGameOver() {
        return gameOver;
    }
}