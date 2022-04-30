namespace AutoPick.Runes
{
    using System.Collections.Generic;

    public interface IRuneSelection
    {
        public RuneType PrimaryRune { get; }

        public RuneType SecondaryRune { get; }

        public IEnumerable<Rune> SelectedRunes { get; }
    }
}