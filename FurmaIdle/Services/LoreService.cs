namespace FurmaIdle.Services
{
    public interface ILoreService
    {
        void LoreTrigger(string loreId, string? helper = "");
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
        public void LoreTrigger(string loreId, string? helper = "")
        {
            switch (loreId)
            {
                #region Stage 0
                case "GameCreation":
                    _log.Lore("Isolados no meio do mar, cercados pela poderosa correnteza Entrilhas, os habitantes da Ilha de" +
                        " Vera se protegem dos perigos da mata na pequena Murada Cairu. Lá fora, uma criatura insana aguarda que" +
                        " qualquer um deles fique desesperado o suficiente para sair. E vire a janta.");
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
                    _log.Ferri("Preciso de uma forma mais eficiente de conseguir Talhos. Talvez eu devesse procurar trabalhos maiores," +
                        " ou reabrir a antiga taberna de meu pai...");
                    break;
                case "ContractLevel0Unlock":
                    _log.Ferri("Vou fazer um acordo, comigo mesmo. Só vou descansar quando tiver resolvido a situação da Murada. Vou estudar" +
                        " tudo o que tiver para conhecer, tudo o que puder me ajudar.");
                    break;
                case "FirstContract0Purchase":
                    break;
                case "5xContract0Purchase":
                    _log.Info("Atingir determinadas quantidades de um Contrato pode liberar Melhorias para ele.");
                    _log.Ferri("Acho que consigo ficar um pouco melhor. Quanto mais eu me dedicar à Estudar, mais consigo" +
                        " melhorar os resultados do Contrato.");
                    break;
                case "ContractLevel1Unlock":
                    break;
                case "FirstContractUnlock":
                    _log.Info("Melhorias de desbloqueio são permanentes, e só são compradas uma vez ao longo do jogo.");
                    break;
                case "FirstContract1Purchase":
                    _log.Ferri("Agora, é só questão de tempo. Vou reabrir a taberna, e vou recrutar uma equipe. Está na hora de fundar" +
                        " a Guilda da Ilha de Vera.");
                    break;
                case "14Contract1Purchase":
                    _log.Info("Começe um Contrato para Varrer o Chão para continuar.");
                    _log.Ferri("Melhor fazer algo um pouco mais lucrativo, pra variar.");
                    break;
                #endregion

                #region Stage 1
                case "Stage1Start":
                    _log.Lore("Ferri gasta suas economias para voltar a ser taberneiro, e começa sua jornada atrás de companheiros" +
                        " para retomar a Ilha de Vera.");
                    _log.Ferri("Se eu conseguir mais Talhos poderei contratar pessoas para me ajudar.");
                    break;
                case "FirstCharacterUnlock":
                    _log.Info("Após desbloquear um novo Personagem, ele será enviado para a Base, e ficará aguardando até que seja alocado" +
                        " em uma Expedição.");
                    _log.Ferri("Ótimo, já consigo ver a Guilda se formando.");

                    if(helper == "p0101")
                    {
                        // Maik
                        _log.Maik("Ferramentas preparadas. Quero saber o que mais essa ilha pode oferecer, além de areia e conchas riscadas.");
                    }
                    if(helper == "p0102")
                    {
                        // Claimi
                        _log.Claimi("Já estava cansada de pescar aqui na Murada, vamos viajar!");
                    }
                    if (helper == "p0103")
                    {
                        // Alan
                        _log.Alan("Nem eu aguento mais as minhas histórias, hora de conhecer o mundo.");
                        _log.Ferri("A ilha, o mundo é grande demais.");
                        _log.Alan("Por enquanto, Ferri, por enquanto.");
                    }
                    break;
                case "FirstResourceUnlock":
                    _log.Ferri("Não dá pra carregar muita comida de uma vez...");
                    if (helper == "p0101")
                    {
                        // Maik
                        _log.Maik("Mas, mesmo da Base, consigo ajudar a gerir.");
                    }

                    if (helper == "p0102")
                    {
                        // Claimi
                        _log.Claimi("Sem problemas, eu fico aqui pescando enquanto vocês passeiam...");
                    }

                    if (helper == "p0103")
                    {
                        // Alan
                        _log.Alan("Que experiência formidável, garantir comida enquanto os outros se aventuram.");
                    }
                    break;
                case "FirstExpeditionUnlock":
                    if (helper == "p0101")
                    {
                        // Maik
                        _log.Maik("Ferri, amigo, acho que posso ajudar bem mais se eu for com você.");
                    }

                    if (helper == "p0102")
                    {
                        // Claimi
                        _log.Claimi("Se me deixar aqui mais um pouco, vou eu mesma virar um peixe.");
                    }

                    if (helper == "p0103")
                    {
                        // Alan
                        _log.Alan("Deixe-me mostrar o quão valiosa é minha companhia.");
                    }
                    break;
                case "FirstExpeditionComplete":
                    _log.Ferri("Certo, descansar e recomeçar.");
                    break;
                case "FirstLocalUnlock":
                    _log.Alan("Encantador... Inspirador... Assustador...");
                    break;
                case "FirstKnowledgeUnlock":
                    _log.Ferri("Sabia que meus estudos seriam recompensados. Tenho certeza que só um pouquinho de conhecimento" +
                        " já vai me ajudar muito.");
                    break;
                case "FirstExpansionUnlock":
                    break;
                case "FirstTechUnlock":
                    _log.Ferri("Tanta coisa pra saber, e eu achei que ia ser só juntar um pessoal e limpar a ilha...");
                    break;
                case "FirstRouteUnlock":
                    _log.Jaime("Ah, sim, uma saída!");
                    _log.Claimi("Pra onde?");
                    _log.Jaime("Não tenho ideia.");
                    _log.Yg("Humpf.");
                    break;
                case "FirstShipUnlock":
                    _log.Claimi("Finalmente! Vamos viajar!");
                    break;
                case "FirstStageUnlock":
                    _log.Ferri("Cumpri com minha promessa. A Ilha é nossa novamente. Mas... O oceano é tão vasto. Sinto que podemos vencer" +
                        " a Entrilhas, podemos conhecer novas ilhas, novas pessoas... Por que parar agora?");
                    break;
                #endregion

                #region Geral

                case "LocalUnlock":
                    if (helper == "l011")
                    {
                        _log.Lore("A pequena Guilda avança até as Pontas Cantarolantes, contornando a Ilha de Vera pela praia." +
                            " Uma subida perigosa, onde qualquer deslize pode ser fatal.");
                    }
                    if (helper == "l012")
                    {
                        _log.Lore("A Guilda avança até o Coração da Ilha, a região das nascentes que garantem água limpa à Murada Cairu." +
                            " A mata fechada é perigosa, e qualquer animal pode estar infectado.");
                    }
                    if (helper == "l013")
                    {
                        _log.Lore("Com todo seu poder, a Guilda entra no Bosque da Raposa, pronta para caçar a maior besta Insana da Ilha." +
                            " Matá-la, e impedir que os esporos se espalhem, pode significar a tão aguardada reconquista.");
                    }
                    break;
                case "CharacterUnlock":
                    if (helper == "p0101")
                    {
                        _log.Maik("Um prazer estar aqui. Sou Maik, aprendiz de Artesão na Murada Cairu.");
                    }
                    if (helper == "p0102")
                    {
                        _log.Claimi("Deixa de moleza, quero sair logo daqui!");
                    }
                    if (helper == "p0103")
                    {
                        _log.Alan("Encantado, meus queridos. Será uma obra maravilhosa a que juntos criaremos.");
                    }
                    if (helper == "p0111")
                    {
                        _log.Jaime("E eu nem achava que teria gente nessa ilha. Prazer, sou Jaime, ao seu dispor.");
                    }
                    if (helper == "p0121")
                    {
                        _log.Lore("Yg encara o resto da Guilda. E acena com a cabeça.");
                    }
                    break;
                case "KnowledgeUnlock":

                    break;
                case "TechUnlock":

                    break;
                case "ShipUnlock":

                    break;
                case "RouteUnlock":

                    break;
                case "StageUnlock":

                    break;
                case "ExpansionUnlock":

                    break;
                case "SpecialtyUsed":
                    if (helper == "e0001")
                    {
                        _log.Ferri("Colaborem com a recuperação da ilha! Pela Guilda da Illha de Vera!");
                    }
                    if (helper == "e0101")
                    {
                        _log.Maik("Deixe-me mostrar meu talento.");
                    }
                    if (helper == "e01002")
                    {
                        _log.Claimi("Sou bem mais forte do que pareço, fiquem tranquilos.");
                    }
                    if (helper == "e0103")
                    {
                        _log.Alan("Um toque de inspiração.");
                    }
                    if (helper == "e0111")
                    {
                        _log.Jaime("Se usar direito, dura muito mais.");
                    }
                    if (helper == "e0121")
                    {
                        _log.Yg("No momento certo.");
                    }
                    break;
                case "ExpeditionStart":
                    break;
                case "ExpeditionEnd":
                    break;
                case "ExpansionEnd":
                    break;
                #endregion
                default: break;
            }
        }
        #endregion
    }
}
