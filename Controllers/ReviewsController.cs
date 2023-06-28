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
using MusicAlbumStore.Areas.Identity.Data;
using MusicAlbumStore.Data;
using MusicAlbumStore.Models;

namespace MusicAlbumStore.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MusicAlbumStoreContext _context;
        private readonly UserManager<MusicAlbumStoreUser> _userManager;


        public ReviewsController(MusicAlbumStoreContext context, UserManager<MusicAlbumStoreUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Reviews
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var username = user?.UserName;
            var UserId = user?.Id;

            var musicAlbumStoreContext = _context.Review.Include(r => r.MusicAlbum).Where(r => r.UserId.Equals(UserId));
            return View(await musicAlbumStoreContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.MusicAlbum)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "User")]

        public async Task<IActionResult> CreateAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;


            var musicAlbums = _context.MusicAlbumUser
                .Where(mau => mau.UserId == userId)
                .Select(mau => mau.MusicAlbum)
                .ToList();

            ViewData["MusicAlbumId"] = new SelectList(/*_context.MusicAlbum*/ musicAlbums, "Id", "MusicAlbumName");
            return View();
        }

        // POST: Reviews/Create
        [Authorize(Roles = "User")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comment,Rating,MusicAlbumId,UserId")] Review review)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                review.SubmissionDate = DateTime.Now;
                review.UserId = userId;
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var musicAlbums= _context.MusicAlbumUser
                .Where(mau => mau.UserId == userId)
                .Select(mau => mau.MusicAlbum)
                .ToList();

            ViewData["MusicAlbumId"] = new SelectList(musicAlbums, "Id", "MusicAlbumName", review.MusicAlbumId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            if (review.UserId != userId)
            {
                return Forbid();
            }


            var musicAlbums = _context.MusicAlbumUser
                .Where(mau => mau.UserId == userId)
                .Select(mau => mau.MusicAlbum)
                .ToList();

            ViewData["MusicAlbumId"] = new SelectList(/*_context.MusicAlbum*/ musicAlbums, "Id", "MusicAlbumName", review.MusicAlbumId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        [Authorize(Roles = "User")]

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment,SubmissionDate,Rating,MusicAlbumId, UserId")] Review review)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            if (id != review.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var oldReview = await _context.Review.FindAsync(id);
                review.UserId = userId;
                review.SubmissionDate = oldReview.SubmissionDate;
                try
                {
                    _context.Entry(oldReview).State = EntityState.Detached;
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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

            var musicAlbums = _context.MusicAlbumUser
                .Where(mau => mau.UserId == userId)
                .Select(mau => mau.MusicAlbum)
                .ToList();

            ViewData["MusicAlbumId"] = new SelectList(/*_context.MusicAlbum*/ musicAlbums, "Id", "MusicAlbumName", review.MusicAlbumId); return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;

            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.MusicAlbum)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != userId)
            {
                return Forbid();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [Authorize(Roles = "User")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Review == null)
            {
                return Problem("Entity set 'MusicAlbumStoreContext.Review'  is null.");
            }
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                _context.Review.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
          return (_context.Review?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
