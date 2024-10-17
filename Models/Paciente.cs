using System;
using System.Collections.Generic;

namespace ConsultorioMedico.Models;

public partial class Paciente
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public int? Contacto { get; set; }

    public string? Email { get; set; }

    public string? Clave { get; set; }

    public string? Sexo { get; set; }

    public string? TipoDeSangre { get; set; }

    public int? Dni { get; set; }

    public DateOnly? FechaDeNacimiento { get; set; }

    public virtual ICollection<HistorialMedico> HistorialMedicos { get; set; } = new List<HistorialMedico>();

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
