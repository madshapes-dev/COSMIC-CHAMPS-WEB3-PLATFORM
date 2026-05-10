using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.Components
{
    public sealed class GridLayoutGroup : LayoutGroup
    {
        [SerializeField]
        private Vector2 _spacing = Vector2.zero;

        [SerializeField, Range (1, 10)]
        private int _columnsCount = 4;

        [SerializeField, Range (0.001f, 10)]
        private float _cellAspectRatio = 1f;

        private Vector2 CellSize
        {
            get
            {
                var width = (rectTransform.rect.width - m_Padding.horizontal - _spacing.x * (_columnsCount - 1)) /
                            _columnsCount;
                var height = width / _cellAspectRatio;

                return new Vector2 (width, height);
            }
        }

        private Vector2 Spacing => _spacing;

        public override void CalculateLayoutInputHorizontal ()
        {
            base.CalculateLayoutInputHorizontal ();

            SetLayoutInputForAxis (
                padding.horizontal + (CellSize.x + Spacing.x) * _columnsCount - Spacing.x,
                padding.horizontal + (CellSize.x + Spacing.x) * _columnsCount - Spacing.x,
                -1,
                0);
        }

        public override void CalculateLayoutInputVertical ()
        {
            var minRows = Mathf.CeilToInt (rectChildren.Count / (float)_columnsCount - 0.001f);
            var minSpace = padding.vertical + (CellSize.y + Spacing.y) * minRows - Spacing.y;
            SetLayoutInputForAxis (minSpace, minSpace, -1, 1);
        }

        public override void SetLayoutHorizontal ()
        {
            SetCellsAlongAxis (0);
        }

        public override void SetLayoutVertical ()
        {
            SetCellsAlongAxis (1);
        }

        private void SetCellsAlongAxis (int axis)
        {
            // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
            // and only vertical values when invoked for the vertical axis.
            // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
            // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
            // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.
            var rectChildrenCount = rectChildren.Count;
            if (axis == 0)
            {
                // Only set the sizes when invoked for horizontal axis, not the positions.

                for (var i = 0; i < rectChildrenCount; i++)
                {
                    var rect = rectChildren[i];

                    m_Tracker.Add (
                        this,
                        rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = CellSize;
                }

                return;
            }

            var actualCellCountX = Mathf.Clamp (_columnsCount, 1, rectChildrenCount);
            var actualCellCountY = Mathf.CeilToInt (rectChildrenCount / (float)_columnsCount);
            var requiredSpace = new Vector2 (
                actualCellCountX * CellSize.x + (actualCellCountX - 1) * Spacing.x,
                actualCellCountY * CellSize.y + (actualCellCountY - 1) * Spacing.y
            );

            var startOffset = new Vector2 (
                GetStartOffset (0, requiredSpace.x),
                GetStartOffset (1, requiredSpace.y));

            for (var i = 0; i < rectChildrenCount; i++)
            {
                var positionX = i % _columnsCount;
                var positionY = i / _columnsCount;

                SetChildAlongAxis (rectChildren[i], 0, startOffset.x + (CellSize[0] + Spacing[0]) * positionX, CellSize[0]);
                SetChildAlongAxis (rectChildren[i], 1, startOffset.y + (CellSize[1] + Spacing[1]) * positionY, CellSize[1]);
            }
        }
    }
}