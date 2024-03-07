namespace TheAgencyApi.Services;

public interface IBaseServiceWrite<T, I>
{
    public Task<T> Create(T value);
    public Task<T> Update(T value);
    public Task Delete(I id);
}