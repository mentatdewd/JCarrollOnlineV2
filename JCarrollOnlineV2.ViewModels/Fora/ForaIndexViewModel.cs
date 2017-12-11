using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Fora
{
    public class ForaIndexViewModel : ForaViewModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<ForaIndexItemViewModel> ForaIndexItems { get; set; }
    }
}
