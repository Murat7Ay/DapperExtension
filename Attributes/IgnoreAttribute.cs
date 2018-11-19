using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        //modelin içinde dbyi ilgilendirmeyen propertylere konularsa bunu atribute olarak ekleyinki sql querylerinde bulunmasın.
    }
}
