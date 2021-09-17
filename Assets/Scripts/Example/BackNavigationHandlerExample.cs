using System.Collections;
using System.Threading.Tasks;
using Input.Swipe.ExampleImplementation.BackNavigation;
using JetBrains.Annotations;
using Library;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
    public class BackNavigationHandlerExample: MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private BackNavigationSwipeInput _backNavigationSwipeInput;

        [SerializeField, UsedImplicitly]
        private int _sampleCounter = 0;

        [SerializeField, UsedImplicitly]
        private Text _outputText;

        [UsedImplicitly]
        private IEnumerator Start()
        {
            yield return TryListenSwipe();
        }

        private IEnumerator TryListenSwipe()
        {
            if (_backNavigationSwipeInput.Show((_sampleCounter, enabled)).TryGetValue(out var swipeCommand))
            {
                yield return AwaitSwipe(swipeCommand);
            }
            else
            {
                yield return null;
                yield return TryListenSwipe();
            }
        }

        private IEnumerator AwaitSwipe(Task<ICommand> asyncSwipeCommand)
        {
            yield return new WaitUntil(() => asyncSwipeCommand.IsCompleted);
            var command = asyncSwipeCommand.Result;

            _outputText.text = $"Detected: {command}";
            Debug.Log(_outputText.text);

            _sampleCounter++;
            yield return TryListenSwipe();
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            _outputText.text = string.Empty;
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            _outputText.text = string.Empty;
        }
    }
}
