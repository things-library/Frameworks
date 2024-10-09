// ================================================================================
// <copyright file="PagedList.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Paged Items Response
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [DebuggerDisplay("Item Count = {Count}, Page = {CurrentPage}, PageSize = {PageSize}")]
    public class PagedList<TEntity> where TEntity : class
    {
        /// <summary>
        /// Total Items Found
        /// </summary>
        [JsonPropertyName("count")]
        [DisplayName("Item Count"), Display(Name = "Count"), Range(0, int.MaxValue), ReadOnly(true)]
        public int Count { get; init; }

        /// <summary>
        /// Current Page Number (0 based)
        /// </summary>
        [JsonPropertyName("page")]
        [Display(Name = "Current Page", Description = "Current page of results.", Prompt = "Current Page"), Range(0, int.MaxValue)]
        public int CurrentPage { get; init; }

        /// <summary>
        /// Total Pages
        /// </summary>
        [JsonPropertyName("totalPages")]
        [Display(Name = "Total Pages")]
        public int TotalPages => (this.PageSize > 0 ? (int)System.Math.Ceiling((double)this.Count / this.PageSize) : 0);

        /// <summary>
        /// Size of each page (0 = no item limit)
        /// </summary>
        [JsonPropertyName("pageSize")]
        [Display(Name = "Page Size"), Range(0, int.MaxValue)]
        public int PageSize { get; init; }

        /// <summary>
        /// List of Items
        /// </summary>
        [JsonPropertyName("items")]
        [Display(Name = "Items"), Required]
        public IEnumerable<TEntity> Items { get; init; } = [];

        /// <summary>
        /// Links
        /// </summary>
        [JsonPropertyName("links")]
        public Dictionary<string, object> Links { get; set; } = [];

        /// <summary>
        /// Paged Listing of items
        /// </summary>
        public PagedList()
        {            
            //nothing
        }

        /// <summary>
        /// Takes a partial data collection
        /// </summary>
        /// <param name="page">Current Page</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="count">Total Items In Set</param>
        /// <param name="data">Collection of items for the page</param>
        public PagedList(int page, int pageSize, int count, IEnumerable<TEntity> data)
        {
            this.CurrentPage = page;
            this.PageSize = pageSize;
            this.Count = count;
            this.Items = data;
        }

        /// <summary>
        /// Takes the entire dataset results
        /// </summary>
        /// <param name="data">Collection of Items</param>
        public PagedList(IEnumerable<TEntity> data)
        {
            this.CurrentPage = 0;
            this.PageSize = data.Count();
            this.Count = data.Count();
            this.Items = data;
        }

        #region IEnumerable<TEntity> Members

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        public void ForEach(Action<TEntity> action)
        {
            foreach (TEntity item in this.Items)
            {
                action(item);
            }
        }

        #endregion


        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
