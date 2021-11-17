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

        public GameFactory(string sceneName = "scene_game")
        {
            var scenesToRemove = new HashSet<Scene>();
            foreach (var scene in ScenesToUnload)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);

                while (!operation.isDone) { }

                scenesToRemove.Add(scene);
            }

            foreach (var sceneToRemove in scenesToRemove) ScenesToUnload.Remove(sceneToRemove);

            var newScene = SceneManager.GetSceneByName(sceneName);

            if (!newScene.IsValid())
            {
                newScene = SceneManager.CreateScene(sceneName);
                SceneManager.SetActiveScene(newScene);
                this.Log($"Game scene with name {sceneName} was created.");
            }
            else SceneManager.SetActiveScene(newScene);

            ScenesToUnload.Add(newScene);

            SceneManager.MoveGameObjectToScene(GameObject, newScene);
        }

        public override GameObject Create(Game game)
        {
            GameObject.name = "game";

            var gameComponent = GameObject.AddComponent<GameComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}