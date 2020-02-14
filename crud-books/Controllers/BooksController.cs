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
    /// Controller Books
    /// <summary>
    /// Контроллер книги
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController: Controller
    {
        private IBookRepository _bookRepository;
        private IAuthorRepository _authorRepository;
        private ICategoryRepository _categoryRepository;
        private IReviewRepository _reviewRepository;

        /// Constructor Books
        public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository, 
            ICategoryRepository categoryRepository, IReviewRepository reviewRepository )
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }

        //api/books
        /// <summary>
        /// Получить список книг
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public ActionResult GetBooks()
        {
            var books = _bookRepository.GetBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var booksDto = new List<BookDto>();

            foreach (var book in books)
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

        //api/books/bookId
        /// <summary>
        /// Получить книгу по Id
        /// </summary>
        [HttpGet("{bookId}", Name = "GetBook")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var book = _bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }

        //api/books/isbn/bookIsbn
        /// <summary>
        /// Получить книгу по Isbn
        /// </summary>
        [HttpGet("ISBN/{bookIsbn}")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBook(string bookIsbn)
        {
            if (!_bookRepository.BookExists(bookIsbn))
                return NotFound();

            var book = _bookRepository.GetBook(bookIsbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }

        //api/books/bookId/rating
        /// <summary>
        /// Получить рейтирг книги
        /// </summary>
        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var rating = _bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }

        private StatusCodeResult ValidateBook(List<int> authorsId, List<int>catId, Book book)
        {
            if(book == null || authorsId.Count() <=0 || catId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing book, author, or category");
                return BadRequest();
            }

            if(_bookRepository.IsDuplicateIsbn(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", "Dublicate ISBN");
                return StatusCode(422);
            }

            foreach (var id in authorsId)
            {
                if(!_authorRepository.AuthorExists(id))
                {
                    ModelState.AddModelError("", "Author Not Found");
                    return StatusCode(404);
                }
            }

             
            foreach (var id in catId)
            {
                if(!_categoryRepository.CategoryExists(id))
                {
                    ModelState.AddModelError("", "Category Not Found");
                    return StatusCode(404);
                }
            }

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();
        }

        //api/books
        //api/books/bookId/rating
        /// <summary>
        /// Создать книгу книги
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody] Book bookToCreate)
        {
            var statusCode = ValidateBook(authId, catId, bookToCreate);

            if(!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!_bookRepository.CreateBook(authId, catId, bookToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving the book" +
                                            $"{bookToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.Id }, bookToCreate);
        }

        // //api/books/bookId
        // [HttpPut]
        // public IActionResult UpdateBook(List<int> authorsId, List<int> categoriesId, Book book)
        // {
            
        // }

        // //api/books/bookId
        // [HttpDelete]
        // public IActionResult DeleteBook(Book book)
        // {
        //     _bookRepository.DeleteBook(book);

        // }

    }
}