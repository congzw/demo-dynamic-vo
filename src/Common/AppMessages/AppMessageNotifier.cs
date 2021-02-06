using System.Threading.Tasks;

namespace Common.AppMessages
{
    public class AppMessageNotifier : IAppMessageNotifier
    {
        public Task NotifyAsync(AppMessage message)
        {
            //todo: logger and socket notify
            return Task.CompletedTask;
        }

        public void Notify(AppMessage message)
        {
            //todo: logger and socket notify
        }
    }
}