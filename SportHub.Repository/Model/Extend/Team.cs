namespace SportHub.Repository.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Zero.Core.Extensions;

public partial class Team
{
    [Timestamp] public uint _version { get; set; }

    [NotMapped]
    public List<AliasInfo>? AliasArrayDetail
    {
        get => AliasArray?.DeserializeJson<List<AliasInfo>>();
        set => AliasArray = value?.ToJson();
    }
}