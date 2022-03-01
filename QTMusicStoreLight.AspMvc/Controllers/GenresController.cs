#nullable disable

namespace QTMusicStoreLight.AspMvc.Controllers
{
    public class GenresController : GenericController<Logic.Entities.Genre, Models.Genre>
    {
        public GenresController(Logic.Controllers.GenresController controller) : base(controller)
        {
        }
    }
}
