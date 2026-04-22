using System;

namespace NutriZeka.Domain.Entities
{
    // abstract: Bu sınıftan tek başına bir nesne üretilemez, 
    // sadece diğer sınıflara (Product, User vb.) miras verir.
    public abstract class BaseEntity
    {
        // Her kaydın dünyada eşi olmayan bir kimlik numarası (GUID) olacak.
        public Guid Id { get; set; } = Guid.NewGuid();

        // Kaydın veritabanına girdiği anı otomatik olarak UTC saatinde tutar.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}