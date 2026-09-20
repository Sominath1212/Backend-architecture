using Backend.Application.DTOs.Companies;

namespace Backend.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IReadOnlyList<CompanyResponse>> GetAllAsync(string traceId);
        Task<CompanyResponse> GetByIdAsync(int companyId, string traceId);
        Task<CompanyResponse> CreateAsync(string userId, CreateCompanyRequest request, string traceId);
        Task<CompanyResponse> UpdateAsync(string userId, int companyId, UpdateCompanyRequest request, string traceId);
        Task DeleteAsync(string userId, int companyId, string traceId);
    }
}