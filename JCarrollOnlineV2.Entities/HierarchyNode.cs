using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Entities
{
    public class HierarchyNode<T> where T : class
    {
        public T Entity { get; set; }
        public List<HierarchyNode<T>> ChildNodes { get; set; }
        public int Depth { get; set; }
        public T Parent { get; set; }
        public List<string> ImageList { get; set; }
    }
}
