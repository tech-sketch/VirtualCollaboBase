//
// This is a modified version of the GuidComponentDrawer from 
// Unity Technologies' Guid Based Reference.
// The copyright notice from the original version is included below.
//
// The original source code of GuidComponentDrawer is available on GitHub.
// https://github.com/Unity-Technologies/guid-based-reference/blob/master/Assets/CrossSceneReference/Editor/GuidComponentDrawer.cs
//

//
// Copyright © 2018 Unity Technologies ApS
// Licensed under the Unity Companion License for Unity-dependent projects
// see Unity Companion License.(https://unity3d.com/legal/licenses/Unity_Companion_License)
//

using UnityEditor;
using UnityEngine;

namespace GuidBasedReference
{
    [CustomEditor(typeof(GuidComponent))]
    public class GuidComponentInspector : Editor
    {
        private GuidComponent guidComp;

        public override void OnInspectorGUI()
        {
            if (guidComp == null)
            {
                guidComp = (GuidComponent)target;
            }

            EditorGUILayout.LabelField("Guid:", guidComp.GetGuid().ToString());

            if (GUILayout.Button("Create New Guid"))
            {
                Undo.RecordObject(guidComp, "Create New Guid");
                guidComp.CreateNewGuid();
            }
        }
    }
}
