package Entities;

import java.awt.Rectangle;

public class Player {

    private int floor;
    private int position;

    private int x;
    private int y;
    private int width;
    private int height;

    public Player(int floor, int position, int x, int y, int width, int height) {
        this.floor = floor;
        this.position = position;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public Rectangle getBounds() {
        return new Rectangle(x, y, width, height);
    }

    public void setFloor(int floor) {
        this.floor = floor;
    }

    public void setPosition(int position) {
        this.position = position;
    }

    public int getFloor() {
        return floor;
    }

    public int getPosition() {
        return position;
    }
}