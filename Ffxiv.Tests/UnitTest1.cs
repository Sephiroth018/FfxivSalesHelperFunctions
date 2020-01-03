using System.Threading.Tasks;
using Ffxiv.Common;
using Ffxiv.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace Ffxiv.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void Test1()
        {
            var configMock = Substitute.For<Config>();
            configMock.FfxivBaseUrl.Returns("https://xivapi.com/");
            configMock.FfxivApiKey.Returns("bf9ad1ee4e944e8f91c4dee7c570e15a55a9076acdc14c18bec6fd61c1c27ddc");

            var configWrapperMock = Substitute.For<IOptions<Config>>();
            configWrapperMock.Value.Returns(configMock);

            var service = new FfxivService(configWrapperMock);
            Parallel.For(28300,
                         28400,
                         (i, state) =>
                         {
                             var result = service.GetItem(i.ToString()).Result;
                             Assert.AreEqual(result.ID, i);
                             Assert.IsNotEmpty(result.Url, $"ID {i} has an empty url property");
                         });
        }
    }
}