namespace AutoPick.Runes
{
    using AutoPick.Persistence;

    public class Rune
    {
        [FieldIndex(0)]
        public int? Id { get; set; }

        [FieldIndex(1)]
        public string Name { get; set; }

        [FieldIndex(2)]
        public string Icon { get; set; }

        [FieldIndex(3)]
        public string Description { get; set; }
    }
}