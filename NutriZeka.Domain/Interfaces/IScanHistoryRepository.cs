// Domain/Interfaces/IScanHistoryRepository.cs
using NutriZeka.Domain.Entities;

public interface IScanHistoryRepository
{
    // --- BU SATIRI EKLE (Admin'in tüm listeyi görebilmesi için şart) ---
    Task<IEnumerable<ScanHistory>> GetAllAsync();
    Task<IEnumerable<ScanHistory>> GetUserHistoriesAsync(Guid userId);
    Task<IEnumerable<ScanHistory>> GetFavoritesAsync(Guid userId); // YENİ
    Task<IEnumerable<ScanHistory>> GetSavedAsync(Guid userId);     // YENİ
    Task<ScanHistory?> GetByIdAsync(Guid id);                      // YENİ
    Task<ScanHistory?> GetHistoryByUserAndProductAsync(Guid userId, Guid productId);
    Task AddAsync(ScanHistory history);
    void Update(ScanHistory history);
    void RemoveRange(IEnumerable<ScanHistory> histories);
    Task SaveChangesAsync();
}