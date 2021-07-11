namespace AutoPick.StateDetection.Definition
{
    public enum State
    {
        [PollingRate(PollingRates.Fast)]
        Idle = 0,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "Lobby.png", 441, 680, 10, 10)]
        Lobby,

        [ZOrder(4)]
        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "Queue.png", 507, 680, 66, 10)]
        Queue,

        [ZOrder(3)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "Accept.png", 646, 383, 107, 20)]
        Accept,

        [ZOrder(2)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "Accepted.png", 619, 383, 119, 20)]
        Accepted,

        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "Declined.png", 623, 383, 129, 20)]
        Declined,

        [ZOrder(1)]
        [PollingRate(PollingRates.VeryFast)]
        [Detector(SearchAlgorithm.Convolution, "ChampSelectTransition.png", 1135, 52, 92, 12)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "ChampSelectTransitionBlank.png", 640, 360, 20, 20)]
        ChampSelectTransition,

        [ZOrder(-1)]
        [PollingRate(PollingRates.VeryFast)]
        // Changed location to aid detection when window resized; original = 32, 644, 72, 10
        [Detector(SearchAlgorithm.Convolution, "Connecting.png", 28, 640, 80, 20, Threshold = 0.92f)]
        [Detector(SearchAlgorithm.Convolution, "ConnectingEarly.png", 32, 644, 67, 12)]
        Connecting,

        [ZOrder(0)]
        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "Pick.png", 612, 602, 57, 10)]
        Pick,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Selected.png", 612, 602, 57, 10)]
        Selected,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.Convolution, "Locked.png", 833, 675, 18, 18)]
        Locked,

        [PollingRate(PollingRates.Fast)]
        [Detector(SearchAlgorithm.ExactPixelMatch, "InGame.png", 1135, 52, 61, 11)]
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