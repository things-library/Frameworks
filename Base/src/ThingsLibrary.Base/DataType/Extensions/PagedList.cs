// ================================================================================
// <copyright file="PagedList.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static class PagedListExtensions
    {
        /// <summary>
        /// Convert a IQuerable query into a paged list object
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="query"><see cref="IQueryable{TEntity}"/></param>
        /// <param name="page">Current Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns><see cref="PagedList{TEntity}"/></returns>
        public static PagedList<T> ToPagedResult<T>(this IQueryable<T> query, int page = 0, int pageSize = 0) where T : class
        {
            int count = query.Count();

            // pageSize of zero means return all
            if(pageSize > 0)
            {
                var skip = (page - 1) * pageSize;

                return new PagedList<T>()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    Count = count,
                    Items = query.Skip(skip).Take(pageSize)
                };
            }
            else
            {                
                return new PagedList<T>()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    Count = count,
                    Items = query
                };
            }            
        }

        /// <summary>
        /// Convert a list of items into a paged list object
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="list"><see cref="List{T}"/></param>
        /// <param name="page">Current Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns><see cref="PagedList{TEntity}"/></returns>
        public static PagedList<T> ToPagedResult<T>(this List<T> list, int page = 0, int pageSize = 0) where T : class
        {
            // pageSize of zero means return all
            if (pageSize > 0)
            {
                var skip = (page - 1) * pageSize;

                return new PagedList<T>()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    Count = list.Count,
                    Items = list.Skip(skip).Take(pageSize)
                };
            }
            else
            {
                return new PagedList<T>()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    Count = list.Count,
                    Items = list
                };
            }
        }
    }
}
