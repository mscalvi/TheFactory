package Services;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.IOException;

public class SpriteService {

    private BufferedImage platform;
    private BufferedImage player;
    private BufferedImage bats;
    private BufferedImage wall;
    private BufferedImage stalagmite;
    private BufferedImage elixirPink;
    private BufferedImage elixirGreen;

    public SpriteService() {

        try {

            platform = loadSprite("/Sprites/platform.png");
            player = loadSprite("/Sprites/player.png");
            bats = loadSprite("/Sprites/bats.png");
            wall = loadSprite("/Sprites/wall.png");
            stalagmite = loadSprite("/Sprites/stalagmite.png");
            elixirPink = loadSprite("/Sprites/elixir_pink.png");
            elixirGreen = loadSprite("/Sprites/elixir_green.png");

        } catch (IOException e) {

            throw new RuntimeException(
                    "Erro ao carregar sprites.",
                    e
            );
        }
    }

    private BufferedImage loadSprite(String path)
            throws IOException {

        var input = getClass().getResourceAsStream(path);

        if (input == null) {
            throw new IOException(
                    "Sprite não encontrado: " + path
            );
        }

        return ImageIO.read(input);
    }

    public BufferedImage getSprite(int type) {

        switch (type) {

            case 0:
                return platform;

            case 1:
                return player;

            case 2:
                return bats;

            case 3:
                return wall;

            case 4:
                return stalagmite;

            case 5:
                return elixirPink;

            case 6:
                return elixirGreen;

            default:
                return null;
        }
    }
}