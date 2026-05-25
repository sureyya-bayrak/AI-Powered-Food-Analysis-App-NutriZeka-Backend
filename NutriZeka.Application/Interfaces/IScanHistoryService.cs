using NutriZeka.Application.DTOs;

public interface IScanHistoryService
{
    Task<IEnumerable<ScanHistoryDto>> GetUserHistoryAsync(Guid userId);
    Task<IEnumerable<ScanHistoryDto>> GetFavoritesAsync(Guid userId); // Yeni eklendi
    Task<IEnumerable<ScanHistoryDto>> GetSavedAsync(Guid userId);     // Yeni eklendi
    Task<ScanHistoryDto> AddOrUpdateHistoryAsync(ScanHistoryCreateDto dto);
    Task UpdateStatusAsync(Guid id, ScanHistoryUpdateDto dto);         // Yeni eklendi
    Task ClearAllHistoryAsync(Guid userId);

    // En üste ekle
    Task<IEnumerable<ScanHistoryDto>> GetAllHistoryAsync();
}