namespace AutoPick.ViewModels
{
    using System;
    using AutoPick.Runes;

    public class RuneTreeViewModel : ViewModelBase
    {
        public RuneTreeViewModel(RunePath runePath)
        {
            RunePath = runePath;
            _isSelected = runePath == RunePath.Sorcery;
        }

        public RunePath RunePath { get; }

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

        public Action Select => () => IsSelected = true;
    }
}