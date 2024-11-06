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
            try
            {
                var Usuario = HttpContext.Session.GetInt32("IdUsuario");
                if (Usuario != null)
                {
                    ViewData["Admin"] = 0;
                }
                return View();
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Turnos()
        {
            try
            {
                var Usuario = HttpContext.Session.GetInt32("IdUsuario");
                var mensaje = HttpContext.Session.GetString("Mensaje");
                var Ocupado = HttpContext.Session.GetString("Ocupado");

                if(mensaje != null)
                {
                    ViewData["ErrorEsp"] = mensaje;
                    HttpContext.Session.Remove("Mensaje");
                }

                if (Usuario != null)
                {
                    ViewData["Admin"] = 0;
                }
                if(Ocupado != null)
                {
                    ViewData["Ocupado"] = Ocupado;
                    HttpContext.Session.Remove("Ocupado");
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

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpPost]
        public IActionResult Turnos(string IdEspecialidad, string Motivo,DateTime Fecha,string Hora,string Minutos)
        {
            try
            {

                Paciente? paciente = DBcontext.Pacientes.FirstOrDefault(p => p.Id == HttpContext.Session.GetInt32("IdUsuario"));
                Medico? medico = DBcontext.Medicos.FirstOrDefault(m => m.IdEspecialidad.ToString() == IdEspecialidad);
                Turno NewTurno = new Turno();

                if (paciente != null && medico != null)
                {


                    NewTurno.IdMedico = medico.Id;
                    NewTurno.IdPaciente = paciente.Id;
                    NewTurno.Estado = true;
                    NewTurno.Motivo = Motivo;


                    string fecha = Fecha.ToString("yyyy-MM-dd");
                    string horacompleta = Hora +":"+ Minutos;
                    string hora = DateTime.Parse(horacompleta).ToString("HH:mm");
                    string? especialidad = DBcontext.Especialidads.FirstOrDefault(x => x.Id == int.Parse(IdEspecialidad)).Nombre;

                    if(fecha != null && hora != null)
                    {
                        List<Turno> listTurnos = DBcontext.Turnos.Include(m=>m.IdMedicoNavigation.IdEspecialidadNavigation).Where(t=>t.Estado == true).ToList();
                        foreach (var item in listTurnos)
                        {
                            string fechaa = DateTime.Parse(item.FechaDeTurno.ToString()).ToString("yyyy-MM-dd");
                            string horaa = DateTime.Parse(item.FechaDeTurno.ToString()).ToString("HH:mm");
                            string esp = item.IdMedicoNavigation.IdEspecialidadNavigation.Nombre;

                            if(fechaa == fecha && horaa == hora && esp == especialidad)
                            {
                                
                                HttpContext.Session.SetString("Ocupado", "Fecha u hora ocupadas. Seleccione otra");
                                return RedirectToAction("Turnos", "Servicios");
                            }

                        }

                        string fechaCompleta = "" + fecha + " " + hora + "";
                        NewTurno.FechaDeTurno = DateTime.ParseExact(fechaCompleta, "yyyy-MM-dd HH:mm", null);
                    }

                    

                    DBcontext.Turnos.Add(NewTurno);
                    DBcontext.SaveChanges();
                }
                else
                {
                   
                    HttpContext.Session.SetString("Mensaje", "Seleccione una especialidad");
                    return RedirectToAction("Turnos","Servicios");
                }

                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

        }


        [HttpGet]
        public IActionResult VerTurno(int id,string medico,string especialidad,string paciente, string motivo, string fecha)
        {


            try
            {
                var IdUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var TurnoDelUsuario = DBcontext.Turnos.FirstOrDefault(t => t.Id == id);

                if (IdUsuario != null)
                {
                    ViewData["Admin"] = 0;
                }
                else
                {
                    var Admin = HttpContext.Session.GetInt32("Admin");
                    if (Admin != null)
                    {
                        ViewData["Admin"] = 1;
                    }
                }

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
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }


            
        }

        public IActionResult EliminarTurno(int IdTurno)
        {
            try
            {
                var Turno = DBcontext.Turnos.FirstOrDefault(t => t.Id == IdTurno);

                if (Turno != null)
                {

                    DBcontext.Turnos.Remove(Turno);
                    DBcontext.SaveChanges();

                }

                return RedirectToAction("Perfil", "Home");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
