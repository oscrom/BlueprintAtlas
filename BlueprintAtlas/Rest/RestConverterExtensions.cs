using System.Collections.Generic;
using System.Linq;
using BlueprintAtlas.Rest.Models;
using BlueprintAtlas.ViewModel;
using Esri.ArcGISRuntime.Geometry;

namespace BlueprintAtlas.Rest
{
  public static class RestConverterExtensions
  {
    /// <summary>
    /// Creates a basic FeatureServiceDefinition in order to serialize and POST to create a FeatureService.
    /// </summary>
    /// <returns>The packaged FeatureServiceDefinition.</returns>
    public static FeatureServiceDefinition CreateServiceDefinition(this ProjectViewModel proj) =>
      new()
      {
        Name = proj.ServiceName,
        Description = proj.Description,
        ServiceDescription = proj.Description,
        AllowGeometryUpdates = true,
        Capabilities = "Create,Update,Delete,Query,Editing",
        MaxRecordCount = 1000,
        SupportedQueryFormats = "HTML,JSON"
      };

    private static readonly Extent WGS1984_Extent = new()
    {
      RestSpatialReference = new RestSpatialReference
      {
        Wkt = Constants.WGS_1984
      },
    };


    /// <summary>
    /// Creates a LayerDefinition with layers out of each DXF file in order to serialize and POST to a FeatureService.
    /// </summary>
    /// <param name="projectViewModel">The project containing the DX Files.</param>
    /// <param name="e">The envelope to set the extent of each layer.</param>
    /// <returns>The packaged LayerDefinition.</returns>
    public static LayerDefinition CreateLayerDefinitions(this ProjectViewModel projectViewModel, Envelope e)
    {
      var ex = WGS1984_Extent;
      if (e != null)
      {
        ex.YMin = e.YMin;
        ex.YMax = e.YMax;
        ex.XMin = e.XMin;
        ex.XMax = e.XMax;
      }

      var adminLayerInfo = new AdminLayerInfo
      {
        TableExtent = ex
      };

      var fields = new List<RestField>
      {
        RestField.ObjectIdRestField,
        new()
        {
          Name = "Description",
          Type = "esriFieldTypeString",
          ActualType = ActualType.Nvarchar,
          Editable = true,
          Length = 256,
          Nullable = true,
        },
        new()
        {
          Name="StartDate",
          Type = "esriFieldTypeDate",
          Editable = true,
          Nullable = true
        },
        new()
        {
          Name = "Status",
          Type = "esriFieldTypeString",
          ActualType = ActualType.Nvarchar,
          Editable = true,
          Length = 256,
          Nullable = true,
        },
      };
      // Create individual layers for each dxf file
      var layers = projectViewModel.DxfFiles.Select(dxf => new RestLayer
      {
        Name = dxf.Name,
        ObjectIdField = RestField.ObjectIdRestField.Name,
        Capabilities = "Create,Update,Delete,Query,Editing",
        SupportedQueryFormats = "JSON",
        GeometryType = "esriGeometryPolyline",
        HasZ = true,
        HasM = false,
        AllowUpdateWithoutMValues = true,
        AllowTrueCurvesUpdates = true,
        OnlyAllowTrueCurveUpdatesByTrueCurveClients = false,
        EnableZDefaults = true,
        ZDefault = 0,
        AdminLayerInfo = adminLayerInfo,
        Extent = ex,
        Fields = fields
      }).ToList();

      // Create the Public Notes layer
      layers.Add(new RestLayer
      {
        Name = "Public Notes",
        ObjectIdField = RestField.ObjectIdRestField.Name,
        Capabilities = "Create,Update,Delete,Query,Editing",
        SupportedQueryFormats = "JSON",
        GeometryType = "esriGeometryPoint",
        HasZ = true,
        HasM = false,
        EnableZDefaults = true,
        ZDefault = 0,
        AllowUpdateWithoutMValues = true,
        AllowTrueCurvesUpdates = true,
        OnlyAllowTrueCurveUpdatesByTrueCurveClients = false,
        HasAttachments = true,
        AdminLayerInfo = adminLayerInfo,
        Extent = ex,
        Fields = new List<RestField> {
          RestField.ObjectIdRestField,
          new()
          {
            Name = "Note",
            Type = "esriFieldTypeString",
            ActualType = ActualType.Nvarchar,
            Editable = true,
            Length = 256,
            Nullable = true,
          }
        }
      });

      return new LayerDefinition
      {
        Layers = layers
      };
    }
  }
}
