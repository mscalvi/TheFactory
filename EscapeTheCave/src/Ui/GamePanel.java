package Ui;

import Input.InputHandler;

import Services.PlayerService;

import Entities.CaveWall;
import Entities.Floor;
import Entities.Platform;
import Entities.Player;

import javax.swing.JPanel;
import java.awt.Color;
import java.awt.Graphics;
import java.util.ArrayList;
import java.util.List;

public class GamePanel extends JPanel {

    private InputHandler inputHandler;

    private List<Floor> floors;

    private CaveWall leftWall;
    private CaveWall rightWall;

    private Player player;
    private PlayerService playerService;

    public GamePanel() {
        setBackground(Color.BLACK);

        inputHandler = new InputHandler();
        addKeyListener(inputHandler);
        setFocusable(true);
        requestFocusInWindow();

        floors = new ArrayList<>();

        for (int i = 0; i < 10; i++) {
            floors.add(new Floor(80 + i * 100));
        }

        player = new Player(3, 3, 285, 80 + 2 * 100 - 30, 30, 30);
        playerService = new PlayerService(player);
        if (inputHandler.isUp()) {
            playerService.moveUp();
        }

        leftWall = new CaveWall(0, 0, 55, 800);
        rightWall = new CaveWall(545, 0, 55, 800);
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
                        platform.getBounds().y,
                        platform.getBounds().width,
                        platform.getBounds().height
                );
            }
        }

        // Player
        g.setColor(Color.RED);

        g.fillRect(
                player.getBounds().x,
                player.getBounds().y,
                player.getBounds().width,
                player.getBounds().height
        );
    }

    public void update() {
        processInput();
    }

    private void processInput() {
        if (inputHandler.isUp()) {
            playerService.moveUp();
        }
    }
}