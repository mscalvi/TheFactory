package Ui;

import Services.DataService;
import Entities.Record;

import javax.swing.*;
import java.awt.*;

public class LandingPanel extends JPanel {

    private JButton playButton;
    private JButton rankingButton;

    private JLabel recordLabel;

    private DataService dataService;

    public LandingPanel(DataService dataService){

        this.dataService = dataService;

        setBackground(Color.BLACK);
        setLayout(new GridBagLayout());

        JPanel content = new JPanel();
        content.setBackground(Color.BLACK);
        content.setLayout(new BoxLayout(content, BoxLayout.Y_AXIS));

        JLabel titleLabel = new JLabel("ESCAPE THE CAVE");
        titleLabel.setForeground(Color.WHITE);
        titleLabel.setFont(new Font("Arial", Font.BOLD, 32));
        titleLabel.setAlignmentX(Component.CENTER_ALIGNMENT);

        playButton = new JButton("JOGAR");
        playButton.setAlignmentX(Component.CENTER_ALIGNMENT);

        rankingButton = new JButton("RANKING");
        rankingButton.setAlignmentX(Component.CENTER_ALIGNMENT);

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

        content.add(titleLabel);
        content.add(Box.createVerticalStrut(40));
        content.add(playButton);
        content.add(Box.createVerticalStrut(20));
        content.add(rankingButton);
        content.add(Box.createVerticalStrut(20));
        content.add(recordLabel);

        add(content);
    }

    public JButton getPlayButton() {
        return playButton;
    }

    public JButton getRankingButton() {
        return rankingButton;
    }
}
