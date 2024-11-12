using ConsultorioMedico.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;

namespace ConsultorioMedico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConsultorioContext DBcontext;

        public HomeController(ConsultorioContext Conexion)
        {
            DBcontext = Conexion;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrarse(UsuarioARegistrar newUsuario)
        {
            try
            {
                Paciente? PacienteRegistrado = DBcontext.Pacientes.FirstOrDefault(p => p.Email == newUsuario.Correo || p.Dni == newUsuario.DNI);

                if (PacienteRegistrado == null)
                {
                    Paciente usuario = new Paciente();
                    usuario.Nombre = newUsuario.Nombre;
                    usuario.Apellido = newUsuario.Apellido;
                    usuario.Dni = newUsuario.DNI;
                    usuario.Email = newUsuario.Correo;
                    usuario.Clave = newUsuario.Clave;


                    if (usuario.Dni == 0 || usuario.Email == null || usuario.Clave == null)
                    {
                        ViewData["Mensaje"] = "Algunos campos son requeridos";
                        return View();
                    }
                    else
                    {
                        DBcontext.Pacientes.Add(usuario);
                        DBcontext.SaveChanges();
                    }

                }


                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }


        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UsuarioARegistrar usuario)
        {

            try
            {
                Paciente? usuarios = DBcontext.Pacientes.FirstOrDefault(x => x.Email == usuario.Correo && x.Clave == usuario.Clave);
                Medico? Medico = DBcontext.Medicos.FirstOrDefault(x => x.Email == usuario.Correo && x.Clave == usuario.Clave);

                if (usuario.Correo == null || usuario.Clave == null)
                {
                    ViewData["Mensaje"] = "Los campos son requeridos";
                    return View();
                }

                if (usuarios != null)
                {
                    HttpContext.Session.SetInt32("IdUsuario", usuarios.Id);
                    return RedirectToAction("Index");

                }
                else if (Medico != null)
                {

                    if (Medico.Administrador == true)
                    {
                        HttpContext.Session.SetInt32("Admin", 1);
                        HttpContext.Session.SetInt32("MedicoId", Medico.Id);
                        return RedirectToAction("Inicio", "AdminMedicos");
                    }


                }
                else
                {
                    ViewData["Mensaje"] = "El usuario ingresado no existe";
                    return View();
                }
            }
            catch (Exception ex)
            {


                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }


            return RedirectToAction("Index");
        }



        public IActionResult Index()
        {
            try
            {
                var Usuario = HttpContext.Session.GetInt32("IdUsuario");
                var Medico = HttpContext.Session.GetInt32("MedicoId");


                if (Usuario != null)
                {
                    ViewData["Admin"] = 0;
                }
                else if (Medico != null)
                {
                    return RedirectToAction("Inicio", "AdminMedicos");
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
        public IActionResult Perfil()
        {
            try
            {
                var IdUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var Admin = HttpContext.Session.GetInt32("Admin");
                List<Turno> ListaDevoluciones = DBcontext.Turnos.Include(m => m.IdMedicoNavigation.IdEspecialidadNavigation).Include(p => p.IdPacienteNavigation).Where(d => d.Devolucion != null && d.IdPaciente == IdUsuario).ToList();


                if (IdUsuario != null)
                {
                    ViewData["Admin"] = 0;
                }
                else if (Admin != null)
                {
                    return RedirectToAction("Inicio", "AdminMedicos");
                }
                if (ListaDevoluciones != null)
                {
                    ViewData["Devoluciones"] = ListaDevoluciones;
                }

                //Guardando lista de turnos
                List<Turno>? TurnosDelUsuario = DBcontext.Turnos.Where(t => t.IdPaciente == IdUsuario && t.Estado == true).ToList();
                List<Medico> medicos = DBcontext.Medicos.ToList();
                List<Especialidad> especialidades = DBcontext.Especialidads.ToList();
                Paciente? paciente = DBcontext.Pacientes.FirstOrDefault(p => p.Id == IdUsuario);
                List<TurnosActivos> turnosActivos = new List<TurnosActivos>();

                if (TurnosDelUsuario != null)
                {
                    foreach (var Turno in TurnosDelUsuario)
                    {
                        foreach (var medico in medicos)
                        {
                            if (Turno.IdMedico == medico.Id)
                            {
                                foreach (var especialidad in especialidades)
                                {
                                    if (medico.IdEspecialidad == especialidad.Id)
                                    {
                                        TurnosActivos NewTurnoActivo = new TurnosActivos();
                                        NewTurnoActivo.Id = Turno.Id;
                                        NewTurnoActivo.Medico = medico.Nombre + " " + medico.Apellido;
                                        NewTurnoActivo.Especialidad = especialidad.Nombre;
                                        NewTurnoActivo.Paciente = paciente.Nombre + " " + paciente.Apellido;
                                        NewTurnoActivo.Motivo = Turno.Motivo;
                                        NewTurnoActivo.FechaHoraTurno = Turno.FechaDeTurno.ToString();

                                        turnosActivos.Add(NewTurnoActivo);

                                    }
                                }
                            }
                        }
                    }
                }

                if (turnosActivos.Count != 0)
                {
                    ViewData["ListaTurnosActivos"] = turnosActivos;
                }
                else
                {
                    ViewData["ListaTurnosActivos"] = null;
                }


                //-----------------------------------------------------------------------------------------------------------------


                if (IdUsuario == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(paciente);
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpPost]
        public IActionResult Perfil(string Nombre, string Apellido, string Contacto, string Contraseña, string Sexo, string Sangre, string fechaNacimiento)
        {
            var IdUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Paciente? paciente = DBcontext.Pacientes.FirstOrDefault(p => p.Id == IdUsuario);

            try
            {


                if (Nombre != null && Apellido != null && Contacto != null && Contraseña != null && Sexo != null && Sangre != null && fechaNacimiento != null)
                {

                    paciente.Nombre = Nombre;
                    paciente.Apellido = Apellido;

                    try
                    {
                        paciente.Contacto = int.Parse(Contacto);
                    }
                    catch (Exception)
                    {

                        ViewData["ErrorContacto"] = "Asegurese de que el formato 'Contacto' sea numerico";
                        return View(paciente);
                    }


                    paciente.Clave = Contraseña;
                    paciente.Sexo = Sexo;
                    paciente.TipoDeSangre = Sangre;

                    try
                    {
                        DateTime fechavieja = DateTime.ParseExact(fechaNacimiento, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        string nuevaFecha = fechavieja.ToString("yyyy-MM-dd");
                        paciente.FechaDeNacimiento = DateOnly.ParseExact(nuevaFecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {

                        ViewData["ErrorFecha"] = "Asegurese de que el formato 'Fecha' sea: yyyy/MM/dd";
                        return View(paciente);
                    }


                    ViewData["GuardadoExitoso"] = "¡Datos guardados exitosamente!";
                    DBcontext.Pacientes.Update(paciente);
                    DBcontext.SaveChanges();
                }
                else
                {
                    ViewData["Error"] = "Algunos campos estan vacios...";
                    return View(paciente);
                }


            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }


            return View(paciente);
        }

        public IActionResult CerrarSesion()
        {
            try
            {
                var Usuario = HttpContext.Session.GetInt32("IdUsuario");
                var Medico = HttpContext.Session.GetInt32("Admin");

                if (Usuario != null)
                {
                    HttpContext.Session.Remove("IdUsuario");
                    return RedirectToAction("Index", "Home");
                }
                else if (Medico != null)
                {
                    HttpContext.Session.Remove("Admin");
                    HttpContext.Session.Remove("MedicoId");
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

        }

        public IActionResult RedirigirContactos()
        {
            return Redirect(Url.Action("Index", "Home") + "#contacto");
        }

        public IActionResult VerDevolucion(int Id)
        {
            try
            {
                var Usuario = HttpContext.Session.GetInt32("IdUsuario");
                var Medico = HttpContext.Session.GetInt32("Admin");

                if (Usuario != null)
                {
                    HttpContext.Session.Remove("IdUsuario");
                    return RedirectToAction("Index", "Home");
                }
                else if (Medico != null)
                {
                    HttpContext.Session.Remove("Admin");
                    HttpContext.Session.Remove("MedicoId");
                    return RedirectToAction("Index", "Home");
                }


                Turno? devolucion = DBcontext.Turnos.Include(m => m.IdMedicoNavigation).Include(p => p.IdPacienteNavigation).Include(e => e.IdMedicoNavigation.IdEspecialidadNavigation).FirstOrDefault(x => x.Id == Id);

                if (devolucion != null)
                {
                    return View(devolucion);
                }

                return View();
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }


        }


        public IActionResult Error()
        {

            var Usuario = HttpContext.Session.GetInt32("IdUsuario");
            var Medico = HttpContext.Session.GetInt32("Admin");

            if (Usuario != null)
            {
                HttpContext.Session.Remove("IdUsuario");
                return RedirectToAction("Index", "Home");
            }
            else if (Medico != null)
            {
                HttpContext.Session.Remove("Admin");
                HttpContext.Session.Remove("MedicoId");
                return RedirectToAction("Index", "Home");
            }


            var ErrorGeneral = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var Error = HttpContext.Session.GetString("Error");

            if (Error != null)
            {
                ViewData["Error"] = Error;

            }
            else if (ErrorGeneral != null)
            {
                var captura = ErrorGeneral.Error;
                var Mensaje = captura.Message;

                ViewData["Error"] = Mensaje;
            }

            return View();

        }
    }
}
