using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Prova.Models;

public class Cliente : ModelBase
{
    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório.")]
    [StringLength(14, ErrorMessage = "O campo deve ter no máximo 14 caracteres.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "O campo deve conter apenas números.")]
    public string CodigoFiscal { get; set; } = null!;

    [StringLength(15, ErrorMessage = "A Inscrição Estadual deve ter no máximo 15 caracteres.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "O campo deve conter apenas números.")]
    public string InscricaoEstadual { get; set; } = null!;

    [Required(ErrorMessage = "O Nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O Nome não pode exceder {1} caracteres.")]
    public string Nome { get; set; } = null!;

    [StringLength(150, ErrorMessage = "O Nome Fantasia não pode exceder {1} caracteres.")]
    public string? NomeFantasia { get; set; }

    [Required(ErrorMessage = "O Endereço é obrigatório.")]
    public string Endereco { get; set; } = null!;

    [Required(ErrorMessage = "O Número é obrigatório.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "O campo deve conter apenas números.")]
    public string Numero { get; set; } = null!;

    [Required(ErrorMessage = "O Bairro é obrigatório.")]
    public string Bairro { get; set; } = null!;

    [Required(ErrorMessage = "A Cidade é obrigatória.")]
    public string Cidade { get; set; } = null!;

    [Required(ErrorMessage = "O Estado é obrigatório.")]
    public string Estado { get; set; } = null!;

    [Required(ErrorMessage = "A Data de Nascimento/Abertura é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; }

    [Display(Name = "Foto ou Logo")]
    [JsonIgnore]
    public IFormFile? Imagem { get; set; }

    public string? ImagemCaminho { get; set; }

    public string Senha { get; set; } = string.Empty;
}
