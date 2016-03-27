namespace Shuttle.Esb.Scheduling
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T item);
    }
}
