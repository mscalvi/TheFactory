using System.Collections.Generic;
using UnityEngine;

public class RangeViewService : MonoBehaviour
{
    public List<WeaponRoomInstance> Rooms;

    public Material LineMaterial;

    private List<LineRenderer> lines = new();

    public void Initialize(List<WeaponRoomInstance> rooms)
    {
        Rooms = rooms;

        DrawAll();
    }

    void DrawAll()
    {
        foreach (var room in Rooms)
        {
            DrawRoom(room);
        }
    }

    void DrawRoom(WeaponRoomInstance room)
    {
        DrawCircle(room.Weapon.Range);

        if (room.Angle < 360)
        {
            DrawAngle(room);
        }
    }

    void DrawCircle(double range)
    {
        int segments = 64;

        var obj = new GameObject("RangeCircle");
        obj.transform.parent = transform;

        var line = obj.AddComponent<LineRenderer>();
        SetupLine(line);

        float radius = (float)range * UiHelper.Scale;

        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;

            float x = Mathf.Sin(angle) * radius;
            float y = -Mathf.Cos(angle) * radius;

            line.SetPosition(i, new Vector3(x, y + 1, 0));
        }

        lines.Add(line);
    }

    void DrawAngle(WeaponRoomInstance room)
    {
        float radius = (float)room.Weapon.Range * UiHelper.Scale;

        DrawLine(Vector3.zero, AngleToPosition(room.AngleMin, radius));
        DrawLine(Vector3.zero, AngleToPosition(room.AngleMax, radius));
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        var obj = new GameObject("AngleLine");
        obj.transform.parent = transform;

        var line = obj.AddComponent<LineRenderer>();
        SetupLine(line);

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        lines.Add(line);
    }

    Vector3 AngleToPosition(double angle, float radius)
    {
        float rad = (float)(angle * Mathf.Deg2Rad);

        float x = Mathf.Sin(rad) * radius;
        float y = -Mathf.Cos(rad) * radius;

        return new Vector3(x, y + 1, 0);
    }

    void SetupLine(LineRenderer line)
    {
        line.material = LineMaterial;
        line.widthMultiplier = 0.005f;
        line.useWorldSpace = false;
        line.loop = false;
    }
}