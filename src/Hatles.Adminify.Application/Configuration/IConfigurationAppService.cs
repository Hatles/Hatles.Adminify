using System.Threading.Tasks;
using Hatles.Adminify.Configuration.Dto;

namespace Hatles.Adminify.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
