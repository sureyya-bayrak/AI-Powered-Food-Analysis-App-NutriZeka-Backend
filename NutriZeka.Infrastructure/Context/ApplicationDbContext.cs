using Microsoft.EntityFrameworkCore;
using NutriZeka.Domain.Entities;

namespace NutriZeka.Infrastructure.Context
{
    // DbContext'ten miras alıyoruz ki EF Core'un tüm yetenekleri (kaydet, sil vb.) bize gelsin
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // --- Veritabanı Tablolarımız ---
        // Bu isimler MSSQL'de oluşacak tabloların isimleri olacak
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ScanHistory> ScanHistories { get; set; }

        // 🚀 YENİ EKLENEN: AI Analiz Önbellek Tablosu
        public DbSet<AIAnalysisCache> AIAnalysisCaches { get; set; }

        // --- Kuralların (Fluent API) Yazıldığı Yer ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Kural: Product tablosundaki Barcode sütunu benzersiz (Unique) olmalı!
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Barcode)
                .IsUnique();

            // 2. Kural: ScanHistory ile User ve Product arasındaki ilişkileri sağlama alıyoruz.
            // (Entity Framework genelde bunu otomatik anlar ama biz işimizi garantiye alıyoruz)
            modelBuilder.Entity<ScanHistory>()
                .HasOne(s => s.User)
                .WithMany(u => u.ScanHistories)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<ScanHistory>()
                .HasOne(s => s.Product)
                .WithMany() // Ürün üzerinden geçmişe gitmeyeceğimiz için burayı boş bırakıyoruz
                .HasForeignKey(s => s.ProductId);
            // 🚀 3. YENİ KURAL: AIAnalysisCache Tablosunun İlişkileri ve Silinme Kuralları
            modelBuilder.Entity<AIAnalysisCache>(entity =>
            {
                // User ile ilişkisi - DÖNGÜYÜ KIRMAK İÇİN NOACTION
                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.NoAction); // 🚀 DÜZELTİLDİ

                // Product ile ilişkisi - DÖNGÜYÜ KIRMAK İÇİN NOACTION
                entity.HasOne(a => a.Product)
                      .WithMany()
                      .HasForeignKey(a => a.ProductId)
                      .OnDelete(DeleteBehavior.NoAction); // 🚀 DÜZELTİLDİ

                // Sadece ScanHistory ile ilişkisinde Cascade kalsın. (Tarama geçmişi silinirse analiz gitsin)
                entity.HasOne(a => a.ScanHistory)
                      .WithMany(sh => sh.AIAnalyses)
                      .HasForeignKey(a => a.ScanHistoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}