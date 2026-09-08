package Entities;

public class Elixir {

    public enum Type {
        PINK,
        GREEN
    }

    private Type type;
    private int position;

    public Elixir(Type type, int position) {
        this.type = type;
        this.position = position;
    }

    public Type getType() {
        return type;
    }

    public int getPosition() {
        return position;
    }
}