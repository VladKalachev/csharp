using BookApiProject.Dtos;
using BookApiProject.Models;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Controllers
{
    /// Controller Authors
    /// <summary>
    /// Контроллер автор
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController: Controller
    {
        private IAuthorRepository _authorRepository;
        private IBookRepository _bookRepository;
        private ICountryRepository _countryRepository;
        
        /// Constructor
        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository,
            ICountryRepository countryRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _countryRepository = countryRepository;
        }

        //api/authors
        /// <summary>
        /// Получить список авторов
        /// </summary>
        /// <returns>Список авторов</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public ActionResult GetAuthors()
        {
            var authors = _authorRepository.GetAuthors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);    
        }

        //api/authors/authorId
        /// <summary>
        /// Получить автора по id
        /// </summary>
        /// <param name="authorId">Id автора</param>
        /// <returns>Получаем автора по его Id</returns>
        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var author = _authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new AuthorDto()
            {
               Id = author.Id,
               FirstName = author.FirstName,
               LastName = author.LastName
            };

            return Ok(authorDto);
        }

        //api/authors/authorId/books
        /// <summary>
        /// Получить список книг по id автора
        /// </summary>
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var books = _authorRepository.GetBooksByAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();
            foreach(var book in books)
            {
                booksDto.Add(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    DatePublished = book.DatePublished
                });
            }

            return Ok(booksDto);
        }
       
        //api/authors/books/bookId
        /// <summary>
        /// Получить список книг автора
        /// </summary>
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthorsOfABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var authors = _authorRepository.GetAuthorsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();
            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }

        //api/authors
        /// <summary>
        /// Создать автора
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody]Author authorToCreate)
        {
            if(authorToCreate == null)
                return BadRequest(ModelState);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving " +
                                            $"{authorToCreate.FirstName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }

        //api/authors/authorId
        /// <summary>
        /// Редактировать автора
        /// </summary>
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)] 
        [ProducesResponseType(500)]        
        public IActionResult UpdateAuthor(int authorId, [FromBody]Author updatedAuthorInfo)
        {
            if (updatedAuthorInfo == null)
                return BadRequest(ModelState);

            if (authorId != updatedAuthorInfo.Id)
                return BadRequest(ModelState);

            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

             if (!_authorRepository.UpdateAuthor(updatedAuthorInfo))
            {
                 ModelState.AddModelError("", $"Something went wrong saving " +
                                            $"{updatedAuthorInfo.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}