namespace AutoPick.Tests.EndToEnd
{
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;

    public class AutoPickAppController
    {
        private readonly TestProcessAndThreadManager _processManager;

        private AppCommunicator _appCommunicator;

        private Process _process;

        public AutoPickAppController(TestProcessAndThreadManager processManager)
        {
            _processManager = processManager;
        }

        public async Task Start()
        {
            _process = _processManager.Start("AutoPick.exe");
            _appCommunicator = new AppCommunicator();
            await _appCommunicator.Connect("localhost", 5556);
        }

        public async Task Shutdown()
        {
            _appCommunicator.Send(0);
            await _process.WaitForExitAsync();
        }

        public async Task<State> GetState()
        {
            return (State)(await _appCommunicator.Send(1))[0];
        }

        public Task SetLane(Lane lane)
        {
            return _appCommunicator.Send(2, new[] { (byte)lane });
        }

        public Task SetChampion(string value)
        {
            return _appCommunicator.Send(3, Encoding.Unicode.GetBytes(value));
        }

        public async Task<Lane> GetLane()
        {
            return (Lane)(await _appCommunicator.Send(4))[0];
        }

        public async Task<string> GetChampion()
        {
            return Encoding.Unicode.GetString(await _appCommunicator.Send(5));
        }
    }
}