using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public sealed class Error
{
  [JsonProperty("code")]
  public int Code { get; set; }

  [JsonProperty("description")]
  public string Description { get; set; }
}