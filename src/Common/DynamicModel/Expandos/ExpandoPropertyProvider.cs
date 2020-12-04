using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Common.DynamicModel.Expandos
{
    /// <summary>
    /// 动态属性的上下文
    /// </summary>
    public class ExpandoPropertyContext<TResult>// where TResult : Expando
    {
        public ExpandoPropertyContext(TResult data)
        {
            Result = data;
        }
        
        /// <summary>
        /// 上下文的结果
        /// </summary>
        public TResult Result { get; set; }
        
        /// <summary>
        /// 上下文的状态
        /// </summary>
        public dynamic State { get; set; } = new ExpandoObject();

        /// <summary>
        /// 应用动态属性的提供者列表，按需提供和调用即可
        /// </summary>
        /// <param name="expandoPropertyProviders"></param>
        /// <returns></returns>
        public async Task ApplyProviders(IEnumerable<IExpandoPropertyProvider<TResult>> expandoPropertyProviders)
        {
            if (expandoPropertyProviders == null)
            {
                return;
            }

            foreach (var provider in expandoPropertyProviders)
            {
                await provider.Process(this);
            }
        }

        /// <summary>
        /// Create ExpandoPropertyContext Factory
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ExpandoPropertyContext<TResult> Create(TResult result)
        {
            var context = new ExpandoPropertyContext<TResult>(result);
            return context;
        }
    }

    /// <summary>
    /// 动态属性的提供者接口
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IExpandoPropertyProvider<TResult>// where TResult : Expando
    {
        /// <summary>
        /// 动态属性上下文的处理逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Process(ExpandoPropertyContext<TResult> context);
    }
}
