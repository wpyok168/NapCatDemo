using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapCatDemo
{
    public class RecMsgMode
    {
        public long GroupID { get; set; }
        public long UserID { get; set; }
        public bool IsFriend { get; set; }
        public bool IsGroupPrivate { get; set; }
        public string RecMsgContent { get; set; }
    }
}
