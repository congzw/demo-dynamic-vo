namespace Common.DynamicModel.Expandos
{
    //public class DynamicObject : System.Dynamic.IDynamicMetaObjectProvider
    //public sealed class ExpandoObject : ICollection<KeyValuePair<string, object>>, IDictionary<string, object>, IEnumerable<KeyValuePair<string, object>>,
    //INotifyPropertyChanged, IDynamicMetaObjectProvider

    //public class DynamicObject : IDynamicMetaObjectProvider
    //{
    //    protected DynamicObject();
    //    public virtual bool TryGetMember(GetMemberBinder binder, out object result);
    //    public virtual bool TrySetMember(SetMemberBinder binder, object value);
    //    public virtual bool TryDeleteMember(DeleteMemberBinder binder);
    //    public virtual bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result);
    //    public virtual bool TryConvert(ConvertBinder binder, out object result);
    //    public virtual bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result);
    //    public virtual bool TryInvoke(InvokeBinder binder, object[] args, out object result);
    //    public virtual bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result);
    //    public virtual bool TryUnaryOperation(UnaryOperationBinder binder, out object result);
    //    public virtual bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result);
    //    public virtual bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value);
    //    public virtual bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes);
    //    public virtual IEnumerable<string> GetDynamicMemberNames();
    //    public virtual DynamicMetaObject GetMetaObject(Expression parameter);
    //}

    
    //public class Employee : DynamicObject
    //{
    //    private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

    //    public override bool TryGetMember(GetMemberBinder binder, out object result)
    //    {
    //        if (_properties.ContainsKey(binder.Name))
    //        {
    //            result = _properties[binder.Name];
    //            return true;
    //        }
    //        else
    //        {
    //            result = "Invalid Property!";
    //            return false;
    //        }
    //    }

    //    public override bool TrySetMember(SetMemberBinder binder, object value)
    //    {
    //        _properties[binder.Name] = value;
    //        return true;
    //    }

    //    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    //    {
    //        dynamic method = _properties[binder.Name];
    //        result = method(args[0].ToString(), args[1].ToString());
    //        return true;
    //    }
    //}
}
