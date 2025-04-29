using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Models;

namespace TodoList.Services
{
    public class TodoService
    {
        private readonly IMongoCollection<Todo> _todoCollection;

        public TodoService(IOptions<TodoDatabaseSettings> todoDatabaseSettings)
        {
            var mongoClient = new MongoClient(todoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(todoDatabaseSettings.Value.DatabaseName);
            _todoCollection = mongoDatabase.GetCollection<Todo>(todoDatabaseSettings.Value.TodoCollectionName);
        }

        public async Task<List<Todo>> GetAsync() =>
            await _todoCollection.Find(_ => true).ToListAsync();

        public async Task<List<Todo>> GetByUserIdAsync(string userId) =>
            await _todoCollection.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Todo?> GetAsync(string id) =>
            await _todoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Todo?> GetByIdAndUserIdAsync(string id, string userId) =>
            await _todoCollection.Find(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();

        public async Task CreateAsync(Todo newTodo) =>
            await _todoCollection.InsertOneAsync(newTodo);

        public async Task UpdateAsync(string id, Todo updatedTodo) =>
            await _todoCollection.ReplaceOneAsync(x => x.Id == id, updatedTodo);

        public async Task RemoveAsync(string id) =>
            await _todoCollection.DeleteOneAsync(x => x.Id == id);
            
        public async Task ToggleStatusAsync(string id, string userId)
        {
            var todo = await GetByIdAndUserIdAsync(id, userId);
            if (todo != null)
            {
                todo.IsDone = !todo.IsDone;
                await UpdateAsync(id, todo);
            }
        }
    }
}