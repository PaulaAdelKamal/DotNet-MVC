using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using site101.Models;
using site101.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace site101.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IToastNotification _toastNotification;

        public MoviesController(ApplicationDBContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Rate).ToListAsync();
            return View(movies);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var view = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };
            return View(view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("Create", model);
            }
            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("poster", "Please select movie poster");
                return View("Create", model);
            }
            var poster = files.FirstOrDefault();
            var allowedExtenstions = new List<string> { ",jpg", ".png" };
            if (allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("poster", "only .png and .jpg are allowed");
                return View("Create", model);
            }

            if (poster.Length > 3145728)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("poster", "poster cannot be more than 3 MB ");
                return View("Create", model);
            }

            using var datastream = new MemoryStream();
            await poster.CopyToAsync(datastream);

            var movies = new Movie
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                Storyline = model.Storyline,
                poster = datastream.ToArray()
            };
            _context.Movies.Add(movies);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("hamdaaa says: movie created successfully");
            return RedirectToAction(nameof(Index));

        }
        
        public async Task<IActionResult> Edit (int? id)
        {
            if (id == null) return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                Rate = movie.Rate,
                Year = movie.Year,
                Storyline = movie.Storyline,
                poster = movie.poster,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };
            return View("MovieForm",viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }

            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null) return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var datastream = new MemoryStream();
                await poster.CopyToAsync(datastream);

                var allowedExtenstions = new List<string> { ",jpg", ".png" };
                model.poster = datastream.ToArray();

                if (allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("poster", "only .png and .jpg are allowed");
                    return View("MovieForm", model);
                }

                if (poster.Length > 3145728)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("poster", "poster cannot be more than 3 MB ");
                    return View("MovieForm", model);
                }
                movie.poster = model.poster;
            }

            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Rate = model.Rate;
            movie.Year = model.Year;
            movie.Storyline = model.Storyline;

            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("hamdaaa says: movie updated successfully");
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            var movie = await _context.Movies.Include(m=>m.Genre).SingleOrDefaultAsync(m=>m.Id == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }

    }
}
