using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperHelper
{
    public class BaseRequest
    {
        public int CompanyId { get; set; }
        public Dictionary<string, string> ConsumerKeys { get; set; }
        public string ConsumerOrderTypeCode { get; set; }
    }
}
