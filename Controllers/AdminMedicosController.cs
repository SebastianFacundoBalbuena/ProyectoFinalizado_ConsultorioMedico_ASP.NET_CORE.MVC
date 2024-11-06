using ConsultorioMedico.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;

namespace ConsultorioMedico.Controllers
{
    public class AdminMedicosController : Controller
    {
        private readonly ConsultorioContext DBcontext;

        public AdminMedicosController(ConsultorioContext Conexion)
        {
            DBcontext = Conexion;
        }



        public IActionResult Inicio()
        {
            try
            {
                var Admin = HttpContext.Session.GetInt32("Admin");
                var MedicoId = HttpContext.Session.GetInt32("MedicoId");
                List<Turno>? listaDevoluciones = DBcontext.Turnos.Include(m => m.IdMedicoNavigation.IdEspecialidadNavigation).Include(p => p.IdPacienteNavigation).Where(t => t.Devolucion != null && t.IdMedico == MedicoId).ToList();

                if (Admin != null && Admin != 0)
                {
                    ViewData["Admin"] = 1;


                    List<Turno> ListaTurnos = new List<Turno>();
                    ListaTurnos = DBcontext.Turnos.Include(p => p.IdPacienteNavigation).Include(m => m.IdMedicoNavigation).Include(e => e.IdMedicoNavigation.IdEspecialidadNavigation).Where(e => e.Estado == true && e.IdMedico == MedicoId).ToList();

                    if (listaDevoluciones != null)
                    {
                        ViewData["Devoluciones"] = listaDevoluciones;
                    }

                    ViewData["Especialista"] = DBcontext.Medicos.Include(e => e.IdEspecialidadNavigation).FirstOrDefault(x => x.Id == MedicoId);
                    return View(ListaTurnos);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Index()
        {

            try
            {
                var Admin = HttpContext.Session.GetInt32("Admin");

                if (Admin != null && Admin == 1)
                {
                    ViewData["Admin"] = 1;

                    List<Medico> listMedicos = DBcontext.Medicos.Include(m => m.IdEspecialidadNavigation).ToList();

                    int? medicoId = HttpContext.Session.GetInt32("MedicoIdEncontrado");
                    Medico? medicoEncontrado = null;

                    if (medicoId != null)
                    {
                        // Buscar al médico por el Id
                        medicoEncontrado = DBcontext.Medicos.Include(x => x.IdEspecialidadNavigation).FirstOrDefault(m => m.Id == medicoId.Value);

                        ViewData["MedicoEncontrado"] = medicoEncontrado;
                        HttpContext.Session.Remove("MedicoIdEncontrado");

                        return View();
                    }

                    return View(listMedicos);
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

        [HttpGet]
        public IActionResult EditarMedico(int IdMedico)
        {
            try
            {
                Medico? medico = DBcontext.Medicos.Include(x => x.IdEspecialidadNavigation).FirstOrDefault(x => x.Id == IdMedico);
                List<Especialidad> especialidades = DBcontext.Especialidads.ToList();

                if (medico != null)
                {
                    ViewData["Medico"] = medico;
                    ViewData["Especialidades"] = especialidades;
                }

                return View();
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult EditarMedico(int IdMedico, string Nombre, string Apellido, string Contacto,string Email,string Clave, int IdEspecialidad)
        {
            try
            {
                Medico? medico = DBcontext.Medicos.FirstOrDefault(x => x.Id == IdMedico);

                if (medico != null)
                {

                    medico.Nombre = Nombre;
                    medico.Apellido = Apellido;
                    medico.Contacto = int.Parse(Contacto);
                    medico.Email = Email;
                    medico.Clave = Clave;
                    medico.IdEspecialidad = IdEspecialidad;

                    DBcontext.Medicos.Update(medico);
                    DBcontext.SaveChanges();
                }

                return RedirectToAction("Index", "AdminMedicos");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult EliminarMedico(int Id)
        {
            try
            {
                Medico? medico = DBcontext.Medicos.FirstOrDefault(x => x.Id == Id);

                if (medico != null)
                {
                    DBcontext.Medicos.Remove(medico);
                    DBcontext.SaveChanges();
                }

                return RedirectToAction("Index", "AdminMedicos");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult AgregarMedico()
        {
            try
            {
                var Admin = HttpContext.Session.GetInt32("Admin");

                if (Admin != null && Admin == 1)
                {
                    ViewData["Admin"] = 1;
                    List<Especialidad> especialidades = DBcontext.Especialidads.ToList();

                    return View(especialidades);
                }

                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpPost]
        public IActionResult AgregarMedico(string Nombre, string Apellido, string Contacto,string Email,string Clave, int IdEspecialidad)
        {

            try
            {
                Medico NewMedico = new Medico();

                if (Nombre != null && Apellido != null && Contacto != null && IdEspecialidad != 0)
                {
                    NewMedico.Nombre = Nombre;
                    NewMedico.Apellido = Apellido;
                    NewMedico.Contacto = int.Parse(Contacto);
                    NewMedico.Email = Email;
                    NewMedico.Clave = Clave;
                    NewMedico.IdEspecialidad = IdEspecialidad;
                    NewMedico.Administrador = true;


                    DBcontext.Medicos.Add(NewMedico);
                    DBcontext.SaveChanges();
                }

                return RedirectToAction("Index", "AdminMedicos");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpPost]
        public IActionResult BuscarMedico(string Nombre)
        {
            try
            {
                if (Nombre != null)
                {
                    string[] partes = Nombre.Split(" ");

                    if (partes.Length == 2)
                    {
                        string NombreMedico = partes[0];
                        string ApellidoMedico = partes[1];

                        Medico? medico = DBcontext.Medicos.Include(x => x.IdEspecialidadNavigation).FirstOrDefault(x => x.Nombre.ToUpper() == NombreMedico.ToUpper() && x.Apellido.ToUpper() == ApellidoMedico.ToUpper());


                        if (medico != null)
                        {
                            HttpContext.Session.SetInt32("MedicoIdEncontrado", medico.Id);
                        }
                    }


                }



                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }



        [HttpGet]
        public IActionResult Devolucion(string id, string medico, string paciente, string motivo,string especialidad,string DNI)
        {

            try
            {
                var Admin = HttpContext.Session.GetInt32("Admin");


                if (Admin != null && Admin == 1)
                {

                    ViewData["Admin"] = 1;

                    TurnosActivos activo = new TurnosActivos();
                    activo.Id = int.Parse(id);
                    activo.Medico = medico;
                    activo.Paciente = paciente;
                    activo.pacienteDNI = DNI;
                    activo.Motivo = motivo;
                    activo.Especialidad = especialidad;


                    return View(activo);
                }

                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Devolucion(string Id,string Devolucion)
        {
            try
            {
                if(Id != null && Devolucion != null)
                {
                    Turno? turno = DBcontext.Turnos.FirstOrDefault(t => t.Id == int.Parse(Id));

                    if(turno != null)
                    {
                        turno.Devolucion = Devolucion;
                        turno.Estado = false;
                        DBcontext.Turnos.Update(turno);
                        DBcontext.SaveChanges();

                        return RedirectToAction("Inicio", "AdminMedicos");
                    }
                }

                return View();
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Error", ex.Message.ToString());
                return RedirectToAction("Error", "Home");
            }

            
        }
    }
}
