namespace SportHub.Repository.Model;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Zero.Core.Extensions;

public partial class Match
{
    [Timestamp] public uint _version { get; set; }

}
