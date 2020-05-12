using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarningsViewExtension
{
  /// <summary>
  /// Interaction logic for WarningsWindow.xaml
  /// </summary>
  public partial class WarningsWindow : Window
  {
    public WarningsWindow()
    {
      InitializeComponent();
    }


    private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        var nodeInfo = e.AddedItems[0] as NodeInfo;
        var viewModel = MainGrid.DataContext as WarningsWindowViewModel;
        if (nodeInfo != null && viewModel != null)
        {
          this.Focus();
          viewModel.ZoomToPosition(nodeInfo);
          this.Focus();          
        }
      }
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
      const string sep = ",";
      var viewModel = MainGrid.DataContext as WarningsWindowViewModel;
      var nodes = viewModel.WarningNodes;
      if (nodes.Count > 0)
      {
        var sfd = new SaveFileDialog();
        sfd.Filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";
        var res = sfd.ShowDialog();
        if (res.HasValue && res.Value)
        {
          var filename = sfd.FileName;
          var writer = new StreamWriter(filename);
          writer.WriteLine("Index" + sep + "Name" + sep + "Guid");
          foreach (var node in nodes)
          {
            writer.WriteLine(node.ID + sep + node.Name + sep + node.GUID);
          }
          writer.Close();
        }
      }
    }
  }
}
