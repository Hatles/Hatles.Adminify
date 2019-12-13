using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Hatles.Adminify.MultiTenancy.Dto;

namespace Hatles.Adminify.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

