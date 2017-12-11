using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Rss
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rss")]
    public class RssFeedViewModel : RssFeedViewModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<RssFeedItemViewModel> RssFeedItems { get; set; }
    }
}
