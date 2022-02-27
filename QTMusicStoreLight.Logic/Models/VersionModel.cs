//@CodeCopy
//MdStart

namespace QTMusicStoreLight.Logic.Models
{
    public abstract partial class VersionModel : IdentityModel
    {
        /// <summary>
        /// Row version of the entity.
        /// </summary>
        public virtual byte[]? RowVersion { get; internal set; }
    }
}
//MdEnd
