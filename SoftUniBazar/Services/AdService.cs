using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models;
using SoftUniBazar.Services.Interfaces;

namespace SoftUniBazar.Services
{
    public class AdService : IAdService
    {
        private readonly BazarDbContext dbContext;

        public AdService(BazarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<AdAllViewModel>> GetAllAdsAsync()
        {
            return await this.dbContext
                    .Ads
                    .Select(a => new AdAllViewModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        ImageUrl = a.ImageUrl,
                        CreatedOn = a.CreatedOn,
                        Category = a.Category.Name,
                        Description = a.Description,
                        Price = a.Price,
                        Owner = a.Owner.UserName
                    }).ToListAsync();
        }

        public async Task AddAdAsync(AddAdViewModel model)
        {
            Ad ad = new Ad
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                Price = model.Price,
                CreatedOn = model.CreatedOn,
                OwnerId = model.OwnerId,
                CategoryId = model.CategoryId
            };

            await dbContext.Ads.AddAsync(ad);
            await dbContext.SaveChangesAsync();
        }

        public async Task<AddAdViewModel> GetNewAddAdModelAsync()
        {
            var categories = await dbContext.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync();

            var model = new AddAdViewModel
            {
                Categories = categories
            };

            return model;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            return await this.dbContext
                .Categories
                .Select(t => new CategoryViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<AdAllViewModel>> GetMyAdsAsync(string userId)
        {
            return await this.dbContext
                .AdsBuyers
                .Where(ab => ab.BuyerId == userId)
                .Select(a => new AdAllViewModel
                {
                    Id = a.Ad.Id,
                    Name = a.Ad.Name,
                    ImageUrl = a.Ad.ImageUrl,
                    CreatedOn = a.Ad.CreatedOn,
                    Category = a.Ad.Category.Name,
                    Description = a.Ad.Description,
                    Price = a.Ad.Price,
                    Owner = a.Ad.Owner.UserName
                }).ToListAsync();
        }

        public async Task<AdViewModel?> GetAdByIdAsync(int id)
        {
            return await this.dbContext
                .Ads
                .Where(a => a.Id == id)
                .Select(a => new AdViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = a.Price,
                    OwnerId = a.OwnerId,
                    CreatedOn = a.CreatedOn,
                    CategoryId = a.CategoryId
                }).FirstOrDefaultAsync();
        }

        public async Task AddAdToCartAsync(string userId, AdViewModel ad)
        {
            bool alreadyAdded = await dbContext.AdsBuyers
               .AnyAsync(ab => ab.BuyerId == userId && ab.AdId == ad.Id);

            if (alreadyAdded == false)
            {
                var adBuyer = new AdBuyer
                {
                    BuyerId = userId,
                    AdId = ad.Id
                };

                await dbContext.AdsBuyers.AddAsync(adBuyer);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveAdFromCartAsync(string userId, AdViewModel ad)
        {
            var adBuyer = await this.dbContext
                .AdsBuyers
                .FirstOrDefaultAsync(ab => ab.BuyerId == userId && ab.AdId == ad.Id);

            if (adBuyer != null)
            {
                dbContext.AdsBuyers.Remove(adBuyer);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<AddAdViewModel?> GetAdByIdForEditAsync(int id)
        {
            var categories = await this.dbContext
                .Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync();

            return await this.dbContext
                .Ads
                .Where(a => a.Id == id)
                .Select(a => new AddAdViewModel
                {
                    Name = a.Name,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = a.Price,
                    OwnerId = a.OwnerId,
                    CreatedOn = a.CreatedOn,
                    CategoryId = a.CategoryId,
                    Categories = categories
                }).FirstOrDefaultAsync();
        }

        public async Task EditAdAsync(AddAdViewModel model, int id)
        {
            var ad = await this.dbContext
                .Ads.FindAsync(id);

            if (ad != null)
            {
                ad.Name = model.Name;
                ad.Description = model.Description;
                ad.ImageUrl = model.ImageUrl;
                ad.Price = model.Price;
                ad.CategoryId = model.CategoryId;

                await this.dbContext.SaveChangesAsync();
            }
        }
    }
}
