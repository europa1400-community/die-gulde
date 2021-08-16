using Gulde;
using Gulde.Timing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace GuldeEditor.Timing
{
    public class Timer : OdinEditorWindow
    {
        [MenuItem("Gulde/Timer")]
        static void ShowWindow() => GetWindow<Timer>();

        [ShowInInspector]
        [ReadOnly]
        int Minute => Locator.TimeComponent.Minute;

        [ShowInInspector]
        [ReadOnly]
        int Hour => Locator.TimeComponent.Hour;

        [ShowInInspector]
        [ReadOnly]
        int Year => Locator.TimeComponent.Year;

        void Update()
        {
            Repaint();
        }
    }
}