using Azure.Data.Tables;
using System.Linq.Expressions;

namespace ThingsLibrary.Entity.AzureTable.Extensions
{
    public static class TableClientExtensions
    {
        public static T QueryFirstOrDefault<T>(this TableClient tableClient, Expression<Func<T, Boolean>> predicate, T defaultValue) where T : class, ITableEntity, new()
        {
            return tableClient.Query<T>(predicate).FirstOrDefault(defaultValue);
        }

        public static T QuerySingleOrDefault<T>(this TableClient tableClient, Expression<Func<T, Boolean>> predicate, T defaultValue) where T : class, ITableEntity, new()
        {
            return tableClient.Query<T>(predicate).SingleOrDefault(defaultValue);
        }
    }
}
