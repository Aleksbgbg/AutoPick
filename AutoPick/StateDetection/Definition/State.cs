namespace AutoPick.StateDetection.Definition
{
    public enum State
    {
        [PollingRate(PollingRates.Fast)]
        Idle = 0,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Lobby.png", 441, 680, 10, 10, Threshold = 0.94f)]
        Lobby,

        [ZOrder(4)]
        [PollingRate(PollingRates.Fast)]
        // Changed location to aid detection when window resized; original = 507, 680, 66, 10
        [Detector(SearchAlgorithm.Convolution, "Queue.png", 500, 675, 80, 20, Threshold = 0.76f)] // Threshold very low
        Queue,

        [ZOrder(3)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "Accept.png", 646, 383, 107, 20, Threshold = 0.96f)]
        Accept,

        [ZOrder(2)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "Accepted.png", 619, 383, 119, 20, Threshold = 0.95f)]
        Accepted,

        [ZOrder(2)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "Declined.png", 623, 383, 129, 20, Threshold = 0.96f)]
        Declined,

        [ZOrder(1)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "ChampSelectTransition.png", 1135, 52, 92, 12, Threshold = 0.88f)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "ChampSelectTransitionBlank.png", 640, 360, 20, 20)]
        ChampSelectTransition,

        [ZOrder(-1)]
        [PollingRate(PollingRates.VeryFast)]
        // Changed location to aid detection when window resized; original = 32, 644, 72, 10
        [Detector(SearchAlgorithm.Convolution, "Connecting.png", 28, 640, 80, 20, Threshold = 0.92f)]
        // Connecting screen before fonts have loaded - this is a rare case.
        // It is here purely for heuristic reasons; it is not guaranteed to work or be supported, but this is okay
        // because it doesn't interfere with normal operation.
        [Detector(SearchAlgorithm.Convolution, "ConnectingEarly.png", 32, 644, 67, 12)]
        Connecting,

        [ZOrder(0)]
        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Pick.png", 555, 588, 172, 42, Threshold = 0.94f)]
        Pick,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Selected.png", 555, 588, 172, 42, Threshold = 0.9f)]
        Selected,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Locked.png", 833, 675, 18, 18, Threshold = 0.91f)]
        Locked,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "InGame.png", 1135, 52, 61, 11, Threshold = 0.85f)] // Threshold low
        InGame,

        [PollingRate(PollingRates.Fast)]
        NotLaunched,

        [PollingRate(PollingRates.Fast)]
        Minimised,

        [PollingRate(PollingRates.Fast)]
        Disabled,

        [PollingRate(PollingRates.Fast)]
        InvalidWindowSize,
    }
}