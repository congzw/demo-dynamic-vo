using System.Threading.Tasks;

namespace Common.AppMessages
{
    public interface IAppMessageNotifier
    {
        void Notify(AppMessage message);
        Task NotifyAsync(AppMessage message);
    }

    public class AppMessage
    {
        /// <summary>
        /// 消息的唯一Id（可用于重复判断）
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 消息类型码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }
    }
}