package Ui;

import Input.InputHandler;

import Services.PlayerService;
import Services.BatsService;
import Services.GameService;
import Services.CameraService;

import Entities.CaveWall;
import Entities.Floor;
import Entities.Platform;
import Entities.Player;
import Entities.Bats;

import javax.swing.JPanel;
import java.awt.Color;
import java.awt.Graphics;
import java.util.ArrayList;
import java.util.List;

public class GamePanel extends JPanel {

    private InputHandler inputHandler;

    private GameService gameService;
    private CameraService cameraService;

    private List<Floor> floors;

    private CaveWall leftWall;
    private CaveWall rightWall;

    private Player player;
    private PlayerService playerService;

    private Bats bats;
    private BatsService batsService;

    public GamePanel() {
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

        g.setColor(Color.WHITE);

        // Paredes
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

        // Plataformas
        for (Floor floor : floors) {

            for (Platform platform : floor.getPlatforms()) {

                g.fillRect(
                        platform.getBounds().x,
                        platform.getBounds().y - cameraService.getCameraY(),
                        platform.getBounds().width,
                        platform.getBounds().height
                );
            }

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
        }

        // Morcegos
        g.setColor(Color.DARK_GRAY);

        g.fillRect(
                0,
                (int) bats.getY(),
                getWidth(),
                getHeight() - (int) bats.getY()
        );

        // Player
        g.setColor(Color.RED);

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
                cameraService.update();
                generateFloorsIfNeeded();

                if (player.getFloor() % 10 == 0) {
                    increaseBatsSpeed();
                }
            }
        }

        if (inputHandler.consumeRight()) {
            if (playerService.moveRight()) {
                cameraService.update();
                generateFloorsIfNeeded();

                if (player.getFloor() % 10 == 0) {
                    increaseBatsSpeed();
                }
            }
        }

        if (inputHandler.consumeLeft()) {
            if (playerService.moveLeft()) {
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