namespace ConsultorioMedico.Models
{
    public class TurnosActivos
    {

        public int Id { get; set; }

        public string Medico {  get; set; }

        public string Especialidad {  get; set; }

        public string Paciente {  get; set; }


        public string Motivo {  get; set; }

        public string FechaHoraTurno { get; set; }


    }
}
