using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reddit.Models
{
    public class RequestPayload
    {
        public int raw_json { get; set; }
        public string after { get; set; }
        public string before { get; set; }
        public int limit { get; set; }
        public int count { get; set; }
        public RequestPayload()
        {
            raw_json = 1;
        }
    }
}
