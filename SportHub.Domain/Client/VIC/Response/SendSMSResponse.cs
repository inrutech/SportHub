
using System.Text.Json.Serialization;

namespace SportHub.Domain.Client.VIC.Response;

public partial class SendSMSResponse
{
    [JsonPropertyName("status")]
    public long Status { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    [JsonPropertyName("success")]
    public long Success { get; set; }

    [JsonPropertyName("fail")]
    public long Fail { get; set; }

    [JsonPropertyName("array")]
    public List<Array> Array { get; set; }
}

public partial class Array
{
    [JsonPropertyName("msgId")]
    public string MsgId { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("orderId")]
    public string OrderId { get; set; }
}