namespace AutoPick.ViewModels
{
    using System;
    using AutoPick.Runes;

    public class RuneTreeViewModel : ViewModelBase
    {
        private readonly IRuneSelector _runeSelector;

        public RuneTreeViewModel(RuneType runePath, IRuneSelector runeSelector)
        {
            _runeSelector = runeSelector;
            RunePath = runePath;
        }

        public RuneType RunePath { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public Action Select => () =>
        {
            if (_isSelected)
            {
                _runeSelector.PickPrimaryKeyStone(RunePath);
            }
            else
            {
                _runeSelector.PickSecondaryKeyStone(RunePath);
            }

            IsSelected = true;
        };

        public Action<Rune> SelectSmall => (rune) =>
        {
            _runeSelector.PickRune(rune);
        };
    }
}