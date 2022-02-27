//@CodeCopy
//MdStart

namespace QTMusicStoreLight.Logic.Entities
{
    public abstract partial class VersionEntity : IdentityEntity
    {
        /// <summary>
        /// Row version of the entity.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; internal set; }
    }
}
//MdEnd
