using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicAlbumStore.Data;
using MusicAlbumStore.Models;
using static System.Reflection.Metadata.BlobBuilder;
using MusicAlbumStore.ViewModels;
using MusicAlbumStore.Areas.Identity.Data;
using MusicAlbumStore.Interfaces;

namespace MusicAlbumStore.Controllers
{
    public class MusicAlbumsController : Controller
    {
        private readonly MusicAlbumStoreContext _context;
        readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly UserManager<MusicAlbumStoreUser> _userManager;


        public MusicAlbumsController(MusicAlbumStoreContext context, IBufferedFileUploadService bufferedFileUploadService, UserManager<MusicAlbumStoreUser> userManager)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
            _userManager = userManager;
        }

        // GET: MusicAlbums
        public async Task<IActionResult> Index(string MusicAlbumGenre, string SearchMusicAlbum, string SearchReleaseYear)
        {
            IQueryable<MusicAlbum> musicAlbums = _context.MusicAlbum.AsQueryable().Include(ma => ma.Artist);
            IQueryable<string> genreQuery = _context.Genre.OrderBy(b => b.Id).Select(b => b.GenreName);

            if (!string.IsNullOrEmpty(SearchMusicAlbum))
            {
                musicAlbums = musicAlbums.Where(ma => ma.MusicAlbumName.Contains(SearchMusicAlbum));
            }
            if (!string.IsNullOrEmpty(MusicAlbumGenre))
            {

                musicAlbums = musicAlbums.Where(ma => ma.Genres.Any(mag => mag.Genre.GenreName == MusicAlbumGenre));
            }
            if (!string.IsNullOrEmpty(SearchReleaseYear))
            {

                //musicAlbums = musicAlbums.Where(ma => ma.ReleaseDate.Year);
                musicAlbums = musicAlbums.Where(ma => ma.ReleaseDate.Value.Year == int.Parse(SearchReleaseYear));

            }

            var averageRatings = await _context.Review
                .GroupBy(r => r.MusicAlbumId)
                .Select(g => new {
                    MusicAlbumId = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToDictionaryAsync(x => x.MusicAlbumId, x => x.AverageRating);

            ViewBag.AverageRatings = averageRatings;

            var MusicAlbumGenreVM = new MusicAlbumGenreViewModel
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                MusicAlbums = await musicAlbums.ToListAsync()
            };
            return View(MusicAlbumGenreVM);
            //var musicAlbumStoreContext = _context.MusicAlbum.Include(m => m.Artist);
            //return View(await musicAlbumStoreContext.ToListAsync());
        }

        
        // GET: GetMyOrders
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyOrders(string MusicAlbumGenre, string SearchMusicAlbum, string SearchReleaseYear)
        {
            string message = Request.Query["message"];
            ViewBag.Message = message;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            IQueryable<MusicAlbum> musicAlbums = _context.MusicAlbum
                .Join(_context.MusicAlbumUser, mau => mau.Id, mau => mau.MusicAlbumId, (ma, mau) => new { MusicAlbum = ma, MusicAlbumUser = mau })
                .Where(x => x.MusicAlbumUser.UserId == userId)
                .Select(x => x.MusicAlbum)
                .Include(ma => ma.Artist); 
            IQueryable<string> genreQuery = _context.Genre.OrderBy(ma => ma.Id).Select(ma => ma.GenreName);
            if (!string.IsNullOrEmpty(SearchMusicAlbum))
            {
                musicAlbums = musicAlbums.Where(ma => ma.MusicAlbumName.Contains(SearchMusicAlbum));
            }
            if (!string.IsNullOrEmpty(MusicAlbumGenre))
            {

                musicAlbums = musicAlbums.Where(ma => ma.Genres.Any(mag => mag.Genre.GenreName == MusicAlbumGenre));
            }
            if (!string.IsNullOrEmpty(SearchReleaseYear))
            {

                musicAlbums = musicAlbums.Where(ma => ma.ReleaseDate.Value.Year == int.Parse(SearchReleaseYear));

            }

            var averageRatings = await _context.Review
               .GroupBy(r => r.MusicAlbumId)
               .Select(g => new {
                   MusicAlbumId = g.Key,
                   AverageRating = g.Average(r => r.Rating)
               })
               .ToDictionaryAsync(x => x.MusicAlbumId, x => x.AverageRating);

            ViewBag.AverageRatings = averageRatings;

            var MusicAlbumGenreVM = new MusicAlbumGenreViewModel
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                MusicAlbums = await musicAlbums.ToListAsync()
            };
            return View(MusicAlbumGenreVM);
        }
       
        // GET: MusicAlbums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MusicAlbum == null)
            {
                return NotFound();
            }

            var musicAlbum = await _context.MusicAlbum
                .Include(m => m.Artist)
                .Include(m => m.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musicAlbum == null)
            {
                return NotFound();
            }

            var averageRating = await _context.Review
                .Where(r => r.MusicAlbumId == musicAlbum.Id)
                .AverageAsync(r => r.Rating);

            ViewBag.AverageRating = averageRating;

            return View(musicAlbum);
        }

        // GET: MusicAlbums/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            var genres = _context.Genre.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName
            }).ToList();

            ViewData["Genres"] = genres;
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "ArtistName");
            return View();
        }

        [Authorize(Roles = "Admin")]

        // POST: MusicAlbums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile coverimgfile, IFormFile pdffile, [Bind("Id,MusicAlbumName,ReleaseDate,Description,Language,LengthInMinutes,Price,NumSongs,CoverImage,PdfUrl,ArtistId")] MusicAlbum musicAlbum, int[] selectedGenres)
        {
            ModelState.Remove("coverimgfile");
            ModelState.Remove("pdffile");

            if (ModelState.IsValid)
            {
                try
                {
                    musicAlbum.CoverImage = await _bufferedFileUploadService.UploadFile(coverimgfile);
                    musicAlbum.PdfUrl = await _bufferedFileUploadService.UploadFile(pdffile);
                    if (!string.IsNullOrEmpty(musicAlbum.CoverImage) && !string.IsNullOrEmpty(musicAlbum.PdfUrl))
                    {
                        ViewBag.Message = "File Upload Successful";
                    }
                    else
                    {
                        ViewBag.Message = "File Upload Failed";
                    }
                }
                catch (Exception ex)
                {
                    //Log ex
                    ViewBag.Message = "File Upload Failed";
                }

                _context.Add(musicAlbum);
                await _context.SaveChangesAsync();
                if (selectedGenres != null)
                {
                    foreach (var genreId in selectedGenres)
                    {
                        var genreMusicAlbum = new MusicAlbumGenre
                        {
                            GenreId = genreId,
                            MusicAlbumId = musicAlbum.Id
                        };

                        _context.Add(genreMusicAlbum);
                    }

                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "ArtistName", musicAlbum.ArtistId);
            return View(musicAlbum);
        }

        // GET: MusicAlbums/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MusicAlbum == null)
            {
                return NotFound();
            }

            var musicAlbum = await _context.MusicAlbum.FindAsync(id);
            if (musicAlbum == null)
            {
                return NotFound();
            }

            var musicAlbumGenres = _context.MusicAlbumGenre
                .Where(mag => mag.MusicAlbumId == musicAlbum.Id)
                .Select(mag => mag.GenreId)
                .ToList();

            var genres = _context.Genre.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName,
                Selected = musicAlbumGenres.Contains(g.Id)
            }).ToList();

            ViewData["Genres"] = genres;
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "ArtistName", musicAlbum.ArtistId);
            return View(musicAlbum);
        }

        [Authorize(Roles = "Admin")]

        // POST: MusicAlbums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile coverimgfile, IFormFile pdffile, [Bind("Id,MusicAlbumName,ReleaseDate,Description,Language,LengthInMinutes,Price,NumSongs,CoverImage,PdfUrl,ArtistId")] MusicAlbum musicAlbum, int[] selectedGenres)
        {
            if (id != musicAlbum.Id)
            {
                return NotFound();
            }

            ModelState.Remove("coverimgfile");
            ModelState.Remove("pdffile");

            if (ModelState.IsValid)
            {
                var oldmusicAlbum = await _context.MusicAlbum.FindAsync(musicAlbum.Id);
                if (coverimgfile?.Length > 0)
                {
                    try
                    {
                        musicAlbum.CoverImage = await _bufferedFileUploadService.UploadFile(coverimgfile);
                        if (!string.IsNullOrEmpty(musicAlbum.CoverImage))
                        {
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log ex
                        ViewBag.Message = "File Upload Failed";
                    }


                }
                else
                {
                    musicAlbum.CoverImage = oldmusicAlbum.CoverImage;

                }

                if (pdffile?.Length > 0)
                {
                    try
                    {
                        musicAlbum.PdfUrl = await _bufferedFileUploadService.UploadFile(pdffile);
                        if (!string.IsNullOrEmpty(musicAlbum.PdfUrl))
                        {
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log ex
                        ViewBag.Message = "File Upload Failed";
                    }
                }


                else
                {
                    musicAlbum.PdfUrl = oldmusicAlbum.PdfUrl;

                }

                try
                {
                    _context.Entry(oldmusicAlbum).State = EntityState.Detached;
                    _context.Update(musicAlbum);
                    await _context.SaveChangesAsync();
                    if (selectedGenres != null)
                    {
                        // brishenje
                        var oldMusicAlbumGenres = _context.MusicAlbumGenre.Where(bg => bg.MusicAlbumId == musicAlbum.Id);
                        _context.MusicAlbumGenre.RemoveRange(oldMusicAlbumGenres);

                        // dodavanje
                        foreach (var genreId in selectedGenres)
                        {
                            var genreBook = new MusicAlbumGenre
                            {
                                GenreId = genreId,
                                MusicAlbumId = musicAlbum.Id
                            };

                            _context.Update(genreBook);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicAlbumExists(musicAlbum.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "ArtistName", musicAlbum.ArtistId);
            return View(musicAlbum);
        }

        // GET: MusicAlbums/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MusicAlbum == null)
            {
                return NotFound();
            }

            var musicAlbum = await _context.MusicAlbum
                .Include(m => m.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musicAlbum == null)
            {
                return NotFound();
            }

            return View(musicAlbum);
        }

        // POST: MusicAlbums/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MusicAlbum == null)
            {
                return Problem("Entity set 'MusicAlbumStoreContext.MusicAlbum'  is null.");
            }
            var musicAlbum = await _context.MusicAlbum.FindAsync(id);
            if (musicAlbum != null)
            {
                _context.MusicAlbum.Remove(musicAlbum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicAlbumExists(int id)
        {
          return (_context.MusicAlbum?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order([Bind("Id,MusicAlbumName,ReleaseDate,Description,Language,LengthInMinutes,Price,NumSongs,CoverImage,PdfUrl,ArtistId")] MusicAlbum musicAlbum)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var username = user?.UserName;

            var userMusicAlbum = new MusicAlbumUser
            {
                UserId = userId,
                MusicAlbumId = musicAlbum.Id
            };
            _context.Add(userMusicAlbum);
            await _context.SaveChangesAsync();


            ViewBag.Message = "You ordered music album successfully";
            //return RedirectToAction(nameof(GetMyBooks));
            return RedirectToAction(nameof(GetMyOrders), new { message = "You ordered music album successfully" });

        }
    }
}
