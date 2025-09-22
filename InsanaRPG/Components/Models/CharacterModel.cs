using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanaRPG.Models
{
    internal class CharacterModel
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

        // Proficiências
        public string AcrobaciaEquilibro { get; set; }
        public string ArmasTiro { get; set; }
        public string ConducaoMontaria { get; set; }
        public string MedicinaPratica {get; set;}
        public string OficioEngenharia { get; set; }
        public string Furtividade { get; set; }
        public string Intuicao { get; set; }
        public string NavegacaoDirecionamento { get; set; }
        public string Percecao { get; set; }
        public string ReflexoEsquiva { get; set; }
        public string AlquimiaNatureza { get; set; }
        public string HistoriaReligiao { get; set; }
        public string LeituraDecifracao { get; set; }
        public string MatematicaFisica { get; set; }
        public string MedicinaTeorica {get; set;}
        public string CarismaCharme { get; set; }
        public string ComercioNegocios { get; set; }
        public string Disfarce { get; set; }
        public string Intimidacao { get; set; }
        public string PersuasaoAnimal {get; set;}
        public string Armamentos { get; set; }
        public string Atletismo { get; set; }
        public string Briga { get; set; }
        public string Constituicao { get; set; }
        public string Forca { get; set; }
        public string Autocontrole { get; set; }
        public string AvaliacaoRiscos { get; set; }
        public string FocoConcentracao { get; set; }
        public string MeditacaoRecobro { get; set; }
        public string ResilienciaDisciplina { get; set; }

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
        public string ObjetivoAtual { get; set; }
    }
}
