package Entities;

import java.util.ArrayList;
import java.util.List;
import java.util.HashSet;
import java.util.Set;

public class Floor {

    private List<Stalagmite> stalagmites;
    private int y;
    private int floorNumber;
    private List<Platform> platforms;

    public Floor(int floorNumber, int y) {
        this.floorNumber = floorNumber;
        this.y = y;
        platforms = new ArrayList<>();

        int platformWidth = 60;
        int platformHeight = 20;
        int gap = 10;

        int startX = 60;

        for (int i = 0; i < 7; i++) {

            int x = startX + i * (platformWidth + gap);

            platforms.add(
                    new Platform(x, y, platformWidth, platformHeight)
            );
        }

        stalagmites = new ArrayList<>();

        int amount = 1;

        if (floorNumber >= 101) {
            amount = 4;
        } else if (floorNumber >= 51) {
            amount = 3;
        } else if (floorNumber >= 26) {
            amount = 2;
        }

        Set<Integer> occupiedPositions = new HashSet<>();

        for (int i = 0; i < amount; i++) {

            int position;

            do {
                position = (int) (Math.random() * 7) + 1;
            } while (occupiedPositions.contains(position));

            occupiedPositions.add(position);

            stalagmites.add(
                    new Stalagmite(position)
            );
        }
    }

    public void moveY(int amount) {

        y += amount;

        for (Platform platform : platforms) {
            platform.moveY(amount);
        }
    }

    public int getY() {
        return y;
    }

    public List<Platform> getPlatforms() {
        return platforms;
    }

    public List<Stalagmite> getStalagmites() {
        return stalagmites;
    }

    public int getFloorNumber() {
        return floorNumber;
    }

    public boolean hasStalagmite(int position) {

        for (Stalagmite stalagmite : stalagmites) {

            if (stalagmite.getPosition() == position) {
                return true;
            }
        }

        return false;
    }

    public boolean isPositionFree(int position) {
        return !hasStalagmite(position);
    }
}