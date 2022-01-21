using System.Collections.Generic;
using GuldeLib.TypeObjects;
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

        public GameFactory(Game game) : base(game, null, null)
        {
        }

        public override GameObject Create()
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

            var newScene = SceneManager.GetSceneByName(TypeObject.SceneName);

            if (!newScene.IsValid())
            {
                newScene = SceneManager.CreateScene(TypeObject.SceneName);
                SceneManager.SetActiveScene(newScene);
                this.Log($"Game scene with name {TypeObject.SceneName} was created.");
            }
            else SceneManager.SetActiveScene(newScene);

            ScenesToUnload.Add(newScene);

            SceneManager.MoveGameObjectToScene(GameObject, newScene);

            GameObject.name = "game";

            var cityFactory = new CityFactory(TypeObject.City.Value, GameObject);
            cityFactory.Create();

            return GameObject;
        }

         
    }
}