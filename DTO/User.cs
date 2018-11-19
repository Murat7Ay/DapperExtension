using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperHelper.Attributes;

namespace DapperHelper.DTO
{
    [DbTableName("dbo.Users", "u")]
    public class User
    {
        [DbKey]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime BornDateTime { get; set; }
        [Ignore]
        public int Age
        {
            get
            {
                int yearsPassed = DateTime.Now.Year - BornDateTime.Year;
                if (DateTime.Now.Month < BornDateTime.Month || (DateTime.Now.Month == BornDateTime.Month && DateTime.Now.Day < BornDateTime.Day))
                {
                    yearsPassed--;
                }
                return yearsPassed;
            }
        }

    }
}
