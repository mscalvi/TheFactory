package Ui;

import Services.ScoreService;
import Services.DataService;
import Entities.Record;

import javax.swing.*;
import java.awt.*;

public class LorePanel extends JPanel {

    private JLabel scoreLabel;
    private JLabel recordLabel;

    private ScoreService scoreService;
    private DataService dataService;

    public LorePanel(ScoreService ScoreService, DataService DataService) {

        scoreService = ScoreService;
        dataService = DataService;

        setBackground(Color.DARK_GRAY);

        setPreferredSize(new Dimension(600, 150));

        setBorder(
                BorderFactory.createMatteBorder(
                        2, 0, 0, 0, Color.BLACK
                )
        );

        setLayout(new GridLayout(1, 2));

        JPanel infoPanel = new JPanel();

        infoPanel.setBackground(Color.DARK_GRAY);
        infoPanel.setLayout(new BoxLayout(infoPanel, BoxLayout.Y_AXIS));

        scoreLabel = new JLabel("Pontos: 0");
        recordLabel = new JLabel("Recorde: --");

        scoreLabel.setForeground(Color.WHITE);
        recordLabel.setForeground(Color.WHITE);

        infoPanel.add(scoreLabel);
        infoPanel.add(recordLabel);

        JPanel lorePanel = new JPanel();

        lorePanel.setBackground(Color.DARK_GRAY);
        lorePanel.setLayout(new BorderLayout());

        JLabel loreLabel = new JLabel(
                "<html><center>LORE<br><br>"
                        + "Você continua subindo a caverna..."
                        + "</center></html>"
        );

        loreLabel.setForeground(Color.WHITE);
        loreLabel.setHorizontalAlignment(SwingConstants.CENTER);

        lorePanel.add(loreLabel, BorderLayout.CENTER);

        add(infoPanel);
        add(lorePanel);
    }

    public void update() {

        scoreLabel.setText(
                "Pontos: " + scoreService.getScore()
        );

        Record record = dataService.getRecord(1);

        if (record != null) {

            recordLabel.setText(
                    "Recorde: "
                            + record.getScore()
                            + " - "
                            + record.getPlayerName()
            );

        } else {

            recordLabel.setText("Recorde: ---");
        }
    }
}

