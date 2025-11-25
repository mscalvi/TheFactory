namespace FurmaIdle.Services
{
    public interface ILoreService
    {
        void LoreTrigger(string itemId, string? help = "");
        void PurchaseInfo(string itemId, string costValue, string costCoin, string? help = "");
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
        public void LoreTrigger(string itemId, string? help = "")
        {
            string itemType1 = itemId.Substring(0, 1);
            string itemType2 = itemId.Substring(0, 2);

            switch (itemId)
            {
                case "GameStart":
                    _log.Unlock("Ferri se levanta, animado para começar a trabalhar. É hora de montar a Guilda.");
                    break;
                case "ExpeditionStart":
                    _log.Lore("Hora de partir!");
                    break;
                case "ExpeditionEnd":
                    if (help == "aprendeu")
                    {
                        _log.Lore("Se aprendemos algo, já valeu a pena.");
                    }
                    else
                    {
                        _log.Lore("Voltar pra casa sem aprender nada é tão... Cansativo.");
                    }
                    break;
                case "ExpansionEnd":
                    _log.Lore("Hora de treinarmos os novos recrutas.");
                    break;
                case "FirstCharacterPurchase":
                    _log.Lore("Agora sim estamos montando uma Guilda!");
                    break;
                case "FirstContractPurchase":
                    _log.Lore("Vou precisar de bem mais dinheiro do que ganho com isso...");
                    break;
                case "FirstCapPurchase":
                    _log.Lore("Vou precisar ser bem melhor em ganhar dinheiro do que isso...");
                    break;
                case "SecondContractPurchase":
                    _log.Lore("E vou precisar aprender muita coisa nova...");
                    break;
                case "ObjetiveUnlock":
                    _log.Lore("Bom, o primeiro passo é fazer a Murada acreditar em mim!");
                    break;
                case "ua01":
                    _log.Lore("Talvez seja hora de chamar reforços.");
                    break;
                case "ua02":
                    _log.Lore("Talvez seja hora de chamar reforços.");
                    break;
                case "ua03":
                    _log.Lore("Talvez seja hora de chamar reforços.");
                    break;
                case "up102":
                    _log.Lore("Ótimo! Maik, o Artesão, entrou para a Guilda.");
                    break;
                case "up103":
                    _log.Lore("Ótimo! Claimi, a Pescadora, entrou para a Guilda.");
                    break;
                case "up104":
                    _log.Lore("Ótimo! Alan, o Bardo, entrou para a Guilda.");
                    break;
                case "up111":
                    _log.Lore("Ótimo! Jaime, o Explorador, entrou para a Guilda.");
                    break;
                case "up121":
                    _log.Lore("Ótimo! Yg, o Caçador, entrou para a Guilda.");
                    break;
                default: break;
            }

            if(itemType1 == "c")
            {
                var contract = _locate.LocateContract(_game.CurrentGame, itemId);

                switch (help) 
                {
                    case "1":
                        _log.Info($"Nosso primero contrato para {contract.Name}. É um começo! Se conseguirmos mais contratos, talvez" +
                            $" a gente ganhe experiência o suficiente para melhorar o serviço.");
                        break;
                    case "5":
                        _log.Info($"Estamos pegando o jeito em {contract.Name}. Aos poucos a gente pode cobrar mais, e terminar mais rápido.");
                        break;
                    case "50":
                        _log.Info($"Já somos bons em {contract.Name}, em? Sem técnicas melhores, esse é o mínimo que vamos gastar para começar" +
                            $" um novo contrato.");
                        break;
                    case "100":
                        _log.Info($"Perfeito, tudo o que sabemos sobre {contract.Name}. Agora, vamos ter que estudar para conseguir melhorar" +
                            $" ainda mais no serviço.");
                        break;
                }
            }
        }

        public void PurchaseInfo(string itemName, string costValue, string costCoin, string? help = "")
        {
            //switch (help)
            //{
            //    case "compra":
            //        _log.Info($"Melhoria: {itemName} - {costValue} {costCoin}.");
            //        break;
            //    case "spec":
            //        _log.Info($"Especialidade: {itemName} - {costValue} {costCoin}!");
            //        break;
            //    case "contract":
            //        _log.Info($"Contrato: {itemName} - {costValue} {costCoin}.");
            //        break;
            //}
        }
        #endregion
    }
}
