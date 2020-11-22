using System;
using System.Dynamic;

namespace Common.DynamicModel.Expandos.Helpers
{
    /// <summary>
    /// Null exception preventer. This allows for hassle-free usage of configuration values that are not
    /// defined in the config file. I.e. we can do Config.Scope.This.Field.Does.Not.Exist.Ever, and it will
    /// not throw an NullPointer exception, but return te NullExceptionPreventer object instead.
    /// 
    /// The NullExceptionPreventer can be cast to everything, and will then return default/empty value of
    /// that dataType.
    /// </summary>
    public class NullExceptionPreventer : DynamicObject
    {
        // all member access to a NullExceptionPreventer will return a new NullExceptionPreventer
        // this allows for infinite nesting levels: var s = Obj1.foo.bar.bla.blubb; is perfectly valid
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return base.TrySetMember(binder, value);
        }

        // Add all kinds of data types we can cast it to, and return default values
        // cast to string will be null
        public static implicit operator string(NullExceptionPreventer nep)
        {
            return null;
        }
        public override string ToString()
        {
            return null;
        }
        public static implicit operator string[](NullExceptionPreventer nep)
        {
            return new string[] { };
        }
        // cast to bool will always be false
        public static implicit operator bool(NullExceptionPreventer nep)
        {
            return false;
        }
        public static implicit operator bool[](NullExceptionPreventer nep)
        {
            return new bool[] { };
        }
        public static implicit operator int[](NullExceptionPreventer nep)
        {
            return new int[] { };
        }
        public static implicit operator long[](NullExceptionPreventer nep)
        {
            return new long[] { };
        }
        public static implicit operator int(NullExceptionPreventer nep)
        {
            return 0;
        }
        public static implicit operator long(NullExceptionPreventer nep)
        {
            return 0;
        }
        // nullable types always return null
        public static implicit operator bool?(NullExceptionPreventer nep)
        {
            return null;
        }
        public static implicit operator int?(NullExceptionPreventer nep)
        {
            return null;
        }
        public static implicit operator long?(NullExceptionPreventer nep)
        {
            return null;
        }
        public static implicit operator Guid?(NullExceptionPreventer nep)
        {
            return null;
        }
        public static implicit operator Guid(NullExceptionPreventer nep)
        {
            return Guid.Empty;
        }
    }
}
