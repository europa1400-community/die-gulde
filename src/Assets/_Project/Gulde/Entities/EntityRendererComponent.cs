using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    [HideMonoScript]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(EntityComponent))]
    public class EntityRendererComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        SpriteRenderer SpriteRenderer { get; set; }

        [OdinSerialize]
        [ReadOnly]
        EntityComponent EntityComponent { get; set; }

        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            EntityComponent = GetComponent<EntityComponent>();
        }

        public void SetVisible(bool isVisible)
        {
            SpriteRenderer.enabled = isVisible;
        }

        #region OdinInspector

        void OnValidate()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            EntityComponent = GetComponent<EntityComponent>();
        }

        #endregion
    }
}