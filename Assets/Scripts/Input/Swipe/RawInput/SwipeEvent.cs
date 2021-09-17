using Input.Swipe.Data;
using UnityEngine.Events;

namespace Input.Swipe.RawInput
{
    [System.Serializable]
    public class SwipeEvent : UnityEvent<SwipeDescription>
    {
    }
}