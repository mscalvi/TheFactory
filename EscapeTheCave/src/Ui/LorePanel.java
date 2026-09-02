package Ui;

import javax.swing.JPanel;
import java.awt.Color;
import java.awt.Dimension;

public class LorePanel extends JPanel {

    public LorePanel() {
        setBackground(Color.DARK_GRAY);
        setPreferredSize(new Dimension(600, 150));

        setBorder(
                javax.swing.BorderFactory.createMatteBorder(
                        2, 0, 0, 0, Color.BLACK
                )
        );
    }
}