using System;

namespace RepoDb.SqlServer.PagingOperations.Reflection
{
    internal class RepoDbQueryGroupProxy
    {
        //BBernard
        //We ONLY ever initialize the Method Definition one time for performance!
        //NOTE: Our method Delegate is a Func<> that matches the signature of the Method we are looking for:
        //      internal static QueryGroupTypeMap MapTo(this QueryGroup queryGroup, Type entityType)
        protected static readonly Delegate _mapToQueryGroupTypeMapProxy;

        //We ONLY ever initialize the Method Definition one time for performance!
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
            _mapToQueryGroupTypeMapProxy = StaticReflectionHelper.FindStaticMethod(
                repoDbAssembly.GetType("RepoDb.Extensions.QueryGroupExtension"),
                "MapTo",
                //Types Stubbed for matching with object placeholder for internal unknown type...
                //Signature: QueryGroupTypeMap MapTo(this QueryGroup queryGroup, Type entityType)
                new Type[] { typeof(QueryGroup), typeof(Type) }
            )?.CreateDynamicDelegate();

            _asMappedParamObjectProxy = StaticReflectionHelper.FindStaticMethod(
                repoDbAssembly.GetType("RepoDb.QueryGroup"),
                "AsMappedObject",
                //Types Stubbed for matching with object placeholder for internal unknown type...
                //Signature: AsMappedObject(QueryGroupTypeMap[] queryGroupTypeMaps, bool fixParameters = true)
                //NOTE: QueryGroupTypeMap is an internal type so we can't use it directly.
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
            var queryGroupTypeMap = _mapToQueryGroupTypeMapProxy.DynamicInvoke(queryGroup, typeof(TEntity));

            //BBernard
            //Create the Array of QueryGroupTypeMap[] objects which is the parameter type expected; but since this is unknown
            //  we must use Reflection to instantiate it with full late binding based on the result of the MapTo(QueryGroup, Type) result above!
            //NOTE: This is needed because the delegate params unknown / internal until runtime are: AsMappedObject(QueryGroupTypeMap[], bool)
            var queryGroupTypeMapArrayArg = Array.CreateInstance(queryGroupTypeMap.GetType(), 1);
            queryGroupTypeMapArrayArg.SetValue(queryGroupTypeMap, 0);

            //To ensure we match the Signature we specify all params, even optional ones.
            bool fixParametersArg = true;
            var paramsObject = _asMappedParamObjectProxy.DynamicInvoke(queryGroupTypeMapArrayArg, fixParametersArg);

            return paramsObject;
        }

    }
}
