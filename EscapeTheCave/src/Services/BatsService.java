package Services;

import Entities.Bats;

public class BatsService {

    private Bats bats;

    public BatsService(Bats bats) {
        this.bats = bats;
    }

    private double speed = 0.1;

    public void update() {
        bats.moveY(-speed);
    }

    public void increaseSpeed(double amount) {
        speed += amount;
    }
    public void decreaseSpeed(double amount) {
        if (speed > 0.3) speed -= amount;
    }
}