using CosmicChamps.Battle.Units;
using Pathfinding;
using ThirdParty.Extensions;
using UnityEditor;
using UnityEngine;

public static class FixCharacterControllerSize
{
    [MenuItem ("Assets/Cosmic Champs/Fix CharacterController Size", true)]
    public static bool Validate ()
    {
        var obj = Selection.activeObject;
        return obj is GameObject gameObject && gameObject.GetComponent<RegularUnitNetworkBehaviour> ();
    }

    [MenuItem ("Assets/Cosmic Champs/Fix CharacterController Size")]
    public static void Fix ()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            var aiPath = gameObject.GetComponent<AIPath> ();
            var characterController = gameObject.GetComponent<CharacterController> ();
            
            if (aiPath == null || characterController == null)
                continue;

            characterController.radius = aiPath.radius;
            characterController.height = aiPath.height;
            characterController.center = Vector3.zero.WithY (characterController.height / 2f);
        }

        AssetDatabase.SaveAssets ();
    }
}