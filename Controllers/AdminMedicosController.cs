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
        public IActionResult AgregarMedico(string Nombre,string Apellido, string Contacto, int IdEspecialidad)
        {

            Medico NewMedico = new Medico();

            if(Nombre != null && Apellido != null && Contacto != null && IdEspecialidad != 0)
            {
                NewMedico.Nombre = Nombre;
                NewMedico.Apellido = Apellido;
                NewMedico.Contacto = int.Parse(Contacto);
                NewMedico.IdEspecialidad = IdEspecialidad;

                DBcontext.Medicos.Add(NewMedico);
                DBcontext.SaveChanges();
            }

            return RedirectToAction("Index","AdminMedicos");
        }

    }
}
