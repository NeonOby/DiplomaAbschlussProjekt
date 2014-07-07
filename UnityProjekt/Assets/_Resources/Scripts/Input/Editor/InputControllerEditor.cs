﻿using UnityEditor;
using UnityEngine;
using System.Collections;


[CustomEditor(typeof(InputController))]
public class InputControllerEditor : Editor {

    [SerializeField]
    private InputController myTarget;

    private Vector2 inputInfoScrollValue = Vector2.zero;
    private Vector2 unityButtonsScrollValue = Vector2.zero;
    private Vector2 unityAxisScrollValue = Vector2.zero;

    private InputInfo deleteInfo = null;
    private InputInfo moveUpInfo = null;
    private InputInfo moveDownInfo = null;

    public override void OnInspectorGUI()
    {
        myTarget = (InputController) target;

        deleteInfo = null;
        moveDownInfo = null;
        moveUpInfo = null;
        
        GUILayout.Label("Input Infos:");
        
        inputInfoScrollValue = EditorGUILayout.BeginScrollView(inputInfoScrollValue, GUILayout.MaxHeight(300), GUILayout.MinHeight(200));

        for (int i = 0; i < myTarget.InputInfos.Count; i++)
        {
            InputInfo inputInfo = myTarget.InputInfos[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label("Input Type:");
            if (i > 0 && GUILayout.Button("/\\", GUILayout.Width(30)))
            {
                moveUpInfo = inputInfo;
            }
            if (i < myTarget.InputInfos.Count - 1 && GUILayout.Button("\\/", GUILayout.Width(30)))
            {
                moveDownInfo = inputInfo;
            }
            if (GUILayout.Button("Remove Info"))
            {
                deleteInfo = inputInfo;
            }
            GUILayout.EndHorizontal();

            inputInfo.Action = EditorGUILayout.TextField(inputInfo.Action);

            GUILayout.BeginHorizontal();
            inputInfo.ActionType = (ActionType)EditorGUILayout.EnumPopup(inputInfo.ActionType);
            inputInfo.InputType = (InputType)EditorGUILayout.EnumPopup(inputInfo.InputType);
            GUILayout.EndHorizontal();

            if (inputInfo.InputType == InputType.KEYCODE)
            {
                inputInfo.key = (KeyCode)EditorGUILayout.EnumPopup(inputInfo.key);
            }
            else
            {
                if (inputInfo.InputType == InputType.MOUSEBUTTON)
                {
                    inputInfo.mouseButtonID = EditorGUILayout.IntField(inputInfo.mouseButtonID);
                }
                else
                {
                    inputInfo.inputString = EditorGUILayout.TextField(inputInfo.inputString);
                }
            }

            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();

        if (deleteInfo != null)
        {
            myTarget.InputInfos.Remove(deleteInfo);
            deleteInfo = null;
        }
        if (moveUpInfo != null)
        {
            moveUpInfo = null;
            int index = myTarget.InputInfos.IndexOf(moveUpInfo);
            InputInfo tmp = myTarget.InputInfos[index - 1];

            myTarget.InputInfos[index] = tmp;
            myTarget.InputInfos[index - 1] = moveUpInfo;
        }
        if (moveDownInfo != null)
        {
            moveDownInfo = null;
            int index = myTarget.InputInfos.IndexOf(moveDownInfo);
            InputInfo tmp = myTarget.InputInfos[index + 1];

            myTarget.InputInfos[index] = tmp;
            myTarget.InputInfos[index + 1] = moveDownInfo;
        }
            

        if (GUILayout.Button("Add new Info"))
        {
            myTarget.InputInfos.Add(new InputInfo());
        }

        GUILayout.Label("UnityButtons:");
        unityButtonsScrollValue = EditorGUILayout.BeginScrollView(unityButtonsScrollValue, GUILayout.MaxHeight(300));

        for (int i = 0; i < myTarget.unityButtonInputs.Count; i++)
        {
            myTarget.unityButtonInputs[i] = EditorGUILayout.TextField(myTarget.unityButtonInputs[i]);
        }

        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Add new UnityButton"))
        {
            myTarget.unityButtonInputs.Add("NewButton");
        }


        GUILayout.Label("Unity Axis:");
        unityAxisScrollValue = EditorGUILayout.BeginScrollView(unityAxisScrollValue, GUILayout.MaxHeight(300));

        for (int i = 0; i < myTarget.unityAxisInputs.Count; i++)
        {
            myTarget.unityAxisInputs[i] = EditorGUILayout.TextField(myTarget.unityAxisInputs[i]);
        }

        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Add new UnityAxis"))
        {
            myTarget.unityAxisInputs.Add("NewAxis");
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }
}

