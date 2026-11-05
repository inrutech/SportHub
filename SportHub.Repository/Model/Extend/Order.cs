namespace SportHub.Repository.Model;

using System.ComponentModel.DataAnnotations;

public partial class Order
{
    [Timestamp] public uint _version { get; set; }
}