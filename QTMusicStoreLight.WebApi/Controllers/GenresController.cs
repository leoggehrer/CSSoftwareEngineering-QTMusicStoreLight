using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QTMusicStoreLight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : GenericController<Logic.Entities.Genre, Models.GenreEdit, Models.Genre>
    {
        public GenresController(Logic.Controllers.GenresController artistsController)
            : base(artistsController)
        {

        }
    }
}
