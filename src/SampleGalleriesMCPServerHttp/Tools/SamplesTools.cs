using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;

namespace SampleGalleriesMCPServer;

/// <summary>
/// Provides a tool to search for samples in the Community Samples Gallery.
/// </summary>
[McpServerToolType]
internal class SamplesTools
{
    private readonly HttpClient _httpClient;
    private readonly SamplesApiConfiguration _configuration;
    private readonly ILogger<SamplesTools> _logger;

    public SamplesTools(
        HttpClient httpClient,
        IOptions<SamplesApiConfiguration> configuration,
        ILogger<SamplesTools> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [McpServerTool]
    [Description("Allows retrieving samples by product in the Community Samples Gallery.")]
    public async Task<PagedResult<Sample>> SamplesByProduct(
        [Description("The product to use for filtering samples.")] string product,
        [Description("The page index for pagination.")] int pageIndex,
        [Description("The page size for pagination.")] int pageSize
    )
    {
        if (string.IsNullOrEmpty(product))
        {
            _logger.LogWarning("Received null or empty product parameter, cannot retrieve samples by product");
            return new PagedResult<Sample>(Array.Empty<Sample>(), 0);
        }

        var search = new SearchSamples
        {
            Filter = new SearchSamplesFilter
            {
                ProductId = new[] { product }
            },
            Pagination = new Pagination
            {
                Index = pageIndex,
                Size = pageSize
            }
        };  

        return await this.SearchSamples(search);
    }

    [McpServerTool]
    [Description("Allows retrieving samples by author in the Community Samples Gallery.")]
    public async Task<PagedResult<Sample>> SamplesByAuthor(
        [Description("The author to use for filtering samples.")] string author,
        [Description("The page index for pagination.")] int pageIndex,
        [Description("The page size for pagination.")] int pageSize
    )
    {
        if (string.IsNullOrEmpty(author))
        {
            _logger.LogWarning("Received null or empty author parameter, cannot retrieve samples by author");
            return new PagedResult<Sample>(Array.Empty<Sample>(), 0);
        }

        var search = new SearchSamples
        {
            Filter = new SearchSamplesFilter
            {
                AuthorId = author 
            },
            Pagination = new Pagination
            {
                Index = pageIndex,
                Size = pageSize
            }
        };  

        return await this.SearchSamples(search);
    }

    [McpServerTool]
    [Description("Allows retrieving samples by keyword in the Community Samples Gallery.")]
    public async Task<PagedResult<Sample>> SamplesByKeyword(
        [Description("The keyword to use for filtering samples.")] string keyword,
        [Description("The page index for pagination.")] int pageIndex,
        [Description("The page size for pagination.")] int pageSize
    )
    {
        if (string.IsNullOrEmpty(keyword))
        {
            _logger.LogWarning("Received null or empty keyword parameter, cannot retrieve samples by keyword");
            return new PagedResult<Sample>(Array.Empty<Sample>(), 0);
        }

        var search = new SearchSamples
        {
            Filter = new SearchSamplesFilter
            {
                Search = keyword
            },
            Pagination = new Pagination
            {
                Index = pageIndex,
                Size = pageSize
            }
        };  

        return await this.SearchSamples(search);
    }

    [McpServerTool]
    [Description("Allows searching for samples in the Community Samples Gallery.")]
    public async Task<PagedResult<Sample>> SearchSamples(
        [Description("Search parameters, including filtering, sorting, and pagination.")] SearchSamples search
    )
    {
        try
        {
            if (search == null)
            {
                _logger.LogWarning("Received null search parameters, using default values");
                search = new SearchSamples();
            }

            _logger.LogDebug("Search parameter details - Filter: {@Filter}, Sort: {@Sort}, Pagination: {@Pagination}",
                search.Filter, search.Sort, search.Pagination);

            if (string.IsNullOrEmpty(_configuration.BaseUrl))
            {
                throw new InvalidOperationException("Samples API base URL is not configured.");
            }

            _logger.LogInformation("Searching for samples with parameters: {@SearchParameters}", search);

            var requestUrl = _configuration.BaseUrl.TrimEnd('/');
            _logger.LogDebug("Making POST request to: {RequestUrl}", requestUrl);

            // Clean up the search parameters to remove null/empty values
            var cleanedSearch = CleanSearchParameters(search);
            _logger.LogDebug("Cleaned search parameters: {@CleanedSearchParameters}", cleanedSearch);

            // Serialize the cleaned search parameters to JSON
            var jsonBody = JsonSerializer.Serialize(cleanedSearch, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            _logger.LogDebug("Request body: {RequestBody}", jsonBody);

            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            var response = await _httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Raw API response: {ResponseContent}", responseContent);

            PagedResult<Sample>? apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<PagedResult<Sample>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
                    }
                });
            }
            catch (JsonException jsonEx) when (jsonEx.Message.Contains("SampleThumbnailType"))
            {
                _logger.LogWarning("Enum deserialization failed, attempting with default converter: {Error}", jsonEx.Message);

                // Try again with a more lenient approach
                apiResponse = JsonSerializer.Deserialize<PagedResult<Sample>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (apiResponse == null)
            {
                _logger.LogWarning("Received null response from samples API");
                return new PagedResult<Sample>(Array.Empty<Sample>(), 0);
            }

            _logger.LogInformation("Successfully retrieved {Count} samples out of {Total} total",
                apiResponse.Items?.Count() ?? 0, apiResponse.Total);

            return apiResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while searching for samples");
            throw new InvalidOperationException("Failed to retrieve samples from the API", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while parsing samples response");
            throw new InvalidOperationException("Failed to parse samples response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching for samples");
            throw;
        }
    }

    /// <summary>
    /// Cleans the search parameters by removing null or empty values
    /// </summary>
    private SearchSamples CleanSearchParameters(SearchSamples search)
    {
        if (search == null)
            return new SearchSamples();

        // Fix pagination if index is 0 (upgrade to 1-based indexing)
        var pagination = search.Pagination;
        if (pagination != null && pagination.Index == 0)
        {
            pagination = new Pagination
            {
                Index = 1,
                Size = pagination.Size
            };
            _logger.LogDebug("Upgraded page index from 0 to 1 for 1-based pagination");
        }

        var cleanedSearch = new SearchSamples
        {
            Sort = search.Sort,
            Pagination = pagination
        };

        // Clean the filter if it exists
        if (search.Filter != null)
        {
            var filter = search.Filter;
            var cleanedFilter = new SearchSamplesFilter();

            // Only include non-null/non-empty string properties
            if (!string.IsNullOrWhiteSpace(filter.Search))
                cleanedFilter.Search = filter.Search;

            if (!string.IsNullOrWhiteSpace(filter.AuthorId))
                cleanedFilter.AuthorId = filter.AuthorId;

            if (!string.IsNullOrWhiteSpace(filter.CategoryId))
                cleanedFilter.CategoryId = filter.CategoryId;

            // Only include non-null/non-empty array properties
            if (filter.ProductId != null && filter.ProductId.Any(p => !string.IsNullOrWhiteSpace(p)))
                cleanedFilter.ProductId = filter.ProductId.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            // Include boolean properties (they always have a value)
            cleanedFilter.FeaturedOnly = filter.FeaturedOnly;

            // Only include metadata if it has valid entries
            if (filter.Metadata != null && filter.Metadata.Any(m => !string.IsNullOrWhiteSpace(m.Key) && !string.IsNullOrWhiteSpace(m.Value)))
            {
                cleanedFilter.Metadata = filter.Metadata
                    .Where(m => !string.IsNullOrWhiteSpace(m.Key) && !string.IsNullOrWhiteSpace(m.Value))
                    .ToList();
            }

            // Only set the filter if it has any meaningful content
            if (!string.IsNullOrWhiteSpace(cleanedFilter.Search) ||
                !string.IsNullOrWhiteSpace(cleanedFilter.AuthorId) ||
                !string.IsNullOrWhiteSpace(cleanedFilter.CategoryId) ||
                (cleanedFilter.ProductId?.Any() == true) ||
                cleanedFilter.FeaturedOnly ||
                (cleanedFilter.Metadata?.Any() == true))
            {
                cleanedSearch.Filter = cleanedFilter;
            }
        }

        return cleanedSearch;
    }
}
