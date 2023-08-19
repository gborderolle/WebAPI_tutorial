using API_testing3.Context;
using API_testing3.Models;
using API_testing3.Models.Dto;
using API_testing3.Repository.Interfaces;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace API_testing3.Controllers
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
        /// </summary>
        /// <returns></returns>
        [HttpGet] // url completa: https://localhost:7003/api/authors/
        [HttpGet("list")] // url completa: https://localhost:7003/api/authors/list
        [HttpGet("/list")] // url completa: https://localhost:7003/list
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AuthorDto>))]
        public async Task<ActionResult<List<AuthorDto>>> GetAuthors()
        {
            try
            {
                var authorList = await _repositoryAuthor.GetAllIncluding(null, a => a.BookList);
                _logger.LogInformation("Obtener todas los autores.");
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<AuthorDto>>(authorList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetAuthorById")] // url completa: https://localhost:7003/api/authors/1
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
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

                _response.Result = _mapper.Map<AuthorDto>(author);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
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

                _response.Result = _mapper.Map<AuthorDto>(author);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
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

                _response.Result = _mapper.Map<AuthorDto>(author);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
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

                _response.Result = _mapper.Map<AuthorDto>(author);
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorCreateDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        public async Task<ActionResult<APIResponse>> CreateAuthor([FromBody] AuthorCreateDto authorCreateDto)
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
                    _logger.LogError("El nombre ya existe en el sistema");
                    ModelState.AddModelError("NameAlreadyExists", "El nombre ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                Author modelo = _mapper.Map<Author>(authorCreateDto);
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _repositoryAuthor.Create(modelo);
                _logger.LogInformation($"Se creó correctamente la Author={modelo.Id}.");

                _response.Result = _mapper.Map<AuthorCreateDto>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetAuthor", new { id = modelo.Id }, _response); // objeto que devuelve (el que creó)
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
                    _logger.LogError($"Datos de entrada no válidos: {id}.");
                    return BadRequest(_response);
                }

                var author = await _repositoryAuthor.Get(v => v.Id == id);
                if (author == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Registro no encontrado: {id}.");
                    return NotFound(_response);
                }

                await _repositoryAuthor.Remove(author);
                _logger.LogInformation($"Se eliminó correctamente la Author={id}.");
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorUpdateDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDto updatedAuthorDto)
        {
            try
            {
                if (updatedAuthorDto == null || id != updatedAuthorDto.Id)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Datos de entrada no válidos: {id}.");
                    return BadRequest(_response);
                }

                var updatedAuthor = await _repositoryAuthor.Update(_mapper.Map<Author>(updatedAuthorDto));
                _logger.LogInformation($"Se actualizó correctamente la Author={id}.");
                _response.Result = _mapper.Map<AuthorUpdateDto>(updatedAuthor);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorUpdateDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        public async Task<ActionResult> UpdatePartialAuthor(int id, JsonPatchDocument<AuthorUpdateDto> patchDto)
        {
            try
            {
                // Validar entrada
                if (patchDto == null || id <= 0)
                {
                    _logger.LogError($"Datos de entrada no válidos: {id}.");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                // Obtener el DTO existente
                AuthorUpdateDto authorDto = _mapper.Map<AuthorUpdateDto>(await _repositoryAuthor.Get(v => v.Id == id, tracked: false));

                // Verificar si el authorDto existe
                if (authorDto == null)
                {
                    _logger.LogError($"No se encontró la Author={id}.");
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
                _logger.LogInformation($"Se actualizó correctamente la Author={id}.");

                _response.Result = _mapper.Map<AuthorUpdateDto>(updatedAuthor);
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
