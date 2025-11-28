namespace FurmaIdle.Services
{
    public interface ILoreService
    {
        void LoreTrigger(string loreId);
    }
    public sealed class LoreService : ILoreService
    {
        private readonly ICurrentGameService _game;
        private readonly IUiLogService _log;
        private readonly ILocateService _locate;

        public LoreService(ICurrentGameService game, IUiLogService log, ILocateService locate)
        {
            _game = game;
            _log = log;
            _locate = locate;
        }

        #region Lore
        public void LoreTrigger(string loreId)
        {
            var game = _game.CurrentGame;

            game.LoreTriggers ??= new Dictionary<string, bool>();

            if (game.LoreTriggers.TryGetValue(loreId, out var seen) && seen)
                return;

            game.LoreTriggers[loreId] = true;

            switch (loreId)
            {
                // Stage 0
                case "GameCreation":
                    _log.Lore("Isolados no meio do mar, cercados pela poderosa correnteza Entrilhas, os habitantes da Ilha de " +
                        "Vera se protegem dos perigos da mata na pequena Murada Cairu. Lá fora, uma criatura insana aguarda que " +
                        "qualquer um deles fique desesperado o suficiente para sair. E vire a janta.");
                    _log.Lore("Ferri Karu não consegue se conformar.");
                    _log.Info("Clique na imagem central para conseguir Talhos, a moeda corrente na Ilha de Vera.");
                    _log.Ferri("Sozinho não tenho nenhuma chance, mas talvez eu consiga ajuda... Se antes eu conseguir dinheiro.");
                    break;
                case "FirstClick":
                    _log.Ferri("Se eu me esforçar, posso conseguir juntar alguma coisa e comprar equipamentos para viagem.");
                    break;
                case "10thClick":
                    _log.Info("Compre Melhorias para conseguir ganhar mais moedas.");
                    _log.Ferri("Acho que consigo ganhar mais Talhos, só preciso melhorar minha técnica.");
                    break;
                case "20thClick":
                    _log.Info("Existem diferentes tipos de Melhorias, divididas em categorias de Permanência.");
                    _log.Ferri("Preciso de uma forma mais eficiente de conseguir Talhos. Talvez eu devesse procurar trabalhos maiores," +
                        " ou reabrir a antiga taberna de meu pai...");
                    break;
                case "ContractLevel0Unlock":
                    _log.Info("Contratos são formas de gerar Moedas automaticamente. São divididos em níveis, onde cada um garante mais" +
                        " moedas que o anterior.");
                    _log.Ferri("Vou fazer um acordo, comigo mesmo. Só vou descansar quando tiver resolvido a situação da Murada. Vou estudar " +
                        "tudo o que tiver para conhecer, tudo o que puder me ajudar.");
                    break;
                case "FirstContract0Purchase":
                    _log.Info("Após escolhido um Contrato de determinado nível, não será possível trocá-lo tão cedo.");
                    _log.Info("Só é possível fechar Contratos até atingir o limite, que pode ser aumentado de várias maneiras," +
                        " como Melhorias.");
                    break;
                case "5xContract0Purchase":
                    _log.Info("Atingir determinadas quantidades de um Contrato pode liberar Melhorias para ele.");
                    _log.Ferri("Acho que consigo ficar um pouco melhor. Quanto mais eu me dedicar à Estudar, mais consigo " +
                        "melhorar os resultados do Contrato.");
                    break;
                case "ContractLevel1Unlock":
                    _log.Info("É possível desbloquear novos tipos de Contratos para todos os níveis. Eles recebem melhorias diferentes," +
                        " além de influenciarem outros aspectos futuros no jogo.");
                    break;
                case "FirstContractUnlock":
                    _log.Info("Melhorias de desbloqueio são permanentes, e só são compradas uma vez ao longo do jogo.");
                    break;
                case "FirstContract1Purchase":
                    _log.Info("Os Objetivos, disponíveis na parte superior do menu de Melhorias, permitem desbloquear a próxima parte" +
                        "da história, quer seja na mesma Região, ou em uma próxima.");
                    _log.Info("Após comprados, os Objetivos causam um Soft Reset de Expansão, reiniciando o progresso e liberando novos" +
                        " recursos.");
                    _log.Ferri("Agora, é só questão de tempo. Vou reabrir a taberna, e vou recrutar uma equipe. Está na hora de fundar" +
                        " a Guilda da Ilha de Vera.");
                    break;

                // Stage 1

                default: break;
            }
        }
        #endregion
    }
}
