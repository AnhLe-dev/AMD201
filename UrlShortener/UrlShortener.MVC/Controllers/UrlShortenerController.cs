using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Common.DTOs;
using UrlShortener.Data.Interfaces;
using UrlShortener.MVC.Models;
using UrlShortener.MVC.Helpers;
using System.Security.Claims;

namespace UrlShortener.MVC.Controllers
{
    [Authorize]
    public class UrlShortenerController : Controller
    {
        private readonly IShortenedUrlService _urlService;
        private readonly IUrlClickService _clickService;
        private readonly IWebHostEnvironment _environment;

        public UrlShortenerController(
            IShortenedUrlService urlService,
            IUrlClickService clickService,
            IWebHostEnvironment environment)
        {
            _urlService = urlService;
            _clickService = clickService;
            _environment = environment;
        }

        // =========================
        // HELPERS
        // =========================

        private Guid? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out Guid guid))
                return guid;

            return null;
        }

        private bool IsOwnerOrAdmin(Guid? urlUserId)
        {
            if (User.IsInRole("Admin"))
                return true;

            var currentUser = GetCurrentUserId();

            if (currentUser == null)
                return false;

            return urlUserId == currentUser;
        }

        // =========================
        // INDEX
        // =========================

        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin");
            ShortenedUrlVM[] urlVMs;

            if (isAdmin)
            {
                var urls = await _urlService.GetAll();
                urlVMs = urls.Select(u => new ShortenedUrlVM(u)).ToArray();
            }
            else
            {
                var userId = GetCurrentUserId();

                if (userId == null)
                    return Unauthorized();

                var urls = await _urlService.GetByUserId(userId.Value);
                urlVMs = urls.Select(u => new ShortenedUrlVM(u)).ToArray();
            }

            ViewBag.IsAdmin = isAdmin;

            return View(urlVMs);
        }

        // =========================
        // CREATE
        // =========================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShortenedUrlVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();

            var dto = new ShortenedUrlDTO
            {
                OriginalUrl = model.OriginalUrl,
                ShortCode = model.ShortCode ?? "",
                Title = model.Title,
                Description = model.Description,
                ExpirationDate = model.ExpirationDate,
                UserId = userId
            };

            var result = await _urlService.Create(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to create shortened URL");
                return View(model);
            }

            TempData["Success"] = "URL shortened successfully!";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var url = await _urlService.GetById(id.Value);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            return View(new ShortenedUrlVM(url));
        }

        // =========================
        // EDIT
        // =========================

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var url = await _urlService.GetById(id.Value);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            return View(new ShortenedUrlVM(url));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ShortenedUrlVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var url = await _urlService.GetById(id);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            var dto = new ShortenedUrlDTO
            {
                Id = model.Id,
                OriginalUrl = model.OriginalUrl,
                Title = model.Title,
                Description = model.Description,
                ExpirationDate = model.ExpirationDate,
                Status = model.Status
            };

            var result = await _urlService.Update(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to update URL");
                return View(model);
            }

            TempData["Success"] = "URL updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var url = await _urlService.GetById(id.Value);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            return View(new ShortenedUrlVM(url));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var url = await _urlService.GetById(id);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            var result = await _urlService.Delete(id);

            TempData[result ? "Success" : "Error"] =
                result ? "URL deleted successfully!" : "Failed to delete URL";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // REDIRECT
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var url = await _urlService.GetByShortCode(shortCode);

            if (url == null || url.Status != Common.Enums.UrlStatus.Active)
                return NotFound();

            if (url.IsExpired)
                return BadRequest("This URL has expired");

            await _urlService.IncrementClickCount(url.Id);

            await _clickService.RecordClick(new UrlClickDTO
            {
                ShortenedUrlId = url.Id,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                Referrer = Request.Headers["Referer"].ToString()
            });

            return Redirect(url.OriginalUrl);
        }

        // =========================
        // QR CODE
        // =========================

        public async Task<IActionResult> GenerateQRCode(Guid id)
        {
            var url = await _urlService.GetById(id);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            var qrCodeBase64 = QRCodeHelper.GenerateQRCodeBase64(url.OriginalUrl);

            return Json(new
            {
                success = true,
                qrCode = qrCodeBase64,
                originalUrl = url.OriginalUrl
            });
        }

        public async Task<IActionResult> DownloadQRCode(Guid id)
        {
            var url = await _urlService.GetById(id);

            if (url == null)
                return NotFound();

            if (!IsOwnerOrAdmin(url.UserId))
                return Forbid();

            using var qrGenerator = new QRCoder.QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(
                url.OriginalUrl,
                QRCoder.QRCodeGenerator.ECCLevel.Q);

            using var qrCode = new QRCoder.QRCode(qrCodeData);
            using var qrCodeImage = qrCode.GetGraphic(20);
            using var ms = new MemoryStream();

            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return File(
                ms.ToArray(),
                "image/png",
                $"QRCode_{url.ShortCode}.png");
        }

        // =========================
        // REFRESH LIST
        // =========================

        public IActionResult RefreshList()
        {
            return ViewComponent("UrlList");
        }
    }
}