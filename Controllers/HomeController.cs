using ConsultorioMedico.Models;
using Microsoft.AspNetCore.Mvc;
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
            Paciente? PacienteRegistrado = DBcontext.Pacientes.FirstOrDefault(p => p.Email == newUsuario.Correo || p.Dni == newUsuario.DNI);

            if(PacienteRegistrado == null)
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UsuarioARegistrar usuario)
        {

            Paciente? usuarios = DBcontext.Pacientes.FirstOrDefault(x => x.Email == usuario.Correo && x.Clave == usuario.Clave);
            
            if(usuario.Correo == null || usuario.Clave == null)
            {
                ViewData["Mensaje"] = "Los campos son requeridos";
                return View();
            }else if (usuarios == null)
            {
                ViewData["Mensaje"] = "El usuario ingresado no existe";
                return View();
            }else if(usuarios.Administrador == true)
            {
                HttpContext.Session.SetInt32("Admin", 1);
                return RedirectToAction("Inicio", "AdminMedicos");
            }

            HttpContext.Session.SetInt32("IdUsuario", usuarios.Id);


            
            var Id = HttpContext.Session.GetInt32("IdUsuario");
            return RedirectToAction("Index");
        }



        public IActionResult Index()
        {
            var Usuario = HttpContext.Session.GetInt32("IdUsuario");
            if (Usuario != null)
            {
                ViewData["Admin"] = 0;
            }

            return View();
        }

        [HttpGet]
       public IActionResult Perfil()
        {
            try
            {
                var IdUsuario = HttpContext.Session.GetInt32("IdUsuario");

               
                if (IdUsuario != null)
                {
                    ViewData["Admin"] = 0;
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

                if(turnosActivos.Count != 0)
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

                throw ex;
            }

        }

        [HttpPost]
        public IActionResult Perfil(string Nombre,string Apellido, string Contacto, string Contraseña, string Sexo, string Sangre,string fechaNacimiento)
        {
            var IdUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Paciente? paciente = DBcontext.Pacientes.FirstOrDefault(p => p.Id == IdUsuario);

            try
            {
                if(Nombre != null && Apellido  != null && Contacto != null && Contraseña != null && Sexo != null && Sangre != null && fechaNacimiento != null){

                    paciente.Nombre = Nombre;
                    paciente.Apellido = Apellido;
                    paciente.Contacto = int.Parse(Contacto);
                    paciente.Clave = Contraseña;
                    paciente.Sexo = Sexo;
                    paciente.TipoDeSangre = Sangre;
                    DateTime fechavieja = DateTime.ParseExact(fechaNacimiento, "dd/M/yyyy", CultureInfo.InvariantCulture);
                    string nuevaFecha = fechavieja.ToString("yyyy-MM-dd");
                    paciente.FechaDeNacimiento = DateOnly.ParseExact(nuevaFecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);

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

                throw ex;
            }


            return RedirectToAction("Perfil","Home");
        }

        public IActionResult CerrarSesion()
        {
            var Usuario = HttpContext.Session.GetInt32("IdUsuario");
            var Medico = HttpContext.Session.GetInt32("Admin");

            if(Usuario != null)
            {
                HttpContext.Session.Remove("IdUsuario");
                return RedirectToAction("Index", "Home");
            }else if(Medico != null)
            {
                HttpContext.Session.Remove("Admin");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");

        }

        public IActionResult RedirigirContactos()
        {
            return Redirect(Url.Action("Index", "Home") + "#contacto");
        }

    }
}
