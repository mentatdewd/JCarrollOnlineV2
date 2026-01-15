using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Blog;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public class BlogViewModelService : IBlogViewModelService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JCarrollOnlineV2DbContext _context;

        public BlogViewModelService(JCarrollOnlineV2DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BlogFeedViewModel> BuildBlogFeedViewModelAsync()
        {
            _logger.Info("Building blog feed view model");

            var blogFeedViewModel = new BlogFeedViewModel();

            try
            {
                // Load blog items with related data
                List<BlogItem> blogItems = await _context.BlogItem
                    .Include(b => b.BlogItemComments)
                    .Include(b => b.Author)
                    .AsNoTracking()
                    .OrderByDescending(m => m.UpdatedAt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                // Transform to view models
                foreach (BlogItem item in blogItems)
                {
                    var blogFeedItemViewModel = new BlogFeedItemViewModel();
                    blogFeedItemViewModel.InjectFrom(item);
                    blogFeedItemViewModel.Comments.BlogItemId = item.Id;

                    if (item.Author != null)
                    {
                        blogFeedItemViewModel.Author.InjectFrom(item.Author);
                    }

                    if (item.BlogItemComments != null)
                    {
                        foreach (BlogItemComment comment in item.BlogItemComments)
                        {
                            var blogCommentItemViewModel = new BlogCommentItemViewModel(item.Id);
                            blogCommentItemViewModel.InjectFrom(comment);
                            blogCommentItemViewModel.BlogItemId = comment.BlogItem.Id;
                            blogCommentItemViewModel.TimeAgo = blogCommentItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                            blogFeedItemViewModel.Comments.BlogComments.Add(blogCommentItemViewModel);
                        }
                    }

                    blogFeedViewModel.BlogFeedItemViewModels.Add(blogFeedItemViewModel);
                }

                _logger.Info($"Successfully built blog feed with {blogFeedViewModel.BlogFeedItemViewModels.Count} items");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error building blog feed view model");
                // Return empty view model rather than throwing
            }

            return blogFeedViewModel;
        }
    }
}