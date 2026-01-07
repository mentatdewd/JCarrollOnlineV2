namespace JCarrollOnlineV2.ViewModels.Home
{
    public class LatestForumThreadItemViewModel : ViewModelBase
    {
        public string ForumTitle { get; set; }
        public string ThreadTitle { get; set; }

        public int ForumId { get; set; }

        public int ThreadId { get; set; }
    }
}
