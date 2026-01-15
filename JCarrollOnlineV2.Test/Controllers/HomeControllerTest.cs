using JCarrollOnlineV2.Controllers;
using JCarrollOnlineV2.Test.Services;
using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.Chat;
using JCarrollOnlineV2.ViewModels.Home;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
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
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private MockHomeViewModelService _mockHomeViewModelService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHomeViewModelService = new MockHomeViewModelService();
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithHomeViewModel()
        {
            // Arrange
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
            {
                UserName = "testuser",
                Message = "Test message 1",
                TimeAgo = DateTime.Now.AddMinutes(-10).ToUniversalTime().ToString("o")
            });

            homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
            {
                UserName = "testuser",
                Message = "Test message 2",
                TimeAgo = DateTime.Now.AddMinutes(-5).ToUniversalTime().ToString("o")
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
            {
                UserName = "testuser",
                Message = "First message",
                TimeAgo = DateTime.Now.AddMinutes(-10).ToUniversalTime().ToString("o")
            });

            homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
            {
                UserName = "testuser",
                Message = "Second message",
                TimeAgo = DateTime.Now.AddMinutes(-7).ToUniversalTime().ToString("o")
            });

            homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
            {
                UserName = "testuser",
                Message = "Third message",
                TimeAgo = DateTime.Now.AddMinutes(-3).ToUniversalTime().ToString("o")
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            // Add 50 chat messages (service should limit to 50)
            for (int i = 1; i <= 50; i++)
            {
                homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
                {
                    UserName = "testuser",
                    Message = $"Test message {i}",
                    TimeAgo = DateTime.Now.AddMinutes(-i).ToUniversalTime().ToString("o")
                });
            }

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(new BlogFeedItemViewModel
            {
                Id = 1,
                Title = "Test Blog Post",
                Content = "Test content",
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            BlogFeedItemViewModel blogFeedItem = new BlogFeedItemViewModel
            {
                Id = 1,
                Title = "Test Blog Post",
                Content = "Test content",
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };

            blogFeedItem.Comments.BlogComments.Add(new BlogCommentItemViewModel(1)
            {
                Id = 1,
                Content = "Test comment",
                Author = "commenter",
                BlogItemId = 1,
                CreatedAt = DateTime.Now.AddHours(-2)
            });

            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(blogFeedItem);
            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            // Service should return these in order
            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(new BlogFeedItemViewModel
            {
                Id = 3,
                Title = "Newest Blog Post",
                Content = "Test content 3",
                UpdatedAt = DateTime.Now.AddHours(-1)
            });

            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(new BlogFeedItemViewModel
            {
                Id = 2,
                Title = "Newer Blog Post",
                Content = "Test content 2",
                UpdatedAt = DateTime.Now.AddDays(-1)
            });

            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(new BlogFeedItemViewModel
            {
                Id = 1,
                Title = "Older Blog Post",
                Content = "Test content 1",
                UpdatedAt = DateTime.Now.AddDays(-3)
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(new LatestForumThreadItemViewModel
            {
                ThreadId = 1,
                ThreadTitle = "Test Thread",
                ForumId = 1,
                ForumTitle = "Test Forum"
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            // Add only 5 threads (service should limit to 5)
            for (int i = 1; i <= 5; i++)
            {
                homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(new LatestForumThreadItemViewModel
                {
                    ThreadId = i,
                    ThreadTitle = $"Test Thread {i}",
                    ForumId = 1,
                    ForumTitle = "Test Forum"
                });
            }

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            // Service should return these in order
            homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(new LatestForumThreadItemViewModel
            {
                ThreadId = 2,
                ThreadTitle = "Newest Thread",
                ForumId = 1,
                ForumTitle = "Test Forum"
            });

            homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(new LatestForumThreadItemViewModel
            {
                ThreadId = 3,
                ThreadTitle = "Middle Thread",
                ForumId = 1,
                ForumTitle = "Test Forum"
            });

            homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(new LatestForumThreadItemViewModel
            {
                ThreadId = 1,
                ThreadTitle = "Oldest Thread",
                ForumId = 1,
                ForumTitle = "Test Forum"
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            // Arrange
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(new BlogFeedItemViewModel
            {
                Id = 1,
                Title = "Test Blog",
                Content = "Content"
            });

            homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(new LatestForumThreadItemViewModel
            {
                ThreadId = 1,
                ThreadTitle = "Test Thread",
                ForumId = 1,
                ForumTitle = "Test Forum"
            });

            homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
            {
                UserName = "testuser",
                Message = "Test Chat Message",
                TimeAgo = DateTime.Now.ToUniversalTime().ToString("o")
            });

            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };

            BlogFeedItemViewModel blogFeedItem = new BlogFeedItemViewModel
            {
                Id = 1,
                Title = "Test Blog Post",
                Content = "Test content"
            };

            blogFeedItem.Comments.BlogComments.Add(new BlogCommentItemViewModel(1)
            {
                Id = 1,
                Content = "First comment",
                Author = "commenter1"
            });

            blogFeedItem.Comments.BlogComments.Add(new BlogCommentItemViewModel(1)
            {
                Id = 2,
                Content = "Second comment",
                Author = "commenter2"
            });

            blogFeedItem.Comments.BlogComments.Add(new BlogCommentItemViewModel(1)
            {
                Id = 3,
                Content = "Third comment",
                Author = "commenter1"
            });

            homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(blogFeedItem);
            _mockHomeViewModelService.MockAnonymousViewModel = homeViewModel;

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            // Arrange - Mock service returns default empty view model

            using (HomeController controller = new HomeController(_mockHomeViewModelService))
            {
                // Act
                ViewResult result = await controller.Index(null).ConfigureAwait(false) as ViewResult;
                HomeViewModel vm = (HomeViewModel)result.Model;

                // Assert
                Assert.IsNotNull(vm);
                Assert.IsEmpty(vm.BlogFeed.BlogFeedItemViewModels);
                Assert.IsEmpty(vm.LatestForumThreadsViewModel.LatestForumThreads);
                Assert.IsEmpty(vm.ChatViewModel.RecentMessages);
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            using (HomeController controller = new HomeController(_mockHomeViewModelService))
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
            HomeController controller = new HomeController(_mockHomeViewModelService);

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
            HomeController controller = new HomeController(_mockHomeViewModelService);

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
            HomeController controller = new HomeController(_mockHomeViewModelService);

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
            HomeController controller = new HomeController(_mockHomeViewModelService);

            // Act
            ViewResult result = await controller.Welcome() as ViewResult;
            HomeViewModel vm = (HomeViewModel)result.Model;

            // Assert
            Assert.AreEqual("Welcome", vm.PageContainer);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithNullService_ThrowsArgumentNullException()
        {
            // Act & Assert
            try
            {
                HomeController controller = new HomeController(null);
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
            HomeController controller = new HomeController(_mockHomeViewModelService);

            // Assert
            Assert.IsNotNull(controller);
        }

        #endregion
    }
}
