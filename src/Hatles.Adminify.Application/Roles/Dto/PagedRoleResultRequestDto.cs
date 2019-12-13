using Abp.Application.Services.Dto;

namespace Hatles.Adminify.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

