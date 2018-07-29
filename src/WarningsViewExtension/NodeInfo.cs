using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WarningsViewExtension
{
  public class NodeInfo : INotifyPropertyChanged
  {
    private int _id;
    private string _name;
    private Guid _guid;

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public NodeInfo(int id, string name, Guid guid)
    {
      _id = id;
      _name = name;
      _guid = guid;
      NotifyPropertyChanged();
    }

    public int ID
    {
      get { return _id; }
      set
      {
        _id = value;
        NotifyPropertyChanged();
      }
    }

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        NotifyPropertyChanged();
      }
    }

    public Guid GUID
    {
      get { return _guid; }
      set
      {
        _guid = value;
        NotifyPropertyChanged();
      }
    }
  }
}
