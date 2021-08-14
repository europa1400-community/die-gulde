using UnityEngine;

namespace Gulde.Utilities
{
    public class EditorUpdateComponent : MonoBehaviour
    {
        void OnDrawGizmos()
        {
#if UNITY_EDITOR

            if (Application.isPlaying) return;
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();

#endif
        }
    }
}