using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion; // YENİ EKLENDİ
using Microsoft.EntityFrameworkCore;
using NutriZeka.Application.Interfaces;
using NutriZeka.Domain.Entities;
using NutriZeka.Infrastructure.Context;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Services
{
    // --- ÖZEL DÖNÜŞTÜRÜCÜLER (Kirli verileri temizleyen kurallar) ---
    public class CustomDoubleConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            // Eğer hücre boşsa veya sadece tire (-) varsa 0.0 dön
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "-")
                return 0.0;

            // Eğer normal bir sayıysa (noktalı vb.) çevirip dön
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;

            return 0.0; // Hata durumunda sistemi çökertmek yerine 0 kabul et
        }
    }

    public class CustomIntConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "-")
                return 0;

            if (int.TryParse(text, out int result))
                return result;

            return 0;
        }
    }

    public class ProductImportService : IProductImportService
    {
        private readonly ApplicationDbContext _context;

        public ProductImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(int AddedCount, int UpdatedCount, string Message)> ImportFromCsvAsync(Stream fileStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, config);

            // 1 ve 0'ları True/False yapma kuralı
            csv.Context.TypeConverterOptionsCache.GetOptions<bool>().BooleanTrueValues.Add("1");
            csv.Context.TypeConverterOptionsCache.GetOptions<bool>().BooleanFalseValues.Add("0");

            // YENİ: Double ve Int (sayısal) alanlar için hazırladığımız kalkanları devreye sokuyoruz
            csv.Context.TypeConverterCache.AddConverter<double>(new CustomDoubleConverter());
            csv.Context.TypeConverterCache.AddConverter<int>(new CustomIntConverter());

            var records = csv.GetRecords<Product>().ToList();
            int addedCount = 0;
            int updatedCount = 0;

            foreach (var record in records)
            {
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Barcode == record.Barcode);

                if (existingProduct == null)
                {
                    _context.Products.Add(record);
                    addedCount++;
                }
                else
                {
                    existingProduct.NameTr = record.NameTr;
                    existingProduct.CategoriesTr = record.CategoriesTr;
                    existingProduct.ContainsGluten = record.ContainsGluten;

                    // Besin değerleri sonradan güncellenirse sıfırlanmasın diye
                    existingProduct.EnergyKcal = record.EnergyKcal;
                    existingProduct.Fat = record.Fat;
                    existingProduct.Sugars = record.Sugars;
                    existingProduct.Proteins = record.Proteins;
                    existingProduct.Salt = record.Salt;

                    _context.Products.Update(existingProduct);
                    updatedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return (addedCount, updatedCount, "İçe aktarma başarıyla tamamlandı.");
        }
    }
}