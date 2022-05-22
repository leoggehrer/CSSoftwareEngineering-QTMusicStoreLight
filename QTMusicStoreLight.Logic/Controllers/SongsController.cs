namespace QTMusicStoreLight.Logic.Controllers
{
    public sealed class SongsController : GenericController<Entities.App.Song>
    {
        public SongsController() : base()
        {
        }

        public SongsController(ControllerObject other) : base(other)
        {
        }

        protected override void ValidateEntity(ActionType actionType, Entities.App.Song entity)
        {
            if (entity.Millisconds <= 1000)
            {
                throw new Modules.Exceptions.LogicException($"The value of '{nameof(entity.Millisconds)}' must be greater than 1000.");
            }
            if (entity.Bytes <= 100)
            {
                throw new Modules.Exceptions.LogicException($"The value of '{nameof(entity.Bytes)}' must be greater than 100.");
            }
            if (entity.UnitPrice < 0)
            {
                throw new Modules.Exceptions.LogicException($"The value of '{nameof(entity.UnitPrice)}' cannot be less than 0.");
            }
            base.ValidateEntity(actionType, entity);
        }
    }
}
