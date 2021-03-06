﻿using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels.HierarchyNode;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.CustomLinqExtensions
{
    public static class IEnumerableExtensions
    {
        public delegate void InjectorDelegate<TDom, TView>(TDom domModel, TView viewModel);

        public static async Task<IEnumerable<HierarchyNodesViewModel<TView>>> ProjectToViewAsync<TDom, TView>(this IEnumerable<HierarchyNode<TDom>> query, InjectorDelegate<TDom, TView> entityInjector)
            where TView : class, new()
            where TDom : class
        {
            List<HierarchyNodesViewModel<TView>> viewList = new List<HierarchyNodesViewModel<TView>>();
            await ConstructTreeAsync(query, viewList, entityInjector).ConfigureAwait(false);
            return viewList;
        }
        private static async Task ConstructTreeAsync<TDom, TView>(IEnumerable<HierarchyNode<TDom>> dataModel, List<HierarchyNodesViewModel<TView>> viewList, InjectorDelegate<TDom, TView> EntityInjector)
            where TView : class, new()
            where TDom : class
        {
            if (dataModel != null)
            {
                foreach (HierarchyNode<TDom> item in dataModel)
                {
                    HierarchyNodesViewModel<TView> hierarchNodesViewModel = new HierarchyNodesViewModel<TView>();
                    //hnVM.ImageList = new List<string>();
                    //hnVM.ImageList.Add("/Content/images/reply-new.gif");
                    //hnVM.Parent = parent;
                    //hnVM.Depth = item.Depth;
                    hierarchNodesViewModel.InjectFrom(item);
                    hierarchNodesViewModel.Entity = new TView();
                    hierarchNodesViewModel.Entity.InjectFrom(item.Entity);

                    // Call injectordelegate here
                    InjectorDelegate<TDom, TView> handler = new InjectorDelegate<TDom, TView>(EntityInjector);
                    handler.Invoke(item.Entity, hierarchNodesViewModel.Entity);

                    viewList.Add(hierarchNodesViewModel);
                    hierarchNodesViewModel.ChildNodes = new List<HierarchyNodesViewModel<TView>>();
                    if (item.ChildNodes.Any())
                    {
                        await AppendChildrenAsync(item.ChildNodes, (List<HierarchyNodesViewModel<TView>>)hierarchNodesViewModel.ChildNodes, hierarchNodesViewModel.Entity, false, handler).ConfigureAwait(false);
                    }
                }
            }
        }
        public static async Task AppendChildrenAsync<TView, TDom>(IEnumerable<HierarchyNode<TDom>> dataModel, List<HierarchyNodesViewModel<TView>> viewList, TView parent, bool hasSiblings, InjectorDelegate<TDom, TView> handler)
            where TView : class, new()
            where TDom : class
        {
            bool hasSibbs = false;

            if (dataModel.Count() > 1)
            {
                hasSibbs = true;
            }

            if (dataModel != null)
            {
                foreach (HierarchyNode<TDom> item in dataModel)
                {
                    HierarchyNodesViewModel<TView> hierarchyNodesViewModel = new HierarchyNodesViewModel<TView>
                    {
                        ImageList = new List<string>
                {
                    "/Content/images/reply-new.gif"
                }
                    };

                    if (item == dataModel.Last())
                    {
                        hasSibbs = false;
                        hierarchyNodesViewModel.ImageList.Insert(0, "/Content/images/rtable-turn.gif");
                    }
                    else
                    {
                        hierarchyNodesViewModel.ImageList.Insert(0, "/Content/images/rtable-fork.gif");
                    }

                    for (int i = 0; i < item.Depth - 2; i++)
                    {
                        if (hasSiblings)
                        {
                            hierarchyNodesViewModel.ImageList.Insert(0, "/Content/images/rtable-line.gif");
                        }
                        else
                        {
                            hierarchyNodesViewModel.ImageList.Insert(0, "/Content/images/rtable-space.gif");
                        }
                    }
                    hierarchyNodesViewModel.Entity = new TView();
                    hierarchyNodesViewModel.Parent = parent;
                    hierarchyNodesViewModel.Depth = item.Depth;
                    hierarchyNodesViewModel.Entity.InjectFrom(item.Entity);
                    handler?.Invoke(item.Entity, hierarchyNodesViewModel.Entity);
                    viewList?.Add(hierarchyNodesViewModel);
                    hierarchyNodesViewModel.ChildNodes = new List<HierarchyNodesViewModel<TView>>();
                    if (item.ChildNodes.Any())
                    {
                        //hnVM.ImageList.Insert(0, "/Content/images/rtable-space.gif");
                        await AppendChildrenAsync(item.ChildNodes, (List<HierarchyNodesViewModel<TView>>)hierarchyNodesViewModel.ChildNodes, hierarchyNodesViewModel.Entity, hasSibbs, handler).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}