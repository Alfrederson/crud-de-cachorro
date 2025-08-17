using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace escola_dos_catioros.Models;


public class Turma
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Apelido { get; set; } = "Turma";

    public virtual ICollection<Matricula> Matriculas { get; set; } = [];

}