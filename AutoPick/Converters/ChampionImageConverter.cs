namespace AutoPick.Converters
{
    public class ChampionImageConverter : OneWayConverterBase<Champion?>
    {
        private readonly ChampionStore _championStore;

        public ChampionImageConverter(ChampionStore championStore)
        {
            _championStore = championStore;
        }

        private protected override object? ConvertValue(Champion? value)
        {
            if (value == null)
            {
                return null;
            }

            return _championStore.ImageForChampion(value);
        }
    }
}