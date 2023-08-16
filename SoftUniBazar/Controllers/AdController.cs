using Microsoft.AspNetCore.Mvc;
using SoftUniBazar.Data;
using SoftUniBazar.Models;
using SoftUniBazar.Services.Interfaces;

namespace SoftUniBazar.Controllers
{
    public class AdController : BaseController
    {
        private readonly IAdService adService;
        private readonly BazarDbContext dbContext;

        public AdController(IAdService adService, BazarDbContext dbContext)
        {
            this.adService = adService;
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> All()
        {
            var model = await adService.GetAllAdsAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddAdViewModel model = await this.adService.GetNewAddAdModelAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAdViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Categories = await this.adService.GetAllCategoriesAsync();
                return View(model);
            }

            model.OwnerId = GetUserId();
            await adService.AddAdAsync(model);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Cart()
        {
            var model = await this.adService.GetMyAdsAsync(GetUserId());

            return View(model);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var ad = await this.adService
                .GetAdByIdAsync(id);

            var model = await this.adService.GetMyAdsAsync(GetUserId());

            if (model.Any(m => m.Id == id))
            {
                return RedirectToAction(nameof(All));
            }

            if (ad == null)
            {
                return RedirectToAction(nameof(All));
            }

            var userId = GetUserId();

            await this.adService.AddAdToCartAsync(userId, ad);

            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var ad = await this.adService
                .GetAdByIdAsync(id);

            if (ad is null)
            {
                return RedirectToAction(nameof(All));
            }

            var userId = GetUserId();

            await this.adService.RemoveAdFromCartAsync(userId, ad);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            AddAdViewModel? ad = await this.adService.GetAdByIdForEditAsync(id);

            if (ad is null)
            {
                return RedirectToAction(nameof(All));
            }

            return View(ad);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddAdViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            await this.adService.EditAdAsync(model, id);

            return RedirectToAction(nameof(All));
        }
    }
}
