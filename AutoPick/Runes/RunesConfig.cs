namespace AutoPick.Runes
{
    using System.IO;
    using System.Text.Json;

    public class RunesConfig
    {
        public RunesConfig(AssemblyDataReader assemblyDataReader)
        {
            Stream stream = assemblyDataReader.Read("Data.Runes.json");
        }

        public RuneType[] RuneTypes { get; }
    }
}