// Services/TooltipService.cs
using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurmaIdle.Services
{
    public enum HoverType { Character, Specialty, Tech, Local, Upgrade, Contract }
    public sealed record HoverTip(string Title, string Body);

    public interface ITooltipService
    {
        HoverTip GetHover(HoverType type, string id, string? stageId = null);

        HoverTip? Current { get; }
        void Show(HoverTip tip);
        void Clear();
        event Action? Changed;
    }

    public sealed class TooltipService : ITooltipService
    {
        private readonly ICurrentGameService _game;
        public HoverTip? Current { get; private set; }
        public event Action? Changed;

        public TooltipService(ICurrentGameService game)
        {
            _game = game;
        }

        public void Show(HoverTip tip)
        {
            Current = tip;
            Changed?.Invoke();
        }

        public void Clear()
        {
            if (Current is null) return;
            Current = null;
            Changed?.Invoke();
        }

        public HoverTip GetHover(HoverType type, string id, string? stageId = null)
        {
            var g = _game.CurrentGame;
            return type switch
            {
                HoverType.Character => BuildCharacterHover(id, g),
                //HoverType.Specialty => BuildSpecialtyHover(id, g),
                //HoverType.Tech => BuildTechHover(id, g),
                //HoverType.Local => BuildLocalHover(id, g),
                HoverType.Upgrade => BuildUpgradeHover(id, g),
                _ => new HoverTip("—", "—")
            };
        }

        // Character
        private HoverTip BuildCharacterHover(string id, GameModel game)
        {
            game.Characters.TryGetValue(id, out var Character);

            CharacterModel? def = null;
            try { def = CharacterData.GetDef(id); } catch { /* seguro */ }

            return new HoverTip($"Personagem ({Character.Id})", $"{Character.Name}");
        }

        // Upgrade
        private HoverTip BuildUpgradeHover(string id, GameModel game)
        {
            game.Upgrades.TryGetValue(id, out var Upgrade);

            UpgradeModel? def = null;
            try { def = UpgradeData.GetDef(id); } catch { /* seguro */ }

            return new HoverTip($"Upgrade ({Upgrade.Id})", $"{Upgrade.Name}");
        }
    }
}
