package Services;

import Entities.Player;

public class PlayerService {

    private Player player;

    public PlayerService(Player player) {
        this.player = player;
    }

    public void moveUp() {
        player.setFloor(player.getFloor() + 1);
    }
}