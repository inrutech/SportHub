namespace SportHub.Repository.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Zero.Core.Extensions;

public partial class League
{
    [Timestamp] public uint _version { get; set; }

    [NotMapped]
    public Dictionary<long, string>? SourceArray
    {
        get => SourceArrayJson?.DeserializeJson<Dictionary<long, string>>();
        set => SourceArrayJson = value?.ToJson();
    }
}