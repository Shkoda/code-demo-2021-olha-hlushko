using System;
using Input.Swipe.Data;
using Input.Swipe.GenericInput;
using Input.Swipe.RawInput;
using JetBrains.Annotations;
using Library;
using UnityEngine;

namespace Input.Swipe.ExampleImplementation.BackNavigation
{
    public class BackNavigationSwipeInput : SwipeInput<int, BackNavigationCommand>
    {
        [SerializeField, UsedImplicitly]
        private SwipeRectBounds _swipeBounds;

        protected override Func<int, SwipeDescription, Option<BackNavigationCommand>> CommandBuilder =>
            (sampleCounter, swipe) => _swipeBounds.IsSwipeInBounds(swipe) && swipe.Direction == SwipeDirection.SwipeLeft
                ? Option.Some(BackNavigationCommand.Create(sampleCounter))
                : Option<BackNavigationCommand>.None;
    }
}