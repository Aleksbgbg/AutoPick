namespace AutoPick.Runes
{
    using System.IO;
    using AutoPick.Persistence;

    public class RunesConfig
    {
        public RunesConfig(AssemblyDataReader assemblyDataReader)
        {
            Stream stream = assemblyDataReader.Read("AutoPick.Data.Runes.bin");
            RunesFile runesFile = new BinaryReadWriter<RunesFile>().Deserialize(stream);
            RuneTypes = runesFile.Runes;
        }

        public RuneType[] RuneTypes { get; }
    }
}