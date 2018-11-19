using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperHelper.Attributes;

namespace DapperHelper.DTO
{
    [DbTableName("dbo.ProjectDetail","pd")]
    public class ProjectDetail
    {
        [DbKey]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        //Tablo alanları

        //Özel Alanlar
        public string Spec1 { get; set; }
    }
}
