using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriZeka.Application.DTOs;
using NutriZeka.Domain.Entities;
using NutriZeka.Infrastructure.Context;

namespace NutriZeka.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // --- MOBİL TARAYICI İÇİN KRİTİK METOT ---
        // Flutter'daki mobile_scanner barkodu okuyunca bu adresi çağıracak:
        // Örn: GET /api/products/barcode/8690767673887
        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductByBarcode(string barcode)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (product == null)
            {
                return NotFound(new { message = "Ürün bulunamadı, lütfen yeni ürün ekleyin." });
            }

            // Entity'yi tasarıma uygun DTO'ya çeviriyoruz
            var productDto = _mapper.Map<ProductDetailDto>(product);

            return Ok(productDto);
        }

        // --- TEMEL CRUD İŞLEMLERİ ---

        // 1. Tüm Ürünleri Getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetailDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<ProductDetailDto>>(products);
            return Ok(dtos);
        }

        // 2. ID ile Ürün Getir
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDto>> GetProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            return Ok(_mapper.Map<ProductDetailDto>(product));
        }

        // 3. Yeni Ürün Ekle (Admin veya Manuel Giriş İçin)
        [HttpPost]
        public async Task<ActionResult<ProductDetailDto>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDetailDto>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        // PUT: api/Products/barcode/8690767673887
        [HttpPut("barcode/{barcode}")]
        public async Task<IActionResult> PutProductByBarcode(string barcode, [FromBody] Product product)
        {
            // 1. Veritabanında o barkoda sahip asıl ürünü buluyoruz
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (existingProduct == null)
            {
                return NotFound(new { message = "Güncellenecek ürün bulunamadı." });
            }

            // 2. Gelen verileri mevcut ürünün üzerine yazıyoruz
            // (Burada ID'yi değiştirmiyoruz, sadece içeriği güncelliyoruz)
            existingProduct.NameTr = product.NameTr;
            existingProduct.NameEn = product.NameEn;
            existingProduct.Brand = product.Brand;
            existingProduct.Quantity = product.Quantity;
            existingProduct.NutriScoreGrade = product.NutriScoreGrade;
            existingProduct.NovaGroup = product.NovaGroup;
            existingProduct.EnergyKcal = product.EnergyKcal;
            existingProduct.Fat = product.Fat;
            existingProduct.Sugars = product.Sugars;
            existingProduct.IngredientsTextTr = product.IngredientsTextTr;
            existingProduct.AllergensTr = product.AllergensTr;
            existingProduct.Categories = product.Categories;
            existingProduct.IsVerified = product.IsVerified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new { message = "Ürün başarıyla güncellendi." });
        }

        // DELETE: api/Products/barcode/8690767673887
        [HttpDelete("barcode/{barcode}")]
        public async Task<IActionResult> DeleteProductByBarcode(string barcode)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (product == null)
            {
                return NotFound(new { message = "Silinecek ürün bulunamadı." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün başarıyla silindi." });
        }
    }
}