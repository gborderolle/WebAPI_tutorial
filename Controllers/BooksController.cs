using WebAPI_tutorial.Context;
using WebAPI_tutorial.Models.Dto;
using WebAPI_tutorial.Models;
using WebAPI_tutorial.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI_tutorial.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger; // Logger para registrar eventos.
        private readonly IMapper _mapper;
        private readonly IBookRepository _repositoryBook; // Servicio que contiene la lógica principal de negocio para libros.
        private readonly IAuthorRepository _repositoryAuthor; 
        protected APIResponse _response;

        public BooksController(ILogger<BooksController> logger, IMapper mapper, IBookRepository repositoryBook, IAuthorRepository repositoryAuthor)
        {
            _logger = logger;
            _mapper = mapper;
            _repositoryBook = repositoryBook;
            _response = new();
            _repositoryAuthor = repositoryAuthor;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookDto>))]
        public async Task<ActionResult<List<BookDto>>> GetBooks()
        {
            try
            {
                var bookList = await _repositoryBook.GetAll(includes: b => b.Author); // incluye los autores de cada libro
                _logger.LogInformation("Obtener todas las libros.");
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<BookDto>>(bookList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBook(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error al obtener el libro = {id}");
                    return BadRequest(_response);
                }

                var book = await _repositoryBook.Get(v => v.Id == id, includes: b => b.Author); // incluye los autores del libro
                if (book == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"El libro ID = {id} no existe.");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<BookDto>(book);
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookCreateDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        public async Task<ActionResult<APIResponse>> CreateBook([FromBody] BookCreateDto libroCreateDto)
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
                if (await _repositoryBook.Get(v => v.Title.ToLower() == libroCreateDto.Title.ToLower()) != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("El nombre ya existe en el sistema.");
                    ModelState.AddModelError("NameAlreadyExists", "El nombre ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                if (await _repositoryAuthor.Get(v => v.Id == libroCreateDto.AuthorId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError("El autor no existe en el sistema.");
                    ModelState.AddModelError("AuthorNotExists", "El autor no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                Book modelo = _mapper.Map<Book>(libroCreateDto);
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _repositoryBook.Create(modelo);
                _logger.LogInformation($"Se creó correctamente el libro = {modelo.Id}.");

                _response.Result = _mapper.Map<BookCreateDto>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetBook", new { id = modelo.Id }, _response); // objeto que devuelve (el que creó)
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
        public async Task<IActionResult> DeleteBook(int id)
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

                var libro = await _repositoryBook.Get(v => v.Id == id);
                if (libro == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Registro no encontrado: {id}.");
                    return NotFound(_response);
                }

                await _repositoryBook.Remove(libro);
                _logger.LogInformation($"Se eliminó correctamente el libro = {id}.");
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

        // Endpoint para actualizar una libro por ID.
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookUpdateDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDto updatedBookDto)
        {
            try
            {
                if (updatedBookDto == null || id != updatedBookDto.Id)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Datos de entrada no válidos: {id}.");
                    return BadRequest(_response);
                }

                var updatedBook = await _repositoryBook.Update(_mapper.Map<Book>(updatedBookDto));
                _logger.LogInformation($"Se actualizó correctamente el libro = {id}.");
                _response.Result = _mapper.Map<BookUpdateDto>(updatedBook);
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

        // Endpoint para hacer una actualización parcial de una libro por ID.
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookUpdateDto))] // tipo de dato del objeto de la respuesta, siempre devolver DTO
        public async Task<IActionResult> UpdatePartialBook(int id, JsonPatchDocument<BookUpdateDto> patchDto)
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
                BookUpdateDto bookDto = _mapper.Map<BookUpdateDto>(await _repositoryBook.Get(v => v.Id == id, tracked: false));

                // Verificar si el libroDto existe
                if (bookDto == null)
                {
                    _logger.LogError($"No se encontró el libro = {id}.");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Aplicar el parche
                patchDto.ApplyTo(bookDto, error =>
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

                Book libro = _mapper.Map<Book>(bookDto);
                var updatedBook = await _repositoryBook.Update(libro);
                _logger.LogInformation($"Se actualizó correctamente el libro = {id}.");

                _response.Result = _mapper.Map<BookUpdateDto>(updatedBook);
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

