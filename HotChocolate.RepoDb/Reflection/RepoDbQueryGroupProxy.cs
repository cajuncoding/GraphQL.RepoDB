using RepoDb;
using System;
using System.Collections.Generic;
using System.Reflection.CustomExtensions;
using System.Text;

namespace HotChocolate.RepoDb.SqlServer.Reflection
{
    public class RepoDbQueryGroupProxy
    {
        //BBernard
        //We ONLY ever intialize the Method Definition one time for performance!
        //NOTE: Our method Delegate is a Func<> that matches the signature of the Method we are looking for:
        //      QueryGroupTypeMap MapTo<TEntity>(this QueryGroup queryGroup)
        protected static readonly Func<QueryGroup, Type, object> _mapToQueryGroupTypeMapProxy;

        //We ONLY ever intialize the Method Definition one time for performance!
        //NOTE: Our method Delegate is not a full Stub here because of Unknown internal types, so we can't use the stub
        //  to find it and instead leave this as a generic Delegate so that it will reference any result 
        //  (even with unknown internal type parameters)a Func<> that matches the signature of the Method we are looking for exactly:
        //      internal static object AsMappedObject(QueryGroupTypeMap[] queryGroupTypeMaps, bool fixParameters = true)
        protected static readonly Delegate _asMappedParamObjectProxy;

        static RepoDbQueryGroupProxy()
        {
            var repoDbAssembly = typeof(QueryGroup).Assembly;

            //BBernard
            //Safely try to resolve a Proxy for 'GetDataReaderToDataEntityFunction' which is the method in RepoDb v1.12.4
            //Find the method via Reflection and compile into a local Expression Delegate for performance!
            _mapToQueryGroupTypeMapProxy = StaticReflectionHelper.FindStaticMethodForDelegate(
                repoDbAssembly.GetType("RepoDb.Extensions.QueryGroupExtension"),
                "MapTo",
                _mapToQueryGroupTypeMapProxy
            )?.CreateDynamicDelegate(
                _mapToQueryGroupTypeMapProxy
            );

            _asMappedParamObjectProxy = StaticReflectionHelper.FindStaticMethod(
                repoDbAssembly.GetType("RepoDb.QueryGroup"),
                "AsMappedObject",
                //Types Stubbed for matching with object placeholder for internal unknown type...
                new Type[] { typeof(object), typeof(bool) } 
            )?.CreateDynamicDelegate();

            //Now Validate that we have at least one Proxy... or else throw an Exception Early
            if (_mapToQueryGroupTypeMapProxy == null || _asMappedParamObjectProxy == null)
                throw new Exception(
                    $"Could not initialize the [{nameof(RepoDbQueryGroupProxy)}];" +
                    $" the methods to be proxied for [{nameof(GetMappedParamsObject)}] is null and/or could not be found." +
                    $" This could be related to change in RepoDb version (source code has changed); please verify the version or update" +
                    $" this method to account for any differences."
                );
        }

        public static object GetMappedParamsObject<TEntity>(QueryGroup queryGroup)
        {
            var queryGroupTypeMap = _mapToQueryGroupTypeMapProxy(queryGroup, typeof(TEntity));

            var array = Array.CreateInstance(queryGroupTypeMap.GetType(), 1);
            array.SetValue(queryGroupTypeMap, 0);

            var paramsObject = _asMappedParamObjectProxy.DynamicInvoke(array, true);
            
            return paramsObject;
        }

    }
}
