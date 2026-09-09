package Entities;

public class Record {

    private String playerName;
    private int score;
    private int floor;

    public Record(String playerName, int score, int floor) {
        this.playerName = playerName;
        this.score = score;
        this.floor = floor;
    }

    public String getPlayerName() {
        return playerName;
    }

    public int getScore() {
        return score;
    }

    public int getFloor() {
        return floor;
    }
}