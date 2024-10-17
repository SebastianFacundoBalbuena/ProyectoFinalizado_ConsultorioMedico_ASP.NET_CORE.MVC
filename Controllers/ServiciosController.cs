using ConsultorioMedico.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ConsultorioMedico.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly ConsultorioContext DBcontext;

        public ServiciosController(ConsultorioContext Conexion)
        {
            DBcontext = Conexion;
        }

        public IActionResult Servicios()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Turnos()
        {
            try
            {
                var Usuario = HttpContext.Session.GetInt32("IdUsuario");
                if (Usuario != null)
                {
                    List<Especialidad> listEsp = DBcontext.Especialidads.ToList();


                    var IdPaciente = HttpContext.Session.GetInt32("IdUsuario");
                    Paciente? paciente = DBcontext.Pacientes.FirstOrDefault(x => x.Id == IdPaciente);

                    if (paciente != null)
                    {
                        ViewData["Nombre"] = paciente.Nombre;
                        ViewData["Apellido"] = paciente.Apellido;
                        ViewData["DNI"] = paciente.Dni;
                    }

                    return View(listEsp);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpPost]
        public IActionResult Turnos(string IdEspecialidad, string Motivo,DateTime FechaHora)
        {
            try
            {

                Paciente? paciente = DBcontext.Pacientes.FirstOrDefault(p => p.Id == HttpContext.Session.GetInt32("IdUsuario"));
                Medico? medico = DBcontext.Medicos.FirstOrDefault(m => m.IdEspecialidad == int.Parse(IdEspecialidad));
                Turno NewTurno = new Turno();

                if(paciente != null && medico != null)
                {
                    

                    NewTurno.IdMedico = medico.Id;
                    NewTurno.IdPaciente = paciente.Id;
                    NewTurno.Estado = true;
                    NewTurno.Motivo = Motivo;
                    string fecha = FechaHora.ToString("yyyy-MM-dd HH:mm");
                    NewTurno.FechaDeTurno = DateTime.Parse(fecha);

                    DBcontext.Turnos.Add(NewTurno);
                    DBcontext.SaveChanges();
                }

                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
