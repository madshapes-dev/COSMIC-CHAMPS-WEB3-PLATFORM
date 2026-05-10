using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CosmicChamps.Editor
{
    public static class LayersGenerator
    {
        private const string Namespace = "CosmicChamps";

        private const string TagsClassPath = "CosmicChamps/Scripts/Tags.cs";
        private const string LayersClassPath = "CosmicChamps/Scripts/Layers.cs";
        private const string SortingLayersClassPath = "CosmicChamps/Scripts/SortingLayers.cs";

        private const string TagsTemplate = @"namespace ${Namespace}
{
    public static class Tags
    {
        ${Tags}
    }
}";

        private const string LayersTemplate = @"namespace ${Namespace}
{
    public static class Layers
    {
        ${Layers}

        public static class Masks
        {
            ${Masks}
        }
    }
}";

        private const string SortingLayersTemplate = @"namespace ${Namespace}
{
    public static class SortingLayers
    {
        ${Layers}
    }
}";

        [MenuItem ("CosmicChamps/Generate/Tags")]
        private static void GenerateTags ()
        {
            var source = UnityEditorInternal
                .InternalEditorUtility
                .tags
                .Select (x => $"public const string {x} = \"{x}\";");

            File.WriteAllText (
                Application.dataPath + "/" + TagsClassPath,
                TagsTemplate
                    .Replace ("${Namespace}", Namespace)
                    .Replace ("${Tags}", string.Join ("\n\t\t", source)));
        }

        [MenuItem ("CosmicChamps/Generate/Sorting Layers")]
        private static void GenerateSortingLayers ()
        {
            var source = SortingLayer
                .layers
                .Select (x => $"public const int {x.name} = {x.id};");

            File.WriteAllText (
                Application.dataPath + "/" + SortingLayersClassPath,
                SortingLayersTemplate
                    .Replace ("${Namespace}", Namespace)
                    .Replace ("${Layers}", string.Join ("\n\t\t", source)));
        }

        [MenuItem ("CosmicChamps/Generate/Layers")]
        public static void GenerateLayers ()
        {
            var layers = new List<(int, string)> ();
            for (var i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName (i).Replace (" ", "");
                if (string.IsNullOrEmpty (layerName))
                    continue;

                layers.Add ((i, layerName));
            }

            var source = layers
                .Select (
                    x => ($"public const int {x.Item2} = {x.Item1};",
                        $"public const int {x.Item2} = 1 << Layers.{x.Item2};"))
                .ToArray ();

            File.WriteAllText (
                Application.dataPath + "/" + LayersClassPath,
                LayersTemplate
                    .Replace ("${Namespace}", Namespace)
                    .Replace ("${Layers}", string.Join ("\n\t\t", source.Select (x => x.Item1)))
                    .Replace ("${Masks}", string.Join ("\n\t\t\t", source.Select (x => x.Item2))));
        }
    }
}