namespace Common.DynamicModel.OData
{
	public  class ODataQueryResult
	{
        internal ODataQueryResult()
		{
        }
        public object Filter { get; set; }
        public object Order { get; set; }
        public int? Top { get; set; }
        public int Skip { get; set; }
        public ODataSelectColumnCollection Select { get; set; }
        public object Format { get; set; }
    }
}
