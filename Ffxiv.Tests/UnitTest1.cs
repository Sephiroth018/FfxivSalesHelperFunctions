using System.Threading.Tasks;
using Ffxiv.Services;
using NSubstitute;
using NUnit.Framework;

namespace Ffxiv.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var configMock = Substitute.For<IConfig>();
            configMock.FfxivBaseUrl.Returns("https://xivapi.com/");
            configMock.FfxivApiKey.Returns("bf9ad1ee4e944e8f91c4dee7c570e15a55a9076acdc14c18bec6fd61c1c27ddc");
            var service = new FfxivService(configMock);
            Parallel.For(28300, 28400, (i, state) =>
                                {
                                    var result = service.GetItem(i.ToString()).Result;
                                    Assert.AreEqual(result.ID, i);
                                    Assert.IsNotEmpty(result.Url, $"ID {i} has an empty url property");
                                });
        }
    }
}