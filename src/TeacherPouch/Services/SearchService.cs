using System.Diagnostics;
using TeacherPouch.Models;

namespace TeacherPouch.Services;

public class SearchService(TagService _tagService)
{
    public async Task<SearchResultsOr> SearchOr(string query, CancellationToken cancellationToken = default)
    {
        query = query.ToLower();

        var stopwatch = Stopwatch.StartNew();

        var results = new SearchResultsOr(query);

        var exactTagMatch = await _tagService.FindTag(query, cancellationToken);
        if (exactTagMatch is not null)
        {
            var exactTagResult = new TagSearchResult(exactTagMatch);
            results.TagResults.Add(exactTagResult);
        }

        Tag? pluralSuffixTagMatch = null;
        Tag? singularSuffixTagMatch = null;
        if (!query.EndsWith('s'))
        {
            pluralSuffixTagMatch = await _tagService.FindTag(query + "s", cancellationToken);
            if (pluralSuffixTagMatch is not null)
            {
                results.TagResults.Add(new TagSearchResult(pluralSuffixTagMatch));
            }
        }
        else // ends with 's'
        {
            singularSuffixTagMatch = await _tagService.FindTag(query.TrimEnd('s'), cancellationToken);
            if (singularSuffixTagMatch is not null)
            {
                results.TagResults.Add(new TagSearchResult(singularSuffixTagMatch));
            }
        }

        var allTags = await _tagService.GetAllTags(cancellationToken);

        var startsWithTagMatches =
            from tag in allTags
            where (exactTagMatch is null || exactTagMatch.Id != tag.Id)
                && (pluralSuffixTagMatch is null || pluralSuffixTagMatch.Id != tag.Id)
                && (singularSuffixTagMatch is null || singularSuffixTagMatch.Id != tag.Id)
                && tag.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)
            orderby tag.Name ascending
            select tag;

        foreach (var tag in startsWithTagMatches)
        {
            var aggregateTag = await _tagService.FindTag(tag.Id, cancellationToken);
            if (aggregateTag is not null)
                results.TagResults.Add(new TagSearchResult(aggregateTag));
        }

        var endsWithTagMatches =
            from tag in allTags
            where (exactTagMatch is null || exactTagMatch.Id != tag.Id)
            && (pluralSuffixTagMatch is null || pluralSuffixTagMatch.Id != tag.Id)
            && (singularSuffixTagMatch is null || singularSuffixTagMatch.Id != tag.Id)
            && tag.Name.EndsWith(query, StringComparison.OrdinalIgnoreCase)
            orderby tag.Name ascending
            select tag;

        foreach (var tag in endsWithTagMatches)
        {
            var aggregateTag = await _tagService.FindTag(tag.Id, cancellationToken);
            if (aggregateTag is not null)
                results.TagResults.Add(new TagSearchResult(aggregateTag));
        }

        // If no results were found using exact tag and plural/singular searching,
        // try splitting the search query into tokens and search for multiple tags.
        if (!results.HasAnyResults)
        {
            var searchWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var tagSearchResults = new List<TagSearchResult>(searchWords.Length);
            foreach (var searchWord in searchWords)
            {
                var tag = await _tagService.FindTag(searchWord, cancellationToken);
                if (tag is not null)
                    results.TagResults.Add(new TagSearchResult(tag));
            }
        }

        stopwatch.Stop();

        results.SearchDuration = stopwatch.Elapsed;

        return results;
    }

    public async Task<SearchResultsAnd> SearchAnd(string query, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var searchWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var matchingTags = new List<Tag>();
        foreach (var searchWord in searchWords)
        {
            var tag = await _tagService.FindTag(searchWord, cancellationToken);
            if (tag is not null)
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

        var results = new SearchResultsAnd(query)
        {
            Tags = matchingTags,
            Photos = uniquePhotos.ToList()
        };

        stopwatch.Stop();

        results.SearchDuration = stopwatch.Elapsed;

        return results;
    }

    public async Task<string[]> TagAutocompleteSearch(string query, CancellationToken cancellationToken = default)
    {
        var tags = await _tagService.FindTagsLike(query, cancellationToken);
        return tags.Select(tag => tag.Name).OrderBy(tagName => tagName).ToArray();
    }
}
