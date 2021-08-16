using Gulde;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace GuldeEditor.Timing
{
    public class RoundAdvancer : OdinEditorWindow
    {
        [MenuItem("Gulde/Round Advancer")]
        static void ShowWindow() => GetWindow<RoundAdvancer>();

        [Button]
        [DisableIf("@Locator.TimeComponent.IsRunning")]
        void Advance()
        {
            Locator.TimeComponent.StartTime();
        }
    }
}