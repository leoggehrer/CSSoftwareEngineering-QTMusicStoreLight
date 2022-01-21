//@CodeCopy
//MdStart

namespace QTMusicStoreLight.Logic.Entities
{
    public abstract partial class VersionObject : IdentityObject
    {
        /// <summary>
        /// Row version of the entity.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; internal set; }
    }
}
//MdEnd
