using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class AdminLayerInfo
{
  // NOTE: It's crucial that the spatial reference in this extent is specified, or WKT will NOT work.
  [JsonProperty("tableExtent", NullValueHandling = NullValueHandling.Ignore)]
  public Extent TableExtent { get; set; }
}