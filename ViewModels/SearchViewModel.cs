namespace TaskManageApp.ViewModels
{
    public sealed class SearchViewModel
    {
        public string Query { get; init; } = string.Empty;
        public IReadOnlyList<SearchResultViewModel> Results { get; init; } = Array.Empty<SearchResultViewModel>();
        public bool IncludesData { get; init; }
    }

    public sealed record SearchResultViewModel(
        string Title,
        string Description,
        string Type,
        string Url);
}
