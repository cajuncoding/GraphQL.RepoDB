# nullable enable

using HotChocolate.Language;
using HotChocolate.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.ResolverProcessingExtensions.Arguments
{
    public static class IResolverContextArgumentExtensions
    {
        /// <summary>
        /// Retrieves all possible Argument names as defined on the Schema; which may or may not have
        /// been provided in the current query.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<string> AllArgumentSchemaNamesSafely(this IResolverContext? context)
        {
            var argsMap = context?.Selection?.Field?.Arguments;
            var argNames = argsMap?.Select(arg => arg.Name);
            return argNames ?? new List<string>();
        }

        /// <summary>
        /// Retrieve all possible Argument Literal values based on valid names from the Schema, and initializing the values
        /// from the current Context; only arguments with non-null values will be returned.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<IArgumentValue> AllArgumentsSafely(this IResolverContext? context)
        {
            var argNames = AllArgumentSchemaNamesSafely(context);

            var argPairs = argNames.Select(n =>
                {
                    try
                    {
                        var argLiteral = context?.ArgumentLiteral<IValueNode>(n);
                        var argValue = argLiteral != null && argLiteral.Location != null && argLiteral.Value != null
                            ? argLiteral.Value
                            : null;

                        return new ArgumentValue(n, argValue);
                    }
                    catch
                    {
                        //Do nothing, if exception is thrown, as Argument just doesn't exist as expected.
                    }

                    return new ArgumentValue(n, null);
                }
            )
            .Where(a => a.Value != null);

            return argPairs;
        }

        /// <summary>
        /// Get an Argument for the specified Name & Type safely; null (default) value will be returned instead
        /// of exceptions being thrown.
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="context"></param>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static TArg ArgumentValueSafely<TArg>(this IResolverContext? context, string argName)
        {
            try
            {
                return context != null
                    ? context.ArgumentValue<TArg>(argName)
                    : default!;
            }
            catch
            {
                return default!;
            }
        }
    }
}
