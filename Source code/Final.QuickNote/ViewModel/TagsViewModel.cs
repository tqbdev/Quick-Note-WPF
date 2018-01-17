using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.QuickNote.ViewModel
{
    public class TagsViewModel : ViewModelBase
    {
        private ObservableCollection<Tag> tags;

        public TagsViewModel(ObservableCollection<Tag> tags)
        {
            this.tags = tags;
        }

        public ObservableCollection<Tag> Tags
        {
            get
            {
                return tags;
            }
            set
            {
                tags = value;
                OnPropertyChanged("Tags");
            }
        }
    }
}
