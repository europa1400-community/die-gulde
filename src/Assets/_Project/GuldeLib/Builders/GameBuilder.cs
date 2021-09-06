using System.Collections;
using System.Collections.Generic;
using MonoLogger.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GuldeLib.Builders
{
    public class GameBuilder : Builder
    {
        CityBuilder CityBuilder { get; set; }
        string SceneName { get; set; } = "game";

        static HashSet<Scene> ScenesToUnload { get; } = new HashSet<Scene>();

        public GameBuilder()
        {

        }

        public GameBuilder WithSceneName(string sceneName)
        {
            SceneName = sceneName;

            return this;
        }

        public GameBuilder WithCity(CityBuilder cityBuilder)
        {
            CityBuilder = cityBuilder;

            return this;
        }

        public GameBuilder WithTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;

            return this;
        }

        public override IEnumerator Build()
        {
            yield return base.Build();

            var scenesToRemove = new HashSet<Scene>();
            foreach (var scene in ScenesToUnload)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
                scenesToRemove.Add(scene);
            }

            foreach (var sceneToRemove in scenesToRemove) ScenesToUnload.Remove(sceneToRemove);
            
            if (CityBuilder == null)
            {
                this.Log("Game can not be created without a city.", LogType.Error);
                yield break;
            }

            var newScene = SceneManager.GetSceneByName(SceneName);

            if (!newScene.IsValid())
            {
                this.Log($"Game scene with name {SceneName} created.");
                newScene = SceneManager.CreateScene(SceneName);
                SceneManager.SetActiveScene(newScene);
            }

            SceneManager.SetActiveScene(newScene);

            ScenesToUnload.Add(newScene);

            yield return CityBuilder.Build();
        }
    }
}