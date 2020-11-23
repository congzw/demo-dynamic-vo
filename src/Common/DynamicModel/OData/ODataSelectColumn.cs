using System.Collections.ObjectModel;

namespace Common.DynamicModel.OData
{
    public class ODataSelectColumn
    {
        /// <summary>
        /// 所有属性
        /// </summary>
        public bool AllColumns { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 子属性
        /// </summary>
        public ODataSelectColumnCollection SubColumns { get; set; }

    }

    public class ODataSelectColumnCollection : Collection<ODataSelectColumn>
    {
        /// <summary>
        /// * 为所有属性
        /// </summary>
        public bool AllColumns { get; set; }
        /// <summary>
        /// 没有$select 属性为默认实体对应的属性 如果没有声明实体 
        /// 就提供vo自身所有属性
        /// </summary>
        public bool DefaultColumns { get; set; }
        /// <summary>
        /// 请求中是否包含Record 
        /// 默认 * 取不到Record
        /// </summary>
        public bool Record { get; set; }
    }
}
