namespace AutoPick.Converters
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoPick.StateDetection.Definition;

    public class InfoTextConverter : OneWayConverterBase<State>
    {
        private readonly Dictionary<State, string> _infoTextPerState;

        public InfoTextConverter(StateInfoDisplay[] infoDisplays)
        {
            _infoTextPerState = infoDisplays.ToDictionary(
                display => display.State,
                display => display.InfoText);
        }

        private protected override object ConvertValue(State value)
        {
            return _infoTextPerState[value];
        }
    }
}