using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.QuickNote.ViewModel
{
    public class TagsViewModelLstBox : ViewModelBase
    {
        private ObservableCollection<Tag> tags;

        public TagsViewModelLstBox(ObservableCollection<Tag> tags)
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
