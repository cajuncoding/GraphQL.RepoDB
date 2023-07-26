using HotChocolate.ResolverProcessingExtensions.Pagination;
using HotChocolate.ResolverProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Data.Sorting;
using HotChocolate.ResolverProcessingExtensions.Arguments;
using HotChocolate.ResolverProcessingExtensions.Selections;

namespace HotChocolate.ResolverProcessingExtensions
{
    public class GraphQLParamsContext : IParamsContext
    {
        protected IResolverContext _resolverContext;
        protected List<string> _argumentSchemaNames;
        protected List<IArgumentValue> _argumentsAvailable;
        protected CursorPagingArguments? _cursorPagingArgs;
        protected OffsetPagingArguments? _offsetPagingArgs;
        protected IReadOnlyList<ISortOrderField> _sortArgs;
        protected IReadOnlyList<IResolverProcessingSelection> _selectionFields;
        protected IReadOnlyList<string> _selectionNames;
        protected IReadOnlyList<ResolverProcessingDependencyLink> _selectionDependencies;

        //It's possible and common for TotalCount selection to not be defined (e.g. null), therefore we must support null value as valid state
        //  while also preventing unnecessary lookups for performance; so we track state with an initialization boolean.
        private bool _isTotalCountSelectionInitialized = false;
        protected ResolverProcessingSelection _totalCountSelectionField;

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

        public virtual IReadOnlyList<IResolverProcessingSelection> AllSelectionFields 
            => _selectionFields ??= _resolverContext.GetResolverProcessingSelections();

        public virtual IReadOnlyList<string> AllSelectionNames
            //When retrieving only Names, we should take all Distinct values...
            => _selectionNames ??= AllSelectionFields?.Select(s => s.SelectionName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        public virtual IReadOnlyList<ResolverProcessingDependencyLink> SelectionDependencies
            //When retrieving only Names, we should take all Distinct values...
            => _selectionDependencies ??= GatherDependencyLinks();

        public virtual IReadOnlyList<IResolverProcessingSelection> GetSelectionFieldsFor<TObjectType>()
            => AllSelectionFields?.Where(s => typeof(TObjectType).IsAssignableFrom(s.RuntimeType)).ToList();

        public virtual IReadOnlyList<ISortOrderField> SortArgs 
            => _sortArgs ??= _resolverContext.GetSortingArgsSafely();

        public virtual void SetSortingIsHandled(bool isHandled = true)
            => _resolverContext.GetSortingContext()?.Handled(isHandled);

        public virtual CursorPagingArguments PagingArgs => CursorPagingArgs;

        public virtual CursorPagingArguments CursorPagingArgs
            => _cursorPagingArgs ??= LoadCursorPagingArgsHelper();

        public virtual OffsetPagingArguments OffsetPagingArgs
            => _offsetPagingArgs ??= LoadOffsetPagingArgsHelper();


        public virtual ResolverProcessingSelection TotalCountSelection
        {
            get
            {
                //It's possible and common for TotalCount selection to not be defined (e.g. null), therefore we must support null value as valid state
                //  while also preventing unnecessary lookups for performance; so we track state with an initialization boolean.
                if (!_isTotalCountSelectionInitialized)
                {
                    _isTotalCountSelectionInitialized = true;
                    _totalCountSelectionField ??= _resolverContext.GetTotalCountSelectionField();
                }

                return _totalCountSelectionField;
            }
        }

        public virtual bool IsTotalCountRequested => TotalCountSelection != null;


        /// <summary>
        /// Get the selection names mapped to underlying class property/member id values, and include
        /// exclude specified selection names based on flags specified (e.g. SelectionNames, DependencyNames, All).
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> GetSelectionMappedNames(SelectionNameFlags flags = SelectionNameFlags.All)
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
        public virtual IEnumerable<string> GetSelectionMappedNamesFor<TObjectType>(SelectionNameFlags flags = SelectionNameFlags.All)
        {
            var results = GatherSelectionNamesInternal(GetSelectionFieldsFor<TObjectType>(), flags);
            return results;
        }

        protected virtual CursorPagingArguments LoadCursorPagingArgsHelper()
        {
            var cursorPagingArgs = _resolverContext.GetCursorPagingArgsSafely();
            return cursorPagingArgs;
        }

        protected virtual OffsetPagingArguments LoadOffsetPagingArgsHelper()
        {
            var offsetPagingArgs = _resolverContext.GetOffsetPagingArgsSafely();
            return offsetPagingArgs;
        }

        protected virtual IEnumerable<string> GatherSelectionNamesInternal(IEnumerable<IResolverProcessingSelection> baseEnumerable, SelectionNameFlags flags)
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

        protected virtual IReadOnlyList<ResolverProcessingDependencyLink> GatherDependencyLinks()
        {
            if (AllSelectionFields == null)
                return null;

            var results = new List<ResolverProcessingDependencyLink>();
            foreach (var selectionField in AllSelectionFields)
            {
                var contextData = selectionField?.graphqlFieldSelection?.Field?.ContextData;
                if (contextData?.ContainsKey(ResolverProcessingParentDependencies.ContextDataKey) == true
                    && contextData[ResolverProcessingParentDependencies.ContextDataKey] is IEnumerable<ResolverProcessingDependencyLink> dependencyLinks)
                {
                    results.AddRange(dependencyLinks);
                }
            }

            return results;
        }

    }
}
