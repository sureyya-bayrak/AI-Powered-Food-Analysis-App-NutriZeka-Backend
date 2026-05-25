using Microsoft.EntityFrameworkCore;
using NutriZeka.Domain.Entities;
using NutriZeka.Domain.Interfaces;
using NutriZeka.Infrastructure.Context;
using System;
using System.Threading.Tasks;

namespace NutriZeka.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            // Kullanıcıyı ilişkili tarama geçmişleriyle birlikte getiriyoruz.
            return await _context.Users
                .Include(u => u.ScanHistories)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // EF Core bu sorguyu otomatik olarak parametrik hale getirir, SQL Injection riski yoktur.
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Tüm kullanıcıları, tarama geçmişlerini de dahil ederek liste olarak döner.
            return await _context.Users
                .Include(u => u.ScanHistories)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}