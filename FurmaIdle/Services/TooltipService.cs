// Services/TooltipService.cs
using System.Linq;
using System.Collections.Generic;
using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public enum HoverType { Personagem, Especialidade, Tecnologia, Destino, Melhoria }

    public sealed record HoverTip(string Title, string Body);

    public interface ITooltipService
    {
        HoverTip GetHover(HoverType type, string id);
    }

    public sealed class TooltipService : ITooltipService
    {
        private readonly IGameService _game;
        public TooltipService(IGameService game) => _game = game;

        public HoverTip GetHover(HoverType type, string id)
        {
            switch (type)
            {
                case HoverType.Personagem:
                    return BuildPersonHover(id);
                case HoverType.Especialidade:
                    return BuildEspecialidadeHover(id);
                case HoverType.Tecnologia:
                    return BuildTecnologiaHover(id);
                case HoverType.Destino:
                    return BuildDestinoHover(id);
                case HoverType.Melhoria:
                    return BuildMelhoriaHover(id);
                default:
                    return new HoverTip("—", "—");
            }
        }

        // ====== Implementações atuais ======
        private HoverTip BuildPersonHover(string id)
        {
            // estado vivo
            _game.Current.Characters.TryGetValue(id, out var live);
            // catálogo imutável
            var def = CharacterData.All.TryGetValue(id, out var d) ? d : null;

            var name = (!string.IsNullOrWhiteSpace(def?.Name)) ? def!.Name
                      : (!string.IsNullOrWhiteSpace(live?.Name)) ? live!.Name
                      : id;

            // Mostra algo útil já: conhecimentos e specialty id (ids por enquanto)
            var knows = (live ?? def) is CharacterModel ch
                ? string.Join(", ", new[] { ch.MainKnowId, ch.SecondKnowId }.Where(s => !string.IsNullOrWhiteSpace(s)))
                : "—";

            var contracts = (live ?? def)?.KnowContractsIds;
            var contractsStr = (contracts != null && contracts.Count > 0) ? string.Join(", ", contracts) : "—";

            var spec = (live ?? def)?.SpecialtyId ?? "—";

            var body =
                $"Conhecimentos: {knows}\n" +
                $"Contratos Disponíveis: {contractsStr}\n" +
                $"Especialidade: {spec}";

            return new HoverTip($"{name} ({id})", body);
        }

        private HoverTip BuildEspecialidadeHover(string id)
        {
            // por enquanto é placeholder, mas já recebe o id do personagem
            return new HoverTip($"Especialidade de {id}", "especialidade vai ficar aqui");
        }

        private HoverTip BuildTecnologiaHover(string id)
        {
            // por enquanto é placeholder, mas já recebe o id do personagem
            return new HoverTip($"Tecnologia {id}", "");
        }

        private HoverTip BuildDestinoHover(string id)
        {
            // por enquanto é placeholder, mas já recebe o id do personagem
            return new HoverTip($"Destino {id}", "");
        }

        private HoverTip BuildMelhoriaHover(string id)
        {
            // por enquanto é placeholder, mas já recebe o id do personagem
            return new HoverTip($"Melhoria {id}", "");
        }
    }
}
