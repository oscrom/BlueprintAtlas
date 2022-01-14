using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models
{
  public class FeatureServiceDefinition
  {
    [JsonProperty("name"), JsonRequired]
    public string Name { get; set; }

    [JsonProperty("serviceDescription")]
    public string ServiceDescription { get; set; }
    
    [JsonProperty("maxRecordCount")]
    public double MaxRecordCount { get; set; }

    [JsonProperty("supportedQueryFormats")]
    public string SupportedQueryFormats { get; set; }

    [JsonProperty("capabilities")]
    public string Capabilities { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("allowGeometryUpdates")]
    public bool AllowGeometryUpdates { get; set; }

    [JsonProperty("spatialReference")]
    public RestSpatialReference SpatialReference { get; set; }

  }
}
