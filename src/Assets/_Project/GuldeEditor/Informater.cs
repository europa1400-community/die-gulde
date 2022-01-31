using GuldeLib;
using GuldeLib.Players;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor
{
    public class Informater : OdinEditorWindow
    {
        [OdinSerialize]
        PlayerComponent Player => Locator.Player;

        [MenuItem("Gulde/Informater")]
        static void ShowWindow() => GetWindow<Informater>();

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Time => Locator.Time ? $"{Locator.Time.Hour} : {Locator.Time.Minute} - {Locator.Time.Year}" : "";

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Points => Player ? $"{Player.ActionPoint.Points} AP" : "";

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Money => Player ? $"{Player.Wealth.Money} Gulden" : "";
    }
}