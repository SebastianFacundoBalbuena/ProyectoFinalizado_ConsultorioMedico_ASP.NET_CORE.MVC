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
            var Usuario = HttpContext.Session.GetInt32("IdUsuario");
            if (Usuario != null)
            {
                ViewData["Admin"] = 0;
            }
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
                    ViewData["Admin"] = 0;
                }

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

                        if (ViewData["Esp"] != null)
                        {
                            ViewData["Especialidad"] = ViewData["Esp"];
                        }

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
                Medico? medico = DBcontext.Medicos.FirstOrDefault(m => m.IdEspecialidad.ToString() == IdEspecialidad);
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


        [HttpGet]
        public IActionResult VerTurno(int id,string medico,string especialidad,string paciente, string motivo, string fecha)
        {
            var IdUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var TurnoDelUsuario = DBcontext.Turnos.FirstOrDefault(t=>t.Id == id);

            if (IdUsuario != null)
            {
                ViewData["Admin"] = 0;
            }
            else
            {
                var Admin = HttpContext.Session.GetInt32("Admin");
                if(Admin != null)
                {
                    ViewData["Admin"] = 1;
                }
            }

            try
            {
                TurnosActivos activo = new TurnosActivos();
                activo.Id = id;
                activo.Medico = medico;

                if(especialidad != null)
                {
                    activo.Especialidad = especialidad;
                }
                else
                {
                    string[] partes = medico.Split(" ");
                    string Nombre = partes[0];
                    string Apellido = partes[1];

                    
                    Medico? medicoEncontrado = DBcontext.Medicos.Include(e=>e.IdEspecialidadNavigation).FirstOrDefault(n => n.Nombre == Nombre && n.Apellido == Apellido);
                    activo.Especialidad = medicoEncontrado.IdEspecialidadNavigation.Nombre;

                }
                

                activo.Paciente = paciente;
                activo.Motivo = motivo;
                activo.FechaHoraTurno = fecha;

                ViewData["TurnoActivo"] = activo;

                return View();
            }
            catch (Exception)
            {

                throw;
            }


            
        }

        public IActionResult EliminarTurno(int IdTurno)
        {
            var Turno = DBcontext.Turnos.FirstOrDefault(t=>t.Id == IdTurno);

            if(Turno != null)
            {

                DBcontext.Turnos.Remove(Turno);
                DBcontext.SaveChanges();

            }

            return RedirectToAction("Perfil","Home");
        }

    }
}
