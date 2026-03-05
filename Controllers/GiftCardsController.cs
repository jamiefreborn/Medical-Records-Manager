using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GiftCardsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GiftCardsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /GiftCards
        public async Task<IActionResult> Index()
        {
            var cards = await _db.GiftCards
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return View(cards);
        }

        // GET: /GiftCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /GiftCards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiftCard model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.RemainingBalance = model.OriginalAmount;
            model.IsActive = true;
            model.CreatedAt = DateTime.Now;

            _db.GiftCards.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}