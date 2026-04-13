namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Permission : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
