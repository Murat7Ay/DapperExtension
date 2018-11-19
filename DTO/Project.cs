using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperHelper.Attributes;

namespace DapperHelper.DTO
{
    [DbTableName("dbo.Projects","p")]
    public class Project
    {
        [DbKey]
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public string Name { get; set; }
        //Tablo alanları

        //Tüketici bazında özel alanlar
        public string Spec1 { get; set; }
        public string Spec2 { get; set; }
        public string Spec3 { get; set; }
    }
}
