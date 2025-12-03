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
            var game = _game.CurrentGame;

            game.LoreTriggers ??= new Dictionary<string, bool>();

            if (game.LoreTriggers.TryGetValue(loreId, out var seen) && seen)
                return;

            game.LoreTriggers[loreId] = true;

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
                    _log.Info("Existem diferentes tipos de Melhorias, divididas em categorias de Permanência.");
                    _log.Ferri("Preciso de uma forma mais eficiente de conseguir Talhos. Talvez eu devesse procurar trabalhos maiores," +
                        " ou reabrir a antiga taberna de meu pai...");
                    break;
                case "ContractLevel0Unlock":
                    _log.Info("Contratos são formas de gerar Moedas automaticamente. São divididos em níveis, onde cada um garante mais" +
                        " moedas que o anterior.");
                    _log.Ferri("Vou fazer um acordo, comigo mesmo. Só vou descansar quando tiver resolvido a situação da Murada. Vou estudar" +
                        " tudo o que tiver para conhecer, tudo o que puder me ajudar.");
                    break;
                case "FirstContract0Purchase":
                    _log.Info("Após escolhido um Contrato de determinado nível, não será possível trocá-lo tão cedo.");
                    _log.Info("Só é possível fechar Contratos até atingir o limite, que pode ser aumentado de várias maneiras," +
                        " como Melhorias.");
                    break;
                case "5xContract0Purchase":
                    _log.Info("Atingir determinadas quantidades de um Contrato pode liberar Melhorias para ele.");
                    _log.Ferri("Acho que consigo ficar um pouco melhor. Quanto mais eu me dedicar à Estudar, mais consigo" +
                        " melhorar os resultados do Contrato.");
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
                        " da história, quer seja na mesma Região, ou em uma próxima.");
                    _log.Info("Após comprados, os Objetivos causam um Soft Reset de Expansão, reiniciando o progresso e liberando novos" +
                        " recursos.");
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

                    if(helper == "p102")
                    {
                        // Maik
                        _log.Maik("Ferramentas preparadas. Quero saber o que mais essa ilha pode oferecer, além de areia e conchas riscadas.");
                    }

                    if(helper == "p103")
                    {
                        // Claimi
                        _log.Claimi("Já estava cansada de pescar aqui na Murada, vamos viajar!");
                    }

                    if (helper == "p104")
                    {
                        // Alan
                        _log.Alan("Nem eu aguento mais as minhas histórias, hora de conhecer o mundo.");
                        _log.Ferri("A ilha, o mundo é grande demais.");
                        _log.Alan("Por enquanto, Ferri, por enquanto.");
                    }
                    break;
                case "FirstResourceUnlock":
                    _log.Info("Recursos são um importante artifício para conseguir vantagens, utilizando as Especialidades dos Personagens," +
                        " como as Gorjetas, de Ferri.");
                    _log.Info("São compartilhadas entre todas as Regiões, e não são perdidas com o fim da Expedição, mas podem demorar um pouco" +
                        " para serem conseguidas.");
                    _log.Ferri("Não dá pra carregar muita comida de uma vez...");
                    // personagem na base.
                    if (helper == "p102")
                    {
                        // Maik
                        _log.Maik("Mas, mesmo da Base, consigo ajudar a gerir.");
                    }

                    if (helper == "p103")
                    {
                        // Claimi
                        _log.Claimi("Sem problemas, eu fico aqui pescando enquanto você passeiam...");
                    }

                    if (helper == "p104")
                    {
                        // Alan
                        _log.Alan("Que experiência formidável, garantir comida enquanto os outros se aventuram.");
                    }
                    break;
                case "FirstExpeditionUnlock":
                    _log.Info("Expedições são o tipo mais rápido de Reset no jogo, e acontecem de forma independente em cada" +
                        " Região. Podem ser utilizadas para realocar Personagens em diferentes Regiões, aumentando o tamanho da" +
                        " equipe quando houver espaço, ou ainda para conseguir Conhecimento.");
                    _log.Info("Por serem um Reset, todas as Moedas daquela Região são consumidas ao encerrar a Expedição, e todas" +
                        " as Melhorias de Expedição são desligadas e voltam ao início.");
                    // personagem na base pedindo pra entrar
                    if (helper == "p102")
                    {
                        // Maik
                        _log.Maik("Ferri, amigo, acho que posso ajudar bem mais se eu for com você.");
                    }

                    if (helper == "p103")
                    {
                        // Claimi
                        _log.Claimi("Se me deixar aqui mais um pouco, vou eu mesma virar um peixe.");
                    }

                    if (helper == "p104")
                    {
                        // Alan
                        _log.Alan("Deixe-me mostrar o quão valiosa é minha companhia.");
                    }
                    break;
                case "FirstKnowledgeUnlock":
                    _log.Info("Conhecimento é uma das maneiras mais poderosas de melhorar os ganhos. Cada ponto conquistado, em" +
                        " qualquer tipo de Conhecimento, aumenta a geração de Moedas, tanto dos Contratos quanto do Click, além" +
                        " de poderem ser utilizados para fazer Pesquisas, que liberam novas Melhorias.");
                    _log.Ferri("Sim! Sabia que meus estudos seriam recompensados. Tenho certeza que só um pouquinho de conhecimento" +
                        " já vai me ajudar muito.");
                    break;
                case "FirstExpansionUnlock":
                    _log.Info("Terminar uma Expansão é a segunda forma de Reset no jogo, dessa vez um pouco mais impactante do que" +
                        " terminar uma Expedição. Todos os Personagens, de todas as Regiões, voltam para a Base, e todo o Conhecimento" +
                        " acumulado volta a zero, assim como Moedas e Recursos. Novas Melhorias são desbloqueadas, e a história progride," +
                        " permitindo chegar ainda mais longe.");
                    break;
                case "FirstTechUnlock":
                    // info.tech
                    _log.Info("Pesquisas são uma importante maneira de conseguir Melhorias poderosas, que podem afetar várias fases do jogo." +
                        " Custam Conhecimento, mas não diminuem o bônus total garantido por ele.");
                    _log.Ferri("Tanta coisa pra saber, e eu achei que ia ser só juntar um pessoal e limpar a ilha...");
                    break;
                case "FirstShipUnlock":
                    _log.Error("NAVIO DESBLOQUEADO");
                    break;
                case "FirstStageUnlock":
                    _log.Info("Cada Região possui a própria Expedição, e pode precisar ou garantir coisas únicas, como Moedas, Recursos," +
                        " Melhorias, Personagens e Pesquisas. É possível trocar a Região selecionada a qualquer momento, mas um mesmo" +
                        " Personagem ou Contrato não pode ser utilizado em dois lugares diferentes.");
                    _log.Lore("A Guilda retorna à Murada Cairu, vitoriosos. A Ilha foi retomada, e agora, com a mesma segurança de antes da" +
                        " Era da Insanidade, seus habitantes podem explorar todo seu território.");
                    _log.Ferri("Cumpri com minha promessa. A Ilha é nossa novamente. Mas... O oceano é tão vasto. Sinto que podemos vencer" +
                        " a Entrilhas, podemos conhecer novas ilhas, novas pessoas... Por que parar agora?");
                    break;
                #endregion

                #region Geral

                case "LocalUnlock":
                    if (helper == "l11")
                    {
                        _log.Info("Explorar Locais permite conhecer a Região com maiores detalhes, aprendendo com ela. É possível encontrar" +
                            " novas Pesquisas e novos Personagens ao desbloquear um Local.");
                        _log.Lore("A pequena Guilda avança até as Pontas Cantarolantes, contornando a Ilha de Vera pela praia." +
                            " Uma subida perigosa, onde qualquer deslize pode ser fatal.");
                    }
                    if (helper == "l12")
                    {
                        _log.Lore("A Guilda avança até o Coração da Ilha, a região das nascentes que garantem água limpa à Murada Cairu." +
                            " A mata fechada é perigosa, e qualquer animal pode estar infectado.");
                    }
                    if (helper == "l13")
                    {
                        _log.Lore("Com todo seu poder, a Guilda entra no Bosque da Raposa, pronta para caçar a maior besta Insana da Ilha." +
                            " Matá-la, e impedir que os esporos se espalhem, pode significar a tão aguardada reconquista.");
                    }
                    break;
                case "CharacterUnlock":
                    if (helper == "p102")
                    {
                        _log.Maik("Um prazer estar aqui. Sou Maik, aprendiz de Artesão na Murada Cairu.");
                    }
                    if (helper == "p103")
                    {
                        _log.Claimi("Deixa de moleza, quero sair logo daqui!");
                    }
                    if (helper == "p104")
                    {
                        _log.Alan("Encantado, meus queridos. Será uma obra maravilhosa a que juntos criaremos.");
                    }
                    if (helper == "p111")
                    {
                        _log.Jaime("E eu nem achava que teria gente nessa ilha. Prazer, sou Jaime, ao seu dispor.");
                    }
                    if (helper == "p121")
                    {
                        _log.Lore("Yg encara o resto da Guilda. E acena com a cabeça.");
                    }
                    break;
                case "SpecialtyUsed":
                    if (helper == "e01")
                    {
                        _log.Ferri("Colaborem com a recuperação da ilha! Pela Guilda da Illha de Vera!");
                    }
                    if (helper == "e02")
                    {
                        _log.Maik("Deixe-me mostrar meu talento.");
                    }
                    if (helper == "e03")
                    {
                        _log.Claimi("Sou bem mais forte do que pareço, fiquem tranquilos.");
                    }
                    if (helper == "e04")
                    {
                        _log.Alan("Um toque de inspiração.");
                    }
                    if (helper == "e05")
                    {
                        _log.Jaime("Se usar direito, dura muito mais.");
                    }
                    if (helper == "e06")
                    {
                        _log.Yg("No momento certo.");
                    }
                    break;
                #endregion
                default: break;
            }
        }
        #endregion
    }
}
