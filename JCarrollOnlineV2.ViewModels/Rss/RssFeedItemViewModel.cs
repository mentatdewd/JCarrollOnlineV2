using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Rss
{
    public class RssFeedItemViewModel : RssFeedViewModelBase
    {
        [DataType(DataType.Url)]
        public Uri Link { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public string Title { get; set; }
    }
}
