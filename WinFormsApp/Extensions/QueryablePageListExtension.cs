using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinFormsApp.Extensions
{
    /// <summary>
    /// Pagination
    /// </summary>
    public static class QueryablePageListExtension
    {

        public static Task<List<T>> ToPage<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return Task.Run(() => { return source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); });
        }


        public static Task<List<T>> ToPage<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return Task.Run(() => { return source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); });
        }
    }
}
