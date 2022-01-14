using System;
using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class FeatureServiceCreateResponse
{
  [JsonProperty("error")]
  public Error Error { get; set; }

  [JsonProperty("encodedServiceURL")]
  public Uri EncodedServiceUri { get; set; }

  [JsonProperty("itemId")]
  public string ItemId { get; set; }

  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("serviceItemId")]
  public string ServiceItemId { get; set; }

  [JsonProperty("serviceurl")]
  public Uri ServiceUri { get; set; }

  [JsonProperty("size")]
  public int Size { get; set; }

  [JsonProperty("success")]
  public bool Success { get; set; }

  [JsonProperty("type")]
  public string Type { get; set; }

  [JsonProperty("description")]
  public string Description { get; set; }

  [JsonProperty("tags")]
  public string Tags { get; set; }

  [JsonProperty("snippet")]
  public string Snippet { get; set; }
}