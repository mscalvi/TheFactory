package Services;

import Entities.Record;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class DataService {

    private Connection connection;

    public void connect() {

        try {
            connection = DriverManager.getConnection(
                    "jdbc:sqlite:escape_the_cave.db"
            );

            System.out.println("Banco conectado!");

        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public Connection getConnection() {
        return connection;
    }

    public void createTables() {

        String sql = """
            CREATE TABLE IF NOT EXISTS records (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_name TEXT NOT NULL,
                score INTEGER NOT NULL,
                floor INTEGER NOT NULL
            )
            """;

        try {
            connection.createStatement().execute(sql);

            System.out.println("Tabela de recordes criada!");

        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public void saveRecord(String playerName, int score, int floor) {

        String sql = """
            INSERT INTO records (player_name, score, floor)
            VALUES (?, ?, ?)
            """;

        try (var statement = connection.prepareStatement(sql)) {

            statement.setString(1, playerName);
            statement.setInt(2, score);
            statement.setInt(3, floor);

            statement.executeUpdate();

            System.out.println("Partida salva!");

        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public Record getRecord(int position) {

        String sql = """
            SELECT player_name, score, floor
            FROM records
            ORDER BY score DESC
            LIMIT 1 OFFSET ?
            """;

        try (var statement = connection.prepareStatement(sql)) {

            statement.setInt(1, position - 1);

            try (var result = statement.executeQuery()) {

                if (result.next()) {

                    return new Record(
                            result.getString("player_name"),
                            result.getInt("score"),
                            result.getInt("floor")
                    );
                }
            }

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }
}