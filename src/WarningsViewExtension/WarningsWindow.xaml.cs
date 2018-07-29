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
  }
}
