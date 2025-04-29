using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TodoList.Models;

namespace TodoList.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserService(IOptions<TodoDatabaseSettings> todoDatabaseSettings)
        {
            var mongoClient = new MongoClient(todoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(todoDatabaseSettings.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<User>(todoDatabaseSettings.Value.UserCollectionName);
            
            var indexKeysDefinition = Builders<User>.IndexKeys.Ascending(user => user.Username);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<User>(indexKeysDefinition, indexOptions);
            _userCollection.Indexes.CreateOne(indexModel);
            
            var emailIndexKeysDefinition = Builders<User>.IndexKeys.Ascending(user => user.Email);
            var emailIndexModel = new CreateIndexModel<User>(emailIndexKeysDefinition, indexOptions);
            _userCollection.Indexes.CreateOne(emailIndexModel);
        }

        public async Task<List<User>> GetAsync() =>
            await _userCollection.Find(_ => true).ToListAsync();

        public async Task<User?> GetAsync(string id) =>
            await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _userCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await _userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task<bool> CreateAsync(User newUser, string password)
        {
            var existingUser = await GetByUsernameAsync(newUser.Username);
            if (existingUser != null)
                return false;

            existingUser = await GetByEmailAsync(newUser.Email);
            if (existingUser != null)
                return false;

            newUser.PasswordHash = HashPassword(password);
            newUser.CreatedAt = DateTime.Now;

            await _userCollection.InsertOneAsync(newUser);
            return true;
        }

        public async Task UpdateAsync(string id, User updatedUser) =>
            await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task RemoveAsync(string id) =>
            await _userCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null)
                return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}