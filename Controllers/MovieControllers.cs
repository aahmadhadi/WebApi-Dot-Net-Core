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
    public class MovieController : ControllerBase
    {
        private readonly BasicDbContext dbContext;

        public MovieController(BasicDbContext dbContext)
        {
            this.dbContext = dbContext;
            if(this.dbContext.Movies.Count() == 0)
            {
                this.dbContext.Movies.Add(new Movie
                {
                    DurationInHour = 2,
                    Title = "HarryPotter"
                });
                this.dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieView>>> Get([FromQuery]GetMovieParam param)
        {
            var dataSource = dbContext.Movies
                .Select(movie => new MovieView 
                {
                    Title = movie.Title,
                    DurationInHour = movie.DurationInHour
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
        public async Task<ActionResult<MovieView>> Get([FromRoute]int id)
        {
            var selectedMovie = await dbContext.Movies
                .Where(movie => movie.Id == id)
                .Select(movie => new MovieView{
                    DurationInHour = movie.DurationInHour,
                    Title = movie.Title
                })
                .SingleOrDefaultAsync();
            if(selectedMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return selectedMovie;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<MovieView>>> Post([FromBody]MovieView param)
        {
            var newMovie = new Movie { Title = param.Title, DurationInHour = param.DurationInHour };
            dbContext.Add(newMovie);
            await this.dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = newMovie.Id }, param);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<MovieView>>> Put([FromRoute]int id,[FromBody]MovieView param)
        {
            var selectedMovie = await dbContext.Movies.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            selectedMovie.Title = param.Title;
            selectedMovie.DurationInHour = param.DurationInHour;
            await dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<MovieView>>> Delete([FromRoute]int id)
        {
            var selectedMovie = await dbContext.Movies.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            dbContext.Remove(selectedMovie);
            await dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}