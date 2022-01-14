using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using netDxf;

namespace BlueprintAtlas.ViewModel;

/// <summary>
/// A class that contains a collection of DXF documents that will eventually be used to be published as a feature service.
/// </summary>
public class ProjectViewModel : ViewModelBase, IDropTarget
{

  private string _serviceName;
  private string _description;
  private string _status;
  private DateTime _startDate;


  /// <summary>
  /// The list of possible project statuses.
  /// </summary>
  // todo: convert to enum
  public static string[] ProjectStatuses { get; } =
  {
    "Not Started",
    "In Progress",
    "Delayed",
    "Completed"
  };


  public ProjectViewModel(string projectName = "New Project", string description = "Enter a description", string projectStatus = "Not Started", DateTime startDate = default)
  {
    ServiceName = projectName;
    Description = description;
    Status = projectStatus;
    if (startDate == default)
      startDate = DateTime.Now;
    StartDate = startDate;
    AddDxfDocumentsCommand = new RelayCommand(_ => true, obj => AddDxfDocuments(obj as string[]));
  }

  /// <summary>
  /// Gets or sets the name of the service. AKA Project Name
  /// </summary>
  public string ServiceName
  {
    get => _serviceName;
    set => SetProperty(ref _serviceName, value);
  }

  /// <summary>
  /// The description of the project.
  /// </summary>
  public string Description
  {
    get => _description;
    set => SetProperty(ref _description, value);
  }

  /// <summary>
  /// The Status of the project.
  /// </summary>
  public string Status
  {
    get => _status;
    set => SetProperty(ref _status, value);
  }

  /// <summary>
  /// The start date of the project.
  /// </summary>
  public DateTime StartDate
  {
    get => _startDate;
    set => SetProperty(ref _startDate, value);
  }

  /// <summary>
  /// Gets the list of this project's DxfDocuments.
  /// </summary>
  public ObservableCollection<DxfDocument> DxfFiles { get; } = new();


  #region Commands
  /// <summary>
  /// Adds a DXF document to this project.
  /// </summary>
  public RelayCommand AddDxfDocumentsCommand { get; }

  #endregion

  #region  Command Implementations


  /// <summary>
  /// Attempts to add DXF Documents to the project by loading dxf files.
  /// </summary>
  /// <param name="files">The list of DXF file paths to attempt to load and add to the project.</param>
  private void AddDxfDocuments(IEnumerable<string> files)
  {
    //todo: expand validation
    try
    {
      foreach (var file in files)
      {
        var fileInfo = new FileInfo(file);
        if (fileInfo.Extension.ToLowerInvariant() != ".dxf") return;
        var dxfDoc = DxfDocument.Load(file);

        if (dxfDoc == null) return;
        DxfFiles.Add(dxfDoc);
      }
    }
    catch (Exception ex)
    {
      // todo: implement error logging
    }
  }
  #endregion


  #region IDropTarget Implementation

  public void DragOver(IDropInfo dropInfo)
  {

    var dxfDoc = dropInfo.Data as DxfDocument;
    var files = (string[])(dropInfo.Data as IDataObject)?.GetData(DataFormats.FileDrop);

    if ((dxfDoc != null || files != null))
    {
      dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
      dropInfo.Effects = DragDropEffects.Move;
    }
  }

  public void Drop(IDropInfo dropInfo)
  {
    if (dropInfo.Data is DxfDocument dxfDoc)
    {
      DxfFiles.Add(dxfDoc);
      var src = dropInfo.DragInfo.SourceCollection as ObservableCollection<DxfDocument>;
      src?.Remove(dxfDoc);

    }
    var files = (string[])(dropInfo.Data as IDataObject)?.GetData(DataFormats.FileDrop);
    if (files != null)
      AddDxfDocuments(files);
  }

  public void DragEnter(IDropInfo dropInfo) { }
  public void DragLeave(IDropInfo dropInfo) { }

  #endregion

}