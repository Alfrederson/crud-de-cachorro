using System.ComponentModel.DataAnnotations;

namespace escola_dos_catioros.Models;

public class Catioro
{
    public int Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Nome { get; set; } = "Viralata";
    [Range(0, 30)]
    public int Idade { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Raca { get; set; } = "Caramelo";

}