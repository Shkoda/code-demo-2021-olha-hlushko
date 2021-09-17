using Library;

namespace Input.Swipe.ExampleImplementation.BackNavigation
{
    public class BackNavigationCommand: ICommand<int>
    {
        private int SampleCounter { get; }
        public int Data => SampleCounter;

        private BackNavigationCommand(int sampleCounter)
        {
            SampleCounter = sampleCounter;
        }

        public static BackNavigationCommand Create(int sampleCounter) => new BackNavigationCommand(sampleCounter);

        public override string ToString()
        {
            return $"{nameof(BackNavigationCommand)}<{SampleCounter}>";
        }
    }
}
