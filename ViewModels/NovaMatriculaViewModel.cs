using System.Collections.Generic;
using escola_dos_catioros.Models;

namespace escola_dos_catioros.ViewModels;

public class NovaMatriculaViewModel
{
    public Turma Turma { get; set; } = null!;
    public List<Catioro> NaoMatriculados { get; set; } = [];
}    
