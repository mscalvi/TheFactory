using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Storage;

namespace FurmaIdle.Services
{
    public interface ICreateGameService
    {
        GameModel NewGame { get; }
        Task<GameModel> InitAsync();
    }
    public sealed class CreateGameService : ICreateGameService
    {
        // Método para Salvar o Novo Jogo
        private readonly IGameStore Store;
        private readonly ICurrentGameService CurrentGame;
        private readonly IUnlockService Unlock;
        public GameModel NewGame { get; private set; } = new();

        public CreateGameService(IGameStore store, ICurrentGameService current, IUnlockService unlock)
        {
            Store = store;
            CurrentGame = current;
            Unlock = unlock;
        }

        public async Task<GameModel> InitAsync()
        {
            Console.WriteLine("[CGS] Init: start");

            var loaded = await Store.LoadAsync("main");

            if (loaded == null)
            {
                NewGame = new GameModel
                {
                    SchemaVersion = 1,
                    LastTick = DateTime.UtcNow,
                    Characters = Seed("Characters", () => CharacterData.CreateInitialStates()),
                    Clicks = Seed("Clicks", () => ClickData.CreateInitialStates()),
                    Coins = Seed("Coins", () => CoinsData.CreateInitialStates()),
                    Contracts = Seed("Contracts", () => ContractData.CreateInitialStates()),
                    Expansions = Seed("Expansions", () => ExpansionData.CreateInitialStates()),
                    Knowledges = Seed("Knowledges", () => KnowledgeData.CreateInitialStates()),
                    Locals = Seed("Locals", () => LocalData.CreateInitialStates()),
                    Resources = Seed("Resources", () => ResourceData.CreateInitialStates()),
                    Specialties = Seed("Specialties", () => SpecialtyData.CreateInitialStates()),
                    Stages = Seed("Stages", () => StageData.CreateInitialStates()),
                    Techs = Seed("Techs", () => TechData.CreateInitialStates()),
                    Traits = Seed("Traits", () => TraitData.CreateInitialStates()),
                    Upgrades = Seed("Upgrades", () => UpgradeData.CreateInitialStates()),
                };

                CurrentGame.Attach(NewGame);

                await Store.SaveAsync(NewGame, "main");

                Console.WriteLine($"[CGS] New game counts: stages={NewGame.Stages?.Count}, locals={NewGame.Locals?.Count}, selected={NewGame.SelectedStageId}");

                NewGame.On = true;

                await Unlock.UnlockStage("s00");
                await Unlock.UnlockExpansion("x00");

                Console.WriteLine($"[CGS] Estado inicial carregado");

                return NewGame;
            }else 
            {
                // Try Load
                if (loaded.Stages.TryGetValue("s00", out var st0))
                {
                    if (st0.State == UnlockHelper.State.Blocked)
                    {
                        loaded = null;
                        return loaded;
                    }
                    else
                    {
                        Console.WriteLine($"[CGS] Loaded counts: stages={loaded.Stages?.Count}, locals={loaded.Locals?.Count}, selected={loaded.SelectedStageId}");

                        var backfilled = BackfillLoad(loaded);
                        Console.WriteLine($"[CGS] Backfill: changed={backfilled}");

                        CurrentGame.Attach(loaded);
                        Console.WriteLine("[CGS] After attach: ok");

                        if (backfilled)
                        {
                            await Store.SaveAsync(loaded);
                            Console.WriteLine("[CGS] Saved after backfill");
                        }

                        Console.WriteLine($"InitAsync: loaded save. Stages={loaded.Stages?.Count}");

                        loaded.On = true;
                        return loaded;
                    }
                } else
                {
                    loaded = null;
                    return loaded;
                }
            }
        }


        private static T Seed<T>(string name, Func<T> factory)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = factory();
                sw.Stop();
                Console.WriteLine($"[CreateGameService] {name} ok ({sw.ElapsedMilliseconds} ms)");
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CreateGameService] ERRO em {name}: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static bool BackfillLoad(GameModel g)
        {
            bool changed = false;

            // ---------- STAGES ----------
            foreach (var id in StageData.ShowOrder)
            {
                if (!g.Stages.TryGetValue(id, out var st))
                {
                    var def = StageData.GetDef(id);
                    g.Stages[id] = st = new StageModel
                    {
                        Id = id,
                        Name = def?.Name,
                        State = UnlockHelper.State.Blocked,
                    };
                    changed = true;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(st.Name))
                    {
                        st.Name = StageData.GetDef(id)?.Name ?? id;
                        changed = true;
                    }
                }

                // Se você usa expedição ativa por stage, garanta que não é nula
                if (st.ActiveExpedition == null)
                {
                    st.ActiveExpedition = new ExpeditionModel
                    {
                        Id = $"exp-{st.Id}-0",
                        PartyIds = new(),
                        ContractsId = new(),
                        ContractsActiveId = new(),
                        ContractsLockedId = new(),
                        TimeStart = new TimeOnly(0, 0),
                        TimeFinish = null
                    };
                    changed = true;
                }
                else
                {
                    // saneamento de nulls internos
                    st.ActiveExpedition.PartyIds ??= new();
                    st.ActiveExpedition.ContractsId ??= new();
                    st.ActiveExpedition.ContractsActiveId ??= new();
                    st.ActiveExpedition.ContractsLockedId ??= new();
                }
            }

            // ---------- LOCALS ----------
            foreach (var id in LocalData.ShowOrder)
            {
                if (!g.Locals.TryGetValue(id, out var loc))
                {
                    var def = LocalData.GetDef(id);
                    g.Locals[id] = loc = new LocalModel
                    {
                        Id = id,
                        Name = def?.Name,
                        State = UnlockHelper.State.Blocked,
                    };
                    changed = true;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(loc.Name))
                    {
                        loc.Name = LocalData.GetDef(id)?.Name ?? id;
                        changed = true;
                    }
                }
            }

            // ---------- TECHS ----------
            // Se o runtime tem Techs e você quer garantir a existência de todas:
            if (g.Techs != null)
            {
                foreach (var id in TechData.ShowOrder)
                {
                    if (!g.Techs.ContainsKey(id))
                    {
                        var def = TechData.GetDef(id);
                        g.Techs[id] = new TechModel
                        {
                            Id = id,
                            // Se quiser nome dinâmico em Tech, adicione propriedade no modelo e preencha aqui
                            State = UnlockHelper.State.Blocked,
                        };
                        changed = true;
                    }
                }
            }

            // ---------- UPGRADES ----------
            if (g.Upgrades != null)
            {
                foreach (var id in UpgradeData.ShowOrder)
                {
                    if (!g.Upgrades.ContainsKey(id))
                    {
                        var def = UpgradeData.GetDef(id);
                        g.Upgrades[id] = new UpgradeModel
                        {
                            Id = id,
                            // Se tiver DisplayName dinâmico, copie aqui
                            State = UnlockHelper.State.Blocked,
                            UnlockId = def?.UnlockId
                        };
                        changed = true;
                    }
                }
            }

            // ---------- RESOURCES / COINS ----------
            if (g.Coins != null)
            {
                foreach (var id in CoinsData.ShowOrder)
                {
                    if (!g.Coins.ContainsKey(id))
                    {
                        var def = CoinsData.GetDef(id);
                        g.Coins[id] = new CoinModel
                        {
                            Id = id,
                            State = UnlockHelper.State.Blocked,
                            UnlockId = def?.UnlockId
                        };
                        changed = true;
                    }
                }
            }
            if (g.Resources != null)
            {
                foreach (var id in ResourceData.ShowOrder)
                {
                    if (!g.Resources.ContainsKey(id))
                    {
                        var def = CoinsData.GetDef(id);
                        g.Resources[id] = new ResourceModel
                        {
                            Id = id,
                            State = UnlockHelper.State.Blocked,
                            UnlockId = def?.UnlockId
                        };
                        changed = true;
                    }
                }
            }

            // ---------- CHARACTERS ----------
            if (g.Characters != null)
            {
                foreach (var id in CharacterData.ShowOrder)
                {
                    if (!g.Characters.ContainsKey(id))
                    {
                        var def = CharacterData.GetDef(id);
                        g.Characters[id] = new CharacterModel
                        {
                            Id = id,
                            // se tiver Name dinâmico no modelo, copie aqui
                            State = UnlockHelper.State.Blocked
                        };
                        changed = true;
                    }
                }
            }

            // ---------- CONTRACTS ----------
            if (g.Contracts != null)
            {
                foreach (var id in ContractData.ShowOrder)
                {
                    if (!g.Contracts.ContainsKey(id))
                    {
                        var def = ContractData.GetDef(id);
                        g.Contracts[id] = new ContractModel
                        {
                            Id = id,
                            // se tiver Name/Level no runtime, inicialize aqui
                            State = UnlockHelper.State.Blocked,
                            Level = def?.Level ?? 1
                        };
                        changed = true;
                    }
                }
            }

            // ---------- KNOWLEDGES ----------
            if (g.Knowledges != null)
            {
                foreach (var id in KnowledgeData.ShowOrder)
                {
                    if (!g.Knowledges.ContainsKey(id))
                    {
                        var def = KnowledgeData.GetDef(id);
                        g.Knowledges[id] = new KnowledgeModel
                        {
                            Id = id,
                            State = UnlockHelper.State.Blocked
                        };
                        changed = true;
                    }
                }
            }

            // ---------- EXPANSIONS ----------
            if (g.Expansions != null)
            {
                foreach (var id in ExpansionData.ShowOrder)
                {
                    if (!g.Expansions.ContainsKey(id))
                    {
                        var def = ExpansionData.GetDef(id);
                        g.Expansions[id] = new ExpansionModel
                        {
                            Id = id,
                            State = UnlockHelper.State.Blocked
                        };
                        changed = true;
                    }
                }
            }

            // Stats
            g.Stats ??= new StatsModel(); // se vier null

            if (g.Stats.Coins is null) { g.Stats.Coins = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }
            if (g.Stats.Resources is null) { g.Stats.Resources = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }
            if (g.Stats.Knowledge is null) { g.Stats.Knowledge = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }

            if (g.Stats.CoinsGain is null) { g.Stats.CoinsGain = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }
            if (g.Stats.ResourcesGain is null) { g.Stats.ResourcesGain = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }
            if (g.Stats.KnowledgeGain is null) { g.Stats.KnowledgeGain = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }

            if (g.Stats.CoinsSpent is null) { g.Stats.CoinsSpent = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }
            if (g.Stats.ResourcesSpent is null) { g.Stats.ResourcesSpent = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }
            if (g.Stats.KnowledgeSpent is null) { g.Stats.KnowledgeSpent = new Dictionary<string, long>(StringComparer.Ordinal); changed = true; }

            return changed;

            // ---------- Defaults globais ----------
            if (string.IsNullOrWhiteSpace(g.SelectedStageId) || !g.Stages.ContainsKey(g.SelectedStageId))
            {
                // fallback seguro
                g.SelectedStageId = StageData.ShowOrder.FirstOrDefault() ?? "s00";
                changed = true;
            }

            return changed;
        }
    }
}
