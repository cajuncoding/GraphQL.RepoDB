using System;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;

namespace RepoDb.SqlServer.PagingOperations.Reflection
{
    /// <summary>
    /// A Proxy Class for the RepoDb FunctionCache static class.  This provides high performance access 
    /// to the sealed/internal methods in the static class that otherwise is not-accessible.
    /// NOTE: This should ONLY be used as a brute force extension, but otherwise functionality that can be
    ///         migrated into official code base or exposed publicly is preferred.
    /// </summary>
    internal class RepoDbFunctionCacheProxy<TEntity>
    {
        //BBernard
        //We ONLY ever initialize the Method Definition one time for performance!
        //NOTE: Our method Delegate is a Func<> that matches the signature of the Method we are looking for exactly:
        //      Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(
        //          DbDataReader reader,
        //          IDbConnection connection,
        //          IDbTransaction transaction,
        //          bool basedOnFields = false)
        //          where TEntity : class
        protected static readonly
            Func<DbDataReader, IDbConnection, IDbTransaction, bool, Func<DbDataReader, TEntity>>
            _getDataReaderToDataEntityFunctionProxy;

        //BBernard
        //We ONLY ever initialize the Method Definition one time for performance!
        //NOTE: Our method Delegate is a Func<> that matches the signature of the Method we are looking for exactly:
        //      internal static Func<DbDataReader, TResult> GetDataReaderToTypeCompiledFunction<TResult>(
        //          DbDataReader reader,
        //          IEnumerable<DbField> dbFields = null,
        //          IDbSetting dbSetting = null)

        protected static readonly
            Func<DbDataReader, DbFieldCollection, IDbSetting, Func<DbDataReader, TEntity>>
            _getDataReaderToTypeCompiledFunctionProxy;

        /// <summary>
        /// BBernard
        /// This Static initializer ensures that the Delegate is created as a SINGLETON for each and every
        /// Generic type of this class that is initialized.  Therefore for each Generic type, this initialization
        /// will only every run one time in the thread safe Static Initializer for each type improving performance 
        /// for ALL future calls of that Generic Type.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        static RepoDbFunctionCacheProxy()
        {
            var repoDbAssembly = typeof(DbConnectionExtension).Assembly;
            var functionCacheType = repoDbAssembly.GetType("RepoDb.FunctionCache");

            //BBernard
            //Safely try to resolve a Proxy for 'GetDataReaderToDataEntityFunction' which is the method used in (older) RepoDb v1.12.4 and earlier...
            //Find the method via Reflection and compile into a local Expression Delegate for performance!
            _getDataReaderToDataEntityFunctionProxy = StaticReflectionHelper.FindStaticMethodForDelegate(
                functionCacheType,
                "GetDataReaderToDataEntityFunction",
                _getDataReaderToDataEntityFunctionProxy
            )?.CreateDynamicDelegate(
                _getDataReaderToDataEntityFunctionProxy, 
                typeof(TEntity)
            );

            //BBernard
            //Safely try to resolve a Proxy for 'GetDataReaderToTypeCompiledFunction' which is the method in (newer) RepoDb v1.13.1+ and later...
            //Find the method via Reflection and compile into a local Expression Delegate for performance!
            _getDataReaderToTypeCompiledFunctionProxy = StaticReflectionHelper.FindStaticMethodForDelegate(
               functionCacheType,
               "GetDataReaderToTypeCompiledFunction",
               _getDataReaderToTypeCompiledFunctionProxy
            )?.CreateDynamicDelegate(
                _getDataReaderToTypeCompiledFunctionProxy,
                typeof(TEntity)
            );

            //Now Validate that we have at least one Proxy... or else throw an Exception Early
            if(_getDataReaderToTypeCompiledFunctionProxy == null && _getDataReaderToDataEntityFunctionProxy == null)
                throw new InvalidOperationException(
                    $"Could not initialize the [{nameof(RepoDbFunctionCacheProxy<TEntity>)}];" +
                    $" the method to be proxied for [{nameof(GetDataReaderToDataEntityFunctionSafely)}] is null and/or could not be found." +
                    $" This could be related to change in RepoDb version (source code has changed); please verify the version or update" +
                    $" this method to account for any differences."
                );
        }

        /// <summary>
        /// Simple Constructor
        /// </summary>
        public RepoDbFunctionCacheProxy()
        {
        }

        /// <summary>
        /// Retrieve the compiled entity mapping function from RepoDb internal FunctionCache, in a compatible
        /// way for version 1.12.4 or the master branch as of 10/21/2020 - yet to be released version.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="basedOnFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public virtual Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunctionSafely(
            DbDataReader reader,
            IDbConnection connection,
            IDbTransaction transaction,
            bool basedOnFields = false,
            DbFieldCollection dbFieldCollection = null,
            IDbSetting dbSetting = null
        )
        {
            //First try the current function for RepoDb v1.12.4
            //Second try the updated Function (as of 10/18/2020) for upcoming RepoDb release...
            var funcResult = _getDataReaderToTypeCompiledFunctionProxy?.Invoke(reader, dbFieldCollection, dbSetting)
                                ?? _getDataReaderToDataEntityFunctionProxy?.Invoke(reader, connection, transaction, basedOnFields);

            return funcResult;
        }
    }
}
