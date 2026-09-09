package Ui;

import Services.ScoreService;
import Services.DataService;
import Entities.Record;

import javax.swing.*;
import java.awt.*;

public class GameOverPanel extends JPanel {

    private JLabel scoreLabel;
    private JLabel recordLabel;

    private JButton playAgainButton;
    private JButton returnButton;

    private ScoreService scoreService;
    private DataService dataService;

    public GameOverPanel(ScoreService scoreService, DataService dataService) {

        this.scoreService = scoreService;
        this.dataService = dataService;

        setBackground(Color.BLACK);
        setLayout(new GridBagLayout());

        JPanel content = new JPanel();
        content.setBackground(Color.BLACK);
        content.setLayout(new BoxLayout(content, BoxLayout.Y_AXIS));

        JLabel titleLabel = new JLabel("GAME OVER");

        titleLabel.setForeground(Color.WHITE);
        titleLabel.setFont(
                new Font("Arial", Font.BOLD, 32)
        );
        titleLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

        scoreLabel = new JLabel(
                "Pontos: " + scoreService.getScore()
        );

        scoreLabel.setForeground(Color.WHITE);
        scoreLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

        Record record = dataService.getRecord(1);

        if (record != null) {

            recordLabel = new JLabel(
                    "Recorde: "
                            + record.getScore()
                            + " - "
                            + record.getPlayerName()
            );

        } else {

            recordLabel = new JLabel("Recorde: ---");
        }

        recordLabel.setForeground(Color.WHITE);
        recordLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

        playAgainButton = new JButton("JOGAR NOVAMENTE");
        playAgainButton.setAlignmentX(Component.CENTER_ALIGNMENT);

        returnButton = new JButton("MENU PRINCIPAL");
        returnButton.setAlignmentX(Component.CENTER_ALIGNMENT);

        content.add(titleLabel);
        content.add(Box.createVerticalStrut(30));

        content.add(scoreLabel);
        content.add(Box.createVerticalStrut(10));

        content.add(recordLabel);
        content.add(Box.createVerticalStrut(30));

        content.add(playAgainButton);
        content.add(Box.createVerticalStrut(10));

        content.add(returnButton);
        content.add(Box.createVerticalStrut(10));

        add(content);
    }

    public JButton getPlayAgainButton() {
        return playAgainButton;
    }

    public JButton getReturnButton() {
        return returnButton;
    }
}

