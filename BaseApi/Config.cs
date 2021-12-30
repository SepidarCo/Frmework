using Microsoft.Extensions.DependencyInjection;
using Sepidar.BlobManagement;

namespace Sepidar.BaseApi
{
    public class Config : Sepidar.Framework.Config
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddScoped<BlobRepository<DiskBlobStorage>, DiskBlobRepository>();
            services.AddScoped<BlobRepository<DatabaseBlobStorage>, DatabaseBlobRepository>();
            services.AddScoped<IMediaBusiness, MediaBusiness>();
        }
    }
}
