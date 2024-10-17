using System;
using System.Collections.Generic;

namespace ConsultorioMedico.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Correo { get; set; }

    public string? Clave { get; set; }

    public string? ImagenUrl { get; set; }

    public int? IdPaciente { get; set; }

    public bool? EsMedico { get; set; }

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
