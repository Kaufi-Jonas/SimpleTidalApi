using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TidalLib;

namespace TidalLibTests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public async Task SimpleSearch()
        {
            var client = new TidalClient();
            var song = await client.Search("Mombasa - Hans Zimmer");
            Assert.IsNull(song.Item1);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(song.Item2.Tracks[0].Url));
        }
    }
} 
