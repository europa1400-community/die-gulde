using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InterfaceChangerWindow : EditorWindow
{
    CanvasGroup _canvasGroupInfo;
    CanvasGroup _canvasGroupBuild;

    [MenuItem("Window/Interface Changer")]
    public static void ShowWindow()
    {
        GetWindow<InterfaceChangerWindow>("Interface Changer");
    }

    void OnEnable()
    {
        _canvasGroupInfo = GameObject.Find("canvas_info").GetComponent<CanvasGroup>();
        _canvasGroupBuild = GameObject.Find("canvas_build").GetComponent<CanvasGroup>();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Info"))
        {
            _canvasGroupInfo.alpha = 1;
            _canvasGroupInfo.blocksRaycasts = true;
            _canvasGroupInfo.interactable = true;

            _canvasGroupBuild.alpha = 0;
            _canvasGroupBuild.blocksRaycasts = false;
            _canvasGroupBuild.interactable = false;
        }

        if (GUILayout.Button("Build"))
        {
            _canvasGroupInfo.alpha = 0;
            _canvasGroupInfo.blocksRaycasts = false;
            _canvasGroupInfo.interactable = false;

            _canvasGroupBuild.alpha = 1;
            _canvasGroupBuild.blocksRaycasts = true;
            _canvasGroupBuild.interactable = true;
        }

        GUILayout.EndVertical();
    }
}
