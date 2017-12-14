using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels.Blog
{
    public class BlogCommentViewModel
    {
        public string Author { get; set; }
        public string Content { get; set; }
        public int BlogItemId { get; set; }
    }
}
