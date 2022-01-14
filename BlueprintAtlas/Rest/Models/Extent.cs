using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class Extent
{
  [JsonProperty("xmin")]
  public double XMin { get; set; }

  [JsonProperty("ymin")]
  public double YMin { get; set; }

  [JsonProperty("xmax")]
  public double XMax { get; set; }

  [JsonProperty("ymax")]
  public double YMax { get; set; }

  [JsonProperty("spatialReference")]
  public RestSpatialReference RestSpatialReference { get; set; }
}