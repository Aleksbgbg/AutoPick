namespace AutoPick
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class SelfExtractingChampionLoader : IChampionLoader
    {
        public Task<byte[]> GetChecksum()
        {
            return Task.FromResult(
                new byte[] { 0x5C, 0xF2, 0xEA, 0x35, 0x95, 0xEC, 0xA9, 0xBD, 0xCC, 0x73, 0x46, 0x7B, 0xC9, 0x32, 0xC8, 0x9A, 0x09, 0x8A, 0xF7, 0xF9, 0x55, 0x15, 0x5D, 0xC7, 0xAB, 0x99, 0xF8, 0x3A, 0x56, 0xD7, 0x5F, 0xE1 });
        }

        public Task<string[]> GetChampionNames()
        {
            int startTrim = "AutoPick.Images.Champions.".Length;
            int endTrim = ".png".Length;

            return Task.FromResult(
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceNames()
                        .Where(file => file.Contains("Images.Champions"))
                        .Select(file =>
                                    file.Substring(startTrim, file.Length - startTrim - endTrim))
                        .ToArray());
        }

        public Task<Stream> GetChampionImage(string championName)
        {
            return Task.FromResult(
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream($"AutoPick.Images.Champions.{championName}.png")!);
        }
    }
}