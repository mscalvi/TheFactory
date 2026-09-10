package Ui;

import Input.InputHandler;

import Services.*;

import Entities.CaveWall;
import Entities.Floor;
import Entities.Platform;
import Entities.Player;
import Entities.Bats;
import Entities.Elixir;

import javax.swing.JPanel;
import java.awt.Color;
import java.awt.Graphics;
import java.util.ArrayList;
import java.util.List;

public class GamePanel extends JPanel {

    private InputHandler inputHandler;

    private GameService gameService;
    private CameraService cameraService;
    private ScoreService scoreService;
    private ElixirService elixirService;
    private ColorService colorService;
    private SpriteService spriteService;
    private LoreService loreService;

    private List<Floor> floors;

    private CaveWall leftWall;
    private CaveWall rightWall;

    private Player player;
    private PlayerService playerService;

    private Bats bats;
    private BatsService batsService;

    public GamePanel(ScoreService ScoreService, LoreService LoreService) {
        setBackground(Color.BLACK);

        inputHandler = new InputHandler();
        addKeyListener(inputHandler);
        setFocusable(true);
        requestFocusInWindow();

        floors = new ArrayList<>();

        for (int i = 0; i < 10; i++) {
            floors.add(new Floor(i + 1, 580 - i * 100));
        }

        player = new Player(
                1,
                4,
                285,
                580 - 2 * 100 - 30,
                30,
                30
        );

        playerService = new PlayerService(player, floors);

        leftWall = new CaveWall(0, 0, 55, 800);
        rightWall = new CaveWall(545, 0, 55, 800);

        bats = new Bats(600);
        batsService = new BatsService(bats);

        cameraService = new CameraService(player);

        scoreService = ScoreService;
        loreService = LoreService;

        elixirService = new ElixirService(
                player,
                floors,
                scoreService,
                batsService,
                loreService
        );

        colorService = new ColorService();
        spriteService = new SpriteService();

        gameService = new GameService(
                player,
                bats,
                floors,
                cameraService
        );

        loreService.startLore();
    }

    @Override
    protected void paintComponent(Graphics g) {
        super.paintComponent(g);

        int elixirCounter = elixirService.getElixirCounter();

        if (elixirCounter >= 2) {
            setBackground(Color.LIGHT_GRAY);
        } else {
            setBackground(Color.BLACK);
        }

        // Paredes
        if (elixirCounter < 2) {

            g.setColor(
                    colorService.getSprite(elixirCounter, 3)
            );

            g.fillRect(
                    leftWall.getBounds().x,
                    leftWall.getBounds().y,
                    leftWall.getBounds().width,
                    leftWall.getBounds().height
            );

            g.fillRect(
                    rightWall.getBounds().x,
                    rightWall.getBounds().y,
                    rightWall.getBounds().width,
                    rightWall.getBounds().height
            );

        } else {
            for (int y = 0; y < getHeight(); y += 30) {

                g.drawImage(
                        spriteService.getSprite(3),
                        leftWall.getBounds().x,
                        y,
                        leftWall.getBounds().width,
                        30,
                        null
                );

                g.drawImage(
                        spriteService.getSprite(3),
                        rightWall.getBounds().x,
                        y,
                        rightWall.getBounds().width,
                        30,
                        null
                );
            }
        }

        // Andares

        for (Floor floor : floors) {

            // Plataformas

            if (elixirCounter < 2) {

                g.setColor(
                        colorService.getSprite(elixirCounter, 0)
                );

                for (Platform platform : floor.getPlatforms()) {

                    g.fillRect(
                            platform.getBounds().x,
                            platform.getBounds().y
                                    - cameraService.getCameraY(),
                            platform.getBounds().width,
                            platform.getBounds().height
                    );
                }

            } else {

                for (Platform platform : floor.getPlatforms()) {

                    g.drawImage(
                            spriteService.getSprite(0),
                            platform.getBounds().x,
                            platform.getBounds().y
                                    - cameraService.getCameraY(),
                            platform.getBounds().width,
                            platform.getBounds().height,
                            null
                    );
                }
            }

            // Stalagmites

            if (elixirCounter < 2) {

                g.setColor(
                        colorService.getSprite(elixirCounter, 4)
                );

                for (Entities.Stalagmite stalagmite
                        : floor.getStalagmites()) {

                    Platform platform =
                            floor.getPlatforms().get(
                                    stalagmite.getPosition() - 1
                            );

                    int x = platform.getBounds().x;

                    int y = platform.getBounds().y
                            - cameraService.getCameraY();

                    int[] xPoints = {
                            x + 15,
                            x + 30,
                            x + 45
                    };

                    int[] yPoints = {
                            y,
                            y - 30,
                            y
                    };

                    g.fillPolygon(
                            xPoints,
                            yPoints,
                            3
                    );
                }

            } else {

                for (Entities.Stalagmite stalagmite
                        : floor.getStalagmites()) {

                    Platform platform =
                            floor.getPlatforms().get(
                                    stalagmite.getPosition() - 1
                            );

                    int x = platform.getBounds().x;

                    int y = platform.getBounds().y
                            - cameraService.getCameraY();

                    g.drawImage(
                            spriteService.getSprite(4),
                            x,
                            y - 30,
                            60,
                            30,
                            null
                    );
                }
            }

            // Elixires
            if (floor.getElixir() != null) {

                Elixir elixir = floor.getElixir();

                Platform platform =
                        floor.getPlatforms().get(
                                elixir.getPosition() - 1
                        );

                int x = platform.getBounds().x;

                int y = platform.getBounds().y
                        - cameraService.getCameraY();

                if (elixir.getType() == Elixir.Type.PINK) {

                    if (elixirCounter < 2) {

                        g.setColor(
                                colorService.getSprite(
                                        elixirCounter,
                                        5
                                )
                        );

                        g.fillOval(
                                x + 20,
                                y - 25,
                                20,
                                20
                        );

                    } else {

                        g.drawImage(
                                spriteService.getSprite(5),
                                x + 20,
                                y - 25,
                                20,
                                20,
                                null
                        );
                    }

                } else if (elixir.getType() == Elixir.Type.GREEN) {

                    if (elixirCounter < 2) {

                        g.setColor(
                                colorService.getSprite(
                                        elixirCounter,
                                        6
                                )
                        );

                        g.fillOval(
                                x + 22,
                                y - 20,
                                15,
                                15
                        );

                    } else {

                        g.drawImage(
                                spriteService.getSprite(6),
                                x + 22,
                                y - 20,
                                15,
                                15,
                                null
                        );
                    }
                }
            }
        }

        // Morcegos
        if (elixirCounter < 2) {

            g.setColor(
                    colorService.getSprite(elixirCounter, 2)
            );

            g.fillRect(
                    0,
                    (int) bats.getY(),
                    getWidth(),
                    getHeight() - (int) bats.getY()
            );

        } else {
            int batsY = (int) bats.getY();

            int spriteWidth =
                    spriteService.getSprite(2).getWidth();

            int spriteHeight =
                    spriteService.getSprite(2).getHeight();

            for (int y = batsY;
                 y < getHeight();
                 y += spriteHeight) {

                for (int x = 0;
                     x < getWidth();
                     x += spriteWidth) {

                    g.drawImage(
                            spriteService.getSprite(2),
                            x,
                            y,
                            null
                    );
                }
            }
        }

        // Player
        if (elixirCounter < 2) {

            g.setColor(
                    colorService.getSprite(elixirCounter, 1)
            );

            g.fillRect(
                    player.getBounds().x,
                    player.getBounds().y
                            - cameraService.getCameraY(),
                    player.getBounds().width,
                    player.getBounds().height
            );

        } else {

            g.drawImage(
                    spriteService.getSprite(1),
                    player.getBounds().x,
                    player.getBounds().y
                            - cameraService.getCameraY(),
                    player.getBounds().width,
                    player.getBounds().height,
                    null
            );
        }
    }

    public void update() {
        processInput();

        batsService.update();

        cameraService.update();

        gameService.update();

        if (gameService.isGameOver()) {
            return;
        }

        repaint();
    }

    private void processInput() {

        if (inputHandler.consumeUp()) {

            if (playerService.moveUp()) {

                elixirService.collect();
                scoreService.addFloor();

                loreService.floorLore(
                        player.getFloor()
                );

                cameraService.update();

                generateFloorsIfNeeded();

                if (player.getFloor() % 10 == 0) {
                    increaseBatsSpeed();
                }
            }
        }

        if (inputHandler.consumeRight()) {

            if (playerService.moveRight()) {

                elixirService.collect();
                scoreService.addFloor();

                loreService.floorLore(
                        player.getFloor()
                );

                cameraService.update();

                generateFloorsIfNeeded();

                if (player.getFloor() % 10 == 0) {
                    increaseBatsSpeed();
                }
            }
        }

        if (inputHandler.consumeLeft()) {

            if (playerService.moveLeft()) {

                elixirService.collect();
                scoreService.addFloor();

                loreService.floorLore(
                        player.getFloor()
                );

                cameraService.update();

                generateFloorsIfNeeded();

                if (player.getFloor() % 10 == 0) {
                    increaseBatsSpeed();
                }
            }
        }
    }

    private void generateFloorsIfNeeded() {

        while (floors.size() - player.getFloor() <= 5) {
            generateNextFloor();
        }
    }

    private void generateNextFloor() {

        int lastFloorNumber =
                floors.get(floors.size() - 1).getFloorNumber();

        int lastFloorY =
                floors.get(floors.size() - 1).getY();

        floors.add(
                new Floor(
                        lastFloorNumber + 1,
                        lastFloorY - 100
                )
        );
    }

    private void increaseBatsSpeed() {
        batsService.increaseSpeed(0.05);
    }

    public GameService getGameService() {
        return gameService;
    }

    public int getPlayerFloor() {
        return player.getFloor();
    }
}