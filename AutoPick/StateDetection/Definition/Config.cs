namespace AutoPick.StateDetection.Definition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Config
    {
        public Config()
        {
            foreach (var stateMetadata in typeof(State).GetMembers(BindingFlags.Static | BindingFlags.Public)
                                                       .Zip(Enum.GetValues<State>(),
                                                            (memberInfo, state) => new
                                                            {
                                                                State = state,
                                                                MemberInfo = memberInfo
                                                            }))
            {
                PollingRateAttribute? pollingRateAttribute = GetAttribute<PollingRateAttribute>(stateMetadata.MemberInfo);

                if (pollingRateAttribute == null)
                {
                    throw new InvalidOperationException("No polling rate for state");
                }

                RefreshRates.Add(stateMetadata.State, pollingRateAttribute.PollingRateMs);

                DetectorAttribute[] detectorAttributes = GetManyAttributes<DetectorAttribute>(stateMetadata.MemberInfo);

                if (detectorAttributes.Length > 0)
                {
                    Detector[] detectorArray = new Detector[detectorAttributes.Length];

                    for (int index = 0; index < detectorArray.Length; index++)
                    {
                        detectorArray[index] = new Detector(detectorAttributes[index]);
                    }

                    ZOrderAttribute? importanceAttribute = GetAttribute<ZOrderAttribute>(stateMetadata.MemberInfo);
                    int zOrder = importanceAttribute?.ZOrder ?? int.MaxValue;

                    StateMatchers.Add(new StateMatcher(zOrder, stateMetadata.State, detectorArray));
                }
            }
        }

        public Dictionary<State, int> RefreshRates { get; } = new();

        public List<StateMatcher> StateMatchers { get; } = new();

        private static T? GetAttribute<T>(MemberInfo enumValue) where T : Attribute
        {
            return (T?)enumValue.GetCustomAttribute(typeof(T));
        }

        private static T[] GetManyAttributes<T>(MemberInfo enumValue) where T : Attribute
        {
            return (T[])enumValue.GetCustomAttributes(typeof(T));
        }
    }
}