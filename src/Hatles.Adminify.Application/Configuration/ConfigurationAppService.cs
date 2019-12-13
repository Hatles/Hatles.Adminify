using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Hatles.Adminify.Configuration.Dto;

namespace Hatles.Adminify.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AdminifyAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
