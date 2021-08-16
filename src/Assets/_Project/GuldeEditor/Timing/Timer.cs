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
        int Minute { get; set; }

        [ShowInInspector]
        int Hour { get; set; }

        [ShowInInspector]
        int Year { get; set; }

        void Update()
        {
            Minute = Locator.TimeComponent.Minute;
            Hour = Locator.TimeComponent.Hour;
            Year = Locator.TimeComponent.Year;

            Repaint();
        }
    }
}