using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using NutriZeka.Application.Interfaces.Repositories;
using NutriZeka.Application.Interfaces.Services;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Services
{
    public class AIAnalysisService : IAIAnalysisService
    {
        private readonly IAIAnalysisCacheRepository _cacheRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenerativeAiService _aiService;
        private readonly IProductService _productService;

        public AIAnalysisService(
            IAIAnalysisCacheRepository cacheRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IGenerativeAiService aiService,
            IProductService productService)
        {
            _cacheRepository = cacheRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _aiService = aiService;
            _productService = productService;
        }

        public async Task<string> GetAnalysisAsync(Guid userId, AIAnalysisRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (user == null || product == null)
                throw new Exception("Kullanıcı veya ürün bulunamadı.");

            // Limit Kontrolü - Tarih Sıfırlama
            if (user.LastAiRequestDate.Date < DateTime.UtcNow.Date)
            {
                user.DailyAiRequestCount = 0;
                user.LastAiRequestDate = DateTime.UtcNow;
            }

            // Cache Kontrolü
            var cachedData = await _cacheRepository.GetCachedAnalysisAsync(
                userId, request.ProductId, request.QuestionType,
                user.IsGlutenFree, user.IsLactoseFree, user.IsPalmOilFree);

            if (cachedData != null)
                return cachedData.AIResponse;

            // Günlük Limit Kontrolü
            int maxDailyLimit = 50;
            if (user.DailyAiRequestCount >= maxDailyLimit)
                throw new Exception($"Günlük yapay zeka limitin doldu ({maxDailyLimit}/{maxDailyLimit}). Yarın tekrar sorabilirsin!");

            // Alternatifleri Çekme (Sadece 3. Soru için)
            string alternativesText = "";
            if (request.QuestionType == 3)
            {
                var alternatives = await _productService.GetBetterAlternativesAsync(product.Barcode);

                if (alternatives != null && alternatives.Any())
                {
                    alternativesText = string.Join(", ", alternatives.Select(a => a.NameTr));
                }
                else
                {
                    alternativesText = "Maalesef şu an veritabanımızda bu ürün için daha iyi bir alternatif bulunamadı.";
                }
            }

            // 🚀 İŞTE HATAYI ÇÖZEN SATIR: Artık 4 parametre gönderiyoruz (alternativesText eklendi)
            string prompt = GeneratePrompt(request.QuestionType, user, product, alternativesText);

            // Yapay Zekaya İstek At
            string aiResponse = await _aiService.GenerateResponseAsync(prompt);

            // Güvenlik Kalkanı
            if (aiResponse.Contains("Gemini API Hatası") ||
                aiResponse.Contains("Sistem Hatası") ||
                aiResponse.Contains("Bağlantı Hatası") ||
                aiResponse.Contains("404") ||
                aiResponse.Contains("503"))
            {
                throw new Exception("Yapay zeka servisine şu an ulaşılamıyor, lütfen kısa bir süre sonra tekrar dene.");
            }

            // Limiti Artır
            user.DailyAiRequestCount++;
            await _userRepository.UpdateAsync(user);

            // Cache'e Kaydet
            var newCache = new AIAnalysisCache
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = request.ProductId,
                ScanHistoryId = request.ScanHistoryId,
                QuestionType = request.QuestionType,
                UserWasGlutenFree = user.IsGlutenFree,
                UserWasLactoseFree = user.IsLactoseFree,
                UserWasPalmOilFree = user.IsPalmOilFree,
                AIResponse = aiResponse,
                CreatedAt = DateTime.UtcNow
            };

            await _cacheRepository.AddAsync(newCache);
            await _cacheRepository.SaveChangesAsync();

            return aiResponse;
        }
        private string GeneratePrompt(int questionType, User user, Product product, string alternativesText)
        {
            // 🚀 DİKKAT: Persona'nın o etik ve bilimsel yapısı korundu, sadece KESİN bir "kısa tut ve selamlama yapma" kuralı eklendi.
            string systemPersona = "Sen 'NutriZeka' uygulamasının uzman ve objektif gıda dedektifisin. Amacın, tıbbi bir tavsiye vermeden, tamamen bilimsel verilere dayanarak kullanıcıya bilgi sunmak. KESİN KURAL: 'Merhaba', 'Ben NutriZeka', 'Hadi başlayalım', 'Sağlıklı günler' gibi hiçbir giriş, selamlama veya kapanış cümlesi KULLANMA. Kullanıcı markette ve acelesi var, bu yüzden doğrudan konuya gir ve en önemli bilgileri (hap bilgi) çok kısa maddeler halinde ver. Daima Markdown formatı (##, **, -) kullan. ";

            string productData = $"İncelenecek Ürün: {product.NameTr}. İçindekiler: {product.IngredientsTextTr}. Alerjen Uyarısı: {product.AllergensTr}. ";

            return questionType switch
            {
                1 => systemPersona + productData +
                     $"Kullanıcı Profili: Laktozsuz: {(user.IsLactoseFree ? "Evet" : "Hayır")}, " +
                     $"Glutensiz: {(user.IsGlutenFree ? "Evet" : "Hayır")}, " +
                     $"Palm Yağsız: {(user.IsPalmOilFree ? "Evet" : "Hayır")}. " +
                     "Görev: Bu ürün kullanıcının profili için içerik bazında uygun mu? Tıbbi tavsiye vermeden ve markayı karalamadan, uyumlu veya uyumsuz olan noktaları maksimum 3-4 maddede çok net ve kısa şekilde özetle.",

                2 => systemPersona + productData +
                     "Görev: Bu üründeki karmaşık kelimeleri ve katkı maddelerini (E kodları vb.) seç ve ne işe yaradıklarını objektif bir şekilde, maksimum 1-2 cümlelik kısa maddeler halinde açıkla. Gereksiz detaylara girme.",

                3 => systemPersona + productData +
                     $"NutriZeka'nın Önerdiği Alternatifler: {alternativesText}. " +
                     "Görev: İncelediğimiz ürünü doğrudan kötülemeden, sunduğumuz bu alternatiflerin besin değeri veya doğallık açısından neden daha iyi bir tercih olabileceğini tarafsızca kıyasla. Alternatifleri maddeler halinde listele ve yanlarına en fazla 1-2 cümlelik çok kısa açıklamalar ekle.",

                _ => systemPersona + productData + "Görev: Bu ürün hakkında genel, çok kısa bir değerlendirme yap."
            };
        }
    }
}