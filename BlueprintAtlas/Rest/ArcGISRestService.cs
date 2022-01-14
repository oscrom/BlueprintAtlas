using BlueprintAtlas.Rest.Models;
using Esri.ArcGISRuntime.Http;
using Esri.ArcGISRuntime.Portal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlueprintAtlas.Authentication;

namespace BlueprintAtlas.Rest
{
  /// <summary>
  /// A class used to interface with ArcGIS REST endpoints.
  /// </summary>
  public class ArcGISRestService
  {
    private static ArcGISRestService _instance;
    private readonly ArcGISPortal _portal;

    private static string CreateUrl(string uri) => string.Concat(uri, "/createService");
    private static string AddToDefinitionUrl(string uri) => string.Concat(uri, "/addToDefinition");
    private static string AdminServicesUri(string uri) => uri.Replace("rest/services", "rest/admin/services");
    private string UsersUri => $"https://www.arcgis.com/sharing/rest/content/users/{_portal.User!.UserName}";
    private ArcGISRestService(ArcGISPortal portal)
    {
      _portal = portal ?? throw new ArgumentNullException(nameof(portal));
      if (portal.User == null) throw new NullReferenceException(nameof(portal.User));
    }

    /// <summary>
    /// Gets the singleton instance of this class; will prompt the user to login.
    /// </summary>
    /// <returns>The Singleton instance.</returns>
    public static async Task<ArcGISRestService> GetInstanceAsync()
    {
      // todo: validate credentials; this method does not yet handle oAuth cancellations.
      if (_instance != null) return _instance;
      var portal = await ArcGISPortal.CreateAsync(new Uri(ArcGISLoginPrompt.ArcGISOnlineUrl), true);
      _instance = new ArcGISRestService(portal);
      return _instance;
    }

    #region Publish Methods

    /// <summary>
    /// Attempts to create a Hosted Feature Service from the FeatureServiceDefinition.
    /// <see cref="https://developers.arcgis.com/rest/users-groups-and-items/create-service.htm"/>
    /// </summary>
    /// <param name="featureServiceDefinition">The feature service to attempt to create.</param>
    /// <returns>The response from the server, including whether creation was successful.</returns>
    public async Task<FeatureServiceCreateResponse> CreateServiceAsync(FeatureServiceDefinition featureServiceDefinition)
    {
      try
      {
        var payload = new Dictionary<string, string>
            {
              {"createParameters", JsonConvert.SerializeObject(featureServiceDefinition)},
              {"outputType", "featureService"},
              {"isView", "false"},
              {"f", "json"}
            };

        var response = await PostAsync(CreateUrl(UsersUri), PackagePayload(payload));
        return JsonConvert.DeserializeObject<FeatureServiceCreateResponse>(response);
      }
      catch (Exception ex)
      {
        return new FeatureServiceCreateResponse()
        {
          Success = false,
          Error = new Error
          {
            Description = ex.Message
          }
        };
      }
    }

    /// <summary>
    /// Attempts to add a LayerDefinition to a Hosted Feature Service.
    /// <see cref="https://developers.arcgis.com/rest/services-reference/online/add-to-definition-feature-service-.htm"/>
    /// </summary>
    /// <param name="featureServiceUri">The Uri of the Hosted Feature Service.</param>
    /// <param name="layerDefinition">The LayerDefinition to attempt to add to the service.</param>
    /// <returns>The response from the service, including the layers that were successfully added.</returns>
    public async Task<LayerDefinitionCreateResponse> AddToLayerDefinitionAsync(Uri featureServiceUri, LayerDefinition layerDefinition)
    {
      try
      {
        var payload = new Dictionary<string, string>
            {
              {"f","json"},
              {"addToDefinition", JsonConvert.SerializeObject(layerDefinition)}
            };

        var adminUrl = AdminServicesUri(featureServiceUri.AbsoluteUri);
        var url = AddToDefinitionUrl(adminUrl);

        var response = await PostAsync(url, PackagePayload(payload));
        return JsonConvert.DeserializeObject<LayerDefinitionCreateResponse>(response);

      }
      catch (Exception ex)
      {
        return new LayerDefinitionCreateResponse()
        {
          Success = false,
          Error = new Error
          {
            Description = ex.Message
          }
        };
      }
    }

    /// <summary>
    /// Creates a Portal Item from the specified item Id.
    /// </summary>
    /// <param name="itemId">The item Id to create a PortalItem around.</param>
    /// <returns>The PortalItem.</returns>
    public async Task<PortalItem> CreatePortalItemAsync(string itemId) =>
      await PortalItem.CreateAsync(_portal, itemId);

    #endregion
    #region Private Methods
    private async Task<string> PostAsync(string url, HttpContent payload)
    {
      if (_portal.Credential == null) throw new ArgumentNullException(nameof(_portal.Credential));

      using var arcGisHttpClient = new ArcGISHttpClientHandler { ArcGISCredential = _portal.Credential };
      using var httpClient = new HttpClient(arcGisHttpClient);
      try
      {
        var response = await httpClient.PostAsync(new Uri(url), payload);
        if (response.StatusCode != HttpStatusCode.OK)
          return string.Empty;

        var json = await response.Content.ReadAsStringAsync();

        payload.Dispose();
        response.Dispose();

        return json;
      }
      catch (Exception e)
      {
        throw;
      }
    }
    private static StringContent PackagePayload<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> payload)
    {
      var encodedItems = payload.Where(i => i.Value != null).Select(i => WebUtility.UrlEncode(i.Key.ToString()) + "=" + WebUtility.UrlEncode(i.Value.ToString()));
      var encodedContent = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");

      return encodedContent;
    }
    #endregion
  }
}

