using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gulde.Logging
{
    public static class Logger
    {
        static Dictionary<MonoBehaviour, LogType> BehaviourToLogLevel { get; } =
            new Dictionary<MonoBehaviour, LogType>();

        public static LogType DefaultLogLevel { get; set; } = LogType.Error;

        public static void SetLogLevel(this MonoBehaviour behaviour, LogType logType)
        {
#if UNITY_EDITOR

            if (!BehaviourToLogLevel.ContainsKey(behaviour)) BehaviourToLogLevel.Add(behaviour, logType);
            else BehaviourToLogLevel[behaviour] = logType;

#endif
        }

        public static void Log(this MonoBehaviour behaviour, object message, LogType logType = LogType.Log)
        {
#if UNITY_EDITOR

            if (!BehaviourToLogLevel.ContainsKey(behaviour)) BehaviourToLogLevel.Add(behaviour, DefaultLogLevel);

            switch (logType)
            {
                case LogType.Log:

                    if (BehaviourToLogLevel[behaviour] == LogType.Exception ||
                        BehaviourToLogLevel[behaviour] == LogType.Error ||
                        BehaviourToLogLevel[behaviour] == LogType.Warning ||
                        BehaviourToLogLevel[behaviour] == LogType.Assert) return;

                    Debug.Log($"{behaviour} - {message}", behaviour);

                    break;
                case LogType.Assert:

                    if (BehaviourToLogLevel[behaviour] == LogType.Exception ||
                        BehaviourToLogLevel[behaviour] == LogType.Error ||
                        BehaviourToLogLevel[behaviour] == LogType.Warning) return;

                    Debug.LogAssertion($"{behaviour} - {message}", behaviour);

                    break;
                case LogType.Warning:

                    if (BehaviourToLogLevel[behaviour] == LogType.Exception ||
                        BehaviourToLogLevel[behaviour] == LogType.Error) return;

                    Debug.LogWarning($"{behaviour} - {message}", behaviour);

                    break;
                case LogType.Error:

                    if (BehaviourToLogLevel[behaviour] == LogType.Exception) return;

                    Debug.LogError($"{behaviour} - {message}", behaviour);

                    break;
                case LogType.Exception:

                    if (!(message is Exception exception)) return;

                    Debug.LogException(exception, behaviour);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);

#endif
            }
        }
    }
}
