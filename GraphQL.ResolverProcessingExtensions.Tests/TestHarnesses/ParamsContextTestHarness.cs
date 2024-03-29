﻿using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate.Language;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.ResolverProcessingExtensions.Arguments;
using HotChocolate.ResolverProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    public class ParamsContextTestHarness : IParamsContext
    {

        public ParamsContextTestHarness(IParamsContext paramsContext)
        {
            //BBernard
            //THIS will force initialization of all data for Test cases
            //  to then have access to even if out of scope, since we have our own
            //  references here!
            this.ResolverContext = paramsContext.ResolverContext;
            this.AllArgumentSchemaNames = paramsContext.AllArgumentSchemaNames;
            this.AllArguments = paramsContext.AllArguments;
            this.AllSelectionFields = paramsContext.AllSelectionFields;
            this.AllSelectionNames = paramsContext.AllSelectionNames;
            this.SelectionDependencies = paramsContext.SelectionDependencies;
            this.SortArgs = paramsContext.SortArgs;
            this.PagingArgs = paramsContext.PagingArgs;
            this.CursorPagingArgs = paramsContext.CursorPagingArgs;
            this.OffsetPagingArgs = paramsContext.OffsetPagingArgs;
            this.TotalCountSelection = paramsContext.TotalCountSelection;
            this.IsTotalCountRequested = paramsContext.IsTotalCountRequested;
        }

        public IResolverContext ResolverContext { get; }

        public IReadOnlyList<string> AllArgumentSchemaNames { get; }
        public IReadOnlyList<IArgumentValue> AllArguments { get; }
        public IReadOnlyList<IResolverProcessingSelection> AllSelectionFields { get; }
        public IReadOnlyList<string> AllSelectionNames { get; }
        public IReadOnlyList<ResolverProcessingDependencyLink> SelectionDependencies { get; }
        public IReadOnlyList<ISortOrderField> SortArgs { get; }
        public void SetSortingIsHandled(bool isHandled = true)
        {
            throw new NotImplementedException();
        }

        public CursorPagingArguments PagingArgs { get; }
        public CursorPagingArguments CursorPagingArgs { get; }
        public OffsetPagingArguments OffsetPagingArgs { get; }
        public ResolverProcessingSelection TotalCountSelection { get; }
        public bool IsTotalCountRequested { get; }

        public IReadOnlyList<IResolverProcessingSelection> GetSelectionFieldsFor<TObjectType>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSelectionMappedNames(SelectionNameFlags flags = SelectionNameFlags.DependencyNames)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSelectionMappedNamesFor<TObjectType>(SelectionNameFlags flags = SelectionNameFlags.DependencyNames)
        {
            throw new NotImplementedException();
        }

    }
}
