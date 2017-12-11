using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.HierarchyNode
{
    public class HierarchyNodesViewModel<T> : ViewModelBase
    {
        [Required]
        public T Entity { get; set; }

        [Required]
        public IEnumerable<HierarchyNodesViewModel<T>> ChildNodes { get; set; }
        
        [Required]
        public int Depth { get; set; }
        
        [Required]
        public T Parent { get; set; }

        public List<string> ImageList { get; set; }
    }
}
