import Ui.GameFrame;
import Services.DataService;

public class Main {
    public static void main(String[] args) {

        DataService dataService = new DataService();

        dataService.connect();
        dataService.createTables();

        new GameFrame(dataService);
    }
}