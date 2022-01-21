using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QTMusicStoreLight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : GenericController<Logic.Entities.Artist, Models.Artist>
    {
        public ArtistController() : base(new Logic.Controllers.ArtistsController())
        {
        }

        // POST api/<ArtistController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ArtistController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ArtistController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
