namespace SportHub.Repository.Model;

using System.ComponentModel.DataAnnotations;

public partial class OrderLeg
{
    [Timestamp] public uint _version { get; set; }
}