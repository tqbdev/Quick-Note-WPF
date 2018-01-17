using Final.QuickNote.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Final.QuickNote.ViewModel
{
    public class NotesViewModel : ViewModelBase
    {
        private ObservableCollection<Note> notes;

        public NotesViewModel(ObservableCollection<Note> notes)
        {
            this.notes = notes;
        }

        public ObservableCollection<Note> Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }
    }
}
