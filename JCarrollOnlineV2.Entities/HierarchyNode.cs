using System.Collections.Generic;

namespace JCarrollOnlineV2.Entities
{
    public class HierarchyNode<T> where T : class
    {
        public T Entity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<HierarchyNode<T>> ChildNodes { get; set; }
        public int Depth { get; set; }
        public T Parent { get; set; }
        public List<string> ImageList { get; private set; }
    }
}
