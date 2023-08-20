using WebAPI_tutorial.Context;
using WebAPI_tutorial.Models;
using WebAPI_tutorial.Models.DTOs;
using WebAPI_tutorial.Repository.Interfaces;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using WebAPI_tutorial.Filters;

namespace WebAPI_tutorial.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger; // Logger para registrar eventos.
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _repositoryAuthor; // Servicio que contiene la lógica principal de negocio para authors.
        protected APIResponse _response;

        public AuthorsController(ILogger<AuthorsController> logger, IMapper mapper, IAuthorRepository repositoryAuthor)
        {
            _logger = logger;
            _mapper = mapper;
            _repositoryAuthor = repositoryAuthor;
            _response = new();
        }

        /// <summary>
        /// Tipos de retorno:
        /// 1. Tipo de dato puro síncrono: List<AutorDto>: no sirve
        /// 2. Tipo de dato puro síncrono: ActionResult<List<AutorDto>>: permite retornar objetos controlados: ResultOk() etc
        /// 3. Tipo de dato puro asíncrono: Task<ActionResult<List<AutorDto>>>: es asíncrono: no espera al método para seguir la ejecución
        /// 4. IActionResult: está depracated, no se usa más.
        /// 
        /// Programación síncrona
        /// Task: retorna void
        /// Task<T>: retorna un tipo de dato T
        /// Sólo usar síncrona cuando el método se conecta con otra API o con la BD: Task, async y await.
        /// 
        /// APIResponse: estandariza las respuestas con mensajes tipo http además de lo solicitado
        /// 
        /// Model binding: para recibir parámetros puede ser:
        /// Como parte de la url con "/" (sin nada): no especifica cómo llega. ej: GetAuthor(int id) --> "https://localhost:7003/api/authors/12"
        /// FromBody: ej: GetAuthor([FromBody] int id) --> "id va oculto en el body"
        /// FromHeader: ej: GetAuthor([FromHeader] int id) --> "id va oculto en el header"
        /// FromQuery: desde el QueryString. ej: GetAuthor([FromQuery] int id) --> ".com?id=12"
        /// "combinación": combina varios tipos de params. ej: GetAuthor([FromHeader] int id, [FromQuery] string nombre) --> ".com?nombre=felipe"
        /// FromForm: [FromForm] para recibir archivos
        /// </summary>
        /// <returns></returns>
        [HttpGet] // url completa: https://localhost:7003/api/authors/
        [HttpGet("list")] // url completa: https://localhost:7003/api/authors/list
        [HttpGet("/list")] // url completa: https://localhost:7003/list
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AuthorDTO>))]
        public async Task<ActionResult<List<AuthorDTO>>> GetAuthors()
        {
            try
            {
                var authorList = await _repositoryAuthor.GetAllIncluding(null, a => a.BookList);
                _logger.LogInformation("Obtener todas los autores.");
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<AuthorDTO>>(authorList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetAuthorById")] // url completa: https://localhost:7003/api/authors/1
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAuthor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error al obtener el autor ID = {id}");
                    return BadRequest(_response);
                }

                var author = await _repositoryAuthor.Get(v => v.Id == id);
                if (author == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"El autor ID = {id} no existe.");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<AuthorDTO>(author);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{name}", Name = "GetAuthorByName")] // url completa: https://localhost:7003/api/authors/gonzalo
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAuthor(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error al obtener el autor nombre = {name}");
                    return BadRequest(_response);
                }

                var author = await _repositoryAuthor.Get(v => v.Name.ToLower().Contains(name.ToLower()));
                if (author == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"El autor nombre = {name} no existe.");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<AuthorDTO>(author);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        /// <summary>
        /// Caso: dos parámetros
        /// parámetro int: {id:int} (el tipo de dato no sirve para string)
        /// parámetro opcional: {name?}
        /// parámetro con valor por defecto: {name=luis}
        /// 
        /// Los parámetros tienen que declararse en la entrada de variables del método también
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{id:int}/{name}", Name = "GetAuthorByIdOrName")] // url completa: https://localhost:7003/api/authors/1/gonzalo
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAuthor(int id, string name)
        {
            try
            {
                if (id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error al obtener el autor ID = {id}");
                    return BadRequest(_response);
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error al obtener el autor nombre = {name}");
                    return BadRequest(_response);
                }

                var author = await _repositoryAuthor.Get(v => v.Id == id);
                if (author == null)
                {
                    author = await _repositoryAuthor.Get(v => v.Name.Contains(name));
                    if (author == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError($"El autor no existe.");
                        return NotFound(_response);
                    }
                }

                _response.Result = _mapper.Map<AuthorDTO>(author);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("GetFirstAuthor")] // url completa: api/authors/GetFirstAuthor
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetFirstAuthor()
        {
            try
            {
                var author = await _repositoryAuthor.Get();
                if (author == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No hay autores.");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<AuthorDTO>(author);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorCreateDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        public async Task<ActionResult<APIResponse>> CreateAuthor([FromBody] AuthorCreateDTO authorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Ocurrió un error en el servidor.");
                    return BadRequest(ModelState);
                }
                if (await _repositoryAuthor.Get(v => v.Name.ToLower() == authorCreateDto.Name.ToLower()) != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"El nombre {authorCreateDto.Name} ya existe en el sistema");
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {authorCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                Author modelo = _mapper.Map<Author>(authorCreateDto);
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _repositoryAuthor.Create(modelo);
                _logger.LogInformation($"Se creó correctamente el autor Id:{modelo.Id}.");

                _response.Result = _mapper.Map<AuthorCreateDTO>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                // Cuidado que exista un endpoint con la misma firma
                return CreatedAtRoute("GetAuthorById", new { id = modelo.Id }, _response); // objeto que devuelve (el que creó). 
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"El Id {id} es inválido.");
                    return BadRequest(_response);
                }

                var author = await _repositoryAuthor.Get(v => v.Id == id);
                if (author == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Autor no encontrado, Id: {id}.");
                    return NotFound(_response);
                }

                await _repositoryAuthor.Remove(author);
                _logger.LogInformation($"Se eliminó correctamente el autor Id:{id}.");
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return BadRequest(_response);
        }

        // Endpoint para actualizar una author por ID.
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorUpdateDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDTO updatedAuthorDto)
        {
            try
            {
                if (updatedAuthorDto == null || id != updatedAuthorDto.Id)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"El Id {id} es inválido.");
                    return BadRequest(_response);
                }

                var updatedAuthor = await _repositoryAuthor.Update(_mapper.Map<Author>(updatedAuthorDto));
                _logger.LogInformation($"Se actualizó correctamente el autor Id:{id}.");
                _response.Result = _mapper.Map<AuthorUpdateDTO>(updatedAuthor);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString()
            };
            }
            return BadRequest(_response);
        }

        // Endpoint para hacer una actualización parcial de una author por ID.
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorUpdateDTO))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        public async Task<ActionResult> UpdatePartialAuthor(int id, JsonPatchDocument<AuthorUpdateDTO> patchDto)
        {
            try
            {
                // Validar entrada
                if (patchDto == null || id <= 0)
                {
                    _logger.LogError($"El Id {id} es inválido.");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                // Obtener el DTO existente
                AuthorUpdateDTO authorDto = _mapper.Map<AuthorUpdateDTO>(await _repositoryAuthor.Get(v => v.Id == id, tracked: false));

                // Verificar si el authorDto existe
                if (authorDto == null)
                {
                    _logger.LogError($"Autor no encontrado, Id: {id}.");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Aplicar el parche
                patchDto.ApplyTo(authorDto, error =>
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"Ocurrió un error en el servidor.");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ModelState);
                }

                Author author = _mapper.Map<Author>(authorDto);
                var updatedAuthor = await _repositoryAuthor.Update(author);
                _logger.LogInformation($"Se actualizó correctamente el autor Id:{id}.");

                _response.Result = _mapper.Map<AuthorUpdateDTO>(updatedAuthor);
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

    }
}
