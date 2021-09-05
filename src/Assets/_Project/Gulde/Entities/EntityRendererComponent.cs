using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(EntityComponent))]
    public class EntityRendererComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        SpriteRenderer SpriteRenderer { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        EntityComponent EntityComponent { get; set; }

        void Awake()
        {
            this.Log("Entity renderer initializing");

            SpriteRenderer = GetComponent<SpriteRenderer>();
            EntityComponent = GetComponent<EntityComponent>();
        }

        public void SetVisible(bool isVisible)
        {
            this.Log($"Entity renderer becoming {(isVisible ? "visible" : "invisible")}");

            SpriteRenderer.enabled = isVisible;
        }
    }
}