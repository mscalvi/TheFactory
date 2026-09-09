package Ui;

import Game.GameLoop;

import Services.ScoreService;
import Services.DataService;

import javax.swing.*;
import java.awt.*;

public class GameFrame extends JFrame {

    private ScoreService scoreService;
    private DataService dataService;

    public GameFrame(DataService dataService) {

        this.dataService = dataService;

        setTitle("Escape the Cave");
        setSize(620, 800);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);

        setLayout(new BorderLayout());

        scoreService = new ScoreService();

        showLanding();

        setVisible(true);
    }

    private void showLanding() {
        getContentPane().removeAll();

        LandingPanel landingPanel = new LandingPanel(dataService);

        landingPanel.getPlayButton().addActionListener(e -> startGame());
        landingPanel.getRankingButton().addActionListener(e -> showRanking());

        add(landingPanel, BorderLayout.CENTER);

        revalidate();
        repaint();
    }

    private void startGame() {
        getContentPane().removeAll();

        scoreService.resetScore();

        GamePanel gamePanel = new GamePanel(scoreService);

        LorePanel lorePanel = new LorePanel(scoreService, dataService);

        GameLoop gameLoop = new GameLoop(gamePanel, lorePanel, this, scoreService, dataService);

        Thread gameThread = new Thread(gameLoop);

        gameThread.start();

        add(gamePanel, BorderLayout.CENTER);
        add(lorePanel, BorderLayout.SOUTH);

        revalidate();
        repaint();

        gamePanel.requestFocusInWindow();
    }

    public void showGameOver() {
        SwingUtilities.invokeLater(() -> {
            getContentPane().removeAll();
            GameOverPanel gameOverPanel = new GameOverPanel(scoreService, dataService);
            gameOverPanel.getPlayAgainButton() .addActionListener(e -> startGame());
            gameOverPanel.getReturnButton() .addActionListener(e -> showLanding());
            add(gameOverPanel, BorderLayout.CENTER); revalidate();
            repaint();
        });
    }

    private void showRanking() {
        getContentPane().removeAll();

        RankingPanel rankingPanel =
                new RankingPanel(dataService);

        rankingPanel.getBackButton()
                .addActionListener(e -> showLanding());

        add(rankingPanel, BorderLayout.CENTER);

        revalidate();
        repaint();
    }

}
