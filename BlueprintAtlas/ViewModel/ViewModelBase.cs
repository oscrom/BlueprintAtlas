using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BlueprintAtlas.ViewModel
{
  public abstract class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
      if (Equals(field, newValue)) return false;
      field = newValue;
      OnPropertyChanged(propertyName);
      return true;
    }

    public static List<T> GetPayload<T>(object obj)
    {
      return obj switch
      {
        T typ => new List<T>(1) { typ },
        List<T> list => list,
        IEnumerable<T> collection => new List<T>(collection),
        _ => new List<T>(0)
      };
    }
  }
}
