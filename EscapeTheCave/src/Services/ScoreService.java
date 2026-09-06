package Services;

public class ScoreService {

    private int score;

    public ScoreService() {
        score = 0;
    }

    public void addFloor() {
        score++;
    }

    public void addTonic() {
        score += 10;
    }

    public int getScore() {
        return score;
    }
}