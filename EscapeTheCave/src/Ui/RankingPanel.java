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

        // TÍTULO
        JLabel titleLabel = new JLabel("RANKING");
        titleLabel.setForeground(Color.WHITE);
        titleLabel.setFont(new Font("Arial", Font.BOLD, 32));
        titleLabel.setHorizontalAlignment(SwingConstants.CENTER);

        titleLabel.setBorder(
                BorderFactory.createEmptyBorder(
                        20, 0, 20, 0
                )
        );

        add(titleLabel, BorderLayout.NORTH);

        // RANKING
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

            JPanel row = new JPanel(new BorderLayout());

            row.setBackground(Color.DARK_GRAY);
            row.setMaximumSize(
                    new Dimension(450, 35)
            );

            row.setBorder(
                    BorderFactory.createEmptyBorder(
                            5, 15, 5, 15
                    )
            );

            JLabel positionLabel =
                    new JLabel(i + ".");

            JLabel nameLabel =
                    new JLabel(record.getPlayerName());

            JLabel scoreLabel =
                    new JLabel(String.valueOf(record.getScore()));

            positionLabel.setForeground(Color.WHITE);
            nameLabel.setForeground(Color.WHITE);
            scoreLabel.setForeground(Color.WHITE);

            positionLabel.setFont(
                    new Font("Arial", Font.BOLD, 16)
            );

            nameLabel.setFont(
                    new Font("Arial", Font.BOLD, 16)
            );

            scoreLabel.setFont(
                    new Font("Arial", Font.BOLD, 16)
            );

            row.add(positionLabel, BorderLayout.WEST);
            row.add(nameLabel, BorderLayout.CENTER);
            row.add(scoreLabel, BorderLayout.EAST);

            rankingPanel.add(row);

            rankingPanel.add(
                    Box.createRigidArea(
                            new Dimension(0, 5)
                    )
            );
        }

        // CENTRALIZA O RANKING
        JPanel centerPanel = new JPanel();
        centerPanel.setBackground(Color.BLACK);
        centerPanel.setLayout(new BoxLayout(
                centerPanel,
                BoxLayout.Y_AXIS
        ));

        centerPanel.add(Box.createVerticalGlue());
        centerPanel.add(rankingPanel);
        centerPanel.add(Box.createVerticalGlue());

        add(centerPanel, BorderLayout.CENTER);

        // BOTÃO
        backButton = new JButton("VOLTAR");

        backButton.setFont(
                new Font("Arial", Font.BOLD, 16)
        );

        backButton.setFocusPainted(false);

        backButton.setPreferredSize(
                new Dimension(120, 40)
        );

        JPanel buttonPanel = new JPanel();
        buttonPanel.setBackground(Color.BLACK);

        buttonPanel.setBorder(
                BorderFactory.createEmptyBorder(
                        15, 0, 20, 0
                )
        );

        buttonPanel.add(backButton);

        add(buttonPanel, BorderLayout.SOUTH);
    }

    public JButton getBackButton() {
        return backButton;
    }
}