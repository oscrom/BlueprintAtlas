using Newtonsoft.Json;

namespace BlueprintAtlas.Rest.Models;

public class RestField
{

  [JsonIgnore]
  public static RestField ObjectIdRestField => new()
  {
    Name = "ObjectID",
    Alias = "OBJECTID",
    Type = "esriFieldTypeOID",
    TypeName = "int4",
    ActualType = Models.ActualType.Int,
    SqlType = "sqlTypeInteger",
    SqlTypeCode = 4,
    Length = 4,
    Nullable = false,
    Editable = false,
  };

  [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
  public string Name { get; set; }

  [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
  public string Type { get; set; }

  [JsonProperty("typeName", NullValueHandling = NullValueHandling.Ignore)]
  public string TypeName { get; set; }

  [JsonProperty("actualType", NullValueHandling = NullValueHandling.Ignore)]
  public ActualType? ActualType { get; set; }

  [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
  public string Alias { get; set; }

  [JsonProperty("sqlType", NullValueHandling = NullValueHandling.Ignore)]
  public string SqlType { get; set; }

  [JsonProperty("sqlTypeCode", NullValueHandling = NullValueHandling.Ignore)]
  public int? SqlTypeCode { get; set; }

  [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
  public int Length { get; set; }

  [JsonProperty("nullable", NullValueHandling = NullValueHandling.Ignore)]
  public bool Nullable { get; set; }

  [JsonProperty("editable", NullValueHandling = NullValueHandling.Ignore)]
  public bool Editable { get; set; }
}