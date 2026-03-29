using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    [Route("PetShop")]
    public class PetShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PetShop - Trang chính
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: PetShop/Shop - Trang shop với filter
        [HttpGet]
        [Route("Shop")]
        public async Task<IActionResult> Shop()
        {
            // Load filter options
            ViewBag.Brands = await _context.FoodProducts
                .Where(f => f.TrangThai == true && !string.IsNullOrEmpty(f.Brand))
                .Select(f => f.Brand!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            ViewBag.Species = await _context.PetProducts
                .Where(p => p.TrangThai == true)
                .Select(p => p.Species)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return View();
        }

        // GET: PetShop/Pets - Danh sách thú cưng
        [HttpGet]
        [Route("Pets")]
        public async Task<IActionResult> Pets(string? species, string? sortBy, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.PetProducts.Where(p => p.TrangThai == true).AsQueryable();

            // Lọc theo giống loài
            if (!string.IsNullOrEmpty(species))
            {
                query = query.Where(p => p.Species == species);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Lọc theo đánh giá (4 sao trở lên)
            // Note: Cần filter theo rating nếu có parameter

            // Sắp xếp
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "bestseller" => query.OrderByDescending(p => p.SoldCount),
                "rating" => query.OrderByDescending(p => p.AverageRating),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var pets = await query.ToListAsync();

            // Lấy danh sách giống loài để hiển thị trong filter
            ViewBag.Species = await _context.PetProducts
                .Where(p => p.TrangThai == true)
                .Select(p => p.Species)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            ViewBag.SelectedSpecies = species;
            ViewBag.SortBy = sortBy;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(pets);
        }

        // GET: PetShop/Foods - Danh sách thức ăn
        [HttpGet]
        [Route("Foods")]
        public async Task<IActionResult> Foods(string? type, string? brand, string? sortBy, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.FoodProducts.Where(f => f.TrangThai == true).AsQueryable();

            // Lọc theo loại thức ăn
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(f => f.Type == type);
            }

            // Lọc theo thương hiệu
            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(f => f.Brand == brand);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(f => f.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(f => f.Price <= maxPrice.Value);
            }

            // Sắp xếp
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(f => f.Price),
                "price_desc" => query.OrderByDescending(f => f.Price),
                "name_asc" => query.OrderBy(f => f.Name),
                "name_desc" => query.OrderByDescending(f => f.Name),
                "bestseller" => query.OrderByDescending(f => f.SoldCount),
                "rating" => query.OrderByDescending(f => f.AverageRating),
                _ => query.OrderByDescending(f => f.CreatedAt)
            };

            var foods = await query.ToListAsync();

            // Lấy danh sách loại và thương hiệu để hiển thị trong filter
            ViewBag.Types = await _context.FoodProducts
                .Where(f => f.TrangThai == true)
                .Select(f => f.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.Brands = await _context.FoodProducts
                .Where(f => f.TrangThai == true && !string.IsNullOrEmpty(f.Brand))
                .Select(f => f.Brand!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            ViewBag.SelectedType = type;
            ViewBag.SelectedBrand = brand;
            ViewBag.SortBy = sortBy;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(foods);
        }

        // GET: PetShop/Accessories - Danh sách phụ kiện
        [HttpGet]
        [Route("Accessories")]
        public async Task<IActionResult> Accessories(string? category, string? sortBy, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.AccessoryProducts.Where(a => a.TrangThai == true).AsQueryable();

            // Lọc theo phân loại
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.Category == category);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(a => a.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(a => a.Price <= maxPrice.Value);
            }

            // Sắp xếp
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(a => a.Price),
                "price_desc" => query.OrderByDescending(a => a.Price),
                "name_asc" => query.OrderBy(a => a.Name),
                "name_desc" => query.OrderByDescending(a => a.Name),
                "bestseller" => query.OrderByDescending(a => a.SoldCount),
                "rating" => query.OrderByDescending(a => a.AverageRating),
                _ => query.OrderByDescending(a => a.CreatedAt)
            };

            var accessories = await query.ToListAsync();

            // Lấy danh sách phân loại để hiển thị trong filter
            ViewBag.Categories = await _context.AccessoryProducts
                .Where(a => a.TrangThai == true)
                .Select(a => a.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.SelectedCategory = category;
            ViewBag.SortBy = sortBy;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(accessories);
        }

        // GET: PetShop/PetDetails/5 - Chi tiết thú cưng
        [HttpGet]
        [Route("PetDetails/{id}")]
        public async Task<IActionResult> PetDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petProduct = await _context.PetProducts
                .FirstOrDefaultAsync(m => m.Id == id && m.TrangThai == true);

            if (petProduct == null)
            {
                return NotFound();
            }

            // Gợi ý dịch vụ cho sản phẩm này
            ViewBag.ProductId = id;
            ViewBag.ProductType = "pet";

            return View(petProduct);
        }

        // GET: PetShop/FoodDetails/5 - Chi tiết thức ăn
        [HttpGet]
        [Route("FoodDetails/{id}")]
        public async Task<IActionResult> FoodDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodProduct = await _context.FoodProducts
                .FirstOrDefaultAsync(m => m.Id == id && m.TrangThai == true);

            if (foodProduct == null)
            {
                return NotFound();
            }

            // Gợi ý dịch vụ cho sản phẩm này
            ViewBag.ProductId = id;
            ViewBag.ProductType = "food";

            return View(foodProduct);
        }

        // GET: PetShop/AccessoryDetails/5 - Chi tiết phụ kiện
        [HttpGet]
        [Route("AccessoryDetails/{id}")]
        public async Task<IActionResult> AccessoryDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessoryProduct = await _context.AccessoryProducts
                .FirstOrDefaultAsync(m => m.Id == id && m.TrangThai == true);

            if (accessoryProduct == null)
            {
                return NotFound();
            }

            // Gợi ý dịch vụ cho sản phẩm này
            ViewBag.ProductId = id;
            ViewBag.ProductType = "accessory";

            return View(accessoryProduct);
        }

        // API: Lấy danh sách sản phẩm với filter nâng cao
        [HttpPost]
        [Route("GetProducts")]
        public async Task<IActionResult> GetProducts([FromBody] ProductFilterRequest request)
        {
            try
            {
                var category = request.Category?.ToLower() ?? "all";
                var page = request.Page > 0 ? request.Page : 1;
                var pageSize = request.PageSize > 0 ? request.PageSize : 20;

                List<ProductDto> allProducts = new();

                // Lấy sản phẩm theo category
                if (category == "pet" || category == "all")
                {
                    var pets = await _context.PetProducts
                        .Where(p => p.TrangThai == true)
                        .ToListAsync();
                    allProducts.AddRange(pets.Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        OldPrice = p.OldPrice,
                        Image = p.Images != null ? p.Images.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() : null,
                        Rating = p.AverageRating,
                        ReviewCount = p.ReviewCount,
                        SoldCount = p.SoldCount,
                        Type = "pet",
                        Species = p.Species
                    }));
                }

                if (category == "food" || category == "all")
                {
                    var foods = await _context.FoodProducts
                        .Where(f => f.TrangThai == true)
                        .ToListAsync();
                    allProducts.AddRange(foods.Select(f => new ProductDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Price = f.Price,
                        OldPrice = f.OldPrice,
                        Image = f.Image,
                        Rating = f.AverageRating,
                        ReviewCount = f.ReviewCount,
                        SoldCount = f.SoldCount,
                        Type = "food",
                        Brand = f.Brand,
                        ProductType = f.Type
                    }));
                }

                if (category == "accessory" || category == "all")
                {
                    var accessories = await _context.AccessoryProducts
                        .Where(a => a.TrangThai == true)
                        .ToListAsync();
                    allProducts.AddRange(accessories.Select(a => new ProductDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Price = a.Price,
                        OldPrice = a.OldPrice,
                        Image = a.Image,
                        Rating = a.AverageRating,
                        ReviewCount = a.ReviewCount,
                        SoldCount = a.SoldCount,
                        Type = "accessory",
                        Category = a.Category
                    }));
                }

                // Apply filters
                var filtered = allProducts.AsQueryable();

                // Filter by category if specified
                if (category != "all")
                {
                    filtered = filtered.Where(p => p.Type == category);
                }

                // Filter by price
                if (request.MinPrice.HasValue)
                {
                    filtered = filtered.Where(p => p.Price >= request.MinPrice.Value);
                }
                if (request.MaxPrice.HasValue)
                {
                    filtered = filtered.Where(p => p.Price <= request.MaxPrice.Value);
                }

                // Filter by rating
                if (request.MinRating.HasValue)
                {
                    filtered = filtered.Where(p => p.Rating >= request.MinRating.Value);
                }

                // Filter by brand
                if (!string.IsNullOrEmpty(request.Brand) && (category == "food" || category == "all"))
                {
                    filtered = filtered.Where(p => p.Type == "food" && p.Brand == request.Brand);
                }

                // Filter by species
                if (!string.IsNullOrEmpty(request.Species) && (category == "pet" || category == "all"))
                {
                    filtered = filtered.Where(p => p.Type == "pet" && p.Species == request.Species);
                }

                // Sort
                var sorted = request.SortBy switch
                {
                    "price_asc" => filtered.OrderBy(p => p.Price),
                    "price_desc" => filtered.OrderByDescending(p => p.Price),
                    "bestseller" => filtered.OrderByDescending(p => p.SoldCount),
                    "rating" => filtered.OrderByDescending(p => p.Rating),
                    _ => filtered.OrderByDescending(p => p.Id)
                };

                var total = sorted.Count();
                var result = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Json(new { success = true, products = result, total = total, page = page, totalPages = (int)Math.Ceiling(total / (double)pageSize) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class ProductFilterRequest
    {
        public string? Category { get; set; } // pet, food, accessory, all
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Brand { get; set; }
        public string? Species { get; set; }
    }
}

