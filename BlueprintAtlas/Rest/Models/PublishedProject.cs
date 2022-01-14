using System;
using System.Collections.Generic;
using Esri.ArcGISRuntime.Portal;

namespace BlueprintAtlas.Rest.Models
{
  internal struct PublishedProject
  {
    public PortalItem PortalItem { get; }
    public Uri FeatureServiceUri { get; }

    public List<long> LayerIds { get; }
    public string Error { get; }

    public PublishedProject(PortalItem portalItem, Uri featureServiceUri, List<long> layerIds)
    {
      PortalItem = portalItem;
      FeatureServiceUri = featureServiceUri;
      LayerIds = layerIds;
      Error = string.Empty;
    }

    public PublishedProject(string error) : this(null, null, null)
    {
      Error = error;
    }
  }
}
