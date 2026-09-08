package Services;

import Entities.Elixir;
import Entities.Floor;
import Entities.Player;

import java.util.List;

public class ElixirService {

    private Player player;
    private List<Floor> floors;
    private ScoreService scoreService;
    private BatsService batsService;
    private int elixirCounter = 0;

    public ElixirService(
            Player player,
            List<Floor> floors,
            ScoreService scoreService,
            BatsService batsService
    ) {
        this.player = player;
        this.floors = floors;
        this.scoreService = scoreService;
        this.batsService = batsService;
    }

    public void collect() {

        Floor floor = floors.get(player.getFloor() - 1);

        Elixir elixir = floor.getElixir();

        if (elixir == null) {
            return;
        }

        if (elixir.getPosition() == player.getPosition()) {

            if (elixir.getType() == Elixir.Type.PINK) {
                scoreService.addPinkElixir();
                batsService.decreaseSpeed(0.2);
                elixirCounter++;
            } else if (elixir.getType() == Elixir.Type.GREEN) {
                scoreService.addGreenElixir();
                batsService.decreaseSpeed(0.1);
            }

            floor.removeElixir();
        }
    }

    public int getElixirCounter() {
        return elixirCounter;
    }
}