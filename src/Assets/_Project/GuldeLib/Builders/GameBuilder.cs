using System.Collections;
using System.Collections.Generic;
using MonoLogger.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Provides functionality to build a game.
    /// </summary>
    public class GameBuilder : Builder
    {
        /// <summary>
        /// Gets or sets the <see cref = "CityBuilder">CityBuilder</see> used to build the city.
        /// </summary>
        CityBuilder CityBuilder { get; set; }

        /// <summary>
        /// Gets or sets the name of the scene created for the game.
        /// </summary>
        string SceneName { get; set; } = "game";

        /// <summary>
        /// Gets or sets the <see cref = "Time.timeScale">timeScale</see>
        /// </summary>
        float TimeScale { get; set; } = 1f;

        /// <summary>
        /// Gets the hash set of previously loaded game scenes to unload before building a new game.
        /// </summary>
        static HashSet<Scene> ScenesToUnload { get; } = new HashSet<Scene>();

        /// <summary>
        /// Sets the name of the scene created for the built game.
        /// </summary>
        public GameBuilder WithSceneName(string sceneName)
        {
            SceneName = sceneName;

            return this;
        }

        /// <summary>
        /// Sets a given city to be built for the built game.
        /// </summary>
        /// <param name="cityBuilder">The <see cref = "CityBuilder">CityBuilder</see> used for building the city.</param>
        public GameBuilder WithCity(CityBuilder cityBuilder)
        {
            CityBuilder = cityBuilder;

            return this;
        }

        /// <summary>
        /// Sets the <see cref = "Time.timeScale">timeScale</see> of the built game.
        /// </summary>
        public GameBuilder WithTimeScale(float timeScale)
        {
            TimeScale = timeScale;

            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
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

            Time.timeScale = TimeScale;

            yield return CityBuilder.Build();
        }
    }
}