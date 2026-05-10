using System;
using UnityEditor;
using UnityEngine;

namespace CosmicChamps.Editor
{
    public static class VersionHandler
    {
        private static SerializedObject GetSerializedBuildInfo ()
        {
            var buildInfo = Resources.Load<BuildInfo> (nameof (BuildInfo));
            if (buildInfo == null)
                throw new InvalidOperationException ("Unable to find BuildInfo asset");

            return new SerializedObject (buildInfo);
        }

        private static void SyncBuildVersionWithBundleVersion (SerializedObject serializedBuildInfo)
        {
            var chunks = Array.ConvertAll (
                serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue.Split ('.'),
                int.Parse);
            var buildVersion = chunks[0] * 100 * 100 * 100 + chunks[1] * 100 * 100 + chunks[2] * 100;

            serializedBuildInfo.FindProperty (BuildInfo.BuildVersionProperty).intValue = buildVersion;
        }

        public static void UpdateBuildInfoDate ()
        {
            var serializedBuildInfo = GetSerializedBuildInfo ();
            serializedBuildInfo.FindProperty (BuildInfo.DateProperty).stringValue = DateTime.Now.ToString ("dd.MMM.yyyy");
            serializedBuildInfo.ApplyModifiedPropertiesWithoutUndo ();
        }

        [MenuItem ("CosmicChamps/Version/Up Build", priority = 22)]
        public static void UpBuild ()
        {
            var serializedBuildInfo = GetSerializedBuildInfo ();
            serializedBuildInfo.FindProperty (BuildInfo.BuildVersionProperty).intValue++;
            serializedBuildInfo.ApplyModifiedPropertiesWithoutUndo ();
        }

        [MenuItem ("CosmicChamps/Version/Up Major", priority = 21)]
        public static void UpMajor ()
        {
            var serializedBuildInfo = GetSerializedBuildInfo ();
            var chunks = Array.ConvertAll (
                serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue.Split ('.'),
                int.Parse);
            chunks[0] += 1;
            chunks[1] = 0;
            chunks[2] = 0;

            serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue = string.Join (".", chunks);
            SyncBuildVersionWithBundleVersion (serializedBuildInfo);
            serializedBuildInfo.ApplyModifiedPropertiesWithoutUndo ();
        }

        [MenuItem ("CosmicChamps/Version/Up Minor", priority = 21)]
        public static void UpMinor ()
        {
            var serializedBuildInfo = GetSerializedBuildInfo ();
            var chunks = Array.ConvertAll (
                serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue.Split ('.'),
                int.Parse);
            chunks[1] += 1;
            chunks[2] = 0;

            serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue = string.Join (".", chunks);
            SyncBuildVersionWithBundleVersion (serializedBuildInfo);
            serializedBuildInfo.ApplyModifiedPropertiesWithoutUndo ();
        }

        [MenuItem ("CosmicChamps/Version/Up Patch", priority = 21)]
        public static void UpPatch ()
        {
            var serializedBuildInfo = GetSerializedBuildInfo ();
            var chunks = Array.ConvertAll (
                serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue.Split ('.'),
                int.Parse);
            chunks[2] += 1;

            serializedBuildInfo.FindProperty (BuildInfo.VersionProperty).stringValue = string.Join (".", chunks);
            SyncBuildVersionWithBundleVersion (serializedBuildInfo);
            serializedBuildInfo.ApplyModifiedPropertiesWithoutUndo ();
        }
    }
}