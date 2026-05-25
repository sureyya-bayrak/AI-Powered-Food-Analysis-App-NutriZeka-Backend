using AutoMapper;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces; // Sözleşmemiz burada
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces; // Veri kurallarımız burada
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Services
{
    public class ScanHistoryService : IScanHistoryService
    {
        private readonly IScanHistoryRepository _repository;
        private readonly IMapper _mapper;

        public ScanHistoryService(IScanHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // 1. Kullanıcının Tüm Geçmişini Getir
        public async Task<IEnumerable<ScanHistoryDto>> GetUserHistoryAsync(Guid userId)
        {
            var histories = await _repository.GetUserHistoriesAsync(userId);
            return _mapper.Map<IEnumerable<ScanHistoryDto>>(histories);
        }

        // 2. Sadece Favoriye Alınanları Getir (YENİ)
        public async Task<IEnumerable<ScanHistoryDto>> GetFavoritesAsync(Guid userId)
        {
            var favorites = await _repository.GetFavoritesAsync(userId);
            return _mapper.Map<IEnumerable<ScanHistoryDto>>(favorites);
        }

        // 3. Sadece Kaydedilenleri Getir (YENİ)
        public async Task<IEnumerable<ScanHistoryDto>> GetSavedAsync(Guid userId)
        {
            var saved = await _repository.GetSavedAsync(userId);
            return _mapper.Map<IEnumerable<ScanHistoryDto>>(saved);
        }

        // 4. Favori veya Kaydedilme Durumunu Güncelle (YENİ)
        public async Task UpdateStatusAsync(Guid id, ScanHistoryUpdateDto dto)
        {
            var history = await _repository.GetByIdAsync(id);
            if (history != null)
            {
                history.IsFavorite = dto.IsFavorite;
                history.IsSaved = dto.IsSaved;

                _repository.Update(history);
                await _repository.SaveChangesAsync();
            }
        }

        // 5. Akıllı Kayıt: Varsa tarih güncelle, yoksa yeni ekle
        public async Task<ScanHistoryDto> AddOrUpdateHistoryAsync(ScanHistoryCreateDto dto)
        {
            var existing = await _repository.GetHistoryByUserAndProductAsync(dto.UserId, dto.ProductId);

            if (existing != null)
            {
                // Mevcut kaydı en üste taşımak için tarihini yeniliyoruz
                existing.CreatedAt = DateTime.UtcNow;
                _repository.Update(existing);
                await _repository.SaveChangesAsync();

                return _mapper.Map<ScanHistoryDto>(existing);
            }

            // Yeni kayıt oluşturma
            var newHistory = _mapper.Map<ScanHistory>(dto);
            newHistory.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(newHistory);
            await _repository.SaveChangesAsync();

            // Ürün bilgilerinin (isim vb.) DTO içinde dolu gelmesi için Repository'den tekrar çekiyoruz
            var result = await _repository.GetByIdAsync(newHistory.Id);
            return _mapper.Map<ScanHistoryDto>(result);
        }

        // 6. Tüm Geçmişi Temizle
        public async Task ClearAllHistoryAsync(Guid userId)
        {
            var histories = await _repository.GetUserHistoriesAsync(userId);
            _repository.RemoveRange(histories);
            await _repository.SaveChangesAsync();
        }
        // Tüm geçmişi getir (Admin için)
        public async Task<IEnumerable<ScanHistoryDto>> GetAllHistoryAsync()
        {
            var histories = await _repository.GetAllAsync(); // Repository'de GetAllAsync olduğunu varsayıyorum
            return _mapper.Map<IEnumerable<ScanHistoryDto>>(histories);
        }

    }
}