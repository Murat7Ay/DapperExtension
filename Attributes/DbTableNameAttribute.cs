using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableNameAttribute : Attribute
    {
        //Sorgular için tablo ismi ve aliasi
        public string TableName { get; set; }
        public string Alias { get; set; }

        internal DbTableNameAttribute(string tableName, string alias)
        {
            TableName = tableName;
            Alias = alias;
        }
    }
}
