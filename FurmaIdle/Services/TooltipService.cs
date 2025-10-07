// Services/TooltipService.cs
using System.Linq;
using System.Collections.Generic;
using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface ITooltipService
    {
        TooltipModel GetCharacterTooltip(string charId);
    }

    public sealed class TooltipService : ITooltipService
    {
        private readonly IGameService _game;

        public TooltipService(IGameService game) => _game = game;

        public TooltipModel GetCharacterTooltip(string charId)
        {
            // estado vivo (save)
            _game.Current.Characters.TryGetValue(charId, out var live);

            // catálogo base (imutável)
            var def = CharacterData.GetDef(charId);

            // Nome (prioriza catálogo; se algum dia permitir rename in-game, ajuste)
            var name = !string.IsNullOrWhiteSpace(def.Name) ? def.Name : (live?.Name ?? charId);

            // Montagem dos campos (IDs por enquanto; depois mapeia p/ nomes nos Resolve*)
            var conhecimentos = ResolveConhecimentos(live ?? def);
            var contratos = ResolveContratos(live ?? def);
            var traco = ResolveTraco(live ?? def);
            var especialidade = ResolveEspecialidade(live ?? def);

            return new TooltipModel(
                Id: charId,
                Name: name,
                Conhecimentos: conhecimentos,
                ContratosDisponiveis: contratos,
                Traco: traco,
                Especialidade: especialidade
            );
        }

        private static string ResolveConhecimentos(CharacterModel c)
        {
            var parts = new[] { c.MainKnowId, c.SecondKnowId }
                        .Where(s => !string.IsNullOrWhiteSpace(s));
            return parts.Any() ? string.Join(", ", parts) : "—";
        }

        private static string ResolveContratos(CharacterModel c)
        {
            var list = c.KnowContractsIds ?? new List<string>();
            return list.Count > 0 ? string.Join(", ", list) : "—";
        }

        private static string ResolveTraco(CharacterModel c)
        {
            // Usa Name do Trait se houver; senão Id; senão "—"
            return c.Trait?.Name ?? c.Trait?.Id ?? "—";
        }

        private static string ResolveEspecialidade(CharacterModel c)
        {
            return string.IsNullOrWhiteSpace(c.SpecialtyId) ? "—" : c.SpecialtyId;
        }
    }
}
