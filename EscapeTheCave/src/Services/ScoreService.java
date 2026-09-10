package Services;

public class ScoreService {

    private int score;
    private int record;
    private String recordName;

    public ScoreService() {
        score = 0;
        record = 0;
        recordName = "AAA";
    }

    public void addFloor() {
        score++;
    }

    public void addPinkElixir() {
        score += 10;
    }

    public void addGreenElixir() {
        score += 5;
    }

    public void checkRecord(String playerName) {
        if (score > record) {
            record = score;
            recordName = playerName;
        }
    }

    public int getScore() {
        return score;
    }

    public void resetScore() {
        score = 0;
    }
}
