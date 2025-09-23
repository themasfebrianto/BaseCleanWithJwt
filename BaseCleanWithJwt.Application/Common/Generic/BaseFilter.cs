using System.Text.Json.Serialization;

namespace BaseCleanWithJwt.Domain.Common.Generic;

public class FilterBase
{
    public long? From { get; set; }
    public long? To { get; set; }

    private int _pageIndex = 1;
    private int _pageSize = 10;

    public int PageIndex
    {
        get => _pageIndex;
        set
        {
            IsPageIndexOrSizeSet = true;
            _pageIndex = Math.Max(1, value);
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            IsPageIndexOrSizeSet = true;
            _pageSize = Math.Max(1, value);
        }
    }

    public string? Sort { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool IsPageIndexOrSizeSet { get; set; }
}
