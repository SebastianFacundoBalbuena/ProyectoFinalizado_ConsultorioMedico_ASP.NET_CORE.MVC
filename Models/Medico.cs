using System;
using System.Collections.Generic;

namespace ConsultorioMedico.Models;

public partial class Medico
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public int? Contacto { get; set; }

    public int? IdEspecialidad { get; set; }

    public bool? Administrador { get; set; }

    public virtual Especialidad? IdEspecialidadNavigation { get; set; }

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
