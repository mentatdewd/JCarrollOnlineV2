using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.Test.DataContexts;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Test.Services
{
    [TestClass]
    public class MicroPostViewModelServiceTest
    {
        private Mock<JCarrollOnlineV2DbContext> _mockContext;
        private Mock<DbSet<MicroPost>> _mockMicroPostSet;
        private Mock<DbSet<ApplicationUser>> _mockUserSet;
        private MicroPostViewModelService _service;
        private List<MicroPost> _testMicroPosts;
        private List<ApplicationUser> _testUsers;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<JCarrollOnlineV2DbContext>() { CallBase = false };
            
            SetupTestData();
            
            _mockMicroPostSet = CreateMockDbSet(_testMicroPosts);
            _mockUserSet = CreateMockDbSet(_testUsers);
            
            _mockContext.Setup(c => c.MicroPost).Returns(_mockMicroPostSet.Object);
            _mockContext.Setup(c => c.ApplicationUser).Returns(_mockUserSet.Object);
            
            _service = new MicroPostViewModelService(_mockContext.Object);
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                new MicroPostViewModelService(null)
            );
            
            Assert.AreEqual("context", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_WithValidContext_CreatesInstance()
        {
            // Act
            MicroPostViewModelService service = new MicroPostViewModelService(_mockContext.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        #endregion

        #region BuildMicroPostFeedViewModelAsync - Parameter Validation Tests

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithNullUserId_ThrowsArgumentException()
        {
            // Act & Assert
            ArgumentException ex = null;
            try
            {
                await _service.BuildMicroPostFeedViewModelAsync(null, 1, 10);
            }
            catch (ArgumentException e)
            {
                ex = e;
            }
            
            Assert.IsNotNull(ex, "Expected ArgumentException to be thrown");
            Assert.AreEqual("userId", ex.ParamName);
            Assert.Contains("User ID cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithEmptyUserId_ThrowsArgumentException()
        {
            // Act & Assert
            ArgumentException ex = null;
            try
            {
                await _service.BuildMicroPostFeedViewModelAsync("", 1, 10);
            }
            catch (ArgumentException e)
            {
                ex = e;
            }
            
            Assert.IsNotNull(ex, "Expected ArgumentException to be thrown");
            Assert.AreEqual("userId", ex.ParamName);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithWhitespaceUserId_ThrowsArgumentException()
        {
            // Act & Assert
            ArgumentException ex = null;
            try
            {
                await _service.BuildMicroPostFeedViewModelAsync("   ", 1, 10);
            }
            catch (ArgumentException e)
            {
                ex = e;
            }
            
            Assert.IsNotNull(ex, "Expected ArgumentException to be thrown");
            Assert.AreEqual("userId", ex.ParamName);
        }

        #endregion

        #region BuildMicroPostFeedViewModelAsync - Happy Path Tests

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithValidUser_ReturnsViewModel()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.OnePageOfMicroPosts);
            Assert.IsNotNull(result.MicroPostFeedItems);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_IncludesUserOwnPosts()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>()); // No followed users

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            // Should include user1's own posts (2 posts)
            Assert.HasCount(2, result.MicroPostFeedItems);
            Assert.IsTrue(result.MicroPostFeedItems.All(m => m.Author.Id == "user1"));
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_IncludesFollowedUsersPosts()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            // Should include user1's posts (2) + user2's post (1) = 3
            Assert.HasCount(3, result.MicroPostFeedItems);

            List<string> authorIds = result.MicroPostFeedItems.Select(m => m.Author.Id).Distinct().ToList();
            Assert.Contains("user1", authorIds);
            Assert.Contains("user2", authorIds);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_ExcludesUnfollowedUsersPosts()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2" }); // Only follows user2

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            List<string> authorIds = result.MicroPostFeedItems.Select(m => m.Author.Id).Distinct().ToList();
            Assert.DoesNotContain("user3", authorIds); // Should NOT include user3's posts
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_OrdersPostsByDateDescending()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            List<MicroPostFeedItemViewModel> orderedPosts = result.MicroPostFeedItems.OrderByDescending(m => m.CreatedAt).ToList();
            CollectionAssert.AreEqual(
                orderedPosts.Select(p => p.CreatedAt).ToList(),
                result.MicroPostFeedItems.Select(p => p.CreatedAt).ToList()
            );
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_SetsTimeAgoCorrectly()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            foreach (MicroPostFeedItemViewModel item in result.MicroPostFeedItems)
            {
                Assert.IsNotNull(item.TimeAgo);
                Assert.IsFalse(string.IsNullOrEmpty(item.TimeAgo));
                // Should be ISO 8601 format
                Assert.IsTrue(DateTime.TryParse(item.TimeAgo, out _));
            }
        }

        #endregion

        #region BuildMicroPostFeedViewModelAsync - Pagination Tests

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithPageSize_LimitsMicroPosts()
        {
            // Arrange
            string userId = "user1";
            int pageSize = 1;
            SetupFollowedUsersQuery(userId, new List<string> { "user2" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, pageSize);

            // Assert
            Assert.AreEqual(pageSize, result.OnePageOfMicroPosts.Count);
            Assert.HasCount(3, result.MicroPostFeedItems); // Total items
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithPage2_ReturnsSecondPage()
        {
            // Arrange
            string userId = "user1";
            int pageSize = 1;
            SetupFollowedUsersQuery(userId, new List<string> { "user2" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 2, pageSize);

            // Assert
            Assert.AreEqual(2, result.OnePageOfMicroPosts.PageNumber);
            Assert.AreEqual(1, result.OnePageOfMicroPosts.Count);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithPageNumberZero_SetsToPageOne()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 0, 10);

            // Assert
            Assert.AreEqual(1, result.OnePageOfMicroPosts.PageNumber);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithNegativePageNumber_SetsToPageOne()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, -5, 10);

            // Assert
            Assert.AreEqual(1, result.OnePageOfMicroPosts.PageNumber);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithPageNumberExceedingTotal_SetsToLastPage()
        {
            // Arrange
            string userId = "user1";
            int pageSize = 1;
            SetupFollowedUsersQuery(userId, new List<string> { "user2" }); // 3 total posts

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 999, pageSize);

            // Assert
            Assert.AreEqual(3, result.OnePageOfMicroPosts.PageNumber); // Should be last page
            Assert.IsTrue(result.OnePageOfMicroPosts.IsLastPage);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_CalculatesPageCountCorrectly()
        {
            // Arrange
            string userId = "user1";
            int pageSize = 2;
            SetupFollowedUsersQuery(userId, new List<string> { "user2" }); // 3 total posts

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, pageSize);

            // Assert
            Assert.AreEqual(2, result.OnePageOfMicroPosts.PageCount); // 3 items / 2 per page = 2 pages
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithLargePageSize_ReturnsAllItems()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2", "user3" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 1000);

            // Assert
            Assert.AreEqual(result.MicroPostFeedItems.Count, result.OnePageOfMicroPosts.Count);
            Assert.AreEqual(1, result.OnePageOfMicroPosts.PageCount);
        }

        #endregion

        #region BuildMicroPostFeedViewModelAsync - Edge Cases

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithNoMicroPosts_ReturnsEmptyViewModel()
        {
            // Arrange
            string userId = "user4"; // User with no posts
            _testUsers.Add(new ApplicationUser { Id = "user4", UserName = "User Four" });
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.MicroPostFeedItems);
            Assert.AreEqual(0, result.OnePageOfMicroPosts.Count);
            Assert.AreEqual(1, result.OnePageOfMicroPosts.PageNumber);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithNoFollowedUsers_OnlyShowsOwnPosts()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>()); // Empty followed list

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.HasCount(2, result.MicroPostFeedItems);
            Assert.IsTrue(result.MicroPostFeedItems.All(m => m.Author.Id == "user1"));
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithMultipleFollowedUsers_CombinesAllPosts()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2", "user3" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            // user1 has 2 posts, user2 has 1, user3 has 1 = 4 total
            Assert.HasCount(4, result.MicroPostFeedItems);

            List<string> authorIds = result.MicroPostFeedItems.Select(m => m.Author.Id).Distinct().ToList();
            Assert.HasCount(3, authorIds);
            Assert.Contains("user1", authorIds);
            Assert.Contains("user2", authorIds);
            Assert.Contains("user3", authorIds);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WithPageSizeOfOne_PaginatesCorrectly()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2" }); // 3 total posts

            // Act - Get all pages
            MicroPostFeedViewModel page1 = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 1);
            MicroPostFeedViewModel page2 = await _service.BuildMicroPostFeedViewModelAsync(userId, 2, 1);
            MicroPostFeedViewModel page3 = await _service.BuildMicroPostFeedViewModelAsync(userId, 3, 1);

            // Assert
            Assert.AreEqual(1, page1.OnePageOfMicroPosts.Count);
            Assert.AreEqual(1, page2.OnePageOfMicroPosts.Count);
            Assert.AreEqual(1, page3.OnePageOfMicroPosts.Count);

            // Make sure we get different posts on each page
            List<string> allContents = new List<string>
            {
                page1.OnePageOfMicroPosts.First().Content,
                page2.OnePageOfMicroPosts.First().Content,
                page3.OnePageOfMicroPosts.First().Content
            };
            Assert.AreEqual(3, allContents.Distinct().Count());
        }

        #endregion

        #region BuildMicroPostFeedViewModelAsync - Error Handling Tests

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WhenDatabaseThrowsException_ReturnsEmptyViewModel()
        {
            // Arrange
            string userId = "user1";

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.OnePageOfMicroPosts);
            Assert.AreEqual(0, result.OnePageOfMicroPosts.Count);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_WhenMicroPostSetIsNull_ReturnsEmptyViewModel()
        {
            // Arrange
            string userId = "user1";
            _mockContext.Setup(c => c.MicroPost).Returns((DbSet<MicroPost>)null);
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.OnePageOfMicroPosts);
            Assert.AreEqual(0, result.OnePageOfMicroPosts.Count);
        }

        #endregion

        #region BuildMicroPostFeedViewModelAsync - Data Integrity Tests

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_MapsAllMicroPostProperties()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            MicroPostFeedItemViewModel firstPost = result.MicroPostFeedItems.First();
            MicroPost originalPost = _testMicroPosts.First(m => m.Author.Id == firstPost.Author.Id && m.Content == firstPost.Content && m.CreatedAt == firstPost.CreatedAt);
            
            Assert.AreEqual(originalPost.Author.Id, firstPost.Author.Id);
            Assert.AreEqual(originalPost.Content, firstPost.Content);
            Assert.AreEqual(originalPost.CreatedAt, firstPost.CreatedAt);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_MapsAuthorProperties()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string>());

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 10);

            // Assert
            MicroPostFeedItemViewModel firstPost = result.MicroPostFeedItems.First();
            MicroPost originalPost = _testMicroPosts.First(m => m.Author.Id == firstPost.Author.Id);
            
            Assert.IsNotNull(firstPost.Author);
            Assert.AreEqual(originalPost.Author.Id, firstPost.Author.Id);
            Assert.AreEqual(originalPost.Author.UserName, firstPost.Author.UserName);
        }

        [TestMethod]
        public async Task BuildMicroPostFeedViewModelAsync_MaintainsAllItemsInFeedItems()
        {
            // Arrange
            string userId = "user1";
            SetupFollowedUsersQuery(userId, new List<string> { "user2" });

            // Act
            MicroPostFeedViewModel result = await _service.BuildMicroPostFeedViewModelAsync(userId, 1, 1);

            // Assert
            // Even though we only show 1 per page, all items should be in MicroPostFeedItems
            Assert.IsGreaterThan(result.OnePageOfMicroPosts.Count, result.MicroPostFeedItems.Count);
            Assert.HasCount(3, result.MicroPostFeedItems);
            Assert.AreEqual(1, result.OnePageOfMicroPosts.Count);
        }

        #endregion

        #region Helper Methods

        private void SetupTestData()
        {
            ApplicationUser user1 = new ApplicationUser
            {
                Id = "user1",
                UserName = "User One",
                Email = "user1@test.com"
            };

            ApplicationUser user2 = new ApplicationUser
            {
                Id = "user2",
                UserName = "User Two",
                Email = "user2@test.com"
            };

            ApplicationUser user3 = new ApplicationUser
            {
                Id = "user3",
                UserName = "User Three",
                Email = "user3@test.com"
            };

            _testUsers = new List<ApplicationUser> { user1, user2, user3 };

            _testMicroPosts = new List<MicroPost>
            {
                new MicroPost
                {
                    Id = 1,
                    Content = "Post 1 from user1",
                    CreatedAt = DateTime.Now.AddHours(-1),
                    AuthorId = user1.Id,
                    Author = user1
                },
                new MicroPost
                {
                    Id = 2,
                    Content = "Post 2 from user1",
                    CreatedAt = DateTime.Now.AddHours(-2),
                    AuthorId = user1.Id,
                    Author = user1
                },
                new MicroPost
                {
                    Id = 3,
                    Content = "Post 1 from user2",
                    CreatedAt = DateTime.Now.AddHours(-3),
                    AuthorId = user2.Id,
                    Author = user2
                },
                new MicroPost
                {
                    Id = 4,
                    Content = "Post 1 from user3",
                    CreatedAt = DateTime.Now.AddHours(-4),
                    AuthorId = user3.Id,
                    Author = user3
                }
            };
        }

        private void SetupFollowedUsersQuery(string userId, List<string> followedUserIds)
        {
            // Find or create the user
            ApplicationUser user = _testUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                user = new ApplicationUser { Id = userId, UserName = $"User {userId}" };
                _testUsers.Add(user);
            }

            // Setup the Following collection
            List<ApplicationUser> followedUsers = _testUsers
                .Where(u => followedUserIds.Contains(u.Id))
                .ToList();
            
            // Use reflection to set the private collection
            System.Reflection.PropertyInfo prop = user.GetType().GetProperty("Following");
            prop.SetValue(user, followedUsers, null);
            
            // Recreate the user DbSet with the updated relationships
            _mockUserSet = CreateMockDbSet(_testUsers);
            _mockContext.Setup(c => c.ApplicationUser).Returns(_mockUserSet.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            IQueryable<T> queryable = data.AsQueryable();
            Mock<DbSet<T>> mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockSet;
        }

        #endregion

        #region Test Helper Classes

        internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestDbAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
            {
                return new TestDbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
            {
                return new TestDbAsyncEnumerable<TElement>(expression);
            }

            public object Execute(System.Linq.Expressions.Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }
        }

        internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
        {
            public TestDbAsyncEnumerable(System.Linq.Expressions.Expression expression)
                : base(expression)
            {
            }

            public IDbAsyncEnumerator<T> GetAsyncEnumerator()
            {
                return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return GetAsyncEnumerator();
            }

            IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this);
        }

        internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestDbAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public void Dispose()
            {
                _inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_inner.MoveNext());
            }

            public T Current => _inner.Current;

            object IDbAsyncEnumerator.Current => Current;
        }

        #endregion
    }
}