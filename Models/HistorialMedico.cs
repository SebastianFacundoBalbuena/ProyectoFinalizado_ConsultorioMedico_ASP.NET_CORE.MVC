using System;
using System.Collections.Generic;

namespace ConsultorioMedico.Models;

public partial class HistorialMedico
{
    public int Id { get; set; }

    public int? IdPaciente { get; set; }

    public int? IdTurno { get; set; }

    public string? Diagnostico { get; set; }

    public string? Tratamiento { get; set; }

    public virtual Paciente? IdPacienteNavigation { get; set; }

    public virtual Turno? IdTurnoNavigation { get; set; }
}
