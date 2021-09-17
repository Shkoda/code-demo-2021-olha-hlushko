using System.Collections;
using Input.Swipe.Data;
using JetBrains.Annotations;
using Library;
using UnityEngine;

namespace Input.Swipe.RawInput
{
    [UsedImplicitly]
    public class RawSwipeInputListener : MonoBehaviour
    {
        public readonly SwipeEvent onSwipe = new SwipeEvent();

        /**
        * <summary>
        * If the touch is longer than this value, we don't consider it a swipe
        * </summary>
        */
        [SerializeField, UsedImplicitly]
        private float _maxSwipeTime = 0.5f;

        /**
         * <summary>
         * Factor of the screen width that we consider a swipe.
         * 0.17 works well for portrait mode 16:9 phone.
         * </summary>
         */
        [SerializeField, UsedImplicitly]
        private float _minSwipeDistance = 0.17f;

        /**
         * <summary>
         * Ensure that we have single active input
         * </summary>
         */
        private static RawSwipeInputListener _instance = default;

        /**
        * <summary>
        * Single active input
        * </summary>
        */
        public static RawSwipeInputListener Instance =>
            _instance
            ?? (_instance = new GameObject($"{nameof(RawSwipeInputListener)}_{nameof(Instance)}").AddComponent<RawSwipeInputListener>());


        private Option<Coroutine> _activeCoroutine = Option<Coroutine>.None;

        [UsedImplicitly]
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning(
                    $"You already have {nameof(RawSwipeInputListener)} attached to {_instance.gameObject.name}. Destroying {name}");
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            _activeCoroutine = Option.Some(StartCoroutine(WaitForSwipeBegin()));
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            _activeCoroutine.ForEach(StopCoroutine);
            _activeCoroutine = Option<Coroutine>.None;
        }

        private IEnumerator WaitForSwipeBegin()
        {
            IEnumerator nextStep;

            if (GetInputSwitchStatus().Where(s => s.state == InputStateSwitch.TouchBegan)
                .TryGetValue(out var swipeBegin))
            {
                nextStep = WaitForSwipeEnd((swipeBegin.position, Time.time));
            }
            else
            {
                nextStep = WaitForSwipeBegin();
            }

            yield return null;
            yield return nextStep;
        }

        private IEnumerator WaitForSwipeEnd((Vector2 position, float time) swipeStart)
        {
            bool IsValidSwipe((Vector2 position, float time) swipeEnd, out SwipeDirection direction)
            {
                var swipeVector = (swipeEnd.position - swipeStart.position) / Screen.width;

                var failure = swipeEnd.time - swipeStart.time > _maxSwipeTime
                    ? Option.Some(SwipeFailure.LongDuration)
                    : swipeVector.magnitude < _minSwipeDistance
                        ? Option.Some(SwipeFailure.ShortDistance)
                        : Option<SwipeFailure>.None;

                direction = failure.IsNone
                    ? Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y)
                        ? swipeVector.x > 0
                            ? SwipeDirection.SwipeRight
                            : SwipeDirection.SwipeLeft
                        : swipeVector.y > 0
                            ? SwipeDirection.SwipeUp
                            : SwipeDirection.SwipeDown
                    : default;

                return failure.IsNone;
            }

            IEnumerator nextStep;

            if (GetInputSwitchStatus().Where(s => s.state == InputStateSwitch.TouchEnded).TryGetValue(out var swipeEnd))
            {
                if (IsValidSwipe((swipeEnd.position, Time.time), out var direction))
                {
                    onSwipe?.Invoke(SwipeDescription.Create(direction, swipeStart.position, swipeEnd.position));
                }

                nextStep = WaitForSwipeBegin();
            }
            else
            {
                nextStep = WaitForSwipeEnd(swipeStart);
            }

            yield return null;
            yield return nextStep;
        }

        private Option<(InputStateSwitch state, Vector2 position)> GetInputSwitchStatus()
        {
            bool IsInputStatusSwitched(InputStateSwitch state, out Vector2 position)
            {
                bool IsTouchStatusSwitched(out Vector2 pos)
                {
                    static Touch InputTouch() => UnityEngine.Input.GetTouch(0);

                    var isStatusSwitched =
                        UnityEngine.Input.touchCount > 0
                        && ((InputTouch().phase == TouchPhase.Began && state == InputStateSwitch.TouchBegan) ||
                            (InputTouch().phase == TouchPhase.Ended && state == InputStateSwitch.TouchEnded));

                    pos = isStatusSwitched ? InputTouch().position : default;
                    return isStatusSwitched;
                }

                bool IsMouseStatusSwitched(out Vector2 pos)
                {
                    var isStatusSwitched =
                        (state == InputStateSwitch.TouchBegan && UnityEngine.Input.GetMouseButtonDown(0))
                        || (state == InputStateSwitch.TouchEnded && UnityEngine.Input.GetMouseButtonUp(0));
                    pos = isStatusSwitched ? UnityEngine.Input.mousePosition : default;
                    return isStatusSwitched;
                }

                return IsTouchStatusSwitched(out position) || IsMouseStatusSwitched(out position);
            }

            return IsInputStatusSwitched(InputStateSwitch.TouchBegan, out var beganPosition)
                ? Option.Some((InputStateSwitch.TouchBegan, beganPosition))
                : IsInputStatusSwitched(InputStateSwitch.TouchEnded, out var endPosition)
                    ? Option.Some((InputStateSwitch.TouchEnded, bp: endPosition))
                    : Option<(InputStateSwitch state, Vector2 position)>.None;
        }

        private enum InputStateSwitch
        {
            TouchBegan,
            TouchEnded
        }

        private enum SwipeFailure
        {
            ShortDistance,
            LongDuration
        }
    }
}