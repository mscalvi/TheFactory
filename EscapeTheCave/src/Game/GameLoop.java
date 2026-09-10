package Game;

import Ui.GameFrame;
import Ui.GamePanel;
import Ui.LorePanel;
import Ui.GameOverPanel;

import Services.ScoreService;
import Services.DataService;

import javax.swing.SwingUtilities;
import javax.swing.JOptionPane;
import javax.swing.JTextField;
import javax.swing.text.PlainDocument;
import javax.swing.text.AttributeSet;
import javax.swing.text.BadLocationException;

public class GameLoop implements Runnable {

    private GamePanel gamePanel;
    private LorePanel lorePanel;
    private GameFrame gameFrame;

    private ScoreService scoreService;
    private DataService dataService;

    private boolean running;

    public GameLoop(
            GamePanel gamePanel,
            LorePanel lorePanel,
            GameFrame gameFrame,
            ScoreService scoreService,
            DataService dataService
    ) {
        this.gamePanel = gamePanel;
        this.lorePanel = lorePanel;
        this.gameFrame = gameFrame;
        this.scoreService = scoreService;
        this.dataService = dataService;

        running = true;
    }

    @Override
    public void run() {

        while (running) {

            gamePanel.update();
            lorePanel.update();

            if (gamePanel.getGameService().isGameOver()) {

                running = false;

                SwingUtilities.invokeLater(() -> {

                    String playerName = askPlayerName();

                    if (!playerName.isBlank()) {

                        scoreService.checkRecord(playerName);

                        dataService.saveRecord(
                                playerName,
                                scoreService.getScore(),
                                gamePanel.getPlayerFloor()
                        );
                    }

                    gameFrame.showGameOver();
                });

                break;
            }

            try {
                Thread.sleep(16);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    public void stop() {
        running = false;
    }

    public void start() {
        running = true;
    }

    private String askPlayerName() {

        JTextField textField = new JTextField(3);

        PlainDocument document = new PlainDocument() {

            @Override
            public void insertString(
                    int offset,
                    String text,
                    AttributeSet attributes
            ) throws BadLocationException {

                if (text == null) {
                    return;
                }

                text = text.replaceAll("[^a-zA-Z]", "");

                if (getLength() + text.length() <= 3) {
                    super.insertString(offset, text, attributes);
                }
            }
        };

        textField.setDocument(document);

        while (true) {

            int result = JOptionPane.showConfirmDialog(
                    gameFrame,
                    textField,
                    "Enter Name",
                    JOptionPane.OK_CANCEL_OPTION,
                    JOptionPane.PLAIN_MESSAGE
            );

            if (result == JOptionPane.OK_OPTION) {

                String name = textField.getText().trim();

                if (name.isBlank()) {
                    return "";
                }

                if (name.length() == 3) {
                    return name.toUpperCase();
                }

                JOptionPane.showMessageDialog(
                        gameFrame,
                        "Only 3 Letters.",
                        "Invalid Name",
                        JOptionPane.WARNING_MESSAGE
                );

            } else {
                return "";
            }
        }
    }
}