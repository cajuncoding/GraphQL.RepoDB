using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Language;
using HotChocolate.PreProcessingExtensions.Arguments;
using HotChocolate.PreProcessingExtensions.Selections;
using HotChocolate.Types;

namespace HotChocolate.PreProcessingExtensions
{
    public class GraphQLParamsContext : IParamsContext
    {
        protected IResolverContext _resolverContext;
        protected List<string> _argumentSchemaNames;
        protected List<IArgumentValue> _argumentsAvailable;
        protected CursorPagingArguments? _cursorPagingArgs;
        protected OffsetPagingArguments? _offsetPagingArgs;
        protected IReadOnlyList<ISortOrderField> _sortArgs;
        protected IReadOnlyList<IPreProcessingSelection> _selectionFields;
        protected IReadOnlyList<PreProcessingDependencyLink> _selectionDependencies;

        public GraphQLParamsContext(IResolverContext resolverContext)
        {
            _resolverContext = resolverContext ?? throw new ArgumentNullException(nameof(resolverContext));
        }

        public virtual IResolverContext ResolverContext => _resolverContext;
        
        public virtual IReadOnlyList<string> AllArgumentSchemaNames
            //When retrieving only Names, we should take all Distinct values...
            => _argumentSchemaNames ??= _resolverContext.AllArgumentSchemaNamesSafely().ToList();

        public virtual IReadOnlyList<IArgumentValue> AllArguments
            //When retrieving only Names, we should take all Distinct values...
            => _argumentsAvailable ??= _resolverContext.AllArgumentsSafely().ToList();

        public virtual IReadOnlyList<IPreProcessingSelection> AllSelectionFields 
            => _selectionFields ??= _resolverContext.GetPreProcessingSelections();

        public virtual IReadOnlyList<string> AllSelectionNames
            //When retrieving only Names, we should take all Distinct values...
            => AllSelectionFields?.Select(s => s.SelectionName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        public virtual IReadOnlyList<PreProcessingDependencyLink> SelectionDependencies
            //When retrieving only Names, we should take all Distinct values...
            => _selectionDependencies ??= GatherDependencyLinks();

        public virtual IReadOnlyList<IPreProcessingSelection> GetSelectionFieldsFor<TObjectType>()
            => AllSelectionFields?.Where(s => typeof(TObjectType).IsAssignableFrom(s.GraphQLObjectType.RuntimeType)).ToList();

        public virtual IReadOnlyList<ISortOrderField> SortArgs 
            => _sortArgs ??= _resolverContext.GetSortingArgsSafely();

        public virtual CursorPagingArguments PagingArgs => CursorPagingArgs;

        public virtual CursorPagingArguments CursorPagingArgs
            => _cursorPagingArgs ??= LoadCursorPagingArgsHelper();

        public virtual OffsetPagingArguments OffsetPagingArgs
            => _offsetPagingArgs ??= LoadOffsetPagingArgsHelper();


        /// <summary>
        /// Get the selection names mapped to underlying class property/member id values, and include
        /// exclude specified selection names based on flags specified (e.g. SelectionNames, DependencyNames, All).
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSelectionMappedNames(SelectionNameFlags flags = SelectionNameFlags.All)
        {
            var results = GatherSelectionNamesInternal(AllSelectionFields, flags);
            return results;
        }

        /// <summary>
        /// Get the selection names mapped to underlying class property/member id values, and include
        /// exclude specified selection names based on flags specified (e.g. SelectionNames, DependencyNames, All).
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSelectionMappedNamesFor<TObjectType>(SelectionNameFlags flags = SelectionNameFlags.All)
        {
            var results = GatherSelectionNamesInternal(GetSelectionFieldsFor<TObjectType>(), flags);
            return results;
        }

        protected CursorPagingArguments LoadCursorPagingArgsHelper()
        {
            var cursorPagingArgs = _resolverContext.GetCursorPagingArgsSafely();
            return cursorPagingArgs.IsPagingArgumentsValid()
                    ? cursorPagingArgs
                    : new CursorPagingArguments();
        }

        protected OffsetPagingArguments LoadOffsetPagingArgsHelper()
        {
            var offsetPagingArgs = _resolverContext.GetOffsetPagingArgsSafely();
            return offsetPagingArgs.IsPagingArgumentsValid()
                    ? offsetPagingArgs
                    : new OffsetPagingArguments(-1, -1);
        }

        protected IEnumerable<string> GatherSelectionNamesInternal(IEnumerable<IPreProcessingSelection> baseEnumerable, SelectionNameFlags flags)
        {
            var results = new List<string>();

            if (flags.HasFlag(SelectionNameFlags.SelectedNames))
            {
                var selectionMemberNames = baseEnumerable?.Select(s => s.SelectionMemberNameOrDefault);
                if (selectionMemberNames != null)
                    results.AddRange(selectionMemberNames);
            }

            if (flags.HasFlag(SelectionNameFlags.DependencyNames))
            {
                var selectionDependencies = SelectionDependencies;
                if (selectionDependencies != null)
                    results.AddRange(selectionDependencies.Select(d => d.DependencyMemberName));
            }

            //When retrieving only Names, we should take all Distinct values...
            return results.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        protected IReadOnlyList<PreProcessingDependencyLink> GatherDependencyLinks()
        {
            if (AllSelectionFields == null)
                return null;

            var results = new List<PreProcessingDependencyLink>();
            foreach (var selectionField in AllSelectionFields)
            {
                var contextData = selectionField?.GraphQLFieldSelection?.Field?.ContextData;
                if (contextData?.ContainsKey(PreProcessingParentDependencies.ContextDataKey) == true)
                {
                    var dependencyLinks = (IEnumerable<PreProcessingDependencyLink>)contextData[PreProcessingParentDependencies.ContextDataKey];
                    results.AddRange(dependencyLinks);
                }
            }

            return results;
        }

    }
}
