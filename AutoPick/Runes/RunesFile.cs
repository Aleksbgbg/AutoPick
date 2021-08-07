namespace AutoPick.Runes
{
    using AutoPick.Persistence;

    public class RunesFile
    {
        [FieldIndex(0)]
        public RuneType[] Runes { get; set; }
    }
}