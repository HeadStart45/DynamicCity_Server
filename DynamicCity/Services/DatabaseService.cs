using DynamicCity_Models;
using DynamicCity_Models.Character;
using DynamicCity_Models.DataObjects;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DynamicCity.Services
{
    public interface IDatabaseService
    {
        public Task<Character> GetCharacterAsync(ObjectId _id);
        public Task<Character> GetCharacterAsync(string _name);
        public Task<List<Character>> GetCharactersAsync();
        public Task<Character> CreateCharacter(NewCharacterData _character);
        
        public Task UpdateCharacter(Character _updatedCharacter);
        public Task UpdateCharacters(List<Character> _characters);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Character> _characterCollection;
        
        public DatabaseService(IOptions<DatabaseAccessData> _accessData)
        {
            var settings = MongoClientSettings.FromConnectionString(_accessData.Value.ConnectionString);
            // Set the ServerApi field of the settings object to set the version of the Stable API on the client
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            var client = new MongoClient(settings);
            // Send a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //Get the database from the client
            _database = client.GetDatabase(_accessData.Value.DatabaseName);
            //Get the character collection reference from the database
            _characterCollection = _database.GetCollection<Character>(_accessData.Value.CharacterCollectionName);
        }

        public async Task<Character> GetCharacterAsync(ObjectId _id) =>
            await _characterCollection.Find(x => x.Id == _id).FirstOrDefaultAsync();

        public async Task<Character> GetCharacterAsync(string _name) =>
            await _characterCollection.Find(x => x.Name == _name).FirstOrDefaultAsync();

        public async Task<List<Character>> GetCharactersAsync() =>
            await _characterCollection.Find(Builders<Character>.Filter.Empty).ToListAsync();

        public async Task UpdateCharacters(List<Character> characters)
        {
            var df = new List<WriteModel<Character>>();

            foreach (var character in characters)
            {
                df.Add(new ReplaceOneModel<Character>(
                    Builders<Character>.Filter.Where(x => x.Id == character.Id), character));
            }


            await _characterCollection.BulkWriteAsync(df);
        }


        public async Task<Character> CreateCharacter(NewCharacterData _character)
        {
            Character newCharacter = new Character(_character.Name);
            await _characterCollection.InsertOneAsync(newCharacter);
            return newCharacter;
        }
            
        public async Task UpdateCharacter(Character _updatedCharacter) =>
            await _characterCollection.ReplaceOneAsync(x => x.Id == _updatedCharacter.Id, _updatedCharacter);

    }
}
