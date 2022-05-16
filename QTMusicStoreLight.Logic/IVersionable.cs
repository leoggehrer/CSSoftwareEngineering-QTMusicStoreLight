//@CodeCopy
//MdStart

namespace QTMusicStoreLight.Logic
{
    public partial interface IVersionable : IIdentifyable
    {
        byte[]? RowVersion { get; }
    }
}
//MdEnd
