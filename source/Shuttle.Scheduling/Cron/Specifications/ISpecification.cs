namespace Shuttle.Scheduling.Cron
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T item);
    }
}
