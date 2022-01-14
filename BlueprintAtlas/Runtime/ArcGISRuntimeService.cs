using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Symbology;
using netDxf.Entities;

namespace BlueprintAtlas.Runtime
{
  /// <summary>
  /// A class used to interface between DXF documents and the ArcGIS .NET Runtime.
  /// </summary>
  public static class ArcGISRuntimeService
  {
    /// <summary>
    /// Adds the lines to a feature service. 
    /// <see cref="https://developers.arcgis.com/net/wpf/sample-code/feature-collection-layer/"/>
    /// </summary>
    /// <param name="uri">The Uri of the feature service.</param>
    /// <param name="lines">The lines to add.</param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static async Task AddLineFeaturesAsync(Uri uri, IEnumerable<Line> lines, Dictionary<string, object?> attributes = null)
    {
      // todo: implement the adding of attributes.

      var table = new ServiceFeatureTable(uri);

      // Create a feature layer to visualize the features in the table.
      var layer = new FeatureLayer(table);
      await layer.LoadAsync();

      var features = new List<Feature>();

      foreach (var l in lines)
      {
        var feature = (ArcGISFeature)table.CreateFeature();
        if (attributes != null)
        {
          feature.SetAttributeValue("Description", attributes["Description"]);
          feature.SetAttributeValue("StartDate", attributes["StartDate"]);
          feature.SetAttributeValue("Status", attributes["Status"]);
        }
        feature.Geometry = new Esri.ArcGISRuntime.Geometry.Polyline(new List<Segment>
        {
          new LineSegment(
            new MapPoint(l.StartPoint.X, l.StartPoint.Y, l.StartPoint.Z),
            new MapPoint(l.EndPoint.X, l.EndPoint.Y, l.EndPoint.Z)),
        });
        features.Add(feature);
      }

      await table.AddFeaturesAsync(features);

      // Apply the edits to the service.
      await table.ApplyEditsAsync();
    }

    /// <summary>
    /// Opens a list of PortalItems as a web map in whichever application the system is configured to open urls.
    /// </summary>
    /// <param name="items">The list of portal items to open in the webmap.</param>
    public static void OpenAsWebMap(List<PortalItem> items)
    {
      var strBuilder = new StringBuilder();
      strBuilder.AppendJoin(',', items.Select(f => f.ItemId));

      var webMapUrl = $"https://arcgis.com/home/webmap/viewer.html?layers={strBuilder}";
      var ps = new ProcessStartInfo(webMapUrl)
      {
        UseShellExecute = true,
      };
      Process.Start(ps);
    }

    /// <summary>
    /// Creates a renderer to use when previewing on a map.
    /// <see cref="https://developers.arcgis.com/net/wpf/sample-code/feature-collection-layer/"/>
    /// </summary>
    /// <param name="geomType">The type of geometry to create a renderer for.</param>
    /// <returns></returns>
    internal static Renderer CreateRenderer(GeometryType geomType)
    {
      // Return a simple renderer to match the geometry type provided
      Symbol symbol = geomType switch
      {
        GeometryType.Point =>
          // Create a marker symbol
          new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Triangle, Color.Red, 18),
        GeometryType.Multipoint =>
          // Create a marker symbol
          new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Triangle, Color.Red, 18),
        GeometryType.Polyline =>
          // Create a line symbol
          new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 2),
        _ => null
      };

      // Return a new renderer that uses the symbol created above
      return new SimpleRenderer(symbol);
    }
  }
}
