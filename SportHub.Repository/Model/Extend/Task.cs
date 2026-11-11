namespace SportHub.Repository.Model;

using System.ComponentModel.DataAnnotations;

public partial class Task
{
    [Timestamp] public uint _version { get; set; }
}
