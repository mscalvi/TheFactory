package Entities;

public class Bats {

    private double y;

    public Bats(double y) {
        this.y = y;
    }

    public double getY() {
        return y;
    }

    public void moveY(double amount) {
        y += amount;

        if (y > 600) y = 600;
    }
}