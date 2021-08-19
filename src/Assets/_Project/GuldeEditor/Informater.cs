using Gulde;
using Gulde.Timing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor
{
    public class Informater : OdinEditorWindow
    {
        [MenuItem("Gulde/Informater")]
        static void ShowWindow() => GetWindow<Informater>();

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Time => Application.isPlaying ? $"{Locator.Time.Hour} : {Locator.Time.Minute} - {Locator.Time.Year}" : "";

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Points => Application.isPlaying ? $"{Locator.Player.Action.Points} AP" : "";

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Money => Application.isPlaying ? $"{Locator.Player.Wealth.Money} Gulden" : "";
    }
}