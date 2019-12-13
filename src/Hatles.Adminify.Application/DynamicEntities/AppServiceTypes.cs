using Abp.Application.Services;

namespace Hatles.Adminify.DynamicEntities
{
    public static class AppServiceTypes
    {
        public static AutoAppServiceTypesAttribute Default { get; }

        static AppServiceTypes()
        {
            Default = new AutoAppServiceTypesAttribute(
                typeof(AdminifyAsyncCrudAppService<,>),
                typeof(AdminifyAsyncCrudAppService<,,>));

        }
    }
}