package Services;

public class ScoreService {

    private int score;

    public ScoreService() {
        score = 0;
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

    public int getScore() {
        return score;
    }
}