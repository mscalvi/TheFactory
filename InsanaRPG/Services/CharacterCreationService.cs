using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InsanaRPG.Data;
using InsanaRPG.Models;

namespace InsanaRPG.Services
{
    public sealed class CharacterCreationService
    {
        // ===================== DERIVADOS =====================
        public void ComputeDerived(CharacterModel m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            m.Vida = 20 + 2 * m.Vigor;
            m.Feridas = m.Vontade;
            m.Fadiga = m.Vigor;
            m.Sanidade = 10 + m.Instinto;
            m.Estresse = m.Inteligencia;
            m.ResistenciaFisica = m.Destreza * 5;
            m.ResistenciaEmocional = m.Vontade * 5;
            m.ToleranciaAlquimica = ((m.Vigor + m.Instinto)) * 5;
            m.Reputacao = 4 + m.Presenca;
        }

        // ===================== PROFICIÊNCIAS =====================
        /// <summary>
        /// Calcula todas as proficiências finais e grava no CharacterModel.
        /// </summary>
        /// <param name="m">Personagem</param>
        /// <param name="treinamentos">Ex.: 2 proficiências escolhidas na aba Treinamentos (+15% cada)</param>
        /// <param name="pontosCegos">Conjunto total de pontos cegos (checkboxes + rádios de atributos ignorados)</param>
        public void ComputeProficiencias(CharacterModel m,
                                         IEnumerable<string> treinamentos,
                                         IEnumerable<string> pontosCegos)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            var treinados = new HashSet<string>(treinamentos ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
            var cegos = new HashSet<string>(pontosCegos ?? Enumerable.Empty<string>(), StringComparer.Ordinal);

            // Mapas úteis
            var gruposPorAtributo = AtrProfRelData.Mapa;            
            var todasProfs = ProficienciasData.Ordem;       
            var hasTrilhaBonus = OrigemData.TrilhaProfs.TryGetValue((m.Origem ?? "", m.Trilha ?? ""), out var bonusTrilha);

            foreach (var prof in todasProfs.OrderBy(p => p, StringComparer.CurrentCultureIgnoreCase))
            {
                double bruto = 0;

                AddAttrBonus(ref bruto, prof, "Vigor", m.Vigor, gruposPorAtributo);
                AddAttrBonus(ref bruto, prof, "Presença", m.Presenca, gruposPorAtributo);
                AddAttrBonus(ref bruto, prof, "Inteligência", m.Inteligencia, gruposPorAtributo);
                AddAttrBonus(ref bruto, prof, "Destreza", m.Destreza, gruposPorAtributo);
                AddAttrBonus(ref bruto, prof, "Vontade", m.Vontade, gruposPorAtributo);
                AddAttrBonus(ref bruto, prof, "Instinto", m.Instinto, gruposPorAtributo);

                // --- Treinamentos: +15% para cada prof escolhida ---
                if (treinados.Contains(prof)) bruto += 15;

                // --- Trilha da origem: +15% / +10% / +5% ---
                if (hasTrilhaBonus)
                {
                    if (StringEqualsProf(prof, bonusTrilha.ProfMais15)) bruto += 15;
                    if (StringEqualsProf(prof, bonusTrilha.ProfMais10)) bruto += 10;
                    if (StringEqualsProf(prof, bonusTrilha.ProfMais5)) bruto += 5;
                }

                // --- Fardo: -15% em cada prof afetada ---
                if (!string.IsNullOrWhiteSpace(m.Fardo) &&
                    FardosData.Afeta.TryGetValue(m.Fardo, out var afetas) &&
                    afetas.Any(p => StringEqualsProf(p, prof)))
                {
                    bruto -= 15;
                }

                // --- Pontos Cegos: -10% por prof escolhida ---
                if (cegos.Contains(prof)) bruto -= 10;

                // --- Curva de retorno decrescente ---
                var efetivo = ApplyDiminishing(bruto);

                // --- Grava no modelo ---
                SetProficiencia(m, prof, efetivo);
            }
        }

        private static void AddAttrBonus(ref double bruto, string prof, string atributoNome, int atributoValor,
                                         IReadOnlyDictionary<string, string[]> grupos)
        {
            if (grupos.TryGetValue(atributoNome, out var lista) && lista.Any(p => StringEqualsProf(p, prof)))
            {
                bruto += 5 * atributoValor;
            }
        }

        /// <summary>
        /// 0–60: 1:1 | 60–80: 0,5:1 | 80+: 0,33:1. Resultado clampado 0..100 e arredondado.
        /// </summary>
        private static int ApplyDiminishing(double bruto)
        {
            // suporta negativos (penalidades): clamp em 0
            if (bruto <= 60) return Math.Max(0, (int)Math.Round(bruto, MidpointRounding.AwayFromZero));

            double eff = 60;
            var rest = bruto - 60;

            if (rest <= 20)
            {
                eff += rest * 0.5;
                return Clamp01((int)Math.Round(eff, MidpointRounding.AwayFromZero));
            }

            // acima de 80
            eff += 20 * 0.5;      // +10
            rest -= 20;
            eff += rest * (1.0 / 3.0);

            return Clamp01((int)Math.Round(eff, MidpointRounding.AwayFromZero));
        }

        private static int Clamp01(int v) => Math.Min(100, Math.Max(0, v));

        private static bool StringEqualsProf(string a, string b)
            => NormalizeKey(a) == NormalizeKey(b);

        // ===================== SET PROFICIÊNCIAS =====================
        // (mantém o que já tínhamos: normalização resiliente)
        public void SetProficiencia(CharacterModel m, string nome, int valorFinal)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome inválido", nameof(nome));

            var key = NormalizeKey(nome);
            if (_profSetters.TryGetValue(key, out var setter))
            {
                setter(m, valorFinal);
            }
            else
            {
                throw new KeyNotFoundException($"Proficiência não reconhecida: '{nome}' (normalizado: '{key}')");
            }
        }

        // ===================== Normalização & Mapeamento =====================
        private static string NormalizeKey(string s)
        {
            var lower = s.ToLowerInvariant();
            var normalized = lower.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);
            var onlyLetters = Regex.Replace(noDiacritics, "[^a-z]", "");
            return onlyLetters;
        }

        private static Dictionary<string, Action<CharacterModel, int>> _profSetters = BuildProfSetters();

        private static Dictionary<string, Action<CharacterModel, int>> BuildProfSetters()
        {
            var d = new Dictionary<string, Action<CharacterModel, int>>(StringComparer.Ordinal);
            void AddMany(Action<CharacterModel, int> setter, params string[] aliases)
            {
                foreach (var a in aliases) d[NormalizeKey(a)] = setter;
            }

            AddMany((m, v) => m.AcrobaciaEquilibro = v, "Acrobacia e Equilíbrio", "Acrobacia e Equilibrio");
            AddMany((m, v) => m.ArmasTiro = v, "Armas de Tiro");
            AddMany((m, v) => m.ConducaoMontaria = v, "Condução e Montaria", "Conducao e Montaria");
            AddMany((m, v) => m.MedicinaPratica = v, "Medicina Prática", "Medicina Pratica");
            AddMany((m, v) => m.OficioEngenharia = v, "Ofício e Engenharia", "Oficio e Engenharia");
            AddMany((m, v) => m.Furtividade = v, "Furtividade");
            AddMany((m, v) => m.Intuicao = v, "Intuição", "Intuicao");
            AddMany((m, v) => m.NavegacaoDirecionamento = v, "Navegação e Direcionamento", "Navegacao e Direcionamento");
            AddMany((m, v) => m.Percecao = v, "Percepção", "Percepcao");
            AddMany((m, v) => m.ReflexoEsquiva = v, "Reflexo e Esquiva", "Reflexos e Esquiva");
            AddMany((m, v) => m.AlquimiaNatureza = v, "Alquimia e Natureza");
            AddMany((m, v) => m.HistoriaReligiao = v, "História e Religião", "Historia e Religiao");
            AddMany((m, v) => m.LeituraDecifracao = v, "Leitura e Decifração", "Leitura e Decifracao");
            AddMany((m, v) => m.MatematicaFisica = v, "Matemática e Física", "Matematica e Fisica");
            AddMany((m, v) => m.MedicinaTeorica = v, "Medicina Teórica", "Medicina Teorica");
            AddMany((m, v) => m.CarismaCharme = v, "Carisma e Charme");
            AddMany((m, v) => m.ComercioNegocios = v, "Comércio e Negócios", "Comercio e Negocios");
            AddMany((m, v) => m.Disfarce = v, "Disfarce");
            AddMany((m, v) => m.Intimidacao = v, "Intimidação", "Intimidacao");
            AddMany((m, v) => m.PersuasaoAnimal = v, "Persuasão Animal", "Persuasao Animal");
            AddMany((m, v) => m.Armamentos = v, "Armamentos");
            AddMany((m, v) => m.Atletismo = v, "Atletismo");
            AddMany((m, v) => m.Briga = v, "Briga");
            AddMany((m, v) => m.Constituicao = v, "Constituição", "Constituicao");
            AddMany((m, v) => m.Forca = v, "Força", "Forca");
            AddMany((m, v) => m.Autocontrole = v, "Autocontrole");
            AddMany((m, v) => m.AvaliacaoRiscos = v, "Avaliação e Riscos", "Avaliacao e Riscos");
            AddMany((m, v) => m.FocoConcentracao = v, "Foco e Concentração", "Foco e Concentracao");
            AddMany((m, v) => m.MeditacaoRecobro = v, "Meditação e Recobro", "Meditacao e Recobro");
            AddMany((m, v) => m.ResilienciaDisciplina = v, "Resiliência e Disciplina", "Resiliencia e Disciplina");

            return d;
        }
    }
}
