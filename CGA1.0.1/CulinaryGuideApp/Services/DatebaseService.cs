using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CulinaryGuideApp.Models;

namespace CulinaryGuideApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        public async Task Init()
        {
            if (_db != null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "restaurants.db");
            _db = new SQLiteAsyncConnection(dbPath);

            // 如果数据库文件存在，就不再创建
            if (!File.Exists(dbPath))
            {
                await _db.CreateTableAsync<User>();
                await _db.CreateTableAsync<Restaurant>();
                await _db.CreateTableAsync<Comment>();
            }
        }

        private string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] passwordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public async Task AddUser(User user)
        {
            await Init();
            string salt = GenerateSalt();
            string passwordHash = HashPassword(user.Password, salt);
            user.Salt = salt;
            user.PasswordHash = passwordHash;
            // 重要：在存储到数据库之前，将 user.Password 设置为 null 或其他安全值，避免意外使用。
            user.Password = null;
            await _db.InsertAsync(user);
        }

        public async Task<User> GetUser(string username, string password)
        {
            await Init();
            var user = await _db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

            if (user != null)
            {
                string hashedPassword = HashPassword(password, user.Salt);
                if (hashedPassword == user.PasswordHash)
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<bool> UserExists(string username)
        {
            await Init();
            var user = await _db.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<List<Restaurant>> GetRestaurants()
        {
            await Init();
            return await _db.Table<Restaurant>().ToListAsync();
        }

        public async Task<List<Comment>> GetComments(int restaurantId)
        {
            await Init();
            return await _db.Table<Comment>().Where(c => c.RestaurantId == restaurantId).ToListAsync();
        }

        public async Task AddRestaurant(Restaurant restaurant)
        {
            await Init();
            await _db.InsertAsync(restaurant);
        }

        public async Task AddComment(Comment comment)
        {
            await Init();
            await _db.InsertAsync(comment);
        }

        public async Task UpdateRestaurantImages(int restaurantId, List<string> imagePaths)
        {
            await Init();
            var restaurant = await _db.Table<Restaurant>().Where(r => r.Id == restaurantId).FirstOrDefaultAsync();
            if (restaurant != null)
            {
                restaurant.ImagePathsJson = JsonSerializer.Serialize(imagePaths);
                await _db.UpdateAsync(restaurant);
            }
        }

        public List<string> ParseImagePaths(string json)
        {
            return string.IsNullOrEmpty(json) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(json);
        }
    }
}