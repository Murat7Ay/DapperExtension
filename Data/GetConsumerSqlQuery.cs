using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperHelper.Data
{
    public class GetConsumerSqlQuery
    {
        private string SelectQuery = " SELECT {0} FROM dbo.Projects p (nolock) ";
        private const string JoinQuery = " JOIN dbo.ProjectDetail pd (nolock) ON pd.ProjectId = p.Id ";
        private const string WhereQuery = " WHERE ";
        private readonly string _workOrderIdQuery;
        private readonly Dictionary<string, string> _tableDictionary = new Dictionary<string, string>();

        public GetConsumerSqlQuery(IList<int> consumerIds,IList<string> customSelect = null)
        {
            if (!consumerIds.Any())
            {
                throw new ArgumentException("ConsumerId boş bırakılamaz.");
            }

            _workOrderIdQuery = consumerIds.Count > 1
                ? $" p.ConsumerId IN ({string.Join(",", consumerIds)}) AND "
                : $" p.ConsumerId = {consumerIds.First()} AND ";

            if (customSelect != null && customSelect.Any())
            {
                SelectQuery = string.Format(SelectQuery, string.Join(",", customSelect));
            }
        }


        public void AddKeyToQuery(string key, string value)
        {
            _tableDictionary.Add(key, value);
        }

        public string BuildSqlQuery()
        {
            StringBuilder sb = new StringBuilder(SelectQuery);

            if (_tableDictionary.Any(x => x.Key.Contains("pd.")))
            {
                sb.Append(JoinQuery);
            }

            sb.Append(WhereQuery);

            sb.Append(_workOrderIdQuery);

            var dictList = _tableDictionary.Select(s => s.Key + " = " + s.Value).ToList();

            sb.Append(string.Join(" AND ", dictList));

            return sb.ToString();
        }

    }
}
