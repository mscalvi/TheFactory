package Ui;

import Services.ScoreService;
import Services.DataService;
import Services.LoreService;

import Entities.Record;

import javax.swing.*;
import java.awt.*;

public class LorePanel extends JPanel {

    private JLabel scoreLabel;
    private JLabel recordLabel;
    private JLabel loreLabel;

    private ScoreService scoreService;
    private DataService dataService;
    private LoreService loreService;

    public LorePanel(ScoreService ScoreService, DataService DataService, LoreService LoreService) {

        scoreService = ScoreService;
        dataService = DataService;
        loreService = LoreService;

        setBackground(Color.DARK_GRAY);

        setPreferredSize(new Dimension(600, 150));

        setBorder(
                BorderFactory.createMatteBorder(
                        2, 0, 0, 0, Color.BLACK
                )
        );

        setLayout(new GridLayout(1, 2, 10, 0));

        // INFORMAÇÕES

        JPanel infoPanel = new JPanel();

        infoPanel.setBackground(Color.DARK_GRAY);
        infoPanel.setLayout(
                new BoxLayout(infoPanel, BoxLayout.Y_AXIS)
        );

        scoreLabel = new JLabel("PONTOS: 0");
        recordLabel = new JLabel("RECORDE: ---");

        scoreLabel.setForeground(Color.WHITE);
        recordLabel.setForeground(Color.WHITE);

        scoreLabel.setFont(
                new Font("Arial", Font.BOLD, 24)
        );

        recordLabel.setFont(
                new Font("Arial", Font.BOLD, 18)
        );

        scoreLabel.setAlignmentX(Component.CENTER_ALIGNMENT);
        recordLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

        infoPanel.add(Box.createVerticalGlue());
        infoPanel.add(scoreLabel);
        infoPanel.add(Box.createRigidArea(new Dimension(0, 15)));
        infoPanel.add(recordLabel);
        infoPanel.add(Box.createVerticalGlue());

        // LORE

        JPanel lorePanel = new JPanel();

        lorePanel.setBackground(Color.DARK_GRAY);
        lorePanel.setLayout(new BorderLayout());

        loreLabel = new JLabel(
                "<html><center>"
                        + "<font size='5'><b>LORE</b></font>"
                        + "<br><br>"
                        + "..."
                        + "</center></html>"
        );

        loreLabel.setForeground(Color.WHITE);
        loreLabel.setHorizontalAlignment(
                SwingConstants.CENTER
        );

        lorePanel.add(
                loreLabel,
                BorderLayout.CENTER
        );

        add(infoPanel);
        add(lorePanel);
    }

    public void update() {

        scoreLabel.setText(
                "PONTOS: " + scoreService.getScore()
        );

        Record record = dataService.getRecord(1);

        if (record != null) {

            recordLabel.setText(
                    "RECORDE: "
                            + record.getScore()
                            + " - "
                            + record.getPlayerName()
            );

        } else {

            recordLabel.setText("RECORDE: ---");
        }

        loreLabel.setText(
                "<html><center>"
                        + "<font size='5'><b>LORE</b></font>"
                        + "<br><br>"
                        + loreService.getCurrentLore()
                        + "</center></html>"
        );
    }
}