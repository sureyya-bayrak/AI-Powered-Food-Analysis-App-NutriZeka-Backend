using System;

namespace NutriZeka.Application.DTOs
{
    public class ScanHistoryCreateDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        // FİLTRELEME İÇİN HAYAT KURTARAN ALAN (Eklendi)
        // Flutter'dan gelen 'scan' veya 'search' kelimesini burada yakalayacağız.
        public string Source { get; set; } = "scan"; // Varsayılan olarak "scan" atadık

        public string? AIAnalysisSummary { get; set; } // Şimdilik opsiyonel
    }
}