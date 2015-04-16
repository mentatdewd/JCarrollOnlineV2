using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JCarrollOnlineV2.Entities;
using System.Linq.Expressions;
using Omu.ValueInjecter;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<HierarchyNode<TView>> ProjectToView<TDom, TView>(this IEnumerable<HierarchyNode<TDom>> query)
            where TView : class, new()
            where TDom : class
        {
            List<HierarchyNode<TView>> viewList = new List<HierarchyNode<TView>>();
            ConstructTree<TDom, TView>(query, viewList, null);
            return viewList;
        }
        private static void ConstructTree<TDom, TView>(IEnumerable<HierarchyNode<TDom>> dataModel, List<HierarchyNode<TView>> viewList, TView parent)
            where TView : class, new()
            where TDom : class
        {
            if (dataModel != null)
            {
                foreach (var item in dataModel)
                {
                    HierarchyNode<TView> hnvm = new HierarchyNode<TView>();
                    hnvm.ImageList = new List<string>();
                    hnvm.ImageList.Add("/Content/images/reply-new.gif");
                    hnvm.Entity = new TView();
                    hnvm.Parent = parent;
                    hnvm.Depth = item.Depth;
                    hnvm.Entity.InjectFrom<FilterId>(item.Entity);
                    viewList.Add(hnvm);
                    hnvm.ChildNodes = new List<HierarchyNode<TView>>();
                    if (item.ChildNodes.Count > 0)
                    {
                        AppendChildren(item.ChildNodes, hnvm.ChildNodes, hnvm.Entity, false);
                    }
                }
            }
        }
        public static void AppendChildren<TView, TDom>(IEnumerable<HierarchyNode<TDom>> dataModel, List<HierarchyNode<TView>> viewList, TView parent, bool hasSibblings)
            where TView : class, new()
            where TDom : class
        {
            bool hasSibbs = false;

            if (dataModel.Count() > 1)
                hasSibbs = true;

            foreach (var item in dataModel)
            {
                HierarchyNode<TView> hnvm = new HierarchyNode<TView>();
                hnvm.ImageList = new List<string>();

                hnvm.ImageList.Add("/Content/images/reply-new.gif");

                if (item == dataModel.Last())
                {
                    hasSibbs = false;
                    hnvm.ImageList.Insert(0, "/Content/images/rtable-turn.gif");
                }
                else
                    hnvm.ImageList.Insert(0, "/Content/images/rtable-fork.gif");

                for (int i = 0; i < item.Depth - 2; i++)
                {
                    if (hasSibblings)
                        hnvm.ImageList.Insert(0, "/Content/images/rtable-line.gif");
                    else
                        hnvm.ImageList.Insert(0, "/Content/images/rtable-space.gif");
                }
                hnvm.Entity = new TView();
                hnvm.Parent = parent;
                hnvm.Depth = item.Depth;
                hnvm.Entity.InjectFrom<FilterId>(item.Entity);
                viewList.Add(hnvm);
                hnvm.ChildNodes = new List<HierarchyNode<TView>>();
                if (item.ChildNodes.Count > 0)
                {
                    //hnvm.ImageList.Insert(0, "/Content/images/rtable-space.gif");
                    AppendChildren(item.ChildNodes, hnvm.ChildNodes, hnvm.Entity, hasSibbs);
                }
            }
        }
    }

    // Stefan Cruysberghs, July 2008, http://www.scip.be
    /// <summary>
    /// AsHierarchy extension methods for LINQ to SQL IQueryable
    /// </summary>
    public static class LinqToSqlExtensionMethods
    {
        private static IEnumerable<HierarchyNode<TEntity>>
          CreateHierarchy<TEntity>(IQueryable<TEntity> allItems,
            TEntity parentItem,
            string propertyNameId,
            string propertyNameParentId,
            object rootItemId,
            int maxDepth,
            int depth) where TEntity : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
            Expression<Func<TEntity, bool>> predicate;

            if (rootItemId != null)
            {
                Expression left = Expression.Property(parameter, propertyNameId);
                left = Expression.Convert(left, rootItemId.GetType());
                Expression right = Expression.Constant(rootItemId);

                predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(left, right), parameter);
            }
            else
            {
                if (parentItem == null)
                {
                    predicate =
                      Expression.Lambda<Func<TEntity, bool>>(
                        Expression.Equal(Expression.Property(parameter, propertyNameParentId),
                                         Expression.Constant(null)), parameter);
                }
                else
                {
                    Expression left = Expression.Property(parameter, propertyNameParentId);
                    left = Expression.Convert(left, parentItem.GetType().GetProperty(propertyNameId).PropertyType);
                    Expression right = Expression.Constant(parentItem.GetType().GetProperty(propertyNameId).GetValue(parentItem, null));

                    predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(left, right), parameter);
                }
            }

            IEnumerable<TEntity> childs = allItems.Where(predicate).ToList();

            if (childs.Count() > 0)
            {
                depth++;

                if ((depth <= maxDepth) || (maxDepth == 0))
                {
                    foreach (var item in childs)
                        yield return
                          new HierarchyNode<TEntity>()
                          {
                              Entity = item,
                              ChildNodes =
                                CreateHierarchy(allItems, item, propertyNameId, propertyNameParentId, null, maxDepth, depth).ToList(),
                              Depth = depth,
                              Parent = parentItem
                          };
                }
            }
        }

        /// <summary>
        /// LINQ to SQL (IQueryable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <param name="allItems">Flat collection of entities</param>
        /// <param name="propertyNameId">String with property name of Id/Key</param>
        /// <param name="propertyNameParentId">String with property name of parent Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity>(
          this IQueryable<TEntity> allItems,
          string propertyNameId,
          string propertyNameParentId) where TEntity : class
        {
            return CreateHierarchy(allItems, null, propertyNameId, propertyNameParentId, null, 0, 0);
        }

        /// <summary>
        /// LINQ to SQL (IQueryable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <param name="allItems">Flat collection of entities</param>
        /// <param name="propertyNameId">String with property name of Id/Key</param>
        /// <param name="propertyNameParentId">String with property name of parent Id/Key</param>
        /// <param name="rootItemId">Value of root item Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity>(
          this IQueryable<TEntity> allItems,
          string propertyNameId,
          string propertyNameParentId,
          object rootItemId) where TEntity : class
        {
            return CreateHierarchy(allItems, null, propertyNameId, propertyNameParentId, rootItemId, 0, 0);
        }

        /// <summary>
        /// LINQ to SQL (IQueryable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <param name="allItems">Flat collection of entities</param>
        /// <param name="propertyNameId">String with property name of Id/Key</param>
        /// <param name="propertyNameParentId">String with property name of parent Id/Key</param>
        /// <param name="rootItemId">Value of root item Id/Key</param>
        /// <param name="maxDepth">Maximum depth of tree</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity>(
          this IQueryable<TEntity> allItems,
          string propertyNameId,
          string propertyNameParentId,
          object rootItemId,
          int maxDepth) where TEntity : class
        {
            return CreateHierarchy(allItems, null, propertyNameId, propertyNameParentId, rootItemId, maxDepth, 0);
        }
    }
}