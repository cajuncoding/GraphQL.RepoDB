using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.PreProcessingExtensions
{
    public class GraphQLParamsContext : IParamsContext
    {
        protected IResolverContext _resolverContext;
        protected CursorPagingArguments? _pagingArgs;
        protected IReadOnlyList<ISortOrderField> _sortArgs;
        protected IReadOnlyList<IPreProcessingSelection> _selectionFields;
        protected IReadOnlyList<PreProcessingDependencyLink> _selectionDependencies;

        public GraphQLParamsContext(IResolverContext resolverContext)
        {
            _resolverContext = resolverContext ?? throw new ArgumentNullException(nameof(resolverContext));
        }

        public virtual IResolverContext ResolverContext => _resolverContext;
        
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
        
        //TODO: TEST lazy loading for Struct Type CursorPagingArguments...
        public virtual CursorPagingArguments PagingArgs 
            => _pagingArgs ??= _resolverContext.GetCursorPagingArgsSafely();

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
