using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PaymentsController(ApplicationDbContext db) => _db = db;

        // GET: /Payments
        public async Task<IActionResult> Index()
        {
            var payments = await _db.Payments
                .Include(p => p.Patient)
                .Include(p => p.Appointment)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // GET: /Payments/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients
                                       .Where(p => p.IsActive).ToListAsync();
            ViewBag.Appointments = await _db.Appointments
                                       .Include(a => a.Patient)
                                       .Where(a => a.Status == "Completed")
                                       .ToListAsync();
            return View();
        }

        // POST: /Payments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment model, string? GiftCardCode)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients
                    .Where(p => p.IsActive).ToListAsync();

                ViewBag.Appointments = await _db.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.Status == "Completed")
                    .ToListAsync();

                return View(model);
            }

            // ───── Gift Card Handling ─────
            if (!string.IsNullOrWhiteSpace(GiftCardCode))
            {
                var giftCard = await _db.GiftCards
                    .FirstOrDefaultAsync(g => g.Code == GiftCardCode);

                if (giftCard == null)
                {
                    ModelState.AddModelError("", "Invalid gift card code.");
                    return View(model);
                }

                if (!giftCard.IsActive)
                {
                    ModelState.AddModelError("", "Gift card is inactive.");
                    return View(model);
                }

                if (giftCard.ExpiryDate.HasValue &&
                    giftCard.ExpiryDate.Value < DateTime.UtcNow)
                {
                    ModelState.AddModelError("", "Gift card has expired.");
                    return View(model);
                }

                if (giftCard.RemainingBalance < model.Amount)
                {
                    ModelState.AddModelError("", "Insufficient gift card balance.");
                    return View(model);
                }

                // Deduct balance
                giftCard.RemainingBalance -= model.Amount;

                if (giftCard.RemainingBalance == 0)
                    giftCard.IsActive = false;

                model.GiftCardId = giftCard.Id;
            }

            model.PaymentDate = DateTime.UtcNow;

            _db.Payments.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Payment recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Payments/MarkPaid
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment != null)
            {
                payment.Status = "Paid";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Payment marked as paid.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Payments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment != null)
            {
                _db.Payments.Remove(payment);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Payment deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}