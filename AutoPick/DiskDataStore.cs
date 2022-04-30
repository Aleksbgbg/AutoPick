namespace AutoPick
{
    using System.IO;
    using AutoPick.Persistence;
    using AutoPick.ViewModels;

    public class DiskDataStore
    {
        private class Data
        {
            [FieldIndex(0)]
            [Encoding(EncodingType.Utf16)]
            public string? SelectedChampionName { get; set; }

            [FieldIndex(2)]
            public Lane? SelectedLane { get; set; }

            [FieldIndex(3)]
            public ViewLayout? ViewLayout { get; set; }
        }

        private const string Filename = "AutoPickData.bin";

        private readonly BinaryReadWriter<Data> _binaryReadWriter = new();

        private readonly MainViewModel _mainViewModel;

        private readonly ChampionStore _championStore;

        public DiskDataStore(MainViewModel mainViewModel, ChampionStore championStore)
        {
            _mainViewModel = mainViewModel;
            _championStore = championStore;
        }

        public void Load()
        {
            using Stream stream = GetReadingStream();
            Data data = _binaryReadWriter.Deserialize(stream);

            if ((data.SelectedLane < Lane.Top) || (data.SelectedLane > Lane.Support))
            {
                data.SelectedLane = Lane.Mid;
            }

            if ((data.SelectedChampionName == null) || !_championStore.ChampionExists(data.SelectedChampionName))
            {
                data.SelectedChampionName = "Katarina";
            }

            if ((data.ViewLayout < ViewLayout.Tiny) || (data.ViewLayout > ViewLayout.Comfortable))
            {
                data.ViewLayout = ViewLayout.Tiny;
            }

            _mainViewModel.SelectedChampion = _championStore.ChampionByName(data.SelectedChampionName);
            _mainViewModel.SelectedLane = data.SelectedLane ?? Lane.Mid;
            _mainViewModel.ViewLayout = data.ViewLayout ?? ViewLayout.Tiny;
        }

        public void Save()
        {
            using Stream stream = File.Open(Filename, FileMode.Create);
            Data data = new()
            {
                SelectedChampionName = _mainViewModel.SelectedChampion.Name,
                SelectedLane = _mainViewModel.SelectedLane,
                ViewLayout = _mainViewModel.ViewLayout
            };
            _binaryReadWriter.Serialize(data, stream);
        }

        private static Stream GetReadingStream()
        {
            if (File.Exists(Filename))
            {
                return File.OpenRead(Filename);
            }

            return File.Create(Filename);
        }
    }
}