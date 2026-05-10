using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CosmicChamps.Battle.Units;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace CosmicChamps.Editor
{
    public static class CreateUnitMenus
    {
        private const string DefaultPreviewPrefabName = "Animator.prefab";
        private const string DefaultCardSpriteName = "Card.png";

        private static string GetSelectionPath ()
        {
            var obj = Selection.activeObject;
            var path = obj == null ? null : AssetDatabase.GetAssetPath (obj.GetInstanceID ());

            return path;
        }

        private static bool CheckIfDirectoryIsSelected ()
        {
            var path = GetSelectionPath ();
            var isDirectory = Directory.Exists (path);

            return isDirectory;
        }

        private static AddressableAssetGroup GetAddressablesGroup (string name)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var g = settings.FindGroup (name);
            if (g != null)
                return g;

            g = settings.CreateGroup (
                name,
                false,
                false,
                false,
                new List<AddressableAssetGroupSchema> (),
                typeof (BundledAssetGroupSchema),
                typeof (ContentUpdateGroupSchema));

            var contentUpdateSchema = g.GetSchema<BundledAssetGroupSchema> ();
            contentUpdateSchema.BuildPath.SetVariableByName (settings, "Remote.BuildPath");
            contentUpdateSchema.LoadPath.SetVariableByName (settings, "Remote.LoadPath");
            contentUpdateSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;

            return g;
        }

        private static void InternalGenerateUnitViewData (
            string path,
            string idPostfix = null,
            string previewPrefabName = DefaultPreviewPrefabName,
            string cardSpriteName = DefaultCardSpriteName)
        {
            var unitId = Path.GetFileNameWithoutExtension (Path.GetDirectoryName (path));
            var ntfId = Path.GetFileNameWithoutExtension (path);

            if (string.IsNullOrEmpty (unitId) || string.IsNullOrEmpty (ntfId))
                throw new InvalidOperationException ("Unable to figure out unit id or ntf id");

            var prefabPath = Path.Combine (path, "Prefabs", "Unit.prefab");
            var prefab = AssetDatabase.LoadAssetAtPath<RegularUnitNetworkBehaviour> (prefabPath);
            if (prefab == null)
                throw new InvalidOperationException ("Unable to find unit prefab");

            var previewPath = Path.Combine (path, "Prefabs", previewPrefabName);
            var preview = AssetDatabase.LoadAssetAtPath<GameObject> (previewPath);
            if (preview == null)
                throw new InvalidOperationException ("Unable to find unit preview");

            var cardSpritePath = Path.Combine (path, "Textures", cardSpriteName);
            var cardSprite = AssetDatabase.LoadAssetAtPath<Sprite> (cardSpritePath);
            if (cardSprite == null)
                throw new InvalidOperationException ("Unable to find card sprite");

            var id = (ntfId == "Basic" ? unitId : $"{unitId}_{ntfId}").ToLower ();
            var postfixedId = (ntfId == "Basic" ? $"{unitId}{idPostfix}" : $"{unitId}{idPostfix}_{ntfId}").ToLower ();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var g = GetAddressablesGroup (unitId);

            var prefabGuid = AssetDatabase.AssetPathToGUID (prefabPath);
            var prefabEntry = settings.CreateOrMoveEntry (prefabGuid, g);
            prefabEntry.labels.Clear ();
            prefabEntry.labels.Add (AddressablesLabels.Client);
            prefabEntry.labels.Add (AddressablesLabels.Server);
            prefabEntry.labels.Add (AddressablesLabels.Units);
            prefabEntry.address = $"units/{id}";

            var previewGuid = AssetDatabase.AssetPathToGUID (previewPath);
            var previewEntry = settings.CreateOrMoveEntry (previewGuid, g);
            previewEntry.labels.Clear ();
            previewEntry.labels.Add (AddressablesLabels.Client);
            previewEntry.labels.Add (AddressablesLabels.Server);
            previewEntry.labels.Add (AddressablesLabels.Units);
            previewEntry.address = $"previews/{postfixedId}";

            var cardSpriteGuid = AssetDatabase.AssetPathToGUID (cardSpritePath);
            var cardSpriteEntry = settings.CreateOrMoveEntry (cardSpriteGuid, g);
            cardSpriteEntry.labels.Clear ();
            cardSpriteEntry.labels.Add (AddressablesLabels.Client);
            cardSpriteEntry.labels.Add (AddressablesLabels.Cards);
            cardSpriteEntry.address = $"cards/{postfixedId}";

            settings.SetDirty (AddressableAssetSettings.ModificationEvent.EntryMoved, cardSpriteEntry, true);
        }

        private static void InternalGenerateUnitViewDatas (
            string idPostfix = null,
            string previewPrefabName = DefaultPreviewPrefabName,
            string cardSpriteName = DefaultCardSpriteName)
        {
            var path = GetSelectionPath ();
            var subPaths = Directory.GetDirectories (Path.Combine (Application.dataPath.Replace ("Assets", string.Empty), path));

            foreach (var absoluteSubPath in subPaths)
            {
                var subPath =
                    Path.Combine (
                        "Assets",
                        absoluteSubPath.Replace (Application.dataPath, string.Empty)
                            .TrimStart (Path.PathSeparator, Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                try
                {
                    var skinId = Path.GetFileName (subPath);
                    if (skinId == "Basic" || Regex.IsMatch (skinId, "[\\d]{9}"))
                    {
                        InternalGenerateUnitViewData (subPath, idPostfix, previewPrefabName, cardSpriteName);
                        Debug.Log ($"Successfully processed skin {skinId}");
                    }
                } catch (Exception e)
                {
                    Debug.LogError ($"Failed to process the path {subPath}");
                    Debug.LogException (e);
                }
            }
        }

        private static void CreateSubfolder (string path, string folder)
        {
            var folderPath = Path.Combine (path, folder);
            Directory.CreateDirectory (folderPath);
            AssetDatabase.ImportAsset (folderPath);
        }

        [MenuItem ("Assets/Cosmic Champs/Generate/Folder Layout/Full", true)]
        public static bool ValidateGenerateUnitFullFolderLayout () => CheckIfDirectoryIsSelected ();

        [MenuItem ("Assets/Cosmic Champs/Generate/Folder Layout/Short", true)]
        public static bool ValidateGenerateUnitShortFolderLayout () => CheckIfDirectoryIsSelected ();

        [MenuItem ("Assets/Cosmic Champs/Generate/Default Unit/Addresables Data", true)]
        public static bool ValidateGenerateUnitViewData () => CheckIfDirectoryIsSelected ();

        [MenuItem ("Assets/Cosmic Champs/Generate/Default Unit/Addresables Datas", true)]
        public static bool ValidateGenerateUnitViewDatas () => CheckIfDirectoryIsSelected ();

        [MenuItem ("Assets/Cosmic Champs/Generate/Tertius Unit/Addresables Data", true)]
        public static bool ValidateGenerateTertiusUnitViewData () => CheckIfDirectoryIsSelected ();

        [MenuItem ("Assets/Cosmic Champs/Generate/Tertius Unit/Addresables Datas", true)]
        public static bool ValidateGenerateTertiusUnitViewDatas () => CheckIfDirectoryIsSelected ();

        [MenuItem ("Assets/Cosmic Champs/Generate/Default Unit/Addresables Datas")]
        public static void GenerateUnitViewDatas ()
        {
            InternalGenerateUnitViewDatas ();
            AssetDatabase.SaveAssets ();
        }

        [MenuItem ("Assets/Cosmic Champs/Generate/Folder Layout/Full")]
        public static void GenerateUnitFullFolderLayout ()
        {
            var path = GetSelectionPath ();

            CreateSubfolder (path, "Animations");
            CreateSubfolder (path, "Materials");
            CreateSubfolder (path, "Models");
            CreateSubfolder (path, "Prefabs");
            CreateSubfolder (path, "Textures");
        }

        [MenuItem ("Assets/Cosmic Champs/Generate/Folder Layout/Short")]
        public static void GenerateUnitShortFolderLayout ()
        {
            var path = GetSelectionPath ();

            CreateSubfolder (path, "Materials");
            CreateSubfolder (path, "Prefabs");
            CreateSubfolder (path, "Textures");
        }

        [MenuItem ("Assets/Cosmic Champs/Generate/Default Unit/Addresables Data")]
        public static void GenerateUnitViewData ()
        {
            var path = GetSelectionPath ();

            InternalGenerateUnitViewData (path);
            AssetDatabase.SaveAssets ();

            Debug.Log ($"Successfully processed {Path.GetFileName (Path.GetDirectoryName (path))}/{Path.GetFileName (path)}");
        }

        [MenuItem ("Assets/Cosmic Champs/Generate/Tertius Unit/Addresables Datas")]
        public static void GenerateTertiusUnitViewDatas ()
        {
            InternalGenerateUnitViewDatas ("iii", "TertiusIIIPreview.prefab", "CardTertiusIII.png");
            InternalGenerateUnitViewDatas ("x", "TertiusXPreview.prefab", "CardTertiusX.png");
            AssetDatabase.SaveAssets ();
        }

        [MenuItem ("Assets/Cosmic Champs/Generate/Tertius Unit/Addresables Data")]
        public static void GenerateTertiusUnitViewData ()
        {
            var path = GetSelectionPath ();

            InternalGenerateUnitViewData (path, "iii", "TertiusIIIPreview.prefab", "CardTertiusIII.png");
            InternalGenerateUnitViewData (path, "x", "TertiusXPreview.prefab", "CardTertiusX.png");
            AssetDatabase.SaveAssets ();

            Debug.Log ($"Successfully processed {Path.GetFileName (Path.GetDirectoryName (path))}/{Path.GetFileName (path)}");
        }

        private static void AlignSelectionGameObjects (Vector3[] positions)
        {
            var gameObjects = Selection.gameObjects;
            var count = Mathf.Min (positions.Length, gameObjects.Length);

            for (var i = 0; i < count; i++)
            {
                gameObjects[i].transform.position = positions[i];
            }
        }


        [MenuItem ("CONTEXT/Transform/CosmicChamps/Align As Tertius 3 Batch")]
        public static void AlignAsTertius3Batch (MenuCommand command) =>
            AlignSelectionGameObjects (
                new[]
                {
                    new Vector3 (0f, 0f, 1),
                    new Vector3 (-1, 0f, -1),
                    new Vector3 (1, 0f, -1)
                });

        [MenuItem ("CONTEXT/Transform/CosmicChamps/Align As Tertius X Batch")]
        public static void AlignAsTertiusXBatch (MenuCommand command) =>
            AlignSelectionGameObjects (
                new[]
                {
                    new Vector3 (0f, 0f, 1.5f),
                    new Vector3 (0.96f, 0f, 1.14f),
                    new Vector3 (1.47f, 0f, 0.255f),
                    new Vector3 (1.29f, 0f, -0.75f),
                    new Vector3 (0.51f, 0f, -1.41f),
                    new Vector3 (-0.51f, 0f, -1.41f),
                    new Vector3 (-1.29f, 0f, -0.75f),
                    new Vector3 (-1.47f, 0f, 0.255f),
                    new Vector3 (-0.96f, 0f, 1.14f),
                    new Vector3 (0f, 0f, 0f),
                });
    }
}