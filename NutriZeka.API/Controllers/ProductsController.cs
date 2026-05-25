using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces; // Repository arayüzü için
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NutriZeka.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // ARTIK DbContext YOK! Sadece Interface'ler var.
        private readonly IMapper _mapper;
        private readonly IProductImportService _importService;
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;

        public ProductsController(
            IMapper mapper,
            IProductImportService importService,
            IProductService productService,
            IProductRepository productRepository)
        {
            _mapper = mapper;
            _importService = importService;
            _productService = productService;
            _productRepository = productRepository;
        }

        // --- YARDIMCI METOT: Dili Ayarla ---
        private void MapDisplayFields(ProductDetailDto dto, string language)
        {
            bool isEnglish = language.StartsWith("en");
            dto.DisplayName = isEnglish && !string.IsNullOrEmpty(dto.NameEn) ? dto.NameEn : dto.NameTr;
            dto.DisplayIngredients = isEnglish && !string.IsNullOrEmpty(dto.IngredientsTextEn) ? dto.IngredientsTextEn : dto.IngredientsTextTr;
            dto.DisplayAllergens = isEnglish && !string.IsNullOrEmpty(dto.AllergensEn) ? dto.AllergensEn : dto.AllergensTr;
            dto.DisplayCategory = isEnglish && !string.IsNullOrEmpty(dto.CategoriesEn) ? dto.CategoriesEn : dto.CategoriesTr;
        }

        [HttpGet("alternatives/{barcode}")]
        public async Task<ActionResult<IEnumerable<ProductDetailDto>>> GetAlternatives(string barcode)
        {
            var language = Request.Headers["Accept-Language"].ToString().ToLower();
            var alternatives = await _productService.GetBetterAlternativesAsync(barcode);

            if (alternatives == null || !alternatives.Any())
                return NotFound(new { message = "Bu ürün için daha iyi bir alternatif bulunamadı." });

            var dtos = _mapper.Map<IEnumerable<ProductDetailDto>>(alternatives);
            foreach (var dto in dtos) MapDisplayFields(dto, language);

            return Ok(dtos);
        }

        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductByBarcode(string barcode)
        {
            var language = Request.Headers["Accept-Language"].ToString().ToLower();
            // DbContext yerine Repository kullanıyoruz
            var product = await _productRepository.GetByBarcodeAsync(barcode);

            if (product == null) return NotFound(new { message = "Ürün bulunamadı." });

            var dto = _mapper.Map<ProductDetailDto>(product);
            MapDisplayFields(dto, language);
            return Ok(dto);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDetailDto>>> SearchProducts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Arama terimi boş olamaz." });

            // 1. Dil bilgisini al ve standardize et
            var language = Request.Headers["Accept-Language"].ToString().ToLower();

            // Sadece "en" veya "tr" kısmına bakmak daha güvenlidir (en-US, en-GB vb. için)
            if (string.IsNullOrEmpty(language)) language = "tr-tr";

            // 2. Repository'ye dili gönderiyoruz (Repository içinde filtreleme yapılacak)
            var products = await _productRepository.SearchAsync(query, language);

            if (products == null || !products.Any())
                return NotFound(new { message = "Ürün bulunamadı." });

            // 3. Mapleme işlemi
            var dtos = _mapper.Map<IEnumerable<ProductDetailDto>>(products);

            // 4. Mapping sonrası UI alanlarını (ProductName vb.) dile göre dolduruyoruz
            foreach (var dto in dtos)
            {
                MapDisplayFields(dto, language);
            }

            return Ok(dtos);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetailDto>>> GetProducts()
        {
            var language = Request.Headers["Accept-Language"].ToString().ToLower();
            var products = await _productRepository.GetAllAsync();

            var dtos = _mapper.Map<IEnumerable<ProductDetailDto>>(products);
            foreach (var dto in dtos) MapDisplayFields(dto, language);

            return Ok(dtos);
        }

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportProductsFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Lütfen geçerli bir CSV dosyası yükleyin." });

            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFromCsvAsync(stream);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailDto>> PostProduct([FromBody] ProductCreateDto productCreateDto)
        {
            var product = _mapper.Map<Product>(productCreateDto);
            await _productRepository.AddAsync(product);

            var productDto = _mapper.Map<ProductDetailDto>(product);
            return CreatedAtAction(nameof(GetProductByBarcode), new { barcode = product.Barcode }, productDto);
        }

        [HttpPut("barcode/{barcode}")]
        public async Task<IActionResult> PutProductByBarcode(string barcode, [FromBody] ProductUpdateDto productUpdateDto)
        {
            var existingProduct = await _productRepository.GetByBarcodeAsync(barcode);
            if (existingProduct == null) return NotFound(new { message = "Güncellenecek ürün bulunamadı." });

            _mapper.Map(productUpdateDto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);

            return Ok(new { message = "Ürün başarıyla güncellendi." });
        }

        [HttpDelete("barcode/{barcode}")]
        public async Task<IActionResult> DeleteProductByBarcode(string barcode)
        {
            var product = await _productRepository.GetByBarcodeAsync(barcode);
            if (product == null) return NotFound(new { message = "Silinecek ürün bulunamadı." });

            await _productRepository.DeleteAsync(product.Id);
            return Ok(new { message = "Ürün başarıyla silindi." });
        }
    }
}