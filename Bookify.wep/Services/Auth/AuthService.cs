using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bookify.Data.Data;
using Bookify.Domain.Entities;

namespace Bookify.wep.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private const int Iterations = 10000;

        public AuthService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Usere?> ValidateUserAsync(string emailOrUsername, string password)
        {
            if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrEmpty(password))
                return null;

            var user = await _db.Useres
                .FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.UserName == emailOrUsername);

            if (user == null) return null;

            return VerifyHashedPassword(user.Epassword ?? string.Empty, password) ? user : null;
        }

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            var res = new byte[1 + salt.Length + hash.Length];
            res[0] = 0;
            Buffer.BlockCopy(salt, 0, res, 1, salt.Length);
            Buffer.BlockCopy(hash, 0, res, 1 + salt.Length, hash.Length);
            return Convert.ToBase64String(res);
        }

        public bool VerifyHashedPassword(string hashed, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashed) || string.IsNullOrEmpty(providedPassword)) return false;
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(hashed);
            }
            catch
            {
                return false;
            }
            if (bytes.Length < 1 + 16 + 32) return false;
            var salt = new byte[16];
            Buffer.BlockCopy(bytes, 1, salt, 0, 16);
            using var pbkdf2 = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            for (int i = 0; i < 32; i++)
            {
                if (bytes[1 + 16 + i] != hash[i]) return false;
            }
            return true;
        }

        public async Task<bool> IsEmailOrUserNameTakenAsync(string userName, string email)
        {
            return await _db.Useres.AnyAsync(u => u.UserName == userName || u.Email == email);
        }


        public async Task<Usere> CreateUserAsync(string userName, string email, string password, string? role = "client")
        {
            userName = userName?.Trim() ?? throw new ArgumentNullException(nameof(userName));
            email = email?.Trim() ?? throw new ArgumentNullException(nameof(email));

            var hashed = HashPassword(password);

            var nextId = await _db.Useres.MaxAsync(u => (int?)u.UserId) ?? 0;
            nextId++;

            var user = new Usere
            {
                UserId = nextId,
                UserName = userName,
                Email = email,
                Epassword = hashed,
                UserRole = role ?? "client"
            };

            _db.Useres.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
