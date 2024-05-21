using Microsoft.Extensions.DependencyInjection;

namespace altium.sorter
{
    public static class DependecyInjection
    {
        public static void AddFileSorter(this IServiceCollection services)
        {
            services.AddSingleton<IFileSorter, FileSorter>();
        }
    }
}