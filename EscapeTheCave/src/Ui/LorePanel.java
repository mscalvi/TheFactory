package Ui;

import Services.ScoreService;

import javax.swing.*;
import java.awt.*;

public class LorePanel extends JPanel {

    private JLabel scoreLabel;
    private JLabel recordLabel;

    private ScoreService scoreService;

    public LorePanel(ScoreService ScoreService) {

        setBackground(Color.DARK_GRAY);

        setPreferredSize(new Dimension(600, 150));

        setBorder(
                BorderFactory.createMatteBorder(
                        2, 0, 0, 0, Color.BLACK
                )
        );

        setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));

        scoreLabel = new JLabel("Pontos: 0");
        recordLabel = new JLabel("Recorde: --");

        scoreLabel.setForeground(Color.WHITE);
        recordLabel.setForeground(Color.WHITE);

        add(scoreLabel);
        add(recordLabel);

        scoreService = ScoreService;
    }

    public void update() {
        System.out.println(scoreService.getScore());
        scoreLabel.setText("Pontos: " + scoreService.getScore());
    }
}