using System.Collections;
using System.Collections.Generic;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Entities;
using GuldeLib.Factories;
using GuldeLib.Generators;
using GuldeLib.Maps;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace GuldeClient
{
    public class GameComponent : SerializedMonoBehaviour
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        GeneratableGame Game { get; set; }

        [OdinSerialize]
        LogType DefaultLogType { get; set; }

        GameObject GameParentObject { get; set; }

        MapHandlerComponent MapHandlerComponent { get; set; }

        Dictionary<EntityComponent, GameObject> EntityComponentToEntityHandler { get; } =
            new Dictionary<EntityComponent, GameObject>();

        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 0)
            {
                MonoLogger.Runtime.MonoLogger.DefaultLogLevel = DefaultLogType;

                this.Log($"Generating game data.");
                Game.Generate();

                this.Log("Creating game objects.");
                var gameFactory = new GameFactory(Game.Value, true);

                Time.timeScale = 0f;

                GameParentObject = gameFactory.Create();

                StartCoroutine(Initialize());
            }
        }

        IEnumerator Initialize()
        {
            var clientScene = SceneManager.GetSceneAt(0);
            SceneManager.SetActiveScene(clientScene);

            foreach (var child in GameParentObject.Children(true))
            {
                yield return InitializeGameObject(child.gameObject);
            }

            Time.timeScale = 1f;
            GameParentObject.SetActive(true);
        }

        IEnumerator InitializeGameObject(GameObject childObject)
        {
            foreach (var component in childObject.GetComponents<Component>())
            {
                yield return InitializeCoreComponent(component);
            }

            foreach (var component in childObject.GetComponents<Component>())
            {
                yield return InitializeComponent(component);
            }
        }

        IEnumerator InitializeCoreComponent(Component component)
        {
            if (component is EntityComponent entityComponent)
            {
                yield return InitializeEntityComponent(entityComponent);
            }

            if (component is MapComponent mapComponent)
            {
                yield return InitializeMapComponent(mapComponent);
            }
        }

        IEnumerator InitializeComponent(Component component)
        {
            if (component is CartComponent cartComponent)
            {
                yield return InitializeCartComponent(cartComponent);
            }

            if (component is EmployeeComponent employeeComponent)
            {
                yield return InitializeEmployeeComponent(employeeComponent);
            }
        }

        IEnumerator InitializeCartComponent(CartComponent cartComponent)
        {
            var entityComponent = cartComponent.GetComponent<EntityComponent>();
            var entityHandler = EntityComponentToEntityHandler[entityComponent];
            var spriteRenderer = entityHandler.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0.5f, 0.35f, 0.35f);
            yield break;
        }

        IEnumerator InitializeEmployeeComponent(EmployeeComponent employeeComponent)
        {
            var entityComponent = employeeComponent.GetComponent<EntityComponent>();
            var entityHandler = EntityComponentToEntityHandler[entityComponent];
            var spriteRenderer = entityHandler.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0f, 0.4f, 0.5f);
            yield break;
        }

        IEnumerator InitializeEntityComponent(EntityComponent entityComponent)
        {
            var handler = Addressables.InstantiateAsync("entity");
            yield return handler;
            Debug.Log($"HAnddler von {entityComponent.name} gespawnt");
            var entityHandlerObject = handler.Result;
            var entityHandlerComponent = entityHandlerObject.GetComponent<EntityHandlerComponent>();

            entityComponent.Initialized += entityHandlerComponent.OnInitialized;
            entityComponent.PositionChanged += entityHandlerComponent.OnPositionChanged;

            EntityComponentToEntityHandler.Add(entityComponent, entityHandlerObject);
        }

        IEnumerator InitializeMapComponent(MapComponent mapComponent)
        {
            var handler = Addressables.InstantiateAsync("map");
            yield return handler;

            var mapHandlerObject = handler.Result;
            MapHandlerComponent = mapHandlerObject.GetComponent<MapHandlerComponent>();

            mapComponent.Initialized += MapHandlerComponent.OnInitialized;
        }
    }
}