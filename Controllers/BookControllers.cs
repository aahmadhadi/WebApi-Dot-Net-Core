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
                    Id = 1,
                    Date = DateTime.UtcNow,
                    Title = "HarryPotter"
                });
                this.dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookView>>> Get([FromQuery]GetBookParam param)
        {
            //SELECT Title, Date FROM Books
            var dataSource = await dbContext.Books
                .Select(book => new BookView 
                {
                    Id = book.Id,
                    Title = book.Title,
                    Date = book.Date
                })
                .ToListAsync();
            if(string.IsNullOrWhiteSpace(param.Title))
            {
                return dataSource;
            }
            return dataSource
                .Where(item => item.Title.Contains(param.Title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookView>>> Post([FromBody]BookView param)
        {
            var newId = await dbContext.Books.CountAsync() + 1;
            dbContext.Add(new Book {Id = newId, Title = param.Title, Date = param.Date });
            await this.dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
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
    }
}