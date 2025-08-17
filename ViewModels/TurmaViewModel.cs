using escola_dos_catioros.Models;

namespace escola_dos_catioros.ViewModels;

    public class TurmaViewModel
{
    public Turma Turma { get; set; } = null!;
    public System.Collections.Generic.List<Matricula> Matriculas { get; set; } = new();
}    
