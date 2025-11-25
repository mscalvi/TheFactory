using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IGameMigrationService
    {
        bool Migrate(GameModel game);
    }

    public sealed class GameMigrationService : IGameMigrationService
    {
        public bool Migrate(GameModel g)
        {
            if (g is null) throw new ArgumentNullException(nameof(g));

            bool changed = false;

            if (g.SchemaVersion <= 0)
            {
                g.SchemaVersion = 1;
                changed = true;
            }

            // 2) Migrações por versão (1 -> 2 -> 3...)
            while (g.SchemaVersion < VersionHelper.Current)
            {
                switch (g.SchemaVersion)
                {
                    case 1:
                        MigrateFrom1To2(g);
                        g.SchemaVersion = 2;
                        changed = true;
                        break;

                    default:
                        Console.WriteLine("Erro de versão. Forçar HardReset manual pelo Application/Storage/ClearData.");
                        return changed;
                }
            }

            if (BackfillNulls(g))
            {
                changed = true;
            }

            return changed;
        }

        #region Updates
        private static void MigrateFrom1To2(GameModel g)
        {

        }
        #endregion

        private static bool BackfillNulls(GameModel g)
        {
            bool changed = false;

            g.Ui ??= new UiState();
            g.NoExpeditionStats ??= new StatsModel();
            g.GameStats ??= new StatsModel();

            g.Coins ??= new Dictionary<string, CoinModel>();
            g.Clicks ??= new Dictionary<string, ClickModel>();
            g.Stages ??= new Dictionary<string, StageModel>();
            g.Locals ??= new Dictionary<string, LocalModel>();
            g.Techs ??= new Dictionary<string, TechModel>();
            g.Upgrades ??= new Dictionary<string, UpgradeModel>();
            g.Resources ??= new Dictionary<string, ResourceModel>();
            g.Characters ??= new Dictionary<string, CharacterModel>();
            g.Contracts ??= new Dictionary<string, ContractModel>();
            g.Knowledges ??= new Dictionary<string, KnowledgeModel>();
            g.Expansions ??= new Dictionary<string, ExpansionModel>();
            g.Specialties ??= new Dictionary<string, SpecialtyModel>();
            g.Traits ??= new Dictionary<string, TraitModel>();

            return changed;
        }
    }
}
