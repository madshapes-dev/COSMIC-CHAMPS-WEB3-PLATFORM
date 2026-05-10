using Pathfinding;
using UnityEngine;

namespace CosmicChamps.Battle.Server
{
    public class MultiTargetPath : Pathfinding.MultiTargetPath
    {
        private float? _seekerAttackRange;

        private bool TryToFindReachableTarget (out int targetIndex)
        {
            targetIndex = -1;
            if (!_seekerAttackRange.HasValue)
                return false;

            var bestG = uint.MaxValue;
            var targetFound = false;

            /**
             * Fore unknown reasons sometimes vectorPath and nodePaths are null.
             * The exception makes the game stuck.
             * Extra check to prevent it.
             */
            if (vectorPath == null || nodePaths == null)
                return false;

            for (var i = 0; i < vectorPaths.Length; i++)
            {
                var currentPath = nodePaths[i];
                if (currentPath == null)
                    continue;

                var node = currentPath[inverted ? 0 : currentPath.Count - 1];
                if ((originalStartPoint - (Vector3)node.position).magnitude > _seekerAttackRange.Value)
                    continue;

                var finalNode = pathHandler.GetPathNode (currentPath[inverted ? 0 : currentPath.Count - 1]);
                var g = finalNode.G;
                if (g >= bestG)
                    continue;

                bestG = g;
                targetIndex = i;
                targetFound = true;
            }

            return targetFound;
        }

        private bool TryToFindSameSideTarget (out int targetIndex)
        {
            targetIndex = -1;

            var bestG = uint.MaxValue;
            var targetFound = false;
            
            if (vectorPath == null || nodePaths == null)
                return false;

            for (var i = 0; i < vectorPaths.Length; i++)
            {
                var currentPath = nodePaths[i];
                if (currentPath == null)
                    continue;

                var node = currentPath[inverted ? 0 : currentPath.Count - 1];
                if (((Vector3)node.position).x * originalStartPoint.x < 0)
                    continue;

                var g = pathHandler.GetPathNode (currentPath[inverted ? 0 : currentPath.Count - 1]).G;
                if (g >= bestG)
                    continue;

                bestG = g;
                targetIndex = i;
                targetFound = true;
            }

            return targetFound;
        }

        private int FindBestGTarget ()
        {
            var targetIndex = -1;
            if (nodePaths != null)
            {
                uint bestG = int.MaxValue;
                for (int i = 0; i < nodePaths.Length; i++)
                {
                    var currentPath = nodePaths[i];
                    if (currentPath != null)
                    {
                        // Get the G score of the first or the last node in the path
                        // depending on if the paths are reversed or not
                        var g = pathHandler.GetPathNode (currentPath[inverted ? 0 : currentPath.Count - 1]).G;
                        if (targetIndex == -1 || g < bestG)
                        {
                            targetIndex = i;
                            bestG = g;
                        }
                    }
                }
            }

            return targetIndex;
        }

        private void ChooseShortestPath ()
        {
            chosenTarget = TryToFindReachableTarget (out var targetIndex) || TryToFindSameSideTarget (out targetIndex)
                ? targetIndex
                : FindBestGTarget ();
        }

        private void ResetFlags ()
        {
            // Reset all flags
            if (targetNodes != null)
            {
                for (int i = 0; i < targetNodes.Length; i++)
                {
                    if (targetNodes[i] != null) pathHandler.GetPathNode (targetNodes[i]).flag1 = false;
                }
            }
        }

        protected void Setup (
            Vector3 start,
            Vector3[] targets,
            float seekerAttackRange,
            OnPathDelegate[] callbackDelegates,
            OnPathDelegate callback)
        {
            _seekerAttackRange = seekerAttackRange;

            Setup (start, targets, callbackDelegates, callback);
        }

        protected override void Cleanup ()
        {
            ChooseShortestPath ();
            ResetFlags ();
        }

        protected override void Reset ()
        {
            base.Reset ();

            _seekerAttackRange = null;
        }

        public static MultiTargetPath Construct (
            Vector3 start,
            Vector3[] targets,
            float seekerAttackRange,
            OnPathDelegate[] callbackDelegates,
            OnPathDelegate callback = null)
        {
            var p = PathPool.GetPath<MultiTargetPath> ();
            p.Setup (start, targets, seekerAttackRange, callbackDelegates, callback);
            p.pathsForAll = true;
            return p;
        }
    }
}