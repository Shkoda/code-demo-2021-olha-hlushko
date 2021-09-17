using UnityEngine;

namespace Input.Swipe.Data
{
    public readonly struct SwipeDescription
    {
        public readonly SwipeDirection Direction;
        public readonly Vector2 StartPosition;
        public readonly Vector2 EndPosition;

        public static SwipeDescription Create(SwipeDirection direction, Vector2 startPosition, Vector2 endPosition) =>
            new SwipeDescription(direction, startPosition, endPosition);

        public SwipeDescription(SwipeDirection direction, Vector2 startPosition, Vector2 endPosition)
        {
            Direction = direction;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}