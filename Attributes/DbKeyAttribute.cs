using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbKeyAttribute : Attribute
    {
        //sql sorgusu üretirken buraya sahip propertyler insert sorgularında ignore edilecek
        //update sorgusu içinse gerekicek
    }
}
