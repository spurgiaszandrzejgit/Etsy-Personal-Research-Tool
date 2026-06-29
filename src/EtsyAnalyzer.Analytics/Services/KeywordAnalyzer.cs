using EtsyAnalyzer.Core.DTOs;
using System.Text.RegularExpressions;

namespace EtsyAnalyzer.Analytics.Services;

public class KeywordAnalyzer
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "are", "as", "at", "be", "by", "for", "from", "has", "he",
        "in", "is", "it", "its", "of", "on", "that", "the", "to", "was", "will", "with",
        "or", "not", "this", "but", "they", "have", "had", "what", "when", "where", "who",
        "which", "why", "how", "all", "each", "every", "both", "few", "more", "most", "other",
        "some", "such", "no", "nor", "too", "very", "can", "just", "should", "now"
    };

    public List<KeywordFrequencyDto> AnalyzeKeywords(IEnumerable<ListingDto> listings, int topCount = 20)
    {
        var wordFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var totalWords = 0;

        foreach (var listing in listings)
        {
            var words = ExtractWords(listing.Title);

            foreach (var word in words)
            {
                if (!StopWords.Contains(word) && word.Length >= 3)
                {
                    if (wordFrequency.ContainsKey(word))
                        wordFrequency[word]++;
                    else
                        wordFrequency[word] = 1;

                    totalWords++;
                }
            }
        }

        return wordFrequency
            .OrderByDescending(kvp => kvp.Value)
            .Take(topCount)
            .Select(kvp => new KeywordFrequencyDto
            {
                Keyword = kvp.Key,
                Frequency = kvp.Value,
                Percentage = totalWords > 0 ? Math.Round((decimal)kvp.Value / totalWords * 100, 2) : 0
            })
            .ToList();
    }

    public List<KeywordFrequencyDto> AnalyzeTags(IEnumerable<ListingDto> listings, int topCount = 20)
    {
        var tagFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var totalTags = 0;

        foreach (var listing in listings)
        {
            foreach (var tag in listing.Tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    if (tagFrequency.ContainsKey(tag))
                        tagFrequency[tag]++;
                    else
                        tagFrequency[tag] = 1;

                    totalTags++;
                }
            }
        }

        return tagFrequency
            .OrderByDescending(kvp => kvp.Value)
            .Take(topCount)
            .Select(kvp => new KeywordFrequencyDto
            {
                Keyword = kvp.Key,
                Frequency = kvp.Value,
                Percentage = totalTags > 0 ? Math.Round((decimal)kvp.Value / totalTags * 100, 2) : 0
            })
            .ToList();
    }

    private List<string> ExtractWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        // Убираем специальные символы и разбиваем на слова
        var cleanText = Regex.Replace(text, @"[^\w\s]", " ");
        return cleanText.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length >= 3)
            .ToList();
    }
}
