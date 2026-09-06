package Ui;

import javax.swing.*;
import java.awt.*;

public class LorePanel extends JPanel {

    private JLabel floorLabel;
    private JLabel scoreLabel;
    private JLabel recordLabel;

    public LorePanel() {

        setBackground(Color.DARK_GRAY);

        setPreferredSize(new Dimension(600, 150));

        setBorder(
                BorderFactory.createMatteBorder(
                        2, 0, 0, 0, Color.BLACK
                )
        );

        setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));

        floorLabel = new JLabel("Andar: 1");
        scoreLabel = new JLabel("Pontos: 0");
        recordLabel = new JLabel("Recorde: --");

        floorLabel.setForeground(Color.WHITE);
        scoreLabel.setForeground(Color.WHITE);
        recordLabel.setForeground(Color.WHITE);

        add(floorLabel);
        add(scoreLabel);
        add(recordLabel);
    }
}