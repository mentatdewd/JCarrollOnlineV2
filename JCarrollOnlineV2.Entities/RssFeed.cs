using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Entities
{
    public class RssFeed
    {
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        public string Summary { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PublishedAt { get; set; }

        [DataType(DataType.Text)]
        [StringLength(38)]
        public string Guid { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }//   :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }//  :null => false
   }
}
