using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class RestSpatialReference
{
  /// <summary>
  /// Well-Known Text (WKT)
  /// </summary>
  [JsonProperty("wkt", NullValueHandling = NullValueHandling.Ignore)]
  public string Wkt { get; set; }
}