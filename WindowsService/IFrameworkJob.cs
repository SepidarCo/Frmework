using System.Threading.Tasks;
using Quartz;

namespace Sepidar.WindowsService
{
    [DisallowConcurrentExecution]
    public abstract class IFrameworkJob :IJob
    {
        public abstract Task Execute(IJobExecutionContext context);

        public void Interrupt()
        {
        }

    }
}
