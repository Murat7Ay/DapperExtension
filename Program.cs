using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperHelper.Data;
using DapperHelper.DTO;

namespace DapperHelper
{
    public class Program
    {
        static void Main(string[] args)
        {
            var request = new BaseRequest
            {
                CompanyId = 1,
                ConsumerKeys = new Dictionary<string, string> { { "Type", "Mekanik" }, { "Order","177"} },
                ConsumerOrderTypeCode = "ProjectType1"
            };


            if (!request.ConsumerKeys.Any())
            {
                throw new ArgumentException("ConsumerKey tanımlanmalı");
            }

            var consumerKeyList = GetConsumerKeys(request);

            if (!consumerKeyList.Any())
            {
                throw new ArgumentException("ConsumerKey tanımlanmalı");
            }

            var queryGenerator =
                new GetConsumerSqlQuery(consumerKeyList.Select(s => s.ConsumerId).Distinct().ToList());

            foreach (var consumerKey in request.ConsumerKeys)
            {
                var valueFromDb = consumerKeyList.Where(x => x.Key == consumerKey.Key && x.IsActive)
                    .Select(s => s.Value).FirstOrDefault();

                if (valueFromDb != null)
                {
                    queryGenerator.AddKeyToQuery(valueFromDb.Split('|')[1], "@" + consumerKey.Key);
                }
            }

            var query = queryGenerator.BuildSqlQuery();

            var paramObject = new ExpandoObject() as IDictionary<string, object>;

            foreach (var kvPair in request.ConsumerKeys)
            {
                paramObject.Add(kvPair.Key, kvPair.Value);
            }

            using (var db = ConnectionFactory.GetOpenConnection("connection string"))
            {
                // Sql sorgusu

                // Select p.* FROM dbo.Projects p (NOLOCK)
                // JOIN dbo.ProjectDetail pd (NOLOCK) ON pd.ProjectId = p.Id
                // WHERE p.ConsumerId = 111 AND p.Spec1 = 'Mekanik' AND pd.Spec1 = '177' 
                var project = db.Query<Project>(query, paramObject).First();


                Console.WriteLine(project.Name);
                //ProjectName

                var updatedProject =  db.Update(project, () => new Project
                {
                    Name = "UpdatedProjectName"
                });

                project = db.Query<Project>(query, paramObject).First();

                Console.WriteLine(project.Name);
                //UpdatedProjectName
                Console.WriteLine(updatedProject.Name);
                //UpdatedProjectName

                var userList = new List<User>
                {
                    new User {UserName = "test1", Password = "123", BornDateTime = DateTime.Today},
                    new User {UserName = "test2", Password = "123", BornDateTime = DateTime.Today},
                    new User {UserName = "test3", Password = "123", BornDateTime = DateTime.Today}
                };

                db.InsertList(userList);

                var newProject = db.Insert(new Project
                {
                    Name = "Project 1",
                    ConsumerId = 1,
                    Spec1 = "Type1"
                });

                Console.WriteLine(newProject.Id);

            }





        }

        /// <summary>
        /// Firmanın proje tablosundaki kayıtları için özelleştirilmiş anahtarlarının karşılığı
        /// Spec1 alanında Type değeri
        /// Spec2 alanında Order değeri
        /// </summary>
        /// <param name="baseRequest"></param>
        /// <returns></returns>
        private static List<ConsumerKeyList> GetConsumerKeys(BaseRequest baseRequest)
        {
            //Şirketin özelinde sql sorgusu için parametreler
            //Key-Value tarzında dönüş. Örn. dbo.Projects|p.Spec1 veya dbo.ProjectDetail|pd.Spec1
            //ProjectType1	Type	dbo.Projects|p.Spec1
            //ProjectType1	Order	dbo.ProjectDetail|pd.Spec1
            throw new NotImplementedException();
        }
    }

    public class ConsumerKeyList
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int ConsumerId { get; set; }
        public bool IsActive { get; set; }
    }

    
}
