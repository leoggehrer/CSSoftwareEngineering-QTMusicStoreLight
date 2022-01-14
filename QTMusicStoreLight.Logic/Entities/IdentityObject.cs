//@CodeCopy
//MdStart
using System.ComponentModel.DataAnnotations;

namespace QTMusicStoreLight.Logic.Entities
{
    public abstract partial class IdentityObject
    {
        /// <summary>
        /// ID of the entity (primary key)
        /// </summary>
        [Key]
        public int Id { get; set; }
    }
}
//MdEnd
