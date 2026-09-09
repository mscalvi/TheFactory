package Ui;

import Entities.Record;
import Services.DataService;

import javax.swing.*;
import java.awt.*;

public class RankingPanel extends JPanel {

    private DataService dataService;

    private JButton backButton;

    public RankingPanel(DataService dataService) {

        this.dataService = dataService;

        setBackground(Color.BLACK);
        setLayout(new BorderLayout());

        JLabel titleLabel = new JLabel("RANKING");
        titleLabel.setForeground(Color.WHITE);
        titleLabel.setFont(new Font("Arial", Font.BOLD, 32));
        titleLabel.setHorizontalAlignment(SwingConstants.CENTER);

        add(titleLabel, BorderLayout.NORTH);

        JPanel rankingPanel = new JPanel();
        rankingPanel.setBackground(Color.BLACK);
        rankingPanel.setLayout(
                new BoxLayout(rankingPanel, BoxLayout.Y_AXIS)
        );

        for (int i = 1; i <= 20; i++) {

            Record record = dataService.getRecord(i);

            if (record == null) {
                break;
            }

            JLabel recordLabel = new JLabel(
                    i + ". " +
                            record.getPlayerName() +
                            "    " +
                            record.getScore()
            );

            recordLabel.setForeground(Color.WHITE);
            recordLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

            rankingPanel.add(recordLabel);
        }

        add(rankingPanel, BorderLayout.CENTER);

        backButton = new JButton("VOLTAR");
        add(backButton, BorderLayout.SOUTH);
    }

    public JButton getBackButton() {
        return backButton;
    }
}