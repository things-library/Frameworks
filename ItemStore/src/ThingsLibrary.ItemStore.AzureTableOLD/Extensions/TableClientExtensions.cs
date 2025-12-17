using Azure.Data.Tables;

using System.Linq;
using System.Linq.Expressions;

namespace Starlight.Entity.AzureTable.Extensions
{
    public static class TableClientExtensions
    {
        public static T QueryFirstOrDefault<T>(this TableClient tableClient, Expression<Func<T, Boolean>> predicate) where T : class, ITableEntity, new()
        {
            return tableClient.Query<T>(predicate).FirstOrDefault();
        }

        public static T QuerySingleOrDefault<T>(this TableClient tableClient, Expression<Func<T, Boolean>> predicate) where T : class, ITableEntity, new()
        {
            return tableClient.Query<T>(predicate).SingleOrDefault();
        }
    }
}
