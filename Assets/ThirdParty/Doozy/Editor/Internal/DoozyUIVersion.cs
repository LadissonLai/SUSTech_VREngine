// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Internal
{
    public class DoozyUIVersion : ScriptableObject
    {
        private static string AssetPath { get { return Path.Combine(DoozyPath.EDITOR_INTERNAL_PATH, "DoozyUIVersion.asset"); } }

        private static DoozyUIVersion s_instance;

        public static DoozyUIVersion Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<DoozyUIVersion>(AssetPath);
                if (s_instance != null) return s_instance;
                s_instance = CreateInstance<DoozyUIVersion>();
                AssetDatabase.CreateAsset(s_instance, AssetPath);
                EditorUtility.SetDirty(s_instance);
                AssetDatabase.SaveAssets();
                return s_instance;
            }
        }


        public int VersionMajor = 3;
        public int VersionMinor = 1;
        public int VersionStatus = 2;
        public int VersionRevision = 1;

        public string VersionReleaseStatus
        {
            get
            {
                switch (VersionStatus)
                {
                    case 0:  return "a"; //alpha
                    case 1:  return "b"; //beta
                    case 2:  return "c"; //commercial distribution
                    case 3:  return "d"; //debug
                    default: return "?";
                }
            }
        }

        public string Version
        {
            get
            {
                return VersionMajor + "." +
                       VersionMinor + "." +
                       VersionRevision;
            }
        }

        /// <summary> Set DoozyUI version that will get installed </summary>
        /// <param name="major"> Major Version Number </param>
        /// <param name="minor"> Minor Version Number </param>
        /// <param name="status"> Version Status (0 - alpha, 1 - beta, 2 - commercial distribution, 3 - debug) (a,b,c,d) </param>
        /// <param name="revision"> Revision Number </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetVersion(int major, int minor, int status, int revision, bool saveAssets = false)
        {
            VersionMajor = major;
            VersionMinor = minor;
            VersionStatus = status;
            VersionRevision = revision;
            SetDirty(saveAssets);
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}