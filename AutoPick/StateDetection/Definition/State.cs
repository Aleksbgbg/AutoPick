namespace AutoPick.StateDetection.Definition
{
    // Thresholds should be above 0.75 where possible to prevent incorrect state detections.
    // Thresholds above 0.9 are good, and above 0.95 are ideal. The default is best.
    // Thresholds between 0.75 and 0.9 are allowed but discouraged.
    // Thresholds should be as high as possible to 2dp.
    [CombinedDetectors(Pick, Selected)]
    public enum State
    {
        [PollingRate(PollingRates.Slow)]
        Idle = 0,

        [PollingRate(PollingRates.Slow)]
        [Detector(SearchAlgorithm.Convolution, "Lobby.png", 441, 680, 10, 10, Threshold = 0.51f)]
        Lobby,

        [ZOrder(4)]
        [PollingRate(PollingRates.Fast)]
        // Changed location to aid detection when window resized; original = 507, 680, 66, 10
        [Detector(SearchAlgorithm.Convolution, "Queue.png", 500, 675, 80, 20, Threshold = 0.76f)]
        Queue,

        [ZOrder(3)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "Accept.png", 646, 383, 107, 20, Threshold = 0.94f)]
        Accept,

        [ZOrder(2)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "Accepted.png", 619, 383, 119, 20, Threshold = 0.94f)]
        Accepted,

        [ZOrder(2)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "Declined.png", 623, 383, 129, 20, Threshold = 0.94f)]
        Declined,

        [ZOrder(1)]
        [PollingRate(PollingRates.VeryFast)]
        // Changed location to aid detection when window resized; original = 1135, 52, 92, 12
        [Detector(SearchAlgorithm.Convolution, "ChampSelectTransition.png", 1131, 48, 100, 20, Threshold = 0.8f)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "ChampSelectTransitionBlank.png", 640, 360, 20, 20)]
        ChampSelectTransition,

        [ZOrder(-1)]
        [PollingRate(PollingRates.VeryFast)]
        // Changed location to aid detection when window resized; original = 32, 644, 72, 10
        [Detector(SearchAlgorithm.Convolution, "Connecting.png", 28, 640, 80, 20, Threshold = 0.65f)]
        // Connecting screen before fonts have loaded - this is a rare case.
        // It is here purely for heuristic reasons; it is not guaranteed to work or be supported, but this is okay
        // because it doesn't interfere with normal operation.
        [Detector(SearchAlgorithm.Convolution, "ConnectingEarly.png", 32, 644, 67, 12)]
        Connecting,

        [ZOrder(0)]
        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Pick.png", 555, 588, 172, 42, Threshold = 0.83f)]
        Pick,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Selected.png", 555, 588, 172, 42, Threshold = 0.79f)]
        Selected,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Locked.png", 833, 675, 18, 18, Threshold = 0.56f)]
        Locked,

        [PollingRate(PollingRates.VerySlow)]
        // Changed location to aid detection when window resized; original = 1135, 52, 61, 11
        [Detector(SearchAlgorithm.Convolution, "InGame.png", 1130, 47, 72, 22, Threshold = 0.79f)]
        InGame,

        [PollingRate(PollingRates.Slow)]
        NotLaunched,

        [PollingRate(PollingRates.Fast)]
        Minimised,

        [PollingRate(PollingRates.Fast)]
        Disabled,

        [PollingRate(PollingRates.Fast)]
        InvalidWindowSize,
    }
}