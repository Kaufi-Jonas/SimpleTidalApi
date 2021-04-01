using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SimpleTidalApi;

namespace SimpleTidalApiTests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public async Task SimpleSearch()
        {
            var key = TidalClient.GetAccessTokenFromTidalDesktop();
            Assert.IsNotNull(key.Item2);
            var song = await TidalClient.Search(key.Item2, "Mombasa - Hans Zimmer");
            Assert.IsNull(song.Item1);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(song.Item2.Tracks[0].Url));
        }
    }
} 
