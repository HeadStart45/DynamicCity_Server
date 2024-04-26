using DynamicCity.Services;
using DynamicCity_Models;
using DynamicCity_Models.Character;
using DynamicCity_Models.DataObjects;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;


namespace DynamicCity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ILogger<CharacterController> _logger;
        private readonly IDatabaseService _databaseService;

        public CharacterController(ILogger<CharacterController> logger, IDatabaseService dbService)
        {
            _logger = logger;
            _databaseService = dbService;
        }

        [HttpGet("{id}")]
        public async Task<SimpleCharacter> Get([FromRoute] string id)
        {
            ObjectId objId = new ObjectId(id);
            Character character = await _databaseService.GetCharacterAsync(objId);
            return new SimpleCharacter(character);
        }
        [HttpGet("{id}/location")]
        public async Task<SimpleLocation> GetLocation([FromRoute] string id)
        {
            ObjectId objId = new ObjectId(id);
            Character character = await _databaseService.GetCharacterAsync(objId);
            return new SimpleLocation(character.CurrentLocation);
        }
        [HttpGet("all")]
        public async Task<CharacterIdList> GetAllCharacterIds()
        {
            List<Character> list = await _databaseService.GetCharactersAsync();
            return new CharacterIdList(list.Select(x => x.Id.ToString()).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<Character>> CreateNewCharacter(NewCharacterData _characterData)
        {
            try
            {
                if (_characterData == null || _characterData.Name == null)
                    return BadRequest();

                Character newCharacter = await _databaseService.CreateCharacter(_characterData);
                return Ok(newCharacter);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Error creating character record: " + ex);
            }
        }
    }
}
