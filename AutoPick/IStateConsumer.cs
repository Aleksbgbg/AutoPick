namespace AutoPick
{
    using AutoPick.StateDetection.Definition;

    public interface IStateConsumer
    {
        void Consume(State state);
    }
}