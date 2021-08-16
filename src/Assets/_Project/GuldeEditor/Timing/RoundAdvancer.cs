using Gulde;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor.Timing
{
    public class RoundAdvancer : OdinEditorWindow
    {
        [MenuItem("Gulde/Round Advancer")]
        static void ShowWindow() => GetWindow<RoundAdvancer>();

        bool IsDisabled => Locator.TimeComponent.IsRunning || !Application.isPlaying;

        [Button]
        [DisableIf("@IsDisabled")]
        void Advance()
        {
            Locator.TimeComponent.StartTime();
        }
    }
}