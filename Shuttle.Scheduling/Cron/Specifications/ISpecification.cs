namespace Shuttle.Scheduling
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T item);
    }
}
