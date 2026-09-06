package Services;

import java.util.ArrayList;
import Entities.Floor;
import Entities.Player;
import Entities.Platform;

import java.util.List;

public class PlayerService {

    private Player player;
    private List<Floor> floors;

    public PlayerService(Player player, List<Floor> floors) {
        this.player = player;
        this.floors = floors;

        updateVisualPosition();
    }

    // Movimento
    public boolean moveUp() {

        if (player.getFloor() >= floors.size()) {
            return false;
        }

        player.setFloor(player.getFloor() + 1);
        updateVisualPosition();

        return true;
    }

    public boolean moveRight() {

        if (player.getFloor() >= floors.size()) {
            return false;
        }

        if (player.getPosition() == 7) {
            return false;
        }

        player.setPosition(player.getFloor() + 1, player.getPosition() + 1);
        updateVisualPosition();

        return true;
    }

    public boolean moveLeft() {

        if (player.getFloor() >= floors.size()) {
            return false;
        }

        if (player.getPosition() == 1) {
            return false;
        }

        player.setPosition(player.getFloor() + 1, player.getPosition() - 1);
        updateVisualPosition();

        return true;
    }

    public void updateVisualPosition() {

        Platform platform = getCurrentPlatform();

        player.setX(platform.getBounds().x + 15);
        player.setY(platform.getBounds().y - player.getBounds().height);
    }

    // Helper
    private Platform getCurrentPlatform() {

        Floor floor = floors.get(player.getFloor() - 1);

        return floor.getPlatforms().get(player.getPosition() - 1);
    }
}