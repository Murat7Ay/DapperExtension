using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DapperHelper.Data
{
    public static class DapperExtension
    {
        /// <summary>
        /// Insert etmek istediğiniz modeli parametre olarak yollayın. Geri dönen modelde sqldeki Id alanı bulunur.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="insertedDto"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static T Insert<T>(this IDbConnection db, T insertedDto, IDbTransaction tran = null)
        {
            var sqlQuery = QueryBuilder.GetInsertQuery(insertedDto);

            return db.QuerySingle<T>(sqlQuery, insertedDto, tran);
        }
        /// <summary>
        /// Bulk insert için listde halinda parametre yollayabilirsiniz. True yada false döner kayıt başarılı olma durumuna göre
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="insertedList"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static bool InsertList<T>(this IDbConnection db, List<T> insertedList, IDbTransaction tran = null)
        {
            var sqlQuery = QueryBuilder.GetInsertQuery(insertedList.First(), false);

            var effectedRows = db.Execute(sqlQuery, insertedList, tran);

            return effectedRows == insertedList.Count;
        }
        /// <summary>
        /// Fetchlediğiniz modeli ( Id değeri bulunan) update eder.Size güncellenmiş model döner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="currentDto"></param>
        /// <param name="updateExpression"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static T Update<T>(this IDbConnection db, T currentDto, Expression<Func<T>> updateExpression, IDbTransaction tran = null)
        {
            IDictionary<string, object> parameters;

            var sqlQuery = QueryBuilder.GetUpdateQuery(currentDto, updateExpression, out parameters, ref currentDto);

            var resultFromUpdate = db.Execute(sqlQuery, parameters, tran);

            if (resultFromUpdate <= 0)
            {
                throw new InvalidCastException(nameof(currentDto) + " update error.");
            }

            return currentDto;
        }
    }
}
