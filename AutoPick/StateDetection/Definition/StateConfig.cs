namespace AutoPick.StateDetection.Definition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class StateConfig
    {
        private const int DefaultZOrder = int.MaxValue / 2;

        public StateConfig()
        {
            StateMemberInfo[] perStateMemberInfo = typeof(State).GetMembers(BindingFlags.Static | BindingFlags.Public)
                                                                .Zip(Enum.GetValues<State>(),
                                                                     (memberInfo, state) =>
                                                                         new StateMemberInfo(state, memberInfo))
                                                                .ToArray();

            PollingRates = new Dictionary<State, int>(perStateMemberInfo.Length);
            StateInfoDisplays = new StateInfoDisplay[perStateMemberInfo.Length];

            for (int stateIndex = 0; stateIndex < perStateMemberInfo.Length; ++stateIndex)
            {
                StateMemberInfo stateMemberInfo = perStateMemberInfo[stateIndex];

                LoadPollingRate(stateMemberInfo);
                LoadStateInfoDisplay(stateIndex, stateMemberInfo);
                LoadDetectors(stateMemberInfo);
            }

            CombinedDetectors = GetManyAttributes<State, CombinedDetectorsAttribute>();
        }

        public Dictionary<State, int> PollingRates { get; } = new();

        public StateInfoDisplay[] StateInfoDisplays { get; }

        public CombinedDetectorsAttribute[] CombinedDetectors { get; }

        public List<StateMatcher> StateMatchers { get; } = new();

        private void LoadPollingRate(StateMemberInfo stateMemberInfo)
        {
            PollingRates.Add(stateMemberInfo.State, GetAttribute<PollingRateAttribute>(stateMemberInfo.MemberInfo).PollingRateMs);
        }

        private void LoadStateInfoDisplay(int stateIndex, StateMemberInfo stateMemberInfo)
        {
            StateInfoDisplays[stateIndex] = new StateInfoDisplay(stateMemberInfo.State,
                                                                 GetAttribute<InfoDisplayAttribute>(
                                                                     stateMemberInfo.MemberInfo));
        }

        private void LoadDetectors(StateMemberInfo stateMemberInfo)
        {
            DetectorAttribute[] detectorAttributes = GetManyAttributes<DetectorAttribute>(stateMemberInfo.MemberInfo);

            if (detectorAttributes.Length > 0)
            {
                Detector[] detectorArray = new Detector[detectorAttributes.Length];

                for (int detectorIndex = 0; detectorIndex < detectorArray.Length; detectorIndex++)
                {
                    detectorArray[detectorIndex] = new Detector(detectorAttributes[detectorIndex]);
                }

                ZOrderAttribute? importanceAttribute = GetAttributeOrNull<ZOrderAttribute>(stateMemberInfo.MemberInfo);
                int zOrder = importanceAttribute?.ZOrder ?? DefaultZOrder;

                StateMatchers.Add(new StateMatcher(zOrder, stateMemberInfo.State, detectorArray));
            }
        }

        private static T GetAttribute<T>(MemberInfo enumValue) where T : Attribute
        {
            T? attribute = (T?)enumValue.GetCustomAttribute(typeof(T));

            if (attribute == null)
            {
                throw new InvalidOperationException($"Null {typeof(T).Name} for {enumValue.Name}.");
            }

            return attribute;
        }

        private static T? GetAttributeOrNull<T>(MemberInfo enumValue) where T : Attribute
        {
            return (T?)enumValue.GetCustomAttribute(typeof(T));
        }

        private static TAttribute[] GetManyAttributes<TTargetType, TAttribute>() where TAttribute : Attribute
        {
            return (TAttribute[])typeof(TTargetType).GetCustomAttributes(typeof(TAttribute));
        }

        private static T[] GetManyAttributes<T>(MemberInfo enumValue) where T : Attribute
        {
            return (T[])enumValue.GetCustomAttributes(typeof(T));
        }

        private class StateMemberInfo
        {
            public StateMemberInfo(State state, MemberInfo memberInfo)
            {
                State = state;
                MemberInfo = memberInfo;
            }

            public State State { get; }

            public MemberInfo MemberInfo { get; }
        }
    }
}