namespace TheAgencyApi.DAL;

public interface IBaseRepositoryWrite<T>
{
    public T Create(T value);
    public T Update(T value);
    public void Delete(T value);
    public Task Save();
}