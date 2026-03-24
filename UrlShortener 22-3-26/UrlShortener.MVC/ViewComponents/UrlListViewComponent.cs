//using Microsoft.AspNetCore.Mvc;
//using UrlShortener.Data.Interfaces;
//using UrlShortener.MVC.Models;

//namespace UrlShortener.MVC.ViewComponents
//{
//    public class UrlListViewComponent : ViewComponent
//    {
//        private readonly IShortenedUrlService _urlService;

//        public UrlListViewComponent(IShortenedUrlService urlService)
//        {
//            _urlService = urlService;
//        }

//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            var urls = await _urlService.GetAll();
//            var urlVMs = urls?.Select(u => new ShortenedUrlVM(u)).ToList() ?? new List<ShortenedUrlVM>();
//            return View(urlVMs);
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data.Interfaces;
using UrlShortener.MVC.Models;
using System.Security.Claims; // ✅ thêm

namespace UrlShortener.MVC.ViewComponents
{
    public class UrlListViewComponent : ViewComponent
    {
        private readonly IShortenedUrlService _urlService;

        public UrlListViewComponent(IShortenedUrlService urlService)
        {
            _urlService = urlService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ❌ CODE CŨ (LUÔN LẤY TẤT CẢ LINK)
            // var urls = await _urlService.GetAll();

            // ✅ CODE MỚI (FILTER THEO USER)
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ShortenedUrlVM> urlVMs;

            if (HttpContext.User.IsInRole("Admin"))
            {
                // ✅ Admin thấy tất cả
                var urls = await _urlService.GetAll();
                urlVMs = urls?.Select(u => new ShortenedUrlVM(u)).ToList() ?? new List<ShortenedUrlVM>();
            }
            else
            {
                // ✅ User chỉ thấy link của mình
                var userId = Guid.Parse(userIdString);
                var urls = await _urlService.GetByUserId(userId);

                urlVMs = urls?.Select(u => new ShortenedUrlVM(u)).ToList() ?? new List<ShortenedUrlVM>();
            }

            return View(urlVMs);
        }
    }
}