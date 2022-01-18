
<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/oscrom/BlueprintAtlas/">
    <img src="https://github.com/oscrom/BlueprintAtlas/blob/main/BlueprintAtlas/Assets/logo.png" alt="Logo" >
  </a>

</div>


<!-- ABOUT THE PROJECT -->
## Inspiration

We are a team who has spent much of our time at Esri focusing on the integration of GIS and CAD. We wanted to use this experience and expertise to help close some of the gaps we see in community improvement projects and citizen engagement. Many organizations work on projects that help communities (water networks, sewers, telecommunications, housing and more) but they often lack an easy way to get the information stored in proprietary design files to the large audience of the community these projects will affect. We feel that the more information we can provide to a community the more we can help improve that community. 

## What it does

Blueprint Atlas, a melding of both CAD and GIS terms, is a simple desktop application that lets you share your CAD designs to a larger audience with relative ease. The tool requires the CAD user to drag and drop design files (of the DXF interchange format) into the program where it is them displayed on the map. The CAD designer can then use the tools within Blueprint Atlas to give some project details such as name, start date and status. The data within the design file is converted to GIS features where is then published to ArcGIS Online as a feature service. We believe that ArcGIS Online provides some of the best tools in the industry to present a community with well organized maps that can also capture requirements and information from that same community. 

![Product Screenshot](https://i.imgur.com/c3y8lHF.png)


### Technology Used

* [.NET 6](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
* [ArcGIS Runtime API for .NET](https://developers.arcgis.com/net/)
* [ArcGIS REST APIs](https://developers.arcgis.com/rest/)
* [netDxf](https://github.com/haplokuon/netDxf)
* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro/)
* [GongSolutions.WPF.DragDrop](https://github.com/punker76/gong-wpf-dragdrop)
* [Newtonsoft.Json](https://www.newtonsoft.com/)

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

* Visual Studio 2022 (v17.0)
* Windows 10, version 2004.

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/oscrom/BlueprintAtlas.git
   ```
2. Restore Nuget Packages
   ```sh
   nuget restore
   ```
3. Rebuild All

**Note: drag/drop while debugging via Visual Studio does not work. Use Release**

## What's next for Blueprint Atlas

- Expanding support for different geometry types
- Add the ability to read coordinate systems embedded into the DXFs by tool such as [ArcGIS for AutoCAD](https://www.esri.com/en-us/arcgis/products/arcgis-for-autocad)
- Expanding support for custom fields
- Adding support for additional file types
- Adding support for the  [Mapping Specification for DWG/CAD](http://webhelp.esri.com/arcgisdesktop/9.3/pdf/Mapping_Specification_for_DWG.pdf)
