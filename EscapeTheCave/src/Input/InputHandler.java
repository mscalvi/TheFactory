package Input;

import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;

public class InputHandler implements KeyListener {

    private boolean up;

    public boolean isUp() {
        return up;
    }

    @Override
    public void keyPressed(KeyEvent e) {

        System.out.println("Tecla pressionada: " + e.getKeyCode());

        if (e.getKeyCode() == KeyEvent.VK_UP) {
            up = true;
        }
    }

    @Override
    public void keyReleased(KeyEvent e) {
        if (e.getKeyCode() == KeyEvent.VK_UP) {
            up = false;
        }
    }

    @Override
    public void keyTyped(KeyEvent e) {
    }
}