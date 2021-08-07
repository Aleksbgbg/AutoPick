namespace AutoPick.Runes
{
    using AutoPick.Persistence;

    public class RuneSlot
    {
        [FieldIndex(0)]
        public Rune[] Runes { get; set; }
    }
}