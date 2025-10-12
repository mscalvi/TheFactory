using System;
using System.Collections.Generic;
using System.Linq;
using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IUnlockService
    {
        void Apply(GameModel m, string type, string id);
        void ApplyStageEntry(GameModel m, string stageId);
        void ApplyTechPurchase(GameModel m, string techId);
        void ApplyDestinationPurchase(GameModel m, string destId);
        void RecomputeUpgradesAvailability(GameModel m);
        void ApplyExpansionPurchase(GameModel m, string expId);
    }

    public sealed class UnlockService : IUnlockService
    {
        private readonly IStageService _stages;
        private readonly IUpgradeService _effects;

        public UnlockService(IStageService stages, IUpgradeService effects)
        {
            _stages = stages;
            _effects = effects;
        }

        public void Apply(GameModel m, string type, string id)
        {
            if (m is null || string.IsNullOrWhiteSpace(type)) return;
            switch (type.Trim().ToLowerInvariant())
            {
                case "stage": ApplyStageEntry(m, id); break;
                case "tech": ApplyTechPurchase(m, id); break;
                case "upgrade": RecomputeUpgradesAvailability(m); break;
            }
        }

        public void ApplyStageEntry(GameModel m, string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId)) return;
            if (!m.Stages.TryGetValue(stageId, out var st)) return;

            var destIds = DestinationData.Order
                .Select(id => DestinationData.GetDef(id))
                .Where(d => string.Equals(d.StageId, stageId, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.Id)
                .ToList();

            foreach (var did in destIds)
            {
                if (m.Destinations.TryGetValue(did, out var live) && live.Unlocked)
                {
                    ApplyDestinationPurchase(m, did);
                }
            }

            _effects.Recompute(m);
        }

        public void ApplyTechPurchase(GameModel m, string techId)
        {
            if (string.IsNullOrWhiteSpace(techId)) return;
            if (!m.Technologies.TryGetValue(techId, out var t)) return;

            t.Unlocked = true;
            t.Avaliable = false;

            RecomputeUpgradesAvailability(m);
            _effects.Recompute(m);
        }

        public void ApplyDestinationPurchase(GameModel m, string destId)
        {
            if (string.IsNullOrWhiteSpace(destId)) return;

            // 0) Valida destino e estágio
            DestinationModel dDef;
            try { dDef = DestinationData.GetDef(destId); } catch { return; }

            if (!m.Destinations.TryGetValue(destId, out var dLive))
            {
                dLive = new DestinationModel { Id = destId };
                m.Destinations[destId] = dLive;
            }

            // reforça estado do catálogo (sem rebaixar nada)
            dLive.Name = dDef.Name;
            dLive.Image = dDef.Image;
            dLive.StageId = dDef.StageId;
            dLive.Cost = dDef.Cost;
            dLive.CostResourceId = dDef.CostResourceId;

            // 1) Personagens com CharDestId == destId → ficam disponíveis para compra
            foreach (var (cid, ch) in m.Characters)
            {
                var cDef = CharacterData.GetDef(cid);
                if (cDef.CharDestId == destId && !ch.Unlocked)
                    ch.Avaliable = true;
            }

            // 2) Tecnologias com DestinationId == destId → ficam disponíveis para compra
            foreach (var (tid, t) in m.Technologies)
            {
                var tDef = TechData.GetDef(tid);
                if (tDef.DestinationId == destId && !t.Unlocked)
                    t.Avaliable = true;
            }

            // 3) Contratos da EXPEDIÇÃO do estágio do destino → ficam disponíveis
            //    (apenas na expedição ativa daquele stage)
            var stageId = dDef.StageId;
            if (!string.IsNullOrWhiteSpace(stageId) &&
                m.Stages.TryGetValue(stageId, out var st) &&
                st.Expedition is not null)
            {
                var ex = st.Expedition;
                foreach (var (contId, cont) in ex.Contracts)
                {
                    var cDef = ContractData.GetDef(contId);
                    if (cDef.ConDestId == destId)
                        cont.Avaliable = true;
                }
            }

            // 4) Expansões
            foreach (var (tid, t) in m.Expansions)
            {
                var tDef = TechData.GetDef(tid);
                if (tDef.DestinationId == destId && !t.Unlocked)
                    t.Avaliable = true;
            }

            _effects.Recompute(m);
        }

        public void ApplyExpansionPurchase(GameModel m, string expId)
        {
        }

        public void RecomputeUpgradesAvailability(GameModel m)
        {
            foreach (var u in m.Upgrades.Values)
            {
                // Se não há TechId, tratamos como "sem pré-requisito de tech"
                // → pode ficar disponível (respeitando Data e Max)
                if (string.IsNullOrWhiteSpace(u.TechId))
                {
                    // Mantém o que o Data já marcou e garante visibilidade até esgotar
                    u.Avaliable = (u.Avaliable || UpgradeData.GetDef(u.Id).Avaliable || true) && !u.IsMaxed;
                    continue;
                }

                // Com TechId: só habilita se tech estiver Unlocked
                if (m.Technologies.TryGetValue(u.TechId, out var t) && t.Unlocked)
                {
                    u.Avaliable = !u.IsMaxed;
                }
                else
                {
                    // NÃO derruba o que o Data trouxe: mantém se já era true
                    u.Avaliable = u.Avaliable && !u.IsMaxed;
                }
            }
        }

    }
}
