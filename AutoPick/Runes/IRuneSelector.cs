namespace AutoPick.Runes
{
    public interface IRuneSelector
    {
        void PickPrimaryKeyStone(RuneType keystone);

        void PickSecondaryKeyStone(RuneType keystone);

        void PickRune(Rune rune);
    }
}