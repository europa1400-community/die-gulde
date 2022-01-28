using GuldeLib.Factories;
using GuldeLib.Generators;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib
{
    public class GameComponent : SerializedMonoBehaviour
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        GeneratableGame Game { get; set; }

        [OdinSerialize]
        LogType DefaultLogType { get; set; }

        void Awake()
        {
            MonoLogger.Runtime.MonoLogger.DefaultLogLevel = DefaultLogType;

            this.Log($"Generating game data.");
            Game.Generate();

            this.Log("Creating game objects.");
            var gameFactory = new GameFactory(Game.Value);
            gameFactory.Create();
        }
    }
}