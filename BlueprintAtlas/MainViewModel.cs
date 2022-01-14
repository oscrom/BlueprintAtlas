using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BlueprintAtlas.Authentication;
using BlueprintAtlas.Rest;
using BlueprintAtlas.Rest.Models;
using BlueprintAtlas.Runtime;
using BlueprintAtlas.ViewModel;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using MahApps.Metro.Controls.Dialogs;
using netDxf;
using Exception = System.Exception;
using Polyline = Esri.ArcGISRuntime.Geometry.Polyline;
using Uri = System.Uri;

namespace BlueprintAtlas
{
  public class MainViewModel : ViewModelBase
  {
    private ProjectViewModel _selectedProject;
    private Envelope _lastExtent;
    private readonly IDialogCoordinator _dialogCoordinator;

    public MainViewModel(IDialogCoordinator dialogCoordinator)
    {
      _dialogCoordinator = dialogCoordinator;
      ArcGISLoginPrompt.SetChallengeHandler();

      SelectedProject = Projects.First();

      AddProjectCommand = new RelayCommand(_ => true, _ => Projects.Add(new ProjectViewModel()));
      PreviewInMapCommand = new RelayCommand(_ => true, async obj =>
      {
        var progress = await _dialogCoordinator.ShowProgressAsync(this, "Previewing", "Loading DXF Documents to preview in map...");
        progress.SetIndeterminate();
        await PreviewInMapAsync(GetPayload<DxfDocument>(obj));
        await progress.CloseAsync();
      });
      PublishAsWebMapCommand = new RelayCommand(_ => true, async obj =>
      {
        var progress = await _dialogCoordinator.ShowProgressAsync(this, "Publishing", "Publishing Projects as a Hosted Feature Layer. This might take a while..");
        progress.SetIndeterminate();
        try
        {
          var projects = GetPayload<ProjectViewModel>(obj);

          // first preview it in the map to seed the last Envelope extent
          await PreviewInMapAsync(GetPayload<DxfDocument>(projects.SelectMany(f => f.DxfFiles)));

          // then publish the projects' dxf files
          var result = await PublishAsFeatureServiceAsync(projects, _lastExtent);

          var published = result.Where(p => string.IsNullOrWhiteSpace(p.Error)).ToList();
          var error = result.Where(p => !string.IsNullOrWhiteSpace(p.Error));

          // open the successfully published dxf files as a web map
          if (published.Any())
            ArcGISRuntimeService.OpenAsWebMap(published.Select(p => p.PortalItem).ToList());

          // but notify of any potential erros
          foreach (var err in error)
          {
            await dialogCoordinator.ShowMessageAsync(this, "Error", err.Error);
          }

        }
        catch (Exception ex)
        {
          await dialogCoordinator.ShowMessageAsync(this, "Exception", ex.Message);
        }
        finally
        {
          await progress.CloseAsync();
        }
      });
    }

    #region Properties

    /// <summary>
    /// The current map.
    /// </summary>
    public Map Map { get; } = new(Basemap.CreateDarkGrayCanvasVector());

    /// <summary>
    /// The map's associated MapView.
    /// </summary>
    public MapView MapView { get; set; }

    /// <summary>
    /// The list of projects in the workspace.
    /// </summary>
    public ObservableCollection<ProjectViewModel> Projects { get; set; } = new() { new ProjectViewModel() };

    /// <summary>
    /// The selected project.
    /// </summary>
    public ProjectViewModel SelectedProject
    {
      get => _selectedProject;
      set => SetProperty(ref _selectedProject, value, nameof(SelectedProject));
    }

    #endregion
    #region Commands

    /// <summary>
    /// Adds the DXF files of a List of Projects to the Map Viewer.
    /// </summary>
    public RelayCommand PreviewInMapCommand { get; }

    /// <summary>
    /// Publishes the DXF files of a List of Projects as a web map.
    /// </summary>
    public RelayCommand PublishAsWebMapCommand { get; }

    /// <summary>
    /// Adds a new Project to the workspace.
    /// </summary>
    public RelayCommand AddProjectCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Creates a feature collection from the line geometry in dxf documents and adds them to the operational layers of the map.
    /// <see cref="https://developers.arcgis.com/net/wpf/sample-code/feature-collection-layer/"/>
    /// </summary>
    /// <param name="dxfDocs">The DXFDocuments source the lines to add to the map.</param>
    /// <returns></returns>
    private async Task PreviewInMapAsync(List<DxfDocument> dxfDocs)
    {
      if (dxfDocs == null || !dxfDocs.Any()) return;

      Map.OperationalLayers.Clear();
      var featuresCollection = new FeatureCollection();

      // todo: read the spatial reference from the dxf document's NOD
      var spRef = new SpatialReference(Constants.WGS_1984);

      foreach (var dxfDoc in dxfDocs)
      {
        var dxfLines = dxfDoc.Lines;

        // Create the schema for a lines table (one text field to contain a name attribute)
        var lineFields = new List<Field>();
        var boundaryField = new Field(FieldType.Text, "Boundary", "Boundary Name", 50);
        lineFields.Add(boundaryField);

        // Instantiate FeatureCollectionTables with schema and geometry type
        var linesTable = new FeatureCollectionTable(lineFields, GeometryType.Polyline, spRef)
        {
          // Set rendering for each table
          Renderer = ArcGISRuntimeService.CreateRenderer(GeometryType.Polyline)
        };

        foreach (var dxfLine in dxfLines)
        {
          // Create a new line feature, provide geometry and attribute values
          var lineFeature = linesTable.CreateFeature();
          lineFeature.SetAttributeValue(boundaryField, "AManAPlanACanalPanama");
          var point1 = new MapPoint(dxfLine.StartPoint.X, dxfLine.StartPoint.Y, spRef);
          var point2 = new MapPoint(dxfLine.EndPoint.X, dxfLine.EndPoint.Y, spRef);
          var line = new Polyline(new MapPoint[] { point1, point2 });
          lineFeature.Geometry = line;
          // Add the new features to the appropriate feature collection table
          await linesTable.AddFeatureAsync(lineFeature);
        }

        // Create a feature collection and add the feature collection tables
        featuresCollection.Tables.Add(linesTable);
      }

      try
      {
        // Create a FeatureCollectionLayer
        var collectionLayer = new FeatureCollectionLayer(featuresCollection);

        // When the layer loads, zoom the map centered on the feature collection
        await collectionLayer.LoadAsync();
        _lastExtent = collectionLayer.FullExtent;
        if (_lastExtent != null)
          await MapView.SetViewpointCenterAsync(_lastExtent.GetCenter(), 10000000);

        // Add the layer to the Map's Operational Layers collection
        Map.OperationalLayers.Add(collectionLayer);
      }
      catch (Exception e)
      {
        Map.OperationalLayers.Clear();
      }
    }

    /// <summary>
    /// Creates a Feature Service and associates it to a Portal Item from the geometry of the list of Projects' DXF files.
    /// </summary>
    /// <param name="projects">The Projects' DXF files to publish.</param>
    /// <param name="extent">The extent to use for the layers.</param>
    /// <returns>The result of published projects; some successful and others not.</returns>
    private static async Task<List<PublishedProject>> PublishAsFeatureServiceAsync(IEnumerable<ProjectViewModel> projects, Envelope extent)
    {
      var publishedProjects = new List<PublishedProject>();
      foreach (var project in projects)
      {
        var publishedProject = await PublishAsFeatureServiceAsync(project, extent);
        publishedProjects.Add(publishedProject);
      }

      return publishedProjects;
    }

    /// <summary>
    /// Creates a Feature Service and associates it to a Portal Item from the geometry of this project's DXF files.
    /// </summary>
    /// <param name="project">The Project's DXF files to publish.</param>
    /// <param name="envelope">The extent to use for the layers.</param>
    /// <returns>The result of attempting to publish the project. Could be unsuccessful.</returns>
    private static async Task<PublishedProject> PublishAsFeatureServiceAsync(ProjectViewModel project, Envelope envelope)
    {
      // First, get an instance of the REST service for authentication purposes
      var restService = await ArcGISRestService.GetInstanceAsync();
      if (restService == null) return new PublishedProject("ArcGISRestService was null.");

      // todo: before continuing, we should:
      // todo: verify the above AGOL credentials
      // todo: check if the feature service name is available (see: https://developers.arcgis.com/rest/users-groups-and-items/check-service-name.htm)

      // Create the Feature Service Definition for publishing
      var featureServiceDefinition = project.CreateServiceDefinition();
      if (featureServiceDefinition == null) return new PublishedProject($"Failed to create service definition.");

      // Attempt to create the service in AGOL. This could fail; cleanup (deleting the service) is not shown here.
      var featureServiceCreateResponse = await restService.CreateServiceAsync(featureServiceDefinition);
      if (featureServiceCreateResponse == null) return new PublishedProject($"Failed to create feature service.");
      if (featureServiceCreateResponse.Error != null) return new PublishedProject(featureServiceCreateResponse.Error.Description);

      // Create the layer definitions from the DXF files.
      var layerDefinition = project.CreateLayerDefinitions(envelope);
      if (layerDefinition == null) return new PublishedProject($"Failed to create layer definition."); ;

      // Attempt to add the layer definitions to the created service.
      var layerDefinitionCreateResponse = await restService.AddToLayerDefinitionAsync(featureServiceCreateResponse.ServiceUri, layerDefinition);
      if (layerDefinitionCreateResponse == null) return new PublishedProject($"Failed to add layer definition to feature service."); ;
      if (layerDefinitionCreateResponse.Layers?.Any() != true) return new PublishedProject($"No layers published."); ;

      // Create and associate a Portal Item with the created feature service.
      var portalItem = await restService.CreatePortalItemAsync(featureServiceCreateResponse.ItemId);
      if (portalItem == null) return new PublishedProject($"Failed to create portal item.");

      var serviceUri = featureServiceCreateResponse.ServiceUri;

      // Now hydrate the geometry of the published layers with the geometry in the DXF files
      for (var index = 0; index < layerDefinitionCreateResponse.Layers?.Count - 1; index++)
      {
        var lyr = layerDefinitionCreateResponse.Layers[index];
        var lyrUri = new Uri($"{serviceUri}/{lyr.Id}");

        var dxfFile = project.DxfFiles[index];
        var attributes = new Dictionary<string, object?>
        {
          { "Description", project.Description },
          { "StartDate", project.StartDate },
          { "Status", project.Status }
        };
        await ArcGISRuntimeService.AddLineFeaturesAsync(lyrUri, dxfFile.Lines, attributes);
      }

      var layerIds = layerDefinitionCreateResponse.Layers.Select(lyr => lyr.Id).ToList();
      return new PublishedProject(portalItem, serviceUri, layerIds);
    }
    #endregion
  }
}

