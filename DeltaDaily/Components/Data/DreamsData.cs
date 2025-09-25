using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class DreamsData
    {
        public enum SonhoTipo
        {
            [Display(Name = "Agitados")] Agitados,
            [Display(Name = "Pesadelo com Trabalho")] PesadelosTrabalho,
            [Display(Name = "Pesadelo com Vi")] PesadelosVi,
            [Display(Name = "Pesadelo com Família")] PesadelosFamilia,
            [Display(Name = "Pesadelos Diferentes")] PesadelosDiferentes,
            [Display(Name = "Sexual")] Sexuais
        }
    }
}
