using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using BlueprintAtlas.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using netDxf;


namespace BlueprintAtlas
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainViewModel(DialogCoordinator.Instance);

      Loaded += (s, e) => ViewModel.MapView = this.MapView;
    }

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    private void ProjectsDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var documents = e.AddedItems.OfType<DxfDocument>().ToList();
      if (documents.Any())
      {
        ViewModel.PreviewInMapCommand.Execute(new List<DxfDocument>(documents));
        return;
      }

      var projects = e.AddedItems.OfType<ProjectViewModel>().ToList();
      if (projects.Any())
        ViewModel.PreviewInMapCommand.Execute(new List<DxfDocument>(projects.SelectMany(f => f.DxfFiles)));

    }
  }
}