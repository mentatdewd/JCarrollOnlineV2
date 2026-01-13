using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.Test.DataContexts;
using JCarrollOnlineV2.Test.Services;
using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.Chat;
using JCarrollOnlineV2.ViewModels.Home;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Test.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private FakeJCarrollOnlineV2DbContext _fakeContext;
        private MockRssService _mockRssService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Setup fake DbContext with empty collections
            _fakeContext = new FakeJCarrollOnlineV2DbContext();
            
            // Initialize required DbSets with empty data
            _fakeContext.BlogItem = new FakeJCarrollOnlineV2Db<BlogItem>();
            _fakeContext.ForumThreadEntry = new FakeJCarrollOnlineV2Db<ThreadEntry>();
            _fakeContext.ChatMessages = new FakeJCarrollOnlineV2Db<ChatMessage>();

            // Initialize mock RSS service
            _mockRssService = new MockRssService();
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithHomeViewModel()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result.Model, typeof(HomeViewModel));
            }
        }

        [TestMethod]
        public async Task Index_SetsCorrectMessage()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.AreEqual("JCarrollOnlineV2 Home - Index", vm.Message);
            }
        }

        [TestMethod]
        public async Task Index_SetsCorrectPageContainer()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.AreEqual("Home", vm.PageContainer);
            }
        }

        [TestMethod]
        public async Task Index_InitializesBlogFeed()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.IsNotNull(vm.BlogFeed);
                Assert.IsNotNull(vm.BlogFeed.BlogFeedItemViewModels);
            }
        }

        [TestMethod]
        public async Task Index_InitializesChatViewModel()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.IsNotNull(vm.ChatViewModel);
                Assert.IsNotNull(vm.ChatViewModel.RecentMessages);
            }
        }

        [TestMethod]
        public async Task Index_InitializesLatestForumThreadsViewModel()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.IsNotNull(vm.LatestForumThreadsViewModel);
                Assert.IsNotNull(vm.LatestForumThreadsViewModel.LatestForumThreads);
            }
        }

        [TestMethod]
        public async Task Index_WithChatMessages_LoadsRecentMessages()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "testuser" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);

            _fakeContext.ChatMessages.Add(new ChatMessage
            {
                Id = 1,
                Message = "Test message 1",
                Author = testUser,
                CreatedAt = DateTime.Now.AddMinutes(-10)
            });
            _fakeContext.ChatMessages.Add(new ChatMessage
            {
                Id = 2,
                Message = "Test message 2",
                Author = testUser,
                CreatedAt = DateTime.Now.AddMinutes(-5)
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(2, vm.ChatViewModel.RecentMessages);
            }
        }

        [TestMethod]
        public async Task Index_WithChatMessages_OrdersChronologically()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "testuser" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);

            _fakeContext.ChatMessages.Add(new ChatMessage
            {
                Id = 1,
                Message = "First message",
                Author = testUser,
                CreatedAt = DateTime.Now.AddMinutes(-10)
            });
            _fakeContext.ChatMessages.Add(new ChatMessage
            {
                Id = 2,
                Message = "Third message",
                Author = testUser,
                CreatedAt = DateTime.Now.AddMinutes(-3)
            });
            _fakeContext.ChatMessages.Add(new ChatMessage
            {
                Id = 3,
                Message = "Second message",
                Author = testUser,
                CreatedAt = DateTime.Now.AddMinutes(-7)
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(3, vm.ChatViewModel.RecentMessages);
                List<ChatMessageViewModel> messages = vm.ChatViewModel.RecentMessages.ToList();
                Assert.AreEqual("First message", messages[0].Message);
                Assert.AreEqual("Second message", messages[1].Message);
                Assert.AreEqual("Third message", messages[2].Message);
            }
        }

        [TestMethod]
        public async Task Index_WithMoreThan50ChatMessages_LoadsOnly50()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "testuser" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);

            // Add 60 chat messages
            for (int i = 1; i <= 60; i++)
            {
                _fakeContext.ChatMessages.Add(new ChatMessage
                {
                    Id = i,
                    Message = $"Test message {i}",
                    Author = testUser,
                    CreatedAt = DateTime.Now.AddMinutes(-i)
                });
            }

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(50, vm.ChatViewModel.RecentMessages);
            }
        }

        [TestMethod]
        public async Task Index_WithBlogItems_LoadsBlogFeed()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "blogauthor" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);

            _fakeContext.BlogItem.Add(new BlogItem
            {
                Id = 1,
                Title = "Test Blog Post",
                Content = "Test content",
                Author = testUser,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(1, vm.BlogFeed.BlogFeedItemViewModels);
            }
        }

        [TestMethod]
        public async Task Index_WithBlogItemsWithComments_LoadsComments()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "blogauthor" };
            ApplicationUser commentUser = new ApplicationUser { Id = "test-user-2", UserName = "commenter" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);
            _fakeContext.ApplicationUser.Add(commentUser);

            BlogItem blogItem = new BlogItem
            {
                Id = 1,
                Title = "Test Blog Post",
                Content = "Test content",
                Author = testUser,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            };

            BlogItemComment comment = new BlogItemComment
            {
                Id = 1,
                Content = "Test comment",
                Author = commentUser.UserName,
                BlogItem = blogItem,
                CreatedAt = DateTime.Now.AddHours(-2)
            };

            blogItem.BlogItemComments.Add(comment);
            _fakeContext.BlogItem.Add(blogItem);

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(1, vm.BlogFeed.BlogFeedItemViewModels);
                BlogFeedItemViewModel firstBlogItem = vm.BlogFeed.BlogFeedItemViewModels.First();
                Assert.HasCount(1, firstBlogItem.Comments.BlogComments);
                Assert.AreEqual("Test comment", firstBlogItem.Comments.BlogComments.First().Content);
            }
        }

        [TestMethod]
        public async Task Index_WithMultipleBlogItems_OrdersByUpdatedAtDescending()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "blogauthor" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);

            _fakeContext.BlogItem.Add(new BlogItem
            {
                Id = 1,
                Title = "Older Blog Post",
                Content = "Test content 1",
                Author = testUser,
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddDays(-3),
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            });

            _fakeContext.BlogItem.Add(new BlogItem
            {
                Id = 2,
                Title = "Newer Blog Post",
                Content = "Test content 2",
                Author = testUser,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            });

            _fakeContext.BlogItem.Add(new BlogItem
            {
                Id = 3,
                Title = "Newest Blog Post",
                Content = "Test content 3",
                Author = testUser,
                CreatedAt = DateTime.Now.AddDays(-2),
                UpdatedAt = DateTime.Now.AddHours(-1), // Most recently updated
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(3, vm.BlogFeed.BlogFeedItemViewModels);
                List<BlogFeedItemViewModel> blogItems = vm.BlogFeed.BlogFeedItemViewModels.ToList();
                Assert.AreEqual("Newest Blog Post", blogItems[0].Title);
                Assert.AreEqual("Newer Blog Post", blogItems[1].Title);
                Assert.AreEqual("Older Blog Post", blogItems[2].Title);
            }
        }

        [TestMethod]
        public async Task Index_WithForumThreads_LoadsLatestThreads()
        {
            // Arrange
            Forum testForum = new Forum
            {
                Id = 1,
                Title = "Test Forum",
                Description = "Test Description",
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };

            _fakeContext.Forum = new FakeJCarrollOnlineV2Db<Forum>();
            _fakeContext.Forum.Add(testForum);

            _fakeContext.ForumThreadEntry.Add(new ThreadEntry
            {
                Id = 1,
                Title = "Test Thread",
                Forum = testForum,
                CreatedAt = DateTime.Now.AddHours(-2),
                UpdatedAt = DateTime.Now.AddHours(-1)
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(1, vm.LatestForumThreadsViewModel.LatestForumThreads);
                LatestForumThreadItemViewModel firstThread = vm.LatestForumThreadsViewModel.LatestForumThreads.First();
                Assert.AreEqual("Test Thread", firstThread.ThreadTitle);
                Assert.AreEqual("Test Forum", firstThread.ForumTitle);
            }
        }

        [TestMethod]
        public async Task Index_WithMoreThanFiveForumThreads_LoadsOnlyTopFive()
        {
            // Arrange
            Forum testForum = new Forum
            {
                Id = 1,
                Title = "Test Forum",
                Description = "Test Description",
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };

            _fakeContext.Forum = new FakeJCarrollOnlineV2Db<Forum>();
            _fakeContext.Forum.Add(testForum);

            // Add 7 threads
            for (int i = 1; i <= 7; i++)
            {
                _fakeContext.ForumThreadEntry.Add(new ThreadEntry
                {
                    Id = i,
                    Title = $"Test Thread {i}",
                    Forum = testForum,
                    CreatedAt = DateTime.Now.AddHours(-i),
                    UpdatedAt = DateTime.Now.AddHours(-i)
                });
            }

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(5, vm.LatestForumThreadsViewModel.LatestForumThreads);
            }
        }

        [TestMethod]
        public async Task Index_WithMultipleForumThreads_OrdersByUpdatedAtDescending()
        {
            // Arrange
            Forum testForum = new Forum
            {
                Id = 1,
                Title = "Test Forum",
                Description = "Test Description",
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };

            _fakeContext.Forum = new FakeJCarrollOnlineV2Db<Forum>();
            _fakeContext.Forum.Add(testForum);

            _fakeContext.ForumThreadEntry.Add(new ThreadEntry
            {
                Id = 1,
                Title = "Oldest Thread",
                Forum = testForum,
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now.AddDays(-5)
            });

            _fakeContext.ForumThreadEntry.Add(new ThreadEntry
            {
                Id = 2,
                Title = "Newest Thread",
                Forum = testForum,
                CreatedAt = DateTime.Now.AddHours(-1),
                UpdatedAt = DateTime.Now.AddHours(-1)
            });

            _fakeContext.ForumThreadEntry.Add(new ThreadEntry
            {
                Id = 3,
                Title = "Middle Thread",
                Forum = testForum,
                CreatedAt = DateTime.Now.AddDays(-2),
                UpdatedAt = DateTime.Now.AddDays(-2)
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(3, vm.LatestForumThreadsViewModel.LatestForumThreads);
                List<LatestForumThreadItemViewModel> threads = vm.LatestForumThreadsViewModel.LatestForumThreads.ToList();
                Assert.AreEqual("Newest Thread", threads[0].ThreadTitle);
                Assert.AreEqual("Middle Thread", threads[1].ThreadTitle);
                Assert.AreEqual("Oldest Thread", threads[2].ThreadTitle);
            }
        }

        [TestMethod]
        public async Task Index_WithAllDataTypes_LoadsAllSections()
        {
            // Arrange - Setup all data types
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "testuser" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);

            // Add blog item
            _fakeContext.BlogItem.Add(new BlogItem
            {
                Id = 1,
                Title = "Test Blog",
                Content = "Content",
                Author = testUser,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            });

            // Add forum thread
            Forum testForum = new Forum
            {
                Id = 1,
                Title = "Test Forum",
                Description = "Description",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _fakeContext.Forum = new FakeJCarrollOnlineV2Db<Forum>();
            _fakeContext.Forum.Add(testForum);

            _fakeContext.ForumThreadEntry.Add(new ThreadEntry
            {
                Id = 1,
                Title = "Test Thread",
                Forum = testForum,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            // Add chat message
            _fakeContext.ChatMessages.Add(new ChatMessage
            {
                Id = 1,
                Message = "Test Chat Message",
                Author = testUser,
                CreatedAt = DateTime.Now
            });

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.IsNotNull(vm);
                Assert.HasCount(1, vm.BlogFeed.BlogFeedItemViewModels);
                Assert.HasCount(1, vm.LatestForumThreadsViewModel.LatestForumThreads);
                Assert.HasCount(1, vm.ChatViewModel.RecentMessages);
                Assert.AreEqual("Test Blog", vm.BlogFeed.BlogFeedItemViewModels.First().Title);
                Assert.AreEqual("Test Thread", vm.LatestForumThreadsViewModel.LatestForumThreads.First().ThreadTitle);
                Assert.AreEqual("Test Chat Message", vm.ChatViewModel.RecentMessages.First().Message);
            }
        }

        [TestMethod]
        public async Task Index_WithBlogItemMultipleComments_LoadsAllComments()
        {
            // Arrange
            ApplicationUser testUser = new ApplicationUser { Id = "test-user-1", UserName = "author" };
            ApplicationUser commenter1 = new ApplicationUser { Id = "test-user-2", UserName = "commenter1" };
            ApplicationUser commenter2 = new ApplicationUser { Id = "test-user-3", UserName = "commenter2" };
            _fakeContext.ApplicationUser = new FakeJCarrollOnlineV2Db<ApplicationUser>();
            _fakeContext.ApplicationUser.Add(testUser);
            _fakeContext.ApplicationUser.Add(commenter1);
            _fakeContext.ApplicationUser.Add(commenter2);

            BlogItem blogItem = new BlogItem
            {
                Id = 1,
                Title = "Test Blog Post",
                Content = "Test content",
                Author = testUser,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                BlogItemComments = new System.Collections.ObjectModel.Collection<BlogItemComment>()
            };

            blogItem.BlogItemComments.Add(new BlogItemComment
            {
                Id = 1,
                Content = "First comment",
                Author = commenter1.UserName,
                BlogItem = blogItem,
                CreatedAt = DateTime.Now.AddHours(-5)
            });

            blogItem.BlogItemComments.Add(new BlogItemComment
            {
                Id = 2,
                Content = "Second comment",
                Author = commenter2.UserName,
                BlogItem = blogItem,
                CreatedAt = DateTime.Now.AddHours(-3)
            });

            blogItem.BlogItemComments.Add(new BlogItemComment
            {
                Id = 3,
                Content = "Third comment",
                Author = commenter1.UserName,
                BlogItem = blogItem,
                CreatedAt = DateTime.Now.AddHours(-1)
            });

            _fakeContext.BlogItem.Add(blogItem);

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.HasCount(1, vm.BlogFeed.BlogFeedItemViewModels);
                Assert.HasCount(3, vm.BlogFeed.BlogFeedItemViewModels.First().Comments.BlogComments);
            }
        }

        [TestMethod]
        public async Task Index_WithEmptyData_ReturnsViewModelWithEmptyCollections()
        {
            // Arrange - No data added

            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.IsNotNull(vm);
                Assert.HasCount(0, vm.BlogFeed.BlogFeedItemViewModels);
                Assert.HasCount(0, vm.LatestForumThreadsViewModel.LatestForumThreads);
                Assert.HasCount(0, vm.ChatViewModel.RecentMessages);
                Assert.AreEqual("JCarrollOnlineV2 Home - Index", vm.Message);
                Assert.AreEqual("Home", vm.PageContainer);
            }
        }

        #endregion

        #region About Tests

        [TestMethod]
        public void About_ReturnsViewResult_WithAboutViewModel()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = controller.About() as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result.Model, typeof(AboutViewModel));
            }
        }

        [TestMethod]
        public void About_SetsCorrectMessage()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = controller.About() as ViewResult;
                AboutViewModel vm = (AboutViewModel)result.Model;

                // Assert
                Assert.AreEqual("About JCarrollOnlineV2", vm.Message);
            }
        }

        [TestMethod]
        public void About_SetsCorrectPageContainer()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = controller.About() as ViewResult;
                AboutViewModel vm = (AboutViewModel)result.Model;

                // Assert
                Assert.AreEqual("AboutPage", vm.PageContainer);
            }
        }

        #endregion

        #region Contact Tests

        [TestMethod]
        public void Contact_ReturnsViewResult_WithContactViewModel()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = controller.Contact() as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result.Model, typeof(ContactViewModel));
            }
        }

        [TestMethod]
        public void Contact_SetsCorrectMessage()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = controller.Contact() as ViewResult;
                ContactViewModel vm = (ContactViewModel)result.Model;
                
                // Assert
                Assert.AreEqual("JCarrollOnlineV2 Contact", vm.Message);
            }
        }

        [TestMethod]
        public void Contact_SetsCorrectPageContainer()
        {
            // Arrange
            using (HomeController controller = new HomeController(_fakeContext, _mockRssService))
            {
                // Act
                ViewResult result = controller.Contact() as ViewResult;
                ContactViewModel vm = (ContactViewModel)result.Model;
                
                // Assert
                Assert.AreEqual("ContactPater", vm.PageContainer);
            }
        }

        #endregion

        #region Welcome Tests

        [TestMethod]
        public async Task Welcome_ReturnsViewResult_WithHomeViewModel()
        {
            // Arrange
            HomeController controller = new HomeController(_fakeContext, _mockRssService);

            // Act
            ViewResult result = await controller.Welcome() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(HomeViewModel));
        }

        [TestMethod]
        public async Task Welcome_WithoutAuthentication_ReturnsWelcomeView()
        {
            // Arrange
            HomeController controller = new HomeController(_fakeContext, _mockRssService);

            // Act
            ViewResult result = await controller.Welcome() as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Welcome", result.ViewName);
            Assert.AreEqual("_LayoutWelcome", result.MasterName);
        }

        [TestMethod]
        public async Task Welcome_SetsCorrectMessage()
        {
            // Arrange
            HomeController controller = new HomeController(_fakeContext, _mockRssService);

            // Act
            ViewResult result = await controller.Welcome() as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            // Assert
            Assert.AreEqual("JCarrollOnlineV2 Home - Welcome", vm.Message);
        }

        [TestMethod]
        public async Task Welcome_SetsCorrectPageContainer()
        {
            // Arrange
            HomeController controller = new HomeController(_fakeContext, _mockRssService);

            // Act
            ViewResult result = await controller.Welcome() as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            // Assert
            Assert.AreEqual("Welcome", vm.PageContainer);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            try
            {
                HomeController controller = new HomeController(null, _mockRssService);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public void Constructor_WithNullRssService_ThrowsArgumentNullException()
        {
            // Act & Assert
            try
            {
                HomeController controller = new HomeController(_fakeContext, null);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public void Constructor_WithValidParameters_CreatesController()
        {
            // Act
            HomeController controller = new HomeController(_fakeContext, _mockRssService);

            // Assert
            Assert.IsNotNull(controller);
        }

        #endregion
    }
}
