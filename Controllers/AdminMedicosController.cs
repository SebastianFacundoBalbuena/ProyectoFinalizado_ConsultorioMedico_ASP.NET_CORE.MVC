﻿using ConsultorioMedico.Models;
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



        public IActionResult Inicio()
        {
            var Admin = HttpContext.Session.GetInt32("Admin");

            if(Admin != null && Admin != 0)
            {
                ViewData["Admin"] = 1;

                List<Turno> ListaTurnos = new List<Turno>();
                ListaTurnos = DBcontext.Turnos.Include(p=>p.IdPacienteNavigation).Include(m=>m.IdMedicoNavigation).Include(e=>e.IdMedicoNavigation.IdEspecialidadNavigation).ToList();



                return View(ListaTurnos);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {

            var Admin = HttpContext.Session.GetInt32("Admin");
            
            if(Admin != null && Admin == 1)
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



        [HttpGet]
        public IActionResult Devolucion(string id, string medico, string paciente, string motivo,string especialidad,string DNI)
        {
            TurnosActivos activo = new TurnosActivos();
            activo.Id = int.Parse(id);
            activo.Medico = medico;
            activo.Paciente = paciente;
            activo.pacienteDNI = DNI;
            activo.Motivo = motivo;
            activo.Especialidad = especialidad;
            

            return View(activo);
        }

        [HttpPost]
        public IActionResult Devolucion()
        {


            return View();
        }
    }
}
