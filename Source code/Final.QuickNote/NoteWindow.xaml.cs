using Final.QuickNote.Class;
using Final.QuickNote.Class.Converter;
using Final.QuickNote.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Final.QuickNote
{
    /// <summary>
    /// Enum for background color - note window
    /// </summary>
    public enum ColorTag
    {
        Yellow,
        Green,
        Blue,
        Purple,
        Pink,
        Gray
    }

    public class NoteUpdateEventArgs : EventArgs
    {
        private Note note_;
        private ObservableCollection<Tag> tags_;

        public NoteUpdateEventArgs(Note note, ObservableCollection<Tag> tags)
        {
            this.note_ = note;
            this.tags_ = tags;
        }

        public Note Note
        {
            get
            {
                return this.note_;
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get
            {
                return this.tags_;
            }
        }
    }

    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        #region Delegates
        public delegate void NoteUpdateHandler(object sender, NoteUpdateEventArgs e);
        #endregion

        #region Members Data
        private ColorTag colorTag;
        private ObservableCollection<Tag> noteTags;
        private Note editableNote = null;
        #endregion

        #region Static Data
        public static readonly Color[] colorsHighLight = { Color.FromRgb(255, 185, 1),  Color.FromRgb(17, 137, 5),  Color.FromRgb(1, 121, 215),
                                                           Color.FromRgb(93, 36, 155),  Color.FromRgb(217, 1, 169), Color.FromRgb(190, 190, 190)};
        public static readonly Color[] colorsBackground = { Color.FromRgb(255, 242, 181),   Color.FromRgb(199, 239, 196),   Color.FromRgb(202, 232, 255),
                                                            Color.FromRgb(225, 215, 237),   Color.FromRgb(225, 199, 245),   Color.FromRgb(243, 243, 243)};
        #endregion

        #region Custom Events
        public event NoteUpdateHandler NoteUpdate;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor Note Window - Create New Note Window
        /// </summary>
        public NoteWindow(IEnumerable<Tag> listAllTags)
        {
            InitializeComponent();

            colorTag = 0;
            titleWindow.Text = "Create New Note";
            noteTags = new ObservableCollection<QuickNote.Tag>(listAllTags);

            if (lstTags != null) lstTags.DataContext = new TagsViewModelLstBox(noteTags);
        }

        /// <summary>
        /// Constructor Note Window - Show Note Only Not Editable
        /// </summary>
        /// <param name="note"></param>
        public NoteWindow(Note note, IEnumerable<Tag> listTagsOfNote)
        {
            InitializeComponent();

            colorTag = note.ColorNote;
            if (this != null) this.Background = new SolidColorBrush(colorsBackground[(int)colorTag]);

            settingBtn.IsEnabled = false;
            settingBtn.Visibility = Visibility.Collapsed;
            addBtn.IsEnabled = false;
            addBtn.Visibility = Visibility.Collapsed;
            addBtnImg.Visibility = Visibility.Collapsed;

            btnsControlNewNote.Visibility = Visibility.Collapsed;
            btnsControlShowNote.Visibility = Visibility.Visible;

            titleNote.Text = note.Title;
            titleNote.IsReadOnly = true;

            ByteArrayToFlowDocument converter = new ByteArrayToFlowDocument();
            contentNote.Document = converter.Convert(note.Document, null, null, null) as FlowDocument;
            contentNote.IsReadOnly = true;

            string tagsName = "";
            bool first = false;
            foreach (var tag in listTagsOfNote)
            {
                if (first) tagsName += ", ";
                tagsName += tag.Name;
                first = true;
            }

            tagNote.Text = tagsName;

            titleWindow.Text = "Show Note";
        }

        /// <summary>
        /// Constructor Note Window - Show Note and Editable Note
        /// </summary>
        /// <param name="note"></param>
        /// <param name="listTagsOfNote"></param>
        /// <param name="listAllTags"></param>
        public NoteWindow(Note note, IEnumerable<Tag> listTagsOfNote, IEnumerable<Tag> listAllTags)
        {
            InitializeComponent();

            editableNote = note;
            colorTag = note.ColorNote;
            if (this != null) this.Background = new SolidColorBrush(colorsBackground[(int)colorTag]);

            titleNote.Text = note.Title;
            ByteArrayToFlowDocument converter = new ByteArrayToFlowDocument();
            contentNote.Document = converter.Convert(note.Document, null, null, null) as FlowDocument;

            noteTags = new ObservableCollection<QuickNote.Tag>(listAllTags);

            foreach (var tag in listTagsOfNote)
            {
                Tag res = null;
                res = noteTags.FirstOrDefault(x => x.Id == tag.Id);
                if (res != null)
                {
                    res.IsSelected = true;
                }
            }

            if (lstTags != null) lstTags.DataContext = new TagsViewModelLstBox(noteTags);
            UpdateTagNoteText();

            titleWindow.Text = "Edit Note";

            foreach (object child in groupColorPicker.Children)
            {
                if (child is RadioButton)
                {
                    RadioButton pick = child as RadioButton;
                    ColorTag colorBtn = (ColorTag)Int32.Parse(pick.Tag as string);

                    if (colorBtn == colorTag)
                    {
                        pick.IsChecked = true;
                    }
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Override OnMouseLeftButtonDown on Window for Drag Move
        /// </summary>
        /// <param name="e">MouseButtonEventArgs</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
        #endregion

        #region NoteWindow Events Handler
        /// <summary>
        /// When Window is deactivated change color of titleBar and tagBar to lightyellow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeActivated(object sender, EventArgs e)
        {
            titleBar.Background = new SolidColorBrush(Colors.Transparent);
            tagBar.Background = new SolidColorBrush(Colors.Transparent);
            controlDock.Visibility = Visibility.Hidden;
            addBtn.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// When Window is activated change color of titleBar and tagBar to coral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivatedWindow(object sender, EventArgs e)
        {
            titleBar.Background = new SolidColorBrush(colorsHighLight[(int)colorTag]);
            tagBar.Background = new SolidColorBrush(colorsHighLight[(int)colorTag]);
            controlDock.Visibility = Visibility.Visible;
            addBtn.Visibility = Visibility.Visible;
        }
        #endregion

        /// <summary>
        /// Detect when user typing content note to hide instruction text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contentTextChanged(object sender, TextChangedEventArgs e)
        {
            FlowDocument content = contentNote.Document;

            TextPointer startPointer = content.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer endPointer = content.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);

            if (startPointer == null || endPointer == null)
            {
                placeHolderContentNote.Visibility = Visibility.Visible;
            }
            else
            {
                if (startPointer.CompareTo(endPointer) == 0)
                {
                    placeHolderContentNote.Visibility = Visibility.Visible;
                }
                else
                {
                    placeHolderContentNote.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handle click when user click on "Hủy" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discardClick(object sender, RoutedEventArgs e)
        {
            FlowDocument content = contentNote.Document;

            TextPointer startPointer = content.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer endPointer = content.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);

            if (startPointer == null || endPointer == null)
            {
                this.Close();
            }
            else
            {
                if (startPointer.CompareTo(endPointer) == 0)
                {
                    this.Close();
                }
                else
                {
                    MessageBoxResult boxResult = MessageBox.Show("Note chưa lưu.\r\nBạn có muốn thoát?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (boxResult)
                    {
                        case MessageBoxResult.Yes:
                            this.Close();
                            break;
                        case MessageBoxResult.No:
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handle click when user click on "Xong" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doneClick(object sender, RoutedEventArgs e)
        {
            FlowDocument content = contentNote.Document;

            TextPointer startPointer = content.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer endPointer = content.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);

            if (startPointer == null || endPointer == null || startPointer.CompareTo(endPointer) == 0)
            {
                MessageBoxResult boxResult = MessageBox.Show("Chưa có nội dung note", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MemoryStream memoryStream = new MemoryStream();
                TextRange textRange = new TextRange(content.ContentStart, content.ContentEnd);
                textRange.Save(memoryStream, System.Windows.DataFormats.XamlPackage);
                byte[] byteArray = new byte[memoryStream.Length];
                byteArray = memoryStream.ToArray();

                Tag itemToRemove = null;
                do
                {
                    itemToRemove = noteTags.FirstOrDefault(r => r.IsSelected == false);
                    if (itemToRemove != null)
                    {
                        noteTags.Remove(itemToRemove);
                    }
                } while (itemToRemove != null);

                if (editableNote == null)
                {
                    editableNote = new Note()
                    {
                        Id = -1,
                        Title = titleNote.Text,
                        Document = byteArray,
                        LastModified = DateTime.Now,
                        ColorNote = colorTag
                    };

                }
                else
                {
                    editableNote.Title = titleNote.Text;
                    editableNote.Document = byteArray;
                    editableNote.LastModified = DateTime.Now;
                    editableNote.ColorNote = colorTag;
                }

                NoteUpdateEventArgs args = new NoteUpdateEventArgs(editableNote, noteTags);
                NoteUpdate(this, args);

                this.Close();
            }
        }

        /// <summary>
        /// Handle click on Setting Button to show popup color picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            colorPopup.IsOpen = true;
        }

        /// <summary>
        /// Popup radio button checked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorRadioBtnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            int tag = -1;
            bool check = Int32.TryParse(radioButton.Tag as string, out tag);

            if (check)
            {
                colorTag = (ColorTag)tag;
                if (titleBar != null) titleBar.Background = new SolidColorBrush(colorsHighLight[tag]);
                if (tagBar != null) tagBar.Background = new SolidColorBrush(colorsHighLight[tag]);
                if (this != null) this.Background = new SolidColorBrush(colorsBackground[tag]);
            }
        }

        /// <summary>
        /// Button Always On Top (ToggleButton) Click handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alwaysOnTopClick(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            this.Topmost = (bool)toggleButton.IsChecked;
        }

        /// <summary>
        /// Button Exit Click handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handle click on add tag button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            addTagPopup.IsOpen = true;
            txtTagName.Clear();
            btnNewTag.Visibility = Visibility.Collapsed;
        }

        #region Filter Tag Item
        private bool allMatch = false;
        /// <summary>
        /// Custom filter tag item in list box with text in text box
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CustomFilter(object obj)
        {
            if (string.IsNullOrEmpty(txtTagName.Text))
            {
                return true;
            }
            else
            {
                Tag tag = obj as Tag;

                if (String.Compare(tag.Name, txtTagName.Text, true) == 0)
                {
                    allMatch = true;
                }
                return (tag.Name.IndexOf(txtTagName.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        /// <summary>
        /// Search or new tag name text box handle text changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTagName_TextChanged(object sender, TextChangedEventArgs e)
        {
            allMatch = false;
            CollectionView view = CollectionViewSource.GetDefaultView(lstTags.ItemsSource) as CollectionView;
            view.Filter = CustomFilter;

            if (allMatch == true) btnNewTag.Visibility = Visibility.Collapsed;
            else
            {
                btnNewTag.Visibility = Visibility.Visible;
                btnNewTag.Content = @"Tạo mới '" + txtTagName.Text + @"'";
            }
        }
        #endregion

        /// <summary>
        /// Update tagNote Text by Tags is selected
        /// </summary>
        private void UpdateTagNoteText()
        {
            tagNote.Text = "";
            string tags = "";
            bool check = false;
            foreach (var obj in noteTags)
            {
                Tag tag = obj as Tag;
                if (tag.IsSelected == true)
                {
                    if (check == true)
                    {
                        tags += ", ";
                    }
                    tags += tag.Name;
                    check = true;
                }
            }

            tagNote.Text = tags;
        }

        /// <summary>
        /// Click on tag item in list box will update tagNote Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tagItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateTagNoteText();
        }

        /// <summary>
        /// Handle click on button new tag when cannot find tag name in list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewTag_Click(object sender, RoutedEventArgs e)
        {
            noteTags.Add(new Tag()
            {
                Name = txtTagName.Text,
                IsSelected = true
            });

            txtTagName.Clear();
            UpdateTagNoteText();
        }
    }
}
