using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace WarningsViewExtension
{
  public class WarningsWindowViewModel : NotificationObject, IDisposable
  {
    private ObservableCollection<NodeInfo> _warningNodes;
    private ReadyParams _readyParams;
    private DynamoViewModel _dynamoViewModel;
    private NodeViewModel _displaying;

    public ObservableCollection<NodeInfo> WarningNodes
    {
      get
      {
        _warningNodes = getWarningNodes();
        return _warningNodes;
      }
      set
      {
        _warningNodes = value;
      }
    }

    public ObservableCollection<NodeInfo> getWarningNodes()
    {
      if (_displaying != null)
      {
        HideTooltip(_displaying);
        _displaying = null;
      }

      // Collect error/warning nodes, sorting by X position
      // The second Select replaces the blank (0) ID with the row number

      var nodeList =
        (from n in _readyParams.CurrentWorkspaceModel.Nodes
        where n.State != ElementState.Active && n.State != ElementState.Dead
        orderby n.Rect.TopLeft.X ascending
        select new NodeInfo(0, n.Name, n.GUID)).Select(
          (item, index) => new NodeInfo(index + 1, item.Name, item.GUID)
        );

      // Return a bindable collection

      return new ObservableCollection<NodeInfo>(nodeList);
    }

    // Construction & disposal

    public WarningsWindowViewModel(ReadyParams p, DynamoViewModel dynamoVM)
    {
      _readyParams = p;
      _dynamoViewModel = dynamoVM;

      _readyParams.CurrentWorkspaceChanged +=
        ReadyParams_CurrentWorkspaceChanged;
      AddEventHandlers(_readyParams.CurrentWorkspaceModel);
    }

    public void Dispose()
    {
      _readyParams.CurrentWorkspaceChanged -=
        ReadyParams_CurrentWorkspaceChanged;
      RemoveEventHandlers(_readyParams.CurrentWorkspaceModel);
    }

    // Event handlers

    void ReadyParams_CurrentWorkspaceChanged(
      Dynamo.Graph.Workspaces.IWorkspaceModel obj
    )
    {
      if (_readyParams != null)
      {
        RemoveEventHandlers(_readyParams.CurrentWorkspaceModel);
      }
      AddEventHandlers(obj);
      RaisePropertyChanged("WarningNodes");
    }

    private void CurrentWorkspaceModel_NodeAdded(NodeModel node)
    {
      node.PropertyChanged += node_PropertyChanged;
    }

    private void CurrentWorkspaceModel_NodeRemoved(NodeModel node)
    {
      node.PropertyChanged -= node_PropertyChanged;
    }

    private void CurrentWorkspaceModel_NodesCleared()
    {
      foreach (var node in _readyParams.CurrentWorkspaceModel.Nodes)
      {
        node.PropertyChanged -= node_PropertyChanged;
      }
      RaisePropertyChanged("WarningNodes");
    }

    private void node_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "State")
      {
        RaisePropertyChanged("WarningNodes");
      }
    }

    // Attach and remove handlers

    private void AddEventHandlers(IWorkspaceModel model)
    {
      foreach (var node in model.Nodes)
      {
        node.PropertyChanged += node_PropertyChanged;
      }
      model.NodeAdded += CurrentWorkspaceModel_NodeAdded;
      model.NodeRemoved += CurrentWorkspaceModel_NodeRemoved;
      model.NodesCleared += CurrentWorkspaceModel_NodesCleared;
    }

    private void RemoveEventHandlers(IWorkspaceModel model)
    {
      foreach (var node in model.Nodes)
      {
        node.PropertyChanged -= node_PropertyChanged;
      }
      model.NodeAdded -= CurrentWorkspaceModel_NodeAdded;
      model.NodeRemoved -= CurrentWorkspaceModel_NodeRemoved;
      model.NodesCleared -= CurrentWorkspaceModel_NodesCleared;
    }

    public void ZoomToPosition(NodeInfo nodeInfo)
    {
      foreach (var node in _readyParams.CurrentWorkspaceModel.Nodes)
      {
        if (node.GUID == nodeInfo.GUID)
        {
          // node.Select();
          var cmd = new DynamoModel.SelectInRegionCommand(node.Rect, false);
          _readyParams.CommandExecutive.ExecuteCommand(cmd, null, null);

          // Call this twice as otherwise the zoom level altertnates been close
          // and far

          _dynamoViewModel.FitViewCommand.Execute(null);
          _dynamoViewModel.FitViewCommand.Execute(null);

          // Display the error/warning message

          var hsvm = (HomeWorkspaceViewModel)_dynamoViewModel.HomeSpaceViewModel;
          foreach (var nodeModel in hsvm.Nodes)
          {
            if (nodeModel.Id == node.GUID)
            {
              // First hide the previously displayed one if there was one

              if (_displaying != null)
              {
                HideTooltip(_displaying);
              }
              ShowTooltip(nodeModel);
              _displaying = nodeModel;

              break;
            }
          }
        }
      }
    }

    // Is the state a warning?

    private bool IsWarning(ElementState state)
    {
      return
        state == ElementState.Warning ||
        state == ElementState.PersistentWarning;
    }

    // Expand the warning bubble for the provided NodeViewModel

    private void ShowTooltip(NodeViewModel nvm)
    {
      var data = new InfoBubbleDataPacket();
      data.Style =
        IsWarning(nvm.State) ?
        InfoBubbleViewModel.Style.Warning :
        InfoBubbleViewModel.Style.Error;
      data.ConnectingDirection = InfoBubbleViewModel.Direction.Bottom;
      nvm.ErrorBubble.ShowFullContentCommand.Execute(data);
    }

    // Collapse he warning bubble for the provided NodeViewModel

    private void HideTooltip(NodeViewModel nvm)
    {
      var data = new InfoBubbleDataPacket();
      data.Style =
        IsWarning(nvm.State) ?
        InfoBubbleViewModel.Style.WarningCondensed :
        InfoBubbleViewModel.Style.ErrorCondensed;
      data.ConnectingDirection = InfoBubbleViewModel.Direction.Bottom;
      nvm.ErrorBubble.ShowCondensedContentCommand.Execute(data);
    }
  }
}
