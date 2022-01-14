namespace QTMusicStoreLight.Logic.Controllers
{
    public sealed class ArtistsController : GenericController<Entities.Artist>
    {
        public ArtistsController() : base()
        {
        }

        public ArtistsController(ControllerObject other) : base(other)
        {
        }
    }
}
