namespace Library
{
    public interface ICommand
    {
    }

    public interface ICommand<out T> : ICommand
    {
        T Data { get; }
    }
}