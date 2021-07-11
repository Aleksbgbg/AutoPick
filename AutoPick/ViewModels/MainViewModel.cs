namespace AutoPick.ViewModels
{
    using AutoPick.StateDetection.Definition;

    public class MainViewModel : ViewModelBase, IUserConfiguration, IStateConsumer
    {
        private string _champText = "Katarina";
        public string ChampText
        {
            get => _champText;

            set
            {
                if (_champText == value)
                {
                    return;
                }

                _champText = value;
                NotifyPropertyChanged();
            }
        }

        private string _laneText = "mid";
        public string LaneText
        {
            get => _laneText;

            set
            {
                if (_laneText == value)
                {
                    return;
                }

                _laneText = value;
                NotifyPropertyChanged();
            }
        }

        private State _state;
        public State State
        {
            get => _state;

            private set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                NotifyPropertyChanged();
            }
        }

        string IUserConfiguration.LaneName => LaneText;

        string IUserConfiguration.ChampionName => ChampText;

        public void Consume(State state)
        {
            State = state;
        }
    }
}