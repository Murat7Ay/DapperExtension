using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DapperHelper.Attributes;

namespace DapperHelper.Data
{
    public static class QueryBuilder
    {
        #region Id_ile_Update_Kullanmayın
        //public static string GetUpdateQueryWithoutUsingDbKey<T>(Expression<Func<T>> updateExpression, Expression<Func<T, bool>> whereExpression, out IDictionary<string, object> updateParameters) where T : new()
        //{
        //    var obj = new T();
        //    var parameters = new ExpandoObject() as IDictionary<string, object>;
        //    IList<string> setParameterSql = new List<string>();
        //    var attribute = GetTableNameAttribute(obj);
        //    string alias = attribute.Alias;
        //    string tableName = attribute.TableName;

        //    var memberInitExpression = updateExpression.Body as MemberInitExpression;

        //    if (memberInitExpression?.Bindings == null)
        //    {
        //        throw new ArgumentNullException(nameof(obj) + " Update expression tanımları yapılmalı");
        //    }

        //    foreach (MemberBinding binding in memberInitExpression.Bindings)
        //    {
        //        var memberAssignment = binding as MemberAssignment;

        //        if (memberAssignment?.Expression == null)
        //        {
        //            throw new ArgumentNullException();
        //        }
        //        Expression memberExpression = memberAssignment.Expression;
        //        LambdaExpression lambda = Expression.Lambda(memberExpression, null);
        //        var name = binding.Member.Name;
        //        var value = lambda.Compile().DynamicInvoke();
        //        parameters.Add(name, value);
        //        setParameterSql.Add($"{name}=@{name}");
        //    }


        //    var t = whereExpression.Body;


        //    BinaryExpression expression = (BinaryExpression) whereExpression.Body;

        //    string leftName = ((MemberExpression)expression.Left).Member.Name;
        //    Expression rightValue = expression.Right;

        //    updateParameters = parameters;
        //    return alias;

        //}

        #endregion

        /// <summary>
        /// Dbkey alanı olan modelin belirtilen propertylerini update sorgusu üretir.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="updateExpression">Hangi propertylerin update edileceğini yolladıgınız yer</param>
        /// <param name="updateParameters">Update edilen parametrelerin parameter halinda kullanılacagı collection</param>
        /// <param name="updatedObj">update edilmiş model</param>
        /// <returns></returns>
        public static string GetUpdateQuery<T>(T obj, Expression<Func<T>> updateExpression, out IDictionary<string, object> updateParameters, ref T updatedObj)
        {
            var parameters = new ExpandoObject() as IDictionary<string, object>;
            IList<string> parameterSql = new List<string>();
            var attribute = GetTableNameAttribute(obj);
            var keyProperty = obj.GetType().GetProperties().Where(x => x.IsDefined(typeof(DbKeyAttribute), false))
                .Select(s => new { s.Name, Value = s.GetValue(obj, null) }).FirstOrDefault();

            if (keyProperty == null)
            {
                throw new ArgumentNullException(nameof(obj) + " DbKeyAttribute tanımlanmamış");
            }

            string alias = attribute.Alias;
            string tableName = attribute.TableName;
            string tableKey = keyProperty.Name;
            object tableKeyObject = keyProperty.Value;

            parameters.Add(tableKey, tableKeyObject);
            var memberInitExpression = updateExpression.Body as MemberInitExpression;

            if (memberInitExpression?.Bindings == null)
            {
                throw new ArgumentNullException(nameof(obj) + " Update expression tanımları yapılmalı");
            }

            foreach (MemberBinding binding in memberInitExpression.Bindings)
            {
                var memberAssignment = binding as MemberAssignment;

                if (memberAssignment?.Expression == null)
                {
                    throw new ArgumentNullException();
                }
                Expression memberExpression = memberAssignment.Expression;
                LambdaExpression lambda = Expression.Lambda(memberExpression, null);
                var name = binding.Member.Name;
                var value = lambda.Compile().DynamicInvoke();
                parameters.Add(name, value);
                parameterSql.Add($" {alias}.{name} = @{name} ");

                obj.GetType().GetProperty(name)?.SetValue(obj, value);
            }

            updateParameters = parameters;
            updatedObj = obj;

            return
                $" UPDATE {alias} SET {string.Join(",", parameterSql)} FROM {tableName} {alias} WHERE {alias}.{tableKey} = @{tableKey} ";
        }
        /// <summary>
        /// Dbkey ve Ignore attributeleri hariç bulunan propertyler ile insert sorgusu üretir.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="returnInsertedObj">True ise kayıt edilen tablonun tum propertyleri sqlden geri döner [OUTPUT.*] sql komutunu üretilen sorguya ekliyor. </param>
        /// <returns></returns>
        public static string GetInsertQuery<T>(T obj, bool returnInsertedObj = true)
        {
            var attribute = GetTableNameAttribute(obj);
            var properties = obj.GetType().GetProperties().Where(x =>
               !x.IsDefined(typeof(DbKeyAttribute), false) && !x.IsDefined(typeof(IgnoreAttribute), false)).Select(s => s.Name).ToList();
            return
                $"INSERT INTO {attribute.TableName} ({string.Join(",", properties)}) {(returnInsertedObj ? " OUTPUT INSERTED.* " : string.Empty)} VALUES ({string.Join(",", properties.Select(s => "@" + s))})";
        }

        /// <summary>
        /// Classın tablodaki adını ve alias değerini geri döner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static DbTableNameAttribute GetTableNameAttribute<T>(T obj)
        {
            var attribute = typeof(T).GetCustomAttributes(
                typeof(DbTableNameAttribute), true
            ).FirstOrDefault() as DbTableNameAttribute;

            if (attribute == null)
            {
                throw new ArgumentNullException();
            }

            return attribute;
        }
    }
}
