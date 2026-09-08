package Ui;

import Game.GameLoop;
import Services.ScoreService;

import javax.swing.JFrame;
import java.awt.BorderLayout;

public class GameFrame extends JFrame {

    public GameFrame() {
        setTitle("Escape the Cave");
        setSize(620, 800);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);

        setLayout(new BorderLayout());

        ScoreService scoreService = new ScoreService();

        GamePanel gamePanel = new GamePanel(scoreService);
        LorePanel lorePanel = new LorePanel(scoreService);

        GameLoop gameLoop = new GameLoop(gamePanel, lorePanel);
        Thread gameThread = new Thread(gameLoop);

        gameThread.start();

        add(gamePanel, BorderLayout.CENTER);
        add(lorePanel, BorderLayout.SOUTH);

        setVisible(true);

        gamePanel.requestFocusInWindow();
    }
}