namespace AutoPick.Runes
{
    using System.Collections.Generic;

    public class SelectedRunesStore : IRuneSelection, IRuneSelector
    {
        private readonly List<Rune> _selectedRunes = new();

        public RuneType PrimaryRune { get; private set; }

        public RuneType SecondaryRune { get; private set; }

        public IEnumerable<Rune> SelectedRunes => _selectedRunes;

        public void PickPrimaryKeyStone(RuneType keystone)
        {
            PrimaryRune = keystone;
        }

        public void PickSecondaryKeyStone(RuneType keystone)
        {
            SecondaryRune = keystone;
        }

        public void PickRune(Rune rune)
        {
            _selectedRunes.Add(rune);
        }
    }
}