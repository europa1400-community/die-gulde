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
        CitizenComponent Citizen => Locator.Citizen;

        [MenuItem("Gulde/Informater")]
        static void ShowWindow() => GetWindow<Informater>();

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Time => Locator.Time ? $"{Locator.Time.Hour} : {Locator.Time.Minute} - {Locator.Time.Year}" : "";

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Points => Citizen ? $"{Citizen.ActionPoint.Points} AP" : "";

        [ShowInInspector]
        [HideLabel]
        [OnInspectorGUI("@GUIHelper.RequestRepaint()")]
        string Money => Citizen ? $"{Citizen.Wealth.Money} Gulden" : "";
    }
}