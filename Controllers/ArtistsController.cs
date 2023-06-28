using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MusicAlbumStore.Data;
using MusicAlbumStore.Interfaces;
using MusicAlbumStore.Models;
using MusicAlbumStore.ViewModels;

namespace MusicAlbumStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly MusicAlbumStoreContext _context;
        readonly IBufferedFileUploadService _bufferedFileUploadService;


        public ArtistsController(MusicAlbumStoreContext context, IBufferedFileUploadService bufferedFileUploadService)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
        }

        // GET: Artists
        public async Task<IActionResult> Index(string ArtistNameSearch, string FormationYearSearch)
        {
            IQueryable<Artist> artists = _context.Artist.AsQueryable();
            if(!string.IsNullOrEmpty(ArtistNameSearch))
            {
                artists = artists.Where(a => a.ArtistName.Contains(ArtistNameSearch));
            }
            if(!string.IsNullOrEmpty(FormationYearSearch))
            {

                artists = artists.Where(a => a.FormationYear == int.Parse(FormationYearSearch));
            }

            var artistVM = new ArtistsViewModel
            {
                Artists = await artists.ToListAsync()
            };
            return View(artistVM);
            /*return _context.Artist != null ? 
                        View(await _context.Artist.ToListAsync()) :
                        Problem("Entity set 'MusicAlbumStoreContext.Artist'  is null."); */
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.MusicAlbums)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            IQueryable<MusicAlbum> musicAlbums = _context.MusicAlbum.AsQueryable();
            musicAlbums = musicAlbums.Include(ma => ma.Artist);
            musicAlbums = musicAlbums.Where(ma => ma.ArtistId == id);

            var averageRatings = await _context.Review
                .GroupBy(r => r.MusicAlbumId)
                .Select(g => new {
                    MusicAlbumId = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToDictionaryAsync(x => x.MusicAlbumId, x => x.AverageRating);

            ViewBag.AverageRatings = averageRatings;

            return View(artist);
        }

        // GET: Artists/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile artistimgfile, [Bind("Id,ArtistName,Biography,Website,Label,FormationYear,DisbandmentYear,ArtistImage")] Artist artist)
        {
            ModelState.Remove("artistimgfile");
            if (ModelState.IsValid)
            {
                try
                {
                    artist.ArtistImage = await _bufferedFileUploadService.UploadFile(artistimgfile);
                    if (!string.IsNullOrEmpty(artist.ArtistImage))
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
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [Authorize(Roles = "Admin")]
        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile artistimgfile, [Bind("Id,ArtistName,Biography,Website,Label,FormationYear,DisbandmentYear,ArtistImage")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            ModelState.Remove("artistimgfile");
            if (ModelState.IsValid)
            {
                var oldArtist = await _context.Artist.FindAsync(artist.Id);
                if (artistimgfile?.Length > 0)
                {
                    try
                    {
                        artist.ArtistImage = await _bufferedFileUploadService.UploadFile(artistimgfile);
                        if (!string.IsNullOrEmpty(artist.ArtistImage))
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
                    artist.ArtistImage = oldArtist.ArtistImage;

                }
                try
                {
                    _context.Entry(oldArtist).State = EntityState.Detached;
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
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
            return View(artist);
        }

        // GET: Artists/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        [Authorize(Roles = "Admin")]

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Artist == null)
            {
                return Problem("Entity set 'MusicAlbumStoreContext.Artist'  is null.");
            }
            var artist = await _context.Artist.FindAsync(id);
            if (artist != null)
            {
                _context.Artist.Remove(artist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
          return (_context.Artist?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
