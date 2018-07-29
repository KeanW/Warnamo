using System;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;

namespace WarningsViewExtension
{
  public class WarningsViewExtension : IViewExtension
  {
    private MenuItem menuItem;

    public void Dispose() {}

    public void Startup(ViewStartupParams p) {}

    public void Loaded(ViewLoadedParams p)
    {
      menuItem = new MenuItem { Header = "Browse Warnings..." };
      menuItem.Click += (sender, args) =>
      {
        var dynViewModel = p.DynamoWindow.DataContext as DynamoViewModel;
        var viewModel = new WarningsWindowViewModel(p, dynViewModel);
        var window = new WarningsWindow
        {
          // Set the data context for the main grid in the window

          MainGrid = { DataContext = viewModel },

          // Set the owner of the window to the Dynamo window

          Owner = p.DynamoWindow
        };

        window.Left = window.Owner.Left + 400;
        window.Top = window.Owner.Top + 200;

        // Show our modeless window

        window.Show();
      };
      p.AddMenuItem(MenuBarType.View, menuItem);
    }

    public void Shutdown() {}

    public string UniqueId
    {
      get { return Guid.NewGuid().ToString(); }
    }

    public string Name
    {
      get { return "Warnamo View Extension"; }
    }
  }
}
