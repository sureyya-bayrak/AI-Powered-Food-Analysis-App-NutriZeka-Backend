using Microsoft.EntityFrameworkCore;
using NutriZeka.Application.Interfaces;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces; // Repository'i tanıyabilmesi için eklendi
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NutriZeka.Application.Services // Senin dosyandaki namespace neyse o kalabilir
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository; // DbContext GİTTİ, Repository GELDİ!

        // Constructor Injection
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Product> GetBetterAlternativeAsync(string barcode)
        {
            // 1. Referans Ürünü Bul (Artık repository üzerinden)
            var refProduct = await _repository.GetByBarcodeAsync(barcode);

            if (refProduct == null || string.IsNullOrWhiteSpace(refProduct.CategoriesTr))
                return null;

            var cleanCategory = refProduct.CategoriesTr.Trim();

            // 2. Dinamik Sorguyu Başlat (Repository'den gelen Queryable ile)
            var query = _repository.GetQueryable()
                .Where(p => p.Barcode != refProduct.Barcode)
                .Where(p => p.CategoriesTr.Trim() == cleanCategory);

            // 3. NutriScore Filtresi
            if (!string.IsNullOrWhiteSpace(refProduct.NutriScoreGrade) && refProduct.NutriScoreGrade != "-")
            {
                query = query.Where(p => p.NutriScoreGrade != "-"
                                      && p.NutriScoreGrade != ""
                                      && p.NutriScoreGrade.CompareTo(refProduct.NutriScoreGrade) <= 0);
            }
            else
            {
                query = query.Where(p => p.NutriScoreGrade == "A" || p.NutriScoreGrade == "B");
            }

            // 4. Nova (İşlenmişlik) Filtresi
            if (refProduct.NovaGroup > 0)
            {
                query = query.Where(p => p.NovaGroup > 0 && p.NovaGroup <= refProduct.NovaGroup);
            }
            else
            {
                query = query.Where(p => p.NovaGroup > 0 && p.NovaGroup <= 3);
            }

            // 5. Final Sıralaması ve İlk Ürünü Çekme
            var betterAlternative = await query
                .OrderBy(p => p.NutriScoreGrade)
                .ThenBy(p => p.NovaGroup)
                .ThenBy(p => p.ContainsPalmOil ? 1 : 0)
                .ThenBy(p => p.Sugars)
                .FirstOrDefaultAsync();

            return betterAlternative;
        }

        public async Task<IEnumerable<Product>> GetBetterAlternativesAsync(string barcode)
        {
            var refProduct = await _repository.GetByBarcodeAsync(barcode);

            if (refProduct == null || string.IsNullOrWhiteSpace(refProduct.CategoriesTr))
                return new List<Product>();

            var cleanCategory = refProduct.CategoriesTr.Trim();

            var query = _repository.GetQueryable()
                .Where(p => p.Barcode != refProduct.Barcode)
                .Where(p => p.CategoriesTr.Trim() == cleanCategory);

            // NutriScore Filtresi
            if (!string.IsNullOrWhiteSpace(refProduct.NutriScoreGrade) && refProduct.NutriScoreGrade != "-")
            {
                query = query.Where(p => p.NutriScoreGrade != "-"
                                      && p.NutriScoreGrade != ""
                                      && p.NutriScoreGrade.CompareTo(refProduct.NutriScoreGrade) <= 0);
            }
            else
            {
                query = query.Where(p => p.NutriScoreGrade == "A" || p.NutriScoreGrade == "B");
            }

            // Nova Filtresi
            if (refProduct.NovaGroup > 0)
            {
                query = query.Where(p => p.NovaGroup > 0 && p.NovaGroup <= refProduct.NovaGroup);
            }
            else
            {
                query = query.Where(p => p.NovaGroup > 0 && p.NovaGroup <= 3);
            }

            // LİSTE OLARAK DÖNME KISMI
            var alternatives = await query
                .OrderBy(p => p.NutriScoreGrade)
                .ThenBy(p => p.NovaGroup)
                .ThenBy(p => p.ContainsPalmOil ? 1 : 0)
                .ThenBy(p => p.Sugars)
                .Take(5)
                .ToListAsync();

            return alternatives;
        }
    }
}