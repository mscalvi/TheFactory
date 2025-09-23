using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanaRPG.Models
{
    public class CharacterModel
    {
        // Atributos
        public int Destreza {  get; set; }
        public int Instinto { get; set; }
        public int Inteligencia { get; set; }
        public int Presenca { get; set; }
        public int Vigor { get; set; }
        public int Vontade { get; set; }

        // Origem
        public string Origem { get; set; }
        public string Trilha { get; set; }

        // Fardos
        public string Fardo { get; set; }
        public string PontoCego1 { get; set; }
        public string PontoCego2 { get; set; }
        public string PontoCego3 { get; set; }
        public string PontoCego4 { get; set; }
        public string PontoCego5 { get; set; }
        public string PontoCego6 { get; set; }
        public string PontoCego7 { get; set; }
        public string PontoCego8 { get; set; }

        // Proficiências Iniciais
        public int AcrobaciaEquilibro { get; set; }
        public int ArmasTiro { get; set; }
        public int ConducaoMontaria { get; set; }
        public int MedicinaPratica {get; set;}
        public int OficioEngenharia { get; set; }
        public int Furtividade { get; set; }
        public int Intuicao { get; set; }
        public int NavegacaoDirecionamento { get; set; }
        public int Percecao { get; set; }
        public int ReflexoEsquiva { get; set; }
        public int AlquimiaNatureza { get; set; }
        public int HistoriaReligiao { get; set; }
        public int LeituraDecifracao { get; set; }
        public int MatematicaFisica { get; set; }
        public int MedicinaTeorica {get; set;}
        public int CarismaCharme { get; set; }
        public int ComercioNegocios { get; set; }
        public int Disfarce { get; set; }
        public int Intimidacao { get; set; }
        public int PersuasaoAnimal {get; set;}
        public int Armamentos { get; set; }
        public int Atletismo { get; set; }
        public int Briga { get; set; }
        public int Constituicao { get; set; }
        public int Forca { get; set; }
        public int Autocontrole { get; set; }
        public int AvaliacaoRiscos { get; set; }
        public int FocoConcentracao { get; set; }
        public int MeditacaoRecobro { get; set; }
        public int ResilienciaDisciplina { get; set; }

        // Quantizadas
        public int Vida {  get; set; }
        public int Feridas { get; set; }
        public int Fadiga {  get; set; }
        public int Sanidade {  get; set; }
        public int Estresse {  get; set; }
        public int ResistenciaFisica { get; set; }
        public int ResistenciaEmocional { get; set; }
        public int ToleranciaAlquimica { get; set; }
        public int Reputacao {  get; set; }

        //Descritivas
        public string DescDestreza { get; set; }
        public string DescInstinto { get; set; }
        public string DescInteligencia { get; set; }
        public string DescPresenca { get; set; }
        public string DescVigor { get; set; }
        public string DescVontade { get; set; }
        public string Nome { get; set; }
        public string Historia { get; set; }
        public string Temperamento { get; set; }
        public string Sonho { get; set; }


        // Proficiências em Jogo
        public int AcrobaciaEquilibroA { get; set; }
        public int ArmasTiroA { get; set; }
        public int ConducaoMontariaA { get; set; }
        public int MedicinaPraticaA { get; set; }
        public int OficioEngenhariaA { get; set; }
        public int FurtividadeA { get; set; }
        public int IntuicaoA { get; set; }
        public int NavegacaoDirecionamentoA { get; set; }
        public int PercecaoA { get; set; }
        public int ReflexoEsquivaA { get; set; }
        public int AlquimiaNaturezaA { get; set; }
        public int HistoriaReligiaoA { get; set; }
        public int LeituraDecifracaoA { get; set; }
        public int MatematicaFisicaA { get; set; }
        public int MedicinaTeoricaA { get; set; }
        public int CarismaCharmeA { get; set; }
        public int ComercioNegociosA { get; set; }
        public int DisfarceA { get; set; }
        public int IntimidacaoA { get; set; }
        public int PersuasaoAnimalA { get; set; }
        public int ArmamentosA { get; set; }
        public int AtletismoA { get; set; }
        public int BrigaA { get; set; }
        public int ConstituicaoA { get; set; }
        public int ForcaA { get; set; }
        public int AutocontroleA { get; set; }
        public int AvaliacaoRiscosA { get; set; }
        public int FocoConcentracaoA { get; set; }
        public int MeditacaoRecobroA { get; set; }
        public int ResilienciaDisciplinaA { get; set; }

        // Quantizadas em Jogo
        public int VidaA { get; set; }
        public int FeridasA { get; set; }
        public int FadigaA { get; set; }
        public int SanidadeA { get; set; }
        public int EstresseA { get; set; }
        public int ResistenciaFisicaA { get; set; }
        public int ResistenciaEmocionalA { get; set; }
        public int ToleranciaAlquimicaA { get; set; }
        public int ReputacaoA { get; set; }
        public string ObjetivoAtual { get; set; }
    }
}
