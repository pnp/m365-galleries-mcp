using System.Text.Json;

namespace SampleGalleriesMCPServer;

public enum Sorting
{
    None,
    Ascending,
    Descending
}

/// <summary>
/// Defines the page and size to use when getting items
/// </summary>
public class Pagination
{
    /// <summary>
    /// The size of the data page for paging
    /// </summary>
    public int Size { get; set; } = 20;

    /// <summary>
    /// The number of page for paging, starting from 1
    /// </summary>
    public int Index { get; set; } = 1;
}

/// <summary>
/// Defines how the search for samples
/// </summary>
public class SearchSamples
{
    /// <summary>
    /// The optional sorting to apply
    /// </summary>
    public SearchSamplesSort Sort { get; set; } = null;

    /// <summary>
    /// The optional filter to apply
    /// </summary>
    public SearchSamplesFilter Filter { get; set; } = null;

    /// <summary>
    /// The optional pagination to apply
    /// </summary>
    public Pagination Pagination { get; set; } = null;
}

/// <summary>
/// Defines how samples should be sorted
/// </summary>
public class SearchSamplesSort
{
    /// <summary>
    /// The field to use for sorting
    /// </summary>
    public SearchSamplesSortField Field { get; set; }

    /// <summary>
    /// Gets or sets if sorting must be in descending order
    /// </summary>
    public bool Descending { get; set; }

    internal Sorting GetTitleSorting()
    {
        return (Field, Descending) switch
        {
            (SearchSamplesSortField.Title, true) => Sorting.Descending,
            (SearchSamplesSortField.Title, false) => Sorting.Ascending,
            _ => Sorting.None
        };
    }

    internal Sorting GetCreationDateTimeSorting()
    {
        return (Field, Descending) switch
        {
            (SearchSamplesSortField.CreationDateTime, true) => Sorting.Descending,
            (SearchSamplesSortField.CreationDateTime, false) => Sorting.Ascending,
            _ => Sorting.None
        };
    }

    internal Sorting GetUpdateDateTimeSorting()
    {
        return (Field, Descending) switch
        {
            (SearchSamplesSortField.UpdateDateTime, true) => Sorting.Descending,
            (SearchSamplesSortField.UpdateDateTime, false) => Sorting.Ascending,
            _ => Sorting.None
        };
    }
}

public enum SearchSamplesSortField
{
    Title,
    CreationDateTime,
    UpdateDateTime
}

/// <summary>
/// Defines the fields
/// </summary>
public class SearchSamplesFilter
{
    private List<SearchSamplesMetadataFilter> _metadata = new List<SearchSamplesMetadataFilter>();

    /// <summary>
    /// The text to search for. Default empty, i.e. retrieve all the samples
    /// </summary>
    public string Search { get; set; }

    /// <summary>
    /// The ID of the product to filter data on
    /// </summary>
    public string[] ProductId { get; set; }

    /// <summary>
    /// The ID of the author to filter data on
    /// </summary>
    public string AuthorId { get; set; }

    /// <summary>
    /// The ID of the category to filter data on
    /// </summary>
    public string CategoryId { get; set; }

    /// <summary>
    /// Declares to retrieve featured items only
    /// </summary>
    public bool FeaturedOnly { get; set; } = false;

    /// <summary>
    /// Additional filters to apply to the metadata field
    /// </summary>
    public List<SearchSamplesMetadataFilter> Metadata
    {
        get => _metadata ??= new List<SearchSamplesMetadataFilter>();
        set => _metadata = value;
    }
}

public class SearchSamplesMetadataFilter
{
    /// <summary>
    /// The key where to search in (ex. CLIENT-SIDE-DEV)
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The value to search
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// Object containing the items and the total number of the items, despite the pagination
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class PagedResult<TModel>
{
    public PagedResult(IEnumerable<TModel> items, int total)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        Total = total;
    }

    /// <summary>
    /// Gets the total number of items
    /// </summary>
    public int Total { get; }

    /// <summary>
    /// Gets the items
    /// </summary>
    public IEnumerable<TModel> Items { get; }
}

/// <summary>
/// Defines a Sample of the Samples Gallery
/// </summary>
public class Sample
{
    /// <summary>
    /// The ID of the Sample
    /// </summary>
    public string SampleId { get; set; }

    /// <summary>
    /// The Name of the Sample
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The Version of the Sample
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// The Source of the Sample
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// The Title of the Sample
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The Short Description of the Sample
    /// </summary>
    public string ShortDescription { get; set; }

    /// <summary>
    /// The Download URL of the Sample (should be absolute)
    /// </summary>
    public string DownloadUrl { get; set; }

    /// <summary>
    /// The Products leveraged by the Sample
    /// </summary>
    public IEnumerable<string> Products { get; set; }

    /// <summary>
    /// The Categories associated with the Sample
    /// </summary>
    public IEnumerable<string> Categories { get; set; }

    /// <summary>
    /// The Preview settings associated with the Sample
    /// </summary>
    public SamplePreview Preview { get; set; }

    /// <summary>
    /// The Metadata of the Sample
    /// </summary>
    public IEnumerable<SampleMetadata> Metadata { get; set; }

    /// <summary>
    /// The Thumbnails of the Sample
    /// </summary>
    public IEnumerable<SampleThumbnail> Thumbnails { get; set; }

    /// <summary>
    /// The Authors of the Sample
    /// </summary>
    public IEnumerable<LightweightAuthor> Authors { get; set; }

    /// <summary>
    /// The Reference of the Sample
    /// </summary>
    public IEnumerable<SampleReference> References { get; set; }

    /// <summary>
    /// The Url of the Sample (should be absolute)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The creation date time of the sample
    /// </summary>
    public DateTimeOffset CreationDateTime { get; set; }

    /// <summary>
    /// The update date time of the sample
    /// </summary>
    public DateTimeOffset UpdateDateTime { get; set; }

    /// <summary>
    /// URL of the tracking image for the current sample
    /// </summary>
    public string TrackingImage { get; set; }

    /// <summary>
    /// Defines whether the item is featured or not
    /// </summary>
    public bool Featured { get; set; }
}

/// <summary>
/// Defines the Metadata of a Sample
/// </summary>
public struct SampleMetadata
{
    /// <summary>
    /// The Key of the Metadata
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The Value of the Metadata
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// Defines a Thumbnail of a Sample
/// </summary>
public struct SampleThumbnail
{
    /// <summary>
    /// The type of Thumbnail
    /// </summary>
    public SampleThumbnailType Type { get; set; }

    /// <summary>
    /// The Sort Order of the Thumbnail
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// The Url of the Thumbnail (can be relative or absolute)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The Alternate Text of the Thumbnail
    /// </summary>
    public string Alt { get; set; }

    public IEnumerable<SampleThumbnailSlide> Slides { get; set; }
}

/// <summary>
/// Defines the flavors of Thumbnails available
/// </summary>
public enum SampleThumbnailType
{
    /// <summary>
    /// Image Thumbnail
    /// </summary>
    Image,
    /// <summary>
    /// Video Thumbnail
    /// </summary>
    Video,
    /// <summary>
    /// Slideshow
    /// </summary>
    Slideshow,
}

/// <summary>
/// Defines a slide of a Sample's slideshow
/// </summary>
public struct SampleThumbnailSlide
{
    /// <summary>
    /// The Sort Order of the Slide
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The Url of the Slide (can be relative or absolute)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The Alternate Text of the Slide
    /// </summary>
    public string Alt { get; set; }
}

/// <summary>
/// Defines the Author of a Sample
/// </summary>
public struct LightweightAuthor
{
    /// <summary>
    /// The GitHub Account of the Author
    /// </summary>
    public string GitHubAccount { get; set; }

    /// <summary>
    /// The Display Name of the Author
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// The Company of the Author
    /// </summary>
    public string Company { get; set; }

    /// <summary>
    /// The PictureUrl of the Author
    /// </summary>
    public Uri PictureUrl { get; set; }
}

/// <summary>
/// Defines a Reference for a Sample
/// </summary>
public struct SampleReference
{
    /// <summary>
    /// The name of the reference
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The URL of the reference
    /// </summary>
    public Uri Url { get; set; }

    /// <summary>
    /// The description of the reference
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// Defines a Preview for a Sample
/// </summary>
public class SamplePreview
{
    /// <summary>
    /// The label of the Preview for the Sample
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The URL of the Preview for the Sample
    /// </summary>
    public Uri Url { get; set; }

    /// <summary>
    /// The settings of the Preview for the Sample
    /// </summary>
    public JsonElement Settings { get; set; }
}
