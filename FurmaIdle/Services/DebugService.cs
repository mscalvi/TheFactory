// Services/DebugDumpService.cs
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;

public interface IDebugService
{
    void BuildDumpSnapshot(int type, string origin);
}

public sealed class DebugService : IDebugService
{
    private readonly ICurrentGameService Game;
    private readonly IUiService Ui;
    private readonly IIncomeService Income;

    public DebugService(ICurrentGameService game, IUiService ui, IIncomeService income)
    {
        Game = game; 
        Ui = ui; 
        Income = income;
    }
    public void BuildDumpSnapshot(int type, string origin)
    {
        var g = Game.CurrentGame;

        if (g is null) { Console.WriteLine($"[SNAP:{origin}] Game=NULL"); return; }

        if (type == 0)
        {
            Console.WriteLine($"[DBG] {origin}");
            DebugCall(g);
        }
        else if (type == 1)
        {
            Console.WriteLine($"[DBG] {origin}");
            MenuChange(g);
        }
        else if (type == 2)
        {
            Console.WriteLine($"[DBG] {origin}");
            Click(g);
        }
        else
        {
            Console.WriteLine($"[DBG] {origin}");
        }
    }

    private void DebugCall(GameModel g)
    {
        Console.WriteLine($"Stage Selecionado = {g.SelectedStageId}");
    }

    private void MenuChange(GameModel g)
    {
        Console.WriteLine($"Menu: {Ui.PreviousMenuId} -> {Ui.OpenMenuId}");

        (IEnumerable<object>? items, Func<object, UnlockHelper.State>? state, string label) target =
            Ui.OpenMenuId switch
            {
                "i1" => (g.Stages?.Values.Cast<object>(), o => ((StageModel)o).State, "Stages"),
                "i2" => (g.Expansions?.Values.Cast<object>(), o => ((ExpansionModel)o).State, "Expansions"),
                "i4" => (g.Characters?.Values.Cast<object>(), o => ((CharacterModel)o).State, "Characters"),
                "i5" => (g.Techs?.Values.Cast<object>(), o => ((TechModel)o).State, "Techs"),
                "i6" => (g.Locals?.Values.Cast<object>(), o => ((LocalModel)o).State, "Locals"),
                "i7" => (g.Upgrades?.Values.Cast<object>(), o => ((UpgradeModel)o).State, "Upgrades"),
                _ => (null, null, "")
            };

        if (target.items is null || target.state is null) return;

        var (u, a, b) = CountStates(target.items, target.state);
        Console.WriteLine($"{target.label}: Unlocked = {u} Avaliable = {a} Blocked = {b} (Total = {u + a + b})");
    }

    private static (int u, int a, int b) CountStates(
        IEnumerable<object> items,
        Func<object, UnlockHelper.State> getState)
    {
        int u = 0, a = 0, b = 0;
        foreach (var it in items)
        {
            switch (getState(it))
            {
                case UnlockHelper.State.Unlocked: u++; break;
                case UnlockHelper.State.Available: a++; break;
                case UnlockHelper.State.Blocked: b++; break;
            }
        }
        return (u, a, b);
    }

    private void Click(GameModel g)
    {
        Console.WriteLine($"Click no Stage = {g.SelectedStageId}");
        Console.WriteLine($"Valor do Click = {Income.AddAmount}");
    }

}
