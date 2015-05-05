using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCarrollOnlineV2.EmailModels
{
    public class EmailModelBase
    {
        /// <summary>
        /// Address to send to
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Contents of email
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Author { get; set; }
    }
}