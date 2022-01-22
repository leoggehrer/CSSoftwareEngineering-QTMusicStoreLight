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
    }
}
