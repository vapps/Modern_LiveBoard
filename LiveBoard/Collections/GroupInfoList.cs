using System.Collections.ObjectModel;

namespace LiveBoard.Collections
{
    public class GroupInfoList<T> : ObservableCollection<T>
    {
        public string Key { get; set; }
    }
}
