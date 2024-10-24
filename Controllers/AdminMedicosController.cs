using ConsultorioMedico.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Controllers
{
    public class AdminMedicosController : Controller
    {
        private readonly ConsultorioContext DBcontext;

        public AdminMedicosController(ConsultorioContext Conexion)
        {
            DBcontext = Conexion;
        }



        public IActionResult Index()
        {
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

        [HttpGet]
        public IActionResult EditarMedico(int IdMedico)
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

        [HttpPost]
        public IActionResult EditarMedico(int IdMedico, string Nombre, string Apellido, string Contacto, int IdEspecialidad)
        {
            Medico? medico = DBcontext.Medicos.FirstOrDefault(x => x.Id == IdMedico);

            if (medico != null)
            {

                medico.Nombre = Nombre;
                medico.Apellido = Apellido;
                medico.Contacto = int.Parse(Contacto);
                medico.IdEspecialidad = IdEspecialidad;

                DBcontext.Medicos.Update(medico);
                DBcontext.SaveChanges();
            }

            return RedirectToAction("Index", "AdminMedicos");
        }

        public IActionResult EliminarMedico(int Id)
        {
            Medico? medico = DBcontext.Medicos.FirstOrDefault(x => x.Id == Id);

            if (medico != null)
            {
                DBcontext.Medicos.Remove(medico);
                DBcontext.SaveChanges();
            }

            return RedirectToAction("Index", "AdminMedicos");
        }

        [HttpGet]
        public IActionResult AgregarMedico()
        {

            List<Especialidad> especialidades = DBcontext.Especialidads.ToList();

            return View(especialidades);
        }

        [HttpPost]
        public IActionResult AgregarMedico(string Nombre, string Apellido, string Contacto, int IdEspecialidad)
        {

            Medico NewMedico = new Medico();

            if (Nombre != null && Apellido != null && Contacto != null && IdEspecialidad != 0)
            {
                NewMedico.Nombre = Nombre;
                NewMedico.Apellido = Apellido;
                NewMedico.Contacto = int.Parse(Contacto);
                NewMedico.IdEspecialidad = IdEspecialidad;

                DBcontext.Medicos.Add(NewMedico);
                DBcontext.SaveChanges();
            }

            return RedirectToAction("Index", "AdminMedicos");
        }

        [HttpPost]
        public IActionResult BuscarMedico(string Nombre)
        {
            if(Nombre != null)
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

    }
}
