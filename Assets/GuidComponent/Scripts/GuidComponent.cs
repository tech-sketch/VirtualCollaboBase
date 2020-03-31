//
// This is a modified version of the GuidComponent from 
// Unity Technologies' Guid Based Reference.
// The copyright notice from the original version is included below.
//
// The original source code of GuidComponent is available on GitHub.
// https://github.com/Unity-Technologies/guid-based-reference/blob/master/Assets/CrossSceneReference/Runtime/GuidComponent.cs
//

//
// Copyright © 2018 Unity Technologies ApS
// Licensed under the Unity Companion License for Unity-dependent projects
// see Unity Companion License.(https://unity3d.com/legal/licenses/Unity_Companion_License)
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GuidBasedReference
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class GuidComponent : MonoBehaviour, ISerializationCallbackReceiver 
    {
        System.Guid guid = System.Guid.Empty;

        [SerializeField]
        private byte[] m_SerializedGuid;

        public void CreateNewGuid()
        {
            guid = System.Guid.NewGuid();
            m_SerializedGuid = guid.ToByteArray();
        }

        public System.Guid GetGuid()
        {
            if (guid == System.Guid.Empty && m_SerializedGuid != null && m_SerializedGuid.Length == 16)
            {
                guid = new System.Guid(m_SerializedGuid);
            }
            return guid;
        }

        public void OnBeforeSerialize()
        {
            if (guid != System.Guid.Empty)
            {
                m_SerializedGuid = guid.ToByteArray();
            }
        }

        public void OnAfterDeserialize()
        {
            if (m_SerializedGuid != null && m_SerializedGuid.Length == 16)
            {
                guid = new System.Guid(m_SerializedGuid);
            }
        }
    }
}
