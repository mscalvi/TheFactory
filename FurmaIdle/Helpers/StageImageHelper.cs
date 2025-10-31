using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Text;

public static class StageImageHelper
{
    public static string GetStageImagePath(GameModel game, StageModel stage)
    {
        if (stage?.Id is null || stage.Id.Length == 0)
            return "images/stages/unknown_0000.jpg";

        int width = 0;
        List<string> localsOrderForStage = new List<string>();

        // Pega o número final do Id do stage -> s00 = 0
        char stageDigit = stage.Id[^1];

        var stageLocals = game.Locals.Values
            .Where(l => !string.IsNullOrEmpty(l.Id) && l.Id.Length >= 2 && l.Id[1] == stageDigit)
            .OrderBy(l => l.Id)
            .ToList();

        if (width == 0)
            return $"images/stages/{stage.Id}_0000.jpg";

        var sb = new StringBuilder(width);

        foreach (var local in stageLocals)
        {
            bool unlocked = local.State == UnlockHelper.State.Unlocked;
            sb.Append(unlocked ? '1' : '0');
        }

        string mask = sb.ToString();
        return $"images/stages/{stage.Id}_{mask}.jpg";
    }
}