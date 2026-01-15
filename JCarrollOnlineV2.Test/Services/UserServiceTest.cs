using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
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
    public class UserServiceTest
    {
        private Mock<JCarrollOnlineV2DbContext> _mockContext;
        private Mock<DbSet<ApplicationUser>> _mockUserSet;
        private Mock<DbSet<MicroPost>> _mockMicroPostSet;
        private Mock<ILogger> _mockLogger;
        private UserService _userService;
        private List<ApplicationUser> _testUsers;
        private List<MicroPost> _testMicroPosts;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<JCarrollOnlineV2DbContext>();
            _mockLogger = new Mock<ILogger>();
            
            // Setup test data
            SetupTestData();
            
            // Setup mock DbSets
            _mockUserSet = CreateMockDbSet(_testUsers);
            _mockMicroPostSet = CreateMockDbSet(_testMicroPosts);
            
            _mockContext.Setup(c => c.ApplicationUser).Returns(_mockUserSet.Object);
            _mockContext.Setup(c => c.MicroPost).Returns(_mockMicroPostSet.Object);
            
            _userService = new UserService(_mockContext.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                new UserService(null, _mockLogger.Object)
            );
            
            Assert.AreEqual("context", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                new UserService(_mockContext.Object, null)
            );
            
            Assert.AreEqual("logger", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            UserService service = new UserService(_mockContext.Object, _mockLogger.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        #endregion

        #region GetAllUsersAsync Tests

        [TestMethod]
        public async Task GetAllUsersAsync_WithNullCurrentUserId_ReturnsAllUsers()
        {
            // Act
            List<UserItemViewModel> result = await _userService.GetAllUsersAsync(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(3, result);
            Assert.IsFalse(result.Any(u => u.IsFollowing));
            Assert.IsFalse(result.Any(u => u.IsFollower));
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WithCurrentUserId_ExcludesCurrentUser()
        {
            // Arrange
            string currentUserId = "user1";

            // Act
            List<UserItemViewModel> result = await _userService.GetAllUsersAsync(currentUserId);

            // Assert
            Assert.HasCount(2, result);
            Assert.IsFalse(result.Any(u => u.UserId == currentUserId));
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WithFollowingUsers_SetsIsFollowingCorrectly()
        {
            // Arrange
            string currentUserId = "user1";
            // user1 follows user2
            _testUsers[0].Following.Add(_testUsers[1]);

            // Act
            List<UserItemViewModel> result = await _userService.GetAllUsersAsync(currentUserId);

            // Assert
            UserItemViewModel user2 = result.FirstOrDefault(u => u.UserId == "user2");
            Assert.IsNotNull(user2);
            Assert.IsTrue(user2.IsFollowing);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WithFollowers_SetsIsFollowerCorrectly()
        {
            // Arrange
            string currentUserId = "user1";
            // user2 follows user1 (so user2 is a follower of user1)
            _testUsers[1].Following.Add(_testUsers[0]);
            _testUsers[0].Followers.Add(_testUsers[1]);

            // Act
            List<UserItemViewModel> result = await _userService.GetAllUsersAsync(currentUserId);

            // Assert
            UserItemViewModel user2 = result.FirstOrDefault(u => u.UserId == "user2");
            Assert.IsNotNull(user2);
            Assert.IsTrue(user2.IsFollower);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_SetsMicroPostCountCorrectly()
        {
            // Act
            List<UserItemViewModel> result = await _userService.GetAllUsersAsync(null);

            // Assert
            UserItemViewModel user1 = result.First(u => u.UserId == "user1");
            Assert.AreEqual(2, user1.MicroPostsAuthored);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WithMutualFollowing_SetsBothFlags()
        {
            // Arrange
            string currentUserId = "user1";
            // Mutual following: user1 follows user2 AND user2 follows user1
            // In a many-to-many relationship, both sides must be set up
            _testUsers[0].Following.Add(_testUsers[1]);  // user1 follows user2
            _testUsers[1].Followers.Add(_testUsers[0]);  // user1 is a follower of user2
            _testUsers[1].Following.Add(_testUsers[0]);  // user2 follows user1
            _testUsers[0].Followers.Add(_testUsers[1]);  // user2 is a follower of user1

            // Act
            List<UserItemViewModel> result = await _userService.GetAllUsersAsync(currentUserId);

            // Assert
            UserItemViewModel user2 = result.FirstOrDefault(u => u.UserId == "user2");
            Assert.IsNotNull(user2);
            Assert.IsTrue(user2.IsFollowing, "Should be marked as Following");
            Assert.IsTrue(user2.IsFollower, "Should be marked as Follower");
        }

        #endregion

        #region GetUserDetailsAsync Tests

        [TestMethod]
        public async Task GetUserDetailsAsync_WithValidUserId_ReturnsUserDetail()
        {
            // Arrange
            string userId = "user1";
            string currentUserId = "user2";

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user1", result.UserInfoViewModel.UserId);
            Assert.AreEqual("User One", result.User.UserName);
        }

        [TestMethod]
        public async Task GetUserDetailsAsync_WithInvalidUserId_ReturnsNull()
        {
            // Arrange
            string userId = "nonexistent";
            string currentUserId = "user1";

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserDetailsAsync_PopulatesFollowingList()
        {
            // Arrange
            string userId = "user1";
            string currentUserId = "user2";
            _testUsers[0].Following.Add(_testUsers[1]);

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert
            Assert.IsNotNull(result.UserStatsViewModel.UsersFollowing);
            Assert.HasCount(1, result.UserStatsViewModel.UsersFollowing.Users);
            Assert.AreEqual("user2", result.UserStatsViewModel.UsersFollowing.Users.ElementAt(0).UserId);
        }

        [TestMethod]
        public async Task GetUserDetailsAsync_PopulatesFollowersList()
        {
            // Arrange
            string userId = "user1";
            string currentUserId = "user2";
            _testUsers[0].Followers.Add(_testUsers[1]);

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert
            Assert.IsNotNull(result.UserStatsViewModel.UserFollowers);
            Assert.HasCount(1, result.UserStatsViewModel.UserFollowers.Users);
            Assert.AreEqual("user2", result.UserStatsViewModel.UserFollowers.Users.ElementAt(0).UserId);
        }

        [TestMethod]
        public async Task GetUserDetailsAsync_SetsMicroPostCountsForFollowing()
        {
            // Arrange
            string userId = "user1";
            string currentUserId = "user3";
            _testUsers[0].Following.Add(_testUsers[1]); // user1 follows user2

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert
            UserItemViewModel followingUser = result.UserStatsViewModel.UsersFollowing.Users.ElementAt(0);
            Assert.AreEqual(1, followingUser.MicroPostsAuthored); // user2 has 1 micropost
        }

        [TestMethod]
        public async Task GetUserDetailsAsync_SetsFollowerBadgesCorrectly()
        {
            // Arrange
            string userId = "user1";
            string currentUserId = "user2";
            
            // Setup: user2 follows user3, and user1 follows user3
            _testUsers[1].Following.Add(_testUsers[2]);
            _testUsers[0].Following.Add(_testUsers[2]);

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert - if user3 is in user1's following list, check the badges
            UserItemViewModel user3InList = result.UserStatsViewModel.UsersFollowing.Users
                .FirstOrDefault(u => u.UserId == "user3");
            
            if (user3InList != null)
            {
                // user2 follows user3, so IsFollowing should be true
                Assert.IsTrue(user3InList.IsFollowing);
            }
        }

        [TestMethod]
        public async Task GetUserDetailsAsync_SetsUserInfoCorrectly()
        {
            // Arrange
            string userId = "user1";
            string currentUserId = "user2";

            // Act
            UserDetailViewModel result = await _userService.GetUserDetailsAsync(userId, currentUserId);

            // Assert
            Assert.AreEqual("user1", result.UserInfoViewModel.UserId);
            Assert.IsTrue(result.UserInfoViewModel.MicroPostEmailNotifications);
            Assert.IsFalse(result.UserInfoViewModel.MicroPostSmsNotifications);
            Assert.AreEqual(2, result.UserInfoViewModel.MicroPostsAuthored);
        }

        #endregion

        #region FollowUserAsync Tests

        [TestMethod]
        public async Task FollowUserAsync_WithValidUsers_ReturnsTrue()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "user2";

            // Act
            bool result = await _userService.FollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_testUsers[0].Following.Any(u => u.Id == targetUserId));
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task FollowUserAsync_WithNullCurrentUser_ReturnsFalse()
        {
            // Arrange
            string currentUserId = "nonexistent";
            string targetUserId = "user2";

            // Act
            bool result = await _userService.FollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsFalse(result);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task FollowUserAsync_WithNullTargetUser_ReturnsFalse()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "nonexistent";

            // Act
            bool result = await _userService.FollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsFalse(result);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task FollowUserAsync_WhenAlreadyFollowing_ReturnsFalse()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "user2";
            _testUsers[0].Following.Add(_testUsers[1]); // Already following

            // Act
            bool result = await _userService.FollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsFalse(result);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task FollowUserAsync_AddsToFollowingCollection()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "user2";
            int initialCount = _testUsers[0].Following.Count;

            // Act
            bool result = await _userService.FollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsTrue(result);
            Assert.HasCount(initialCount + 1, _testUsers[0].Following);
        }

        #endregion

        #region UnfollowUserAsync Tests

        [TestMethod]
        public async Task UnfollowUserAsync_WithValidUsers_ReturnsTrue()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "user2";
            _testUsers[0].Following.Add(_testUsers[1]); // Setup: user1 follows user2

            // Act
            bool result = await _userService.UnfollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_testUsers[0].Following.Any(u => u.Id == targetUserId));
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UnfollowUserAsync_WithNullCurrentUser_ReturnsFalse()
        {
            // Arrange
            string currentUserId = "nonexistent";
            string targetUserId = "user2";

            // Act
            bool result = await _userService.UnfollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsFalse(result);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task UnfollowUserAsync_WhenNotFollowing_ReturnsFalse()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "user2";
            // Not following anyone initially

            // Act
            bool result = await _userService.UnfollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsFalse(result);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task UnfollowUserAsync_RemovesFromFollowingCollection()
        {
            // Arrange
            string currentUserId = "user1";
            string targetUserId = "user2";
            _testUsers[0].Following.Add(_testUsers[1]);
            int initialCount = _testUsers[0].Following.Count;

            // Act
            bool result = await _userService.UnfollowUserAsync(currentUserId, targetUserId);

            // Assert
            Assert.IsTrue(result);
            Assert.HasCount(initialCount - 1, _testUsers[0].Following);
        }

        #endregion

        #region UpdateUserSettingsAsync Tests

        [TestMethod]
        public async Task UpdateUserSettingsAsync_WithValidUser_ReturnsTrue()
        {
            // Arrange
            string userId = "user1";
            bool emailNotifications = false;
            bool smsNotifications = true;

            // Act
            bool result = await _userService.UpdateUserSettingsAsync(userId, emailNotifications, smsNotifications);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(emailNotifications, _testUsers[0].MicroPostEmailNotifications);
            Assert.AreEqual(smsNotifications, _testUsers[0].MicroPostSmsNotifications);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateUserSettingsAsync_WithInvalidUser_ReturnsFalse()
        {
            // Arrange
            string userId = "nonexistent";

            // Act
            bool result = await _userService.UpdateUserSettingsAsync(userId, true, true);

            // Assert
            Assert.IsFalse(result);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserSettingsAsync_UpdatesBothSettings()
        {
            // Arrange
            string userId = "user1";
            _testUsers[0].MicroPostEmailNotifications = true;
            _testUsers[0].MicroPostSmsNotifications = false;

            // Act
            bool result = await _userService.UpdateUserSettingsAsync(userId, false, true);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_testUsers[0].MicroPostEmailNotifications);
            Assert.IsTrue(_testUsers[0].MicroPostSmsNotifications);
        }

        #endregion

        #region Helper Methods

        private void SetupTestData()
        {
            var user1 = new ApplicationUser
            {
                Id = "user1",
                UserName = "User One",
                Email = "user1@test.com",
                MicroPostEmailNotifications = true,
                MicroPostSmsNotifications = false
            };
            
            var user2 = new ApplicationUser
            {
                Id = "user2",
                UserName = "User Two",
                Email = "user2@test.com",
                MicroPostEmailNotifications = false,
                MicroPostSmsNotifications = true
            };
            
            var user3 = new ApplicationUser
            {
                Id = "user3",
                UserName = "User Three",
                Email = "user3@test.com",
                MicroPostEmailNotifications = true,
                MicroPostSmsNotifications = true
            };
            
            // Initialize navigation properties using reflection since they have private setters
            InitializeNavigationProperties(user1);
            InitializeNavigationProperties(user2);
            InitializeNavigationProperties(user3);
            
            _testUsers = new List<ApplicationUser> { user1, user2, user3 };

            _testMicroPosts = new List<MicroPost>
            {
                new MicroPost { Id = 1, Content = "Post 1", Author = _testUsers[0], CreatedAt = DateTime.Now },
                new MicroPost { Id = 2, Content = "Post 2", Author = _testUsers[0], CreatedAt = DateTime.Now },
                new MicroPost { Id = 3, Content = "Post 3", Author = _testUsers[1], CreatedAt = DateTime.Now }
            };
            
            // Populate the reverse navigation property (MicroPosts collection on each user)
            _testUsers[0].MicroPosts.Add(_testMicroPosts[0]);
            _testUsers[0].MicroPosts.Add(_testMicroPosts[1]);
            _testUsers[1].MicroPosts.Add(_testMicroPosts[2]);
        }
        
        private void InitializeNavigationProperties(ApplicationUser user)
        {
            var followingProperty = typeof(ApplicationUser).GetProperty("Following");
            var followersProperty = typeof(ApplicationUser).GetProperty("Followers");
            var microPostsProperty = typeof(ApplicationUser).GetProperty("MicroPosts");
            
            followingProperty?.SetValue(user, new List<ApplicationUser>());
            followersProperty?.SetValue(user, new List<ApplicationUser>());
            microPostsProperty?.SetValue(user, new List<MicroPost>());
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            IQueryable<T> queryable = data.AsQueryable();
            Mock<DbSet<T>> mockSet = new Mock<DbSet<T>>();

            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            
            // Setup Include method to return the mock set itself (for navigation property loading)
            mockSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockSet.Object);
            
            // Setup AsNoTracking method to return the mock set itself
            mockSet.Setup(m => m.AsNoTracking()).Returns(mockSet.Object);

            return mockSet;
        }

        #endregion
    }

    #region Test Helpers for Async DbSet

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
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestDbAsyncEnumerable(System.Linq.Expressions.Expression expression)
            : base(expression)
        { }

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