using Input.Swipe.Data;
using JetBrains.Annotations;
using UnityEngine;

namespace Input.Swipe.RawInput
{
    public class SwipeRectBounds : MonoBehaviour
    {
        /**
         * <summary>Any rect on scene</summary>
         */
        [SerializeField, UsedImplicitly]
        private RectTransform _swipeAreaAnchor;

        /**
         * <summary>Swipe should start inside _startSwipeAreaOffset + _swipeAreaAnchor World Rectangle</summary>
         */
        [SerializeField, UsedImplicitly]
        private RectOffset _startSwipeAreaOffset;

        /**
        * <summary>Swipe should start inside _endSwipeAreaOffset + _swipeAreaAnchor World Rectangle</summary>
        */
        [SerializeField, UsedImplicitly]
        private RectOffset _endSwipeAreaOffset;

        private Vector3[] _cachedWorldCorners = new Vector3[4];

        public bool IsSwipeInBounds(SwipeDescription swipe)
        {
            _swipeAreaAnchor.GetWorldCorners(_cachedWorldCorners);

            Vector2 WorldCorner(CornerPosition position) => _cachedWorldCorners[(int) position];

            var anchorRectPosition = WorldCorner(CornerPosition.BottomLeft);
            var anchorRectSize = WorldCorner(CornerPosition.TopRight) - WorldCorner(CornerPosition.BottomLeft);
            var anchorWorldRect = new Rect(anchorRectPosition, anchorRectSize);

            var startSwipeArea = _startSwipeAreaOffset.Add(anchorWorldRect);
            var endSwipeArea = _endSwipeAreaOffset.Add(anchorWorldRect);

            return startSwipeArea.Contains(swipe.StartPosition) && endSwipeArea.Contains(swipe.EndPosition);
        }

        private enum CornerPosition
        {
            BottomLeft = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
        }
    }
}