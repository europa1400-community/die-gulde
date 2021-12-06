using System.Collections.Generic;
using MonoLogger.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GuldeLib.Factories
{
    public class GameFactory : Factory<Game>
    {
        /// <summary>
        /// Gets the hash set of previously loaded game scenes to unload before building a new game.
        /// </summary>
        static HashSet<Scene> ScenesToUnload { get; } = new HashSet<Scene>();

        public GameFactory() : base(null, null)
        {
        }

        public override GameObject Create(Game game)
        {
            var scenesToRemove = new HashSet<Scene>();
            foreach (var scene in ScenesToUnload)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);

                while (!operation.isDone)
                {
                }

                scenesToRemove.Add(scene);
            }

            foreach (var sceneToRemove in scenesToRemove) ScenesToUnload.Remove(sceneToRemove);

            var newScene = SceneManager.GetSceneByName(game.SceneName);

            if (!newScene.IsValid())
            {
                newScene = SceneManager.CreateScene(game.SceneName);
                SceneManager.SetActiveScene(newScene);
                this.Log($"Game scene with name {game.SceneName} was created.");
            }
            else SceneManager.SetActiveScene(newScene);

            ScenesToUnload.Add(newScene);

            SceneManager.MoveGameObjectToScene(GameObject, newScene);

            GameObject.name = "game";

            var cityFactory = new CityFactory(GameObject);
            cityFactory.Create(game.City.Value);

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}