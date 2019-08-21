using System.Collections.Generic;
using BasicWebApi.Models.Param;
using System.Linq;
using BasicWebApi.Models.View;
using System;
using Microsoft.AspNetCore.Mvc;
using BasicWebApi.Models;
using BasicWebApi.Models.Entity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BasicDbContext dbContext;

        public BookController(BasicDbContext dbContext)
        {
            this.dbContext = dbContext;
            if(this.dbContext.Books.Count() == 0)
            {
                this.dbContext.Books.Add(new Book
                {
                    Date = DateTime.UtcNow,
                    Title = "HarryPotter 1"
                });
                this.dbContext.Books.Add(new Book
                {
                    Date = DateTime.UtcNow,
                    Title = "HarryPotter 2"
                });
                this.dbContext.Books.Add(new Book
                {
                    Date = DateTime.UtcNow,
                    Title = "HarryPotter 3"
                });
                this.dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookView>>> Get([FromQuery]GetBookParam param)
        {
            var dataSource = dbContext.Books
                .Select(book => new BookView 
                {
                    Title = book.Title,
                    Date = book.Date
                });
            if(string.IsNullOrWhiteSpace(param.Title))
            {
                return await dataSource.ToListAsync();
            }
            return await dataSource
                .Where(item => item.Title.ToLower().Contains(param.Title.ToLower()))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookView>> Get([FromRoute]int id)
        {
            var selectedBook = await dbContext.Books
                .Where(book => book.Id == id)
                .Select(book => new BookView{
                    Date = book.Date,
                    Title = book.Title
                })
                .SingleOrDefaultAsync();
            if(selectedBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return selectedBook;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookView>>> Post([FromBody]BookView param)
        {
            var newBook = new Book { Title = param.Title, Date = param.Date };
            dbContext.Add(newBook);
            await this.dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, param);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<BookView>>> Put([FromRoute]int id,[FromBody]BookView param)
        {
            var selectedBook = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            selectedBook.Title = param.Title;
            selectedBook.Date = param.Date;
            await dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<BookView>>> Delete([FromRoute]int id)
        {
            var selectedBook = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            dbContext.Remove(selectedBook);
            await dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}