package Ui;

import Game.GameLoop;

import javax.swing.JFrame;
import java.awt.BorderLayout;

public class GameFrame extends JFrame {

    public GameFrame() {
        setTitle("Escape the Cave");
        setSize(620, 800);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);

        setLayout(new BorderLayout());

        GamePanel gamePanel = new GamePanel();

        GameLoop gameLoop = new GameLoop(gamePanel);
        Thread gameThread = new Thread(gameLoop);

        gameThread.start();

        add(gamePanel, BorderLayout.CENTER);
        add(new LorePanel(), BorderLayout.SOUTH);

        setVisible(true);

        gamePanel.requestFocusInWindow();
    }
}