using System.ComponentModel.DataAnnotations;
namespace escola_dos_catioros.Models;

public class Matricula
{
    public int Id { get; set; }

    [Required]
    public int CatioroId { get; set; }

    [Required]
    public int TurmaId { get; set; }

    public Catioro Catioro { get; set; } = null!;
    public Turma Turma { get; set; } = null!;
}