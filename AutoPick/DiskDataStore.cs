﻿namespace AutoPick
{
    using System.IO;
    using AutoPick.Persistence;
    using AutoPick.ViewModels;

    public class DiskDataStore
    {
        private class Data
        {
            [FieldIndex(0)]
            public string? ChampionText { get; set; }

            [FieldIndex(2)]
            public Lane? SelectedLane { get; set; }
        }

        private const string Filename = "AutoPickData.bin";

        private readonly BinaryReadWriter<Data> _binaryReadWriter = new();

        private readonly MainViewModel _mainViewModel;

        public DiskDataStore(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public void Load()
        {
            using Stream stream = GetReadingStream();
            Data data = _binaryReadWriter.Deserialize(stream);

            _mainViewModel.ChampText = data.ChampionText ?? "Katarina";
            _mainViewModel.SelectedLane = data.SelectedLane ?? Lane.Mid;
        }

        public void Save()
        {
            using Stream stream = File.Open(Filename, FileMode.Create);
            Data data = new()
            {
                ChampionText = _mainViewModel.ChampText,
                SelectedLane = _mainViewModel.SelectedLane
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