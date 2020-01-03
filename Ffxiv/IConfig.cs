namespace Ffxiv
{
    public interface IConfig
    {
        string FfxivApiKey { get; }

        string FfxivBaseUrl { get; }
    }
}