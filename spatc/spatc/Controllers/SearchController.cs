using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    [Route("Search")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Search - Trang tìm kiếm
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        // API: Tìm kiếm sản phẩm và dịch vụ
        [HttpGet]
        [Route("SearchAll")]
        public async Task<IActionResult> SearchAll(string? keyword, string? type, int page = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return Json(new { success = true, products = new List<object>(), services = new List<object>(), total = 0 });
                }

                keyword = keyword.Trim().ToLower();
                var skip = (page - 1) * pageSize;

                var results = new
                {
                    Products = new List<object>(),
                    Services = new List<object>(),
                    TotalProducts = 0,
                    TotalServices = 0
                };

                // Tìm kiếm sản phẩm nếu type là "all" hoặc "product"
                if (type == null || type == "all" || type == "product")
                {
                    // Tìm trong PetProducts
                    var petsData = await _context.PetProducts
                        .Where(p => p.TrangThai == true && 
                                   (p.Name.ToLower().Contains(keyword) || 
                                    p.Species.ToLower().Contains(keyword) ||
                                    (p.Description != null && p.Description.ToLower().Contains(keyword))))
                        .ToListAsync();
                    
                    var pets = petsData.Select(p => 
                    {
                        string? image = null;
                        if (p.Images != null)
                        {
                            var firstImage = p.Images.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                            image = firstImage != null ? firstImage.Trim() : null;
                        }
                        return new
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Type = "pet",
                            Category = "Thú cưng",
                            Price = p.Price,
                            OldPrice = p.OldPrice,
                            Image = image,
                            Rating = p.AverageRating,
                            ReviewCount = p.ReviewCount,
                            SoldCount = p.SoldCount,
                            Species = p.Species
                        };
                    }).ToList();

                    // Tìm trong FoodProducts
                    var foods = await _context.FoodProducts
                        .Where(f => f.TrangThai == true && 
                                   (f.Name.ToLower().Contains(keyword) || 
                                    f.Brand != null && f.Brand.ToLower().Contains(keyword) ||
                                    f.Type.ToLower().Contains(keyword) ||
                                    (f.Description != null && f.Description.ToLower().Contains(keyword))))
                        .Select(f => new
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Type = "food",
                            Category = "Thức ăn",
                            Price = f.Price,
                            OldPrice = f.OldPrice,
                            Image = f.Image,
                            Rating = f.AverageRating,
                            ReviewCount = f.ReviewCount,
                            SoldCount = f.SoldCount,
                            Brand = f.Brand,
                            ProductType = f.Type
                        })
                        .ToListAsync();

                    // Tìm trong AccessoryProducts
                    var accessories = await _context.AccessoryProducts
                        .Where(a => a.TrangThai == true && 
                                   (a.Name.ToLower().Contains(keyword) || 
                                    a.Category.ToLower().Contains(keyword) ||
                                    (a.Description != null && a.Description.ToLower().Contains(keyword))))
                        .Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Type = "accessory",
                            Category = "Phụ kiện",
                            Price = a.Price,
                            OldPrice = a.OldPrice,
                            Image = a.Image,
                            Rating = a.AverageRating,
                            ReviewCount = a.ReviewCount,
                            SoldCount = a.SoldCount,
                            CategoryName = a.Category
                        })
                        .ToListAsync();

                    var allProducts = pets.Cast<object>()
                        .Concat(foods.Cast<object>())
                        .Concat(accessories.Cast<object>())
                        .ToList();

                    results = new
                    {
                        Products = allProducts.Skip(skip).Take(pageSize).ToList(),
                        Services = new List<object>(),
                        TotalProducts = allProducts.Count,
                        TotalServices = 0
                    };
                }

                // Tìm kiếm dịch vụ nếu type là "all" hoặc "service"
                if (type == null || type == "all" || type == "service")
                {
                    var services = await _context.DichVus
                        .Where(d => d.TrangThai == true && 
                                   (d.TenDichVu.ToLower().Contains(keyword) || 
                                    d.LoaiDichVu.ToLower().Contains(keyword) ||
                                    (d.MoTa != null && d.MoTa.ToLower().Contains(keyword))))
                        .Select(d => new
                        {
                            Id = d.Id,
                            Name = d.TenDichVu,
                            Type = d.LoaiDichVu.ToLower(),
                            Category = d.LoaiDichVu == "Grooming" ? "Grooming" : "Khách sạn",
                            Price = d.Gia,
                            Image = d.HinhAnh,
                            Description = d.MoTa
                        })
                        .ToListAsync();

                    var serviceSkip = (page - 1) * pageSize;
                    var serviceResults = services.Skip(serviceSkip).Take(pageSize).Cast<object>().ToList();

                    results = new
                    {
                        Products = results.Products,
                        Services = serviceResults,
                        TotalProducts = results.TotalProducts,
                        TotalServices = services.Count
                    };
                }

                var total = results.TotalProducts + results.TotalServices;
                var totalPages = (int)Math.Ceiling(total / (double)pageSize);

                return Json(new
                {
                    success = true,
                    products = results.Products,
                    services = results.Services,
                    total = total,
                    totalProducts = results.TotalProducts,
                    totalServices = results.TotalServices,
                    page = page,
                    totalPages = totalPages,
                    keyword = keyword
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Gợi ý dịch vụ cho sản phẩm
        [HttpGet]
        [Route("SuggestServicesForProduct")]
        public async Task<IActionResult> SuggestServicesForProduct(int productId, string productType, int limit = 5)
        {
            try
            {
                var suggestions = new List<object>();

                // Lấy thông tin sản phẩm
                string? species = null;
                string? category = null;

                if (productType == "pet")
                {
                    var pet = await _context.PetProducts.FindAsync(productId);
                    if (pet != null)
                    {
                        species = pet.Species;
                    }
                }
                else if (productType == "accessory")
                {
                    var accessory = await _context.AccessoryProducts.FindAsync(productId);
                    if (accessory != null)
                    {
                        category = accessory.Category;
                    }
                }

                // Gợi ý dịch vụ Grooming cho thú cưng hoặc phụ kiện liên quan đến chăm sóc
                if (productType == "pet" || (productType == "accessory" && category != null && 
                    (category.Contains("Tắm") || category.Contains("Chăm sóc") || category.Contains("Làm đẹp"))))
                {
                    var groomingServices = await _context.DichVus
                        .Where(d => d.TrangThai == true && d.LoaiDichVu == "Grooming")
                        .OrderByDescending(d => d.Gia)
                        .Take(limit)
                        .Select(d => new
                        {
                            Id = d.Id,
                            Name = d.TenDichVu,
                            Type = "Grooming",
                            Price = d.Gia,
                            Image = d.HinhAnh,
                            Description = d.MoTa
                        })
                        .ToListAsync();

                    suggestions.AddRange(groomingServices.Cast<object>());
                }

                // Gợi ý dịch vụ Khách sạn cho thú cưng
                if (productType == "pet")
                {
                    var hotelServices = await _context.DichVus
                        .Where(d => d.TrangThai == true && d.LoaiDichVu == "KhachSan")
                        .OrderBy(d => d.Gia)
                        .Take(limit)
                        .Select(d => new
                        {
                            Id = d.Id,
                            Name = d.TenDichVu,
                            Type = "KhachSan",
                            Price = d.Gia,
                            Image = d.HinhAnh,
                            Description = d.MoTa
                        })
                        .ToListAsync();

                    suggestions.AddRange(hotelServices.Cast<object>());
                }

                // Gợi ý dịch vụ Grooming cho thức ăn (có thể cần chăm sóc sau khi ăn)
                if (productType == "food")
                {
                    var groomingForFood = await _context.DichVus
                        .Where(d => d.TrangThai == true && d.LoaiDichVu == "Grooming")
                        .OrderByDescending(d => d.NgayTao)
                        .Take(limit)
                        .Select(d => new
                        {
                            Id = d.Id,
                            Name = d.TenDichVu,
                            Type = "Grooming",
                            Price = d.Gia,
                            Image = d.HinhAnh,
                            Description = d.MoTa
                        })
                        .ToListAsync();

                    suggestions.AddRange(groomingForFood.Cast<object>());
                }

                return Json(new { success = true, suggestions = suggestions.Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Gợi ý sản phẩm cho dịch vụ
        [HttpGet]
        [Route("SuggestProductsForService")]
        public async Task<IActionResult> SuggestProductsForService(int serviceId, int limit = 5)
        {
            try
            {
                var service = await _context.DichVus.FindAsync(serviceId);
                if (service == null)
                {
                    return Json(new { success = false, message = "Dịch vụ không tồn tại" });
                }

                var suggestions = new List<object>();

                // Nếu là dịch vụ Grooming, gợi ý phụ kiện chăm sóc và thức ăn
                if (service.LoaiDichVu == "Grooming")
                {
                    // Phụ kiện chăm sóc
                    var careAccessories = await _context.AccessoryProducts
                        .Where(a => a.TrangThai == true && 
                                   (a.Category.Contains("Tắm") || 
                                    a.Category.Contains("Chăm sóc") || 
                                    a.Category.Contains("Làm đẹp") ||
                                    a.Category.Contains("Dụng cụ")))
                        .OrderByDescending(a => a.SoldCount)
                        .Take(limit)
                        .Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Type = "accessory",
                            Category = a.Category,
                            Price = a.Price,
                            OldPrice = a.OldPrice,
                            Image = a.Image,
                            Rating = a.AverageRating
                        })
                        .ToListAsync();

                    suggestions.AddRange(careAccessories.Cast<object>());

                    // Thức ăn chất lượng cao
                    var premiumFoods = await _context.FoodProducts
                        .Where(f => f.TrangThai == true)
                        .OrderByDescending(f => f.AverageRating)
                        .ThenByDescending(f => f.SoldCount)
                        .Take(limit)
                        .Select(f => new
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Type = "food",
                            Brand = f.Brand,
                            Price = f.Price,
                            OldPrice = f.OldPrice,
                            Image = f.Image,
                            Rating = f.AverageRating
                        })
                        .ToListAsync();

                    suggestions.AddRange(premiumFoods.Cast<object>());
                }

                // Nếu là dịch vụ Khách sạn, gợi ý thức ăn và phụ kiện cần thiết
                if (service.LoaiDichVu == "KhachSan")
                {
                    // Thức ăn
                    var foods = await _context.FoodProducts
                        .Where(f => f.TrangThai == true)
                        .OrderByDescending(f => f.SoldCount)
                        .Take(limit)
                        .Select(f => new
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Type = "food",
                            Brand = f.Brand,
                            Price = f.Price,
                            OldPrice = f.OldPrice,
                            Image = f.Image,
                            Rating = f.AverageRating
                        })
                        .ToListAsync();

                    suggestions.AddRange(foods.Cast<object>());

                    // Phụ kiện cần thiết (lồng, đồ chơi, v.v.)
                    var essentialAccessories = await _context.AccessoryProducts
                        .Where(a => a.TrangThai == true && 
                                   (a.Category.Contains("Lồng") || 
                                    a.Category.Contains("Đồ chơi") ||
                                    a.Category.Contains("Vòng cổ")))
                        .OrderByDescending(a => a.SoldCount)
                        .Take(limit)
                        .Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Type = "accessory",
                            Category = a.Category,
                            Price = a.Price,
                            OldPrice = a.OldPrice,
                            Image = a.Image,
                            Rating = a.AverageRating
                        })
                        .ToListAsync();

                    suggestions.AddRange(essentialAccessories.Cast<object>());
                }

                return Json(new { success = true, suggestions = suggestions.Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Tìm kiếm nhanh (autocomplete)
        [HttpGet]
        [Route("QuickSearch")]
        public async Task<IActionResult> QuickSearch(string? keyword, int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
                {
                    return Json(new { success = true, results = new List<object>() });
                }

                keyword = keyword.Trim().ToLower();
                var results = new List<object>();

                // Tìm sản phẩm
                var productsData = await _context.PetProducts
                    .Where(p => p.TrangThai == true && p.Name.ToLower().Contains(keyword))
                    .Take(limit / 3)
                    .ToListAsync();
                
                var products = productsData.Select(p => 
                {
                    string? image = null;
                    if (p.Images != null)
                    {
                        var firstImage = p.Images.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                        image = firstImage != null ? firstImage.Trim() : null;
                    }
                    return new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Type = "pet",
                        Category = "Thú cưng",
                        Image = image,
                        Price = p.Price
                    };
                }).ToList();

                var foods = await _context.FoodProducts
                    .Where(f => f.TrangThai == true && f.Name.ToLower().Contains(keyword))
                    .Take(limit / 3)
                    .Select(f => new
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Type = "food",
                        Category = "Thức ăn",
                        Image = f.Image,
                        Price = f.Price
                    })
                    .ToListAsync();

                var accessories = await _context.AccessoryProducts
                    .Where(a => a.TrangThai == true && a.Name.ToLower().Contains(keyword))
                    .Take(limit / 3)
                    .Select(a => new
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Type = "accessory",
                        Category = "Phụ kiện",
                        Image = a.Image,
                        Price = a.Price
                    })
                    .ToListAsync();

                // Tìm dịch vụ
                var services = await _context.DichVus
                    .Where(d => d.TrangThai == true && d.TenDichVu.ToLower().Contains(keyword))
                    .Take(limit / 3)
                    .Select(d => new
                    {
                        Id = d.Id,
                        Name = d.TenDichVu,
                        Type = d.LoaiDichVu.ToLower(),
                        Category = d.LoaiDichVu == "Grooming" ? "Grooming" : "Khách sạn",
                        Image = d.HinhAnh,
                        Price = d.Gia
                    })
                    .ToListAsync();

                results.AddRange(products.Cast<object>());
                results.AddRange(foods.Cast<object>());
                results.AddRange(accessories.Cast<object>());
                results.AddRange(services.Cast<object>());

                return Json(new { success = true, results = results.Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

