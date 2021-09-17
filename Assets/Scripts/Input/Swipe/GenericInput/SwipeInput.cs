using System;
using System.Threading.Tasks;
using Input.Swipe.Data;
using Input.Swipe.RawInput;
using JetBrains.Annotations;
using Library;
using UnityEngine;

namespace Input.Swipe.GenericInput
{
    public abstract class SwipeInput<TSourceData, TCommand> : MonoBehaviour
        where TCommand : ICommand<TSourceData>
    {
        private TaskCompletionSource<ICommand> _tcs;
        private TSourceData _sourceData;

        protected abstract Func<TSourceData, SwipeDescription, Option<TCommand>> CommandBuilder { get; }

        public Option<Task<ICommand>> Show((TSourceData data, bool interactable) viewState)
        {
            var (data, isInteractable) = viewState;

            _sourceData = data;
            _tcs = _tcs == null || _tcs.Task.IsCompleted
                ? new TaskCompletionSource<ICommand>()
                : _tcs;

            return isInteractable ? Option.Some(_tcs.Task) : Option<Task<ICommand>>.None;
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            RawSwipeInputListener.Instance.onSwipe.AddListener(SwipeHandler);
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            RawSwipeInputListener.Instance.onSwipe.RemoveListener(SwipeHandler);
        }

        private void SwipeHandler(SwipeDescription swipeEventData)
        {
            if (CommandBuilder(_sourceData, swipeEventData).TryGetValue(out var command))
            {
                _tcs?.TrySetResult(command);
            }
        }
    }
}