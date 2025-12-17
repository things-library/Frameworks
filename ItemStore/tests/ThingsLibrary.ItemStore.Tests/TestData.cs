// ================================================================================
// <copyright file="TestData.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Schema.Library;

namespace ThingsLibrary.ItemStore.Tests
{
    [ExcludeFromCodeCoverage]
    public static class TestData
    {
        public static ItemDto GetTestClass() => new ItemDto("movie", "The Martian (2015)", DateTime.UtcNow)
        {
            Tags = new Dictionary<string, string>
            {
                { "year", "2015" },
                { "genre", "Science Fiction" },
                { "director", "Ridley Scott" },
                { "writers", "|Drew Goddard|Andy Weir|" },

                { "budget", "108000000" },
                { "upc", "100000000004" },
                { "aspect_ratio", "2.39:1" },
                { "run_time", "144" }
            },

            Meta = new Dictionary<string, string>
            {
                { "source", "IMDb" },
                { "source_url", "https://www.imdb.com/title/tt3659388/" }
            },

            Items = new Dictionary<string, ItemDto>
            {
                {
                    "revenues", new ItemDto("revenues", "Revenue Details")
                    {
                        Tags = new Dictionary<string, string>
                        {
                            {  "revenue_opening_weekend", "54308575" },
                            {  "revenue_domestic", "228433131" },
                            {  "revenue_international", "402331797" },
                            {  "revenue_world", "630764928" }
                        },

                        Meta = new Dictionary<string, string>
                        {
                            { "currency", "USD" }
                        }

                    }
                },
                {
                    "release", new ItemDto("release", "Release Details")
                    {
                        Tags = new Dictionary<string, string>
                        {
                            {  "release_date", "2015-10-02" },
                            {  "release_theaters", "3854" }
                        },

                        Meta = new Dictionary<string, string>
                        {
                            { "country", "USA" }
                        }
                    }
                }
            }
        };

        public static ItemDto GetTestClass2() => new ItemDto("movie", "Batman: The Dark Knight (2008)", DateTime.UtcNow)
        {
            Tags = new Dictionary<string, string>
            {
                { "year", "2008" },
                { "genre", "Action" },
                { "director", "Christopher Nolan" },
                { "writers", "|JonathanNolan|Christopher Nolan|David S. Goyer|" },

                { "budget", "185000000" },
                { "upc", "100000000003" },
                { "aspect_ratio", "2.39:1" },
                { "run_time", "152" }
            },

            Meta = new Dictionary<string, string>
            {
                { "source", "IMDb" },
                { "source_url", "https://www.imdb.com/title/tt0468569/" }
            },

            Items = new Dictionary<string, ItemDto>
            {
                {
                    "revenues", new ItemDto("revenues", "Revenue Details")
                    {
                        Tags = new Dictionary<string, string>
                        {
                            {  "revenue_opening_weekend", "158411483" },
                            {  "revenue_domestic", "533345358" },
                            {  "revenue_international", "470500000" },
                            {  "revenue_world", "1008287756" }
                        },

                        Meta = new Dictionary<string, string>
                        {
                            { "currency", "USD" } 
                        }
                    }
                },
                {
                    "release", new ItemDto("release", "Release Details")
                    {
                        Tags = new Dictionary<string, string>
                        {
                            {  "release_date", "2008-07-18" },
                            {  "release_theaters", "4366" }
                        },

                        Meta = new Dictionary<string, string>
                        {
                            { "country", "USA" }
                        }
                    }
                }
            }
        };
    }       
}
