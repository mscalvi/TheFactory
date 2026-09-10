package Services;

import java.awt.*;

public class ColorService {
    public Color getSprite(int elixirCounter, int type)
    {
        // 0 plataforma
        // 1 player
        // 2 morcego
        // 3 paredes
        // 4 stalagmite
        // 5 elixir rosa
        // 6 elixir verde

        switch (elixirCounter) {
            case 0:
                return Color.WHITE;

            case 1:
                switch (type){
                    case 0:
                        return new Color(77, 77, 88);
                    case 1:
                        return new Color(50, 9, 9);
                    case 2:
                        return new Color(34, 34, 41);
                    case 3:
                        return new Color(57, 57, 66);
                    case 4:
                        return new Color(143, 143, 159);
                    case 5:
                        return new Color(128, 0, 128);
                    case 6:
                        return new Color(43, 101, 19);
                    default:
                        return Color.RED;
                }
            default:
                switch (type){
                    case 0:
                        return new Color(77, 77, 88);
                    case 1:
                        return new Color(50, 9, 9);
                    case 2:
                        return new Color(34, 34, 41);
                    case 3:
                        return new Color(57, 57, 66);
                    case 4:
                        return new Color(143, 143, 159);
                    case 5:
                        return new Color(128, 0, 128);
                    case 6:
                        return new Color(43, 101, 19);
                    default:
                        return Color.RED;
                }
        }
    }

}
