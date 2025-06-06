﻿using GestionUniversidad.Dtos.Usuario;
using GestionUniversidad.Services;
using GestionUniversidad.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestionUniversidad.Authentication
{
    [Route("api/login")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EstudianteService estudianteService;
        private readonly DocenteService docenteService;
        private readonly AuthService authService;

        public AuthController(DocenteService docenteService, AuthService authService, EstudianteService estudianteService)
        {
            this.docenteService = docenteService;
            this.authService = authService;
            this.estudianteService = estudianteService;
        }

        [HttpPost]
        [Route("docentes/ingresar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> IngresarDocente(UsuarioLoginDto usuarioLoginDto)
        {

            try
            {
                var usuario = await docenteService.Autenticar(usuarioLoginDto);

                if (usuario == null)
                    return ManejoException.BadRequest("Credenciales incorrectas");

                string nombreCompleto = usuario.Nombre + " " + usuario.Apellido;
                string token = authService.GenerarToken(usuario.Id, nombreCompleto, usuario.Email, usuario.Rol!.NombreRol);

                return Ok(new
                {
                    codigo = 200,
                    usuario = new
                    {
                        id = usuario.Id,
                        nombre = nombreCompleto,
                        token
                    },
                });

            }
            catch (Exception error)
            {
                return ManejoException.ServerError(error.Message);
            }


        }

        [HttpPost]
        [Route("estudiantes/ingresar")]
        public async Task<ActionResult> IngresarEstudiante(UsuarioLoginDto usuarioLoginDto)
        {
            try
            {
                var usuario = await estudianteService.Autenticar(usuarioLoginDto);

                if (usuario == null)
                    return ManejoException.BadRequest("Credenciales Incorrectas");

                string nombreCompleto = usuario.Nombre + " " + usuario.Apellido;
                string token = authService.GenerarToken(usuario.Id, nombreCompleto, usuario.Email, usuario.Rol!.NombreRol);

                return Ok(new
                {
                    codigo = 200,
                    usuario = new
                    {
                        id = usuario.Id,
                        nombre = nombreCompleto,
                        token
                    },
                });

            }
            catch (Exception error)
            {
                return ManejoException.ServerError(error.Message);
            }
        }
    }
}
