using System;
using System.Collections.Generic;

namespace ConsultorioMedico.Models;

public partial class Especialidad
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
}
