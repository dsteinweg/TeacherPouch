using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using TeacherPouch.Models;

namespace TeacherPouch.Services
{
    public class SearchService
    {
        public SearchService(
            PhotoService photoService,
            TagService tagService)
        {
            _photoService = photoService;
            _tagService = tagService;
        }

        private readonly PhotoService _photoService;
        private readonly TagService _tagService;

        public SearchResultsOr SearchOr(string query)
        {
            query = query.ToLower();

            var stopwatch = Stopwatch.StartNew();

            var results = new SearchResultsOr(query);

            var exactTagMatch = _tagService.FindTag(query);
            if (exactTagMatch != null)
            {
                var exactTagResult = new TagSearchResult(exactTagMatch);

                results.TagResults.Add(exactTagResult);
            }

            Tag pluralSuffixTagMatch = null;
            Tag singularSuffixTagMatch = null;
            if (!query.EndsWith("s"))
            {
                pluralSuffixTagMatch = _tagService.FindTag(query + "s");
                if (pluralSuffixTagMatch != null)
                {
                    results.TagResults.Add(new TagSearchResult(pluralSuffixTagMatch));
                }
            }
            else // ends with 's'
            {
                singularSuffixTagMatch = _tagService.FindTag(query.TrimEnd('s'));
                if (singularSuffixTagMatch != null)
                {
                    results.TagResults.Add(new TagSearchResult(singularSuffixTagMatch));
                }
            }

            var allTags = _tagService.GetAllTags();

            var startsWithTagMatches = from tag in allTags
                                       where (exactTagMatch == null || exactTagMatch.Id != tag.Id)
                                          && (pluralSuffixTagMatch == null || pluralSuffixTagMatch.Id != tag.Id)
                                          && (singularSuffixTagMatch == null || singularSuffixTagMatch.Id != tag.Id)
                                          && tag.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)
                                       orderby tag.Name ascending
                                       select tag;

            foreach (var tag in startsWithTagMatches)
            {
                var aggregateTag = _tagService.FindTag(tag.Id);
                results.TagResults.Add(new TagSearchResult(aggregateTag));
            }

            var endsWithTagMatches = from tag in allTags
                                     where (exactTagMatch == null || exactTagMatch.Id != tag.Id)
                                        && (pluralSuffixTagMatch == null || pluralSuffixTagMatch.Id != tag.Id)
                                        && (singularSuffixTagMatch == null || singularSuffixTagMatch.Id != tag.Id)
                                        && tag.Name.EndsWith(query, StringComparison.OrdinalIgnoreCase)
                                     orderby tag.Name ascending
                                     select tag;

            foreach (var tag in endsWithTagMatches)
            {
                var aggregateTag = _tagService.FindTag(tag.Id);
                results.TagResults.Add(new TagSearchResult(aggregateTag));
            }

            // If no results were found using exact tag and plural/singular searching,
            // try splitting the search query into tokens and search for multiple tags.
            if (!results.HasAnyResults)
            {
                var searchWords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var tagSearchResults = new List<TagSearchResult>(searchWords.Length);
                foreach (var searchWord in searchWords)
                {
                    var tag = _tagService.FindTag(searchWord);
                    if (tag != null)
                    {
                        results.TagResults.Add(new TagSearchResult(tag));
                    }
                }
            }

            stopwatch.Stop();

            results.SearchDuration = stopwatch.Elapsed;

            return results;
        }

        public SearchResultsAnd SearchAnd(string query)
        {
            var stopwatch = Stopwatch.StartNew();

            var searchWords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var matchingTags = new List<Tag>();
            foreach (var searchWord in searchWords)
            {
                var tag = _tagService.FindTag(searchWord);
                if (tag != null)
                    matchingTags.Add(tag);
            }

            var uniquePhotos = Enumerable.Empty<Photo>();
            foreach (var tag in matchingTags)
            {
                var photos = tag.PhotoTags.Select(pt => pt.Photo);

                if (!uniquePhotos.Any())
                {
                    uniquePhotos = photos;
                }
                else
                {
                    uniquePhotos = uniquePhotos.Intersect(photos.ToList());
                }
            }

            var results = new SearchResultsAnd(query);
            results.Tags = matchingTags;
            results.Photos = uniquePhotos.ToList();

            stopwatch.Stop();

            results.SearchDuration = stopwatch.Elapsed;

            return results;
        }

        public IEnumerable<string> TagAutocompleteSearch(string query)
        {
            return _tagService
                .FindTagsLike(query)
                .Select(tag => tag.Name)
                .OrderBy(tagName => tagName);
        }
    }
}
