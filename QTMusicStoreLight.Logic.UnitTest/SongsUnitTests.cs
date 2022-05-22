using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTMusicStoreLight.Logic.UnitTest
{
    [TestClass]
    public class SongsUnitTests : EntityUnitTest<Entities.App.Song>
    {
        public override Controllers.GenericController<Entities.App.Song> CreateController()
        {
            return new Controllers.SongsController();
        }
    }
}
