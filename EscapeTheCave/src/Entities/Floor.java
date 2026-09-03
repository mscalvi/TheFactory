package Entities;

import java.util.ArrayList;
import java.util.List;

public class Floor {

    private List<Platform> platforms;

    public Floor(int y) {
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
    }

    public List<Platform> getPlatforms() {
        return platforms;
    }
}