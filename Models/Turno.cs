using System;
using System.Collections.Generic;

namespace ConsultorioMedico.Models;

public partial class Turno
{
    public int Id { get; set; }

    public int? IdMedico { get; set; }

    public int? IdPaciente { get; set; }

    public DateTime? FechaDeTurno { get; set; }

    public bool? Estado { get; set; }

    public string? Motivo { get; set; }

    public string? Devolucion { get; set; }

    public virtual ICollection<HistorialMedico> HistorialMedicos { get; set; } = new List<HistorialMedico>();

    public virtual Medico? IdMedicoNavigation { get; set; }

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
