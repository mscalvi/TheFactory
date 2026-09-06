package Input;

import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;

public class InputHandler implements KeyListener {

    private boolean upPressed;
    private boolean rightPressed;
    private boolean leftPressed;

    public boolean consumeUp() {
        if (upPressed) {
            upPressed = false;
            return true;
        }

        return false;
    }
    public boolean consumeLeft() {
        if (leftPressed) {
            leftPressed = false;
            return true;
        }

        return false;
    }
    public boolean consumeRight() {
        if (rightPressed) {
            rightPressed = false;
            return true;
        }

        return false;
    }

    @Override
    public void keyPressed(KeyEvent e) {

        if (e.getKeyCode() == KeyEvent.VK_UP) {
            upPressed = true;
        }

        if (e.getKeyCode() == KeyEvent.VK_LEFT) {
            leftPressed = true;
        }

        if (e.getKeyCode() == KeyEvent.VK_RIGHT) {
            rightPressed = true;
        }
    }

    @Override
    public void keyReleased(KeyEvent e) {
    }

    @Override
    public void keyTyped(KeyEvent e) {
    }
}