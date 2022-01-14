using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class RestLayer
{
  [JsonProperty("adminLayerInfo", NullValueHandling = NullValueHandling.Ignore)]
  public AdminLayerInfo AdminLayerInfo { get; set; }

  [JsonProperty("id")]
  public long Id { get; set; }

  [JsonProperty("name")]
  public string Name { get; set; }
  
  [JsonProperty("geometryType")]
  public string GeometryType { get; set; }

  [JsonProperty("extent")]
  public Extent Extent { get; set; }

  //[JsonProperty("drawingInfo")]
  //public DrawingInfo DrawingInfo { get; set; }

  [JsonProperty("allowGeometryUpdates")]
  public bool AllowGeometryUpdates { get; set; }

  [JsonProperty("objectIdField")]
  public string ObjectIdField { get; set; }

  [JsonProperty("fields")]
  public List<RestField> Fields { get; set; }

  [JsonProperty("supportedQueryFormats")] //Added at 10.1
  public string SupportedQueryFormats { get; set; }

  [JsonProperty("capabilities")]
  public string Capabilities { get; set; }

  [JsonProperty("hasM")]
  public bool HasM { get; set; }

  [JsonProperty("hasZ")]
  public bool HasZ { get; set; }

  [JsonProperty("hasAttachments")]
  public bool HasAttachments { get; set; }

  [JsonProperty("allowUpdateWithoutMValues")]
  public bool AllowUpdateWithoutMValues { get; set; }

  [JsonProperty("allowTrueCurvesUpdates")]
  public bool AllowTrueCurvesUpdates { get; set; }

  [JsonProperty("onlyAllowTrueCurveUpdatesByTrueCurveClients")]
  public bool OnlyAllowTrueCurveUpdatesByTrueCurveClients { get; set; }

  [JsonProperty("enableZDefaults")]
  public bool EnableZDefaults { get; set; }

  [JsonProperty("zDefault")]
  public double ZDefault { get; set; }
}