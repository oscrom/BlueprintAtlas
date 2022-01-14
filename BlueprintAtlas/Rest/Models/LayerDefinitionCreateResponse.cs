using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class LayerDefinitionCreateResponse
{
  [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
  public bool? Success { get; set; }

  [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
  public Error Error { get; set; }

  [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
  public long? Code { get; set; }

  [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
  public string Message { get; set; }

  [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
  public List<string> Details { get; set; }

  [JsonProperty("layers", NullValueHandling = NullValueHandling.Ignore)]
  public List<RestLayer> Layers { get; set; }
}