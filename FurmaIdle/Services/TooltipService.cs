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
    public enum HoverType { Character, Specialty, Tech, Local, Upgrade, Contract, Stage, Expansion, Expedition, Knowledge, Coins, Resources }
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
        private readonly ILocateService _locate;

        public HoverTip? Current { get; private set; }
        public event Action? Changed;

        public TooltipService(ICurrentGameService game, ILocateService locate)
        {
            _game = game;
            _locate = locate;
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
                HoverType.Contract => BuildContractHover(id, g),
                HoverType.Specialty => BuildSpecialtyHover(id, g),
                HoverType.Tech => BuildTechHover(id, g),
                HoverType.Local => BuildLocalHover(id, g),
                HoverType.Upgrade => BuildUpgradeHover(id, g),
                HoverType.Knowledge => BuildKnowledgeHover(id, g),
                HoverType.Resources => BuildResourcesHover(id, g),
                HoverType.Coins => BuildCoinsHover(id, g),
                _ => new HoverTip("—", "—")
            };
        }

        // Character
        private HoverTip BuildCharacterHover(string id, GameModel game)
        {
            game.Characters.TryGetValue(id, out var character);

            return new HoverTip($"Character ({character.Name})", $"{character.State}");
        }

        // Upgrade
        private HoverTip BuildUpgradeHover(string id, GameModel game)
        {
            game.Upgrades.TryGetValue(id, out var upgrade);

            return new HoverTip($"Upgrade ({upgrade.Name})", $"{upgrade.Description}");
        }

        // Contract
        private HoverTip BuildContractHover(string id, GameModel game)
        {
            game.Contracts.TryGetValue(id, out var contract);

            return new HoverTip($"Contract ({contract.Name})", $"{contract.State}");
        }

        // Local
        private HoverTip BuildLocalHover(string id, GameModel game)
        {
            game.Locals.TryGetValue(id, out var local);

            return new HoverTip($"Local ({local.Name})", $"{local.State}");
        }

        // Techs
        private HoverTip BuildTechHover(string id, GameModel game)
        {
            game.Techs.TryGetValue(id, out var tech);

            return new HoverTip($"Tech ({tech.Name})", $"{tech.State}");
        }

        // Techs
        private HoverTip BuildKnowledgeHover(string id, GameModel game)
        {
            game.Knowledges.TryGetValue(id, out var know);

            return new HoverTip($"Knowledge ({know.Name})", $"{know.State}");
        }

        // Techs
        private HoverTip BuildSpecialtyHover(string id, GameModel game)
        {
            game.Specialties.TryGetValue(id, out var specialty);

            return new HoverTip($"Specialty ({specialty.Name})", $"{specialty.Description}");
        }

        // Resources
        private HoverTip BuildResourcesHover(string id, GameModel game)
        {
            game.Resources.TryGetValue(id, out var resource);

            return new HoverTip($"Resource ({resource.Name})", $"{resource.Lore}");
        }

        // Coins
        private HoverTip BuildCoinsHover(string id, GameModel game)
        {
            game.Coins.TryGetValue(id, out var coin);

            return new HoverTip($"Coin ({coin.Name})", $"{coin.Lore}");
        }
    }
}
