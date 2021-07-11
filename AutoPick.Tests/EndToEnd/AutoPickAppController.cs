namespace AutoPick.Tests.EndToEnd
{
    using System.Text;
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;

    public class AutoPickAppController
    {
        private readonly TestProcessAndThreadManager _processManager;

        private AppCommunicator _appCommunicator;

        public AutoPickAppController(TestProcessAndThreadManager processManager)
        {
            _processManager = processManager;
        }

        public async Task Start()
        {
            _processManager.Start("AutoPick.exe");
            _appCommunicator = new AppCommunicator();
            await _appCommunicator.Connect("localhost", 5556);
        }

        public async Task<State> GetState()
        {
            return (State)(await _appCommunicator.Send(0))[0];
        }

        public Task SetLane(string value)
        {
            return _appCommunicator.Send(1, Encoding.Unicode.GetBytes(value));
        }

        public Task SetChampion(string value)
        {
            return _appCommunicator.Send(2, Encoding.Unicode.GetBytes(value));
        }
    }
}