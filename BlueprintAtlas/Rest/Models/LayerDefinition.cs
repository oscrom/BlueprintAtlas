using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models
{
  public class LayerDefinition
  {
    [JsonProperty("layers")]
    public List<RestLayer> Layers { get; set; }
  }
}
