using Orders.Shared.Responses;

namespace Orders.Backend.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string servicePrefix, string controller);
    }

}
