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
    private SpriteService spriteService;

    private List<Floor> floors;

    private CaveWall leftWall;
    private CaveWall rightWall;

    private Player player;
    private PlayerService playerService;

    private Bats bats;
    private BatsService batsService;

    public GamePanel(ScoreService ScoreService) {
        setBackground(Color.BLACK);

        inputHandler = new InputHandler();
        addKeyListener(inputHandler);
        setFocusable(true);
        requestFocusInWindow();

        floors = new ArrayList<>();

        for (int i = 0; i < 10; i++) {
            floors.add(new Floor(i + 1, 580 - i * 100));
        }

        player = new Player(3, 4, 285, 580 - 2 * 100 - 30, 30, 30);
        playerService = new PlayerService(player, floors);

        leftWall = new CaveWall(0, 0, 55, 800);
        rightWall = new CaveWall(545, 0, 55, 800);

        bats = new Bats(600);
        batsService = new BatsService(bats);

        cameraService = new CameraService(player);
        scoreService = ScoreService;
        elixirService = new ElixirService(player, floors, scoreService, batsService);
        spriteService = new SpriteService();

        gameService = new GameService(
                player,
                bats,
                floors,
                cameraService
        );

    }

    @Override
    protected void paintComponent(Graphics g) {
        super.paintComponent(g);


        // Paredes
        g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 3));
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

        // Andares
        for (Floor floor : floors) {

            // Plataforma
            g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 0));
            for (Platform platform : floor.getPlatforms()) {

                g.fillRect(
                        platform.getBounds().x,
                        platform.getBounds().y - cameraService.getCameraY(),
                        platform.getBounds().width,
                        platform.getBounds().height
                );
            }

            // Stalagmites
            g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 4));
            for (Entities.Stalagmite stalagmite : floor.getStalagmites()) {

                Platform platform =
                        floor.getPlatforms().get(stalagmite.getPosition() - 1);

                int x = platform.getBounds().x;
                int y = platform.getBounds().y - cameraService.getCameraY();

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

                g.fillPolygon(xPoints, yPoints, 3);
            }

            // Elixir
            if (floor.getElixir() != null) {

                Elixir elixir = floor.getElixir();

                Platform platform =
                        floor.getPlatforms().get(elixir.getPosition() - 1);

                int x = platform.getBounds().x;
                int y = platform.getBounds().y - cameraService.getCameraY();

                if (elixir.getType() == Elixir.Type.PINK) {

                    g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 5));

                    g.fillOval(
                            x + 20,
                            y - 25,
                            20,
                            20
                    );

                } else if (elixir.getType() == Elixir.Type.GREEN) {

                    g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 6));

                    g.fillOval(
                            x + 22,
                            y - 20,
                            15,
                            15
                    );
                }
            }
        }

        // Morcegos
        g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 2));

        g.fillRect(
                0,
                (int) bats.getY(),
                getWidth(),
                getHeight() - (int) bats.getY()
        );

        // Player
        g.setColor(spriteService.getSprite(elixirService.getElixirCounter(), 1));

        g.fillRect(
                player.getBounds().x,
                player.getBounds().y - cameraService.getCameraY(),
                player.getBounds().width,
                player.getBounds().height
        );
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
                cameraService.update();
                generateFloorsIfNeeded();

                if (player.getFloor() % 10 == 0) {
                    increaseBatsSpeed();
                }
            }
        }
    }

    private void generateFloorsIfNeeded() {

        while (floors.size() - player.getFloor() <= 3) {
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

    private void increaseBatsSpeed(){
        batsService.increaseSpeed(0.1);
    }
}