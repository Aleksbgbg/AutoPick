namespace AutoPick.Execution
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;

    public class PerStateActionExecutor
    {
        private readonly Dictionary<State, ActionExecutor> _actionsPerState = new();

        private State _lastState;

        public void RegisterAction(State state, ActionExecutor actionExecutor)
        {
            _actionsPerState[state] = actionExecutor;
        }

        public async Task ExecuteAction(State currentState, ILeagueClientManipulator clientManipulator)
        {
            if (_actionsPerState.ContainsKey(currentState))
            {
                await _actionsPerState[currentState].Execute(_lastState, currentState, clientManipulator);
            }

            _lastState = currentState;
        }
    }
}