using Final.QuickNote.Class;
using Final.QuickNote.PieChart;
using Final.QuickNote.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Final.QuickNote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members Data
        private GlobalKeyboardHook keyboardHook;
        //private NoteWindow noteWindow = null;
        private System.Windows.Forms.NotifyIcon notifyIcon;

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        // Data
        private DatabaseManager databaseManager = null;
        private ObservableCollection<Tag> dataTags = null;
        private ObservableCollection<Note> dataNotes = null;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            keyboardHook = new GlobalKeyboardHook();
            keyboardHook.Install();
            keyboardHook.HotKeyDown += HotKeyDown;

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "Click to open view note window";
            notifyIcon.Icon = Properties.Resources.noteIcon;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseClick);

            System.Windows.Forms.ContextMenu notifyIconMenu = new System.Windows.Forms.ContextMenu();
            notifyIconMenu.MenuItems.Add("New note" + " (" + keyboardHook.HotKey1.ToString() + "+" + keyboardHook.HotKey2.ToString() + ")", new EventHandler(NewNote_MenuItem));
            notifyIconMenu.MenuItems.Add("-");
            notifyIconMenu.MenuItems.Add("View notes", new EventHandler(ViewNotes_MenuItem));
            notifyIconMenu.MenuItems.Add("View statistics", new EventHandler(ViewStat_MenuItem));
            notifyIconMenu.MenuItems.Add("-");
            notifyIconMenu.MenuItems.Add("Exit", new EventHandler(TrayExit_MenuItem));

            notifyIcon.ContextMenu = notifyIconMenu;

            // Initial Database
            databaseManager = new DatabaseManager("NoteTagDatabase.db");
            databaseManager.LoadData();
            dataTags = new ObservableCollection<QuickNote.Tag>(databaseManager.GetListTag());

            dataNotes = new ObservableCollection<Note>(databaseManager.GetListNote());
            listTag.DataContext = new TagsViewModel(dataTags);
            listTag.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            listNote.DataContext = new NotesViewModel(dataNotes);
        }
        #endregion

        #region Note Window
        /// <summary>
        /// Handle HotKey is Pressed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyDown(object sender, EventArgs e)
        {
            NoteWindow noteWindow = null;

            noteWindow = new NoteWindow(databaseManager.GetListTag());

            noteWindow.ShowInTaskbar = false;
            noteWindow.Owner = Application.Current.MainWindow;

            noteWindow.Closed += (s, args) => Application.Current.MainWindow.Focus();
            noteWindow.NoteUpdate += new NoteWindow.NoteUpdateHandler(NoteWindow_DoneClick);
            noteWindow.Show();
            noteWindow.Activate();
            noteWindow.Topmost = true;
            noteWindow.Topmost = false;
            noteWindow.Focus();
        }

        /// <summary>
        /// Handle Done Click on NoteWindow send
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoteWindow_DoneClick(object sender, NoteUpdateEventArgs e)
        {
            if (e.Note.Id == -1 || databaseManager.CheckNoteExist(e.Note.Id) == false)
            {
                databaseManager.InsertNote(e.Note, e.Tags);
            }
            else
            {
                databaseManager.ModifyNote(e.Note, e.Tags);
            }

            dataTags = new ObservableCollection<QuickNote.Tag>(databaseManager.GetListTag());
            listTag.DataContext = new TagsViewModel(dataTags);
            listTag.Items.Refresh();
            listTag.Focus();

            OnClick_AllNotes(allNotesBtn, null);
        }
        #endregion

        #region Notify Icon Handler
        /// <summary>
        /// Handle user click exit on context menu in notify icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrayExit_MenuItem(object sender, EventArgs e)
        {
            //if (noteWindow != null) noteWindow.Close();
            this.Close();
        }

        /// <summary>
        /// Handle user click new note on context menu in notify icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewNote_MenuItem(object sender, EventArgs e)
        {
            HotKeyDown(null, null);
        }

        /// <summary>
        /// Handle user click view notes on context menu in notify icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewNotes_MenuItem(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Show();
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        /// <summary>
        /// Handle user click view stat (statistics of tag) on context menu in notify icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewStat_MenuItem(object sender, EventArgs e)
        {
            PieChartWindow pieChartWindow = null;

            pieChartWindow = new PieChartWindow(databaseManager.GetListTag());
            notifyIcon.ContextMenu.MenuItems[3].Enabled = false;

            pieChartWindow.ShowInTaskbar = true;
            pieChartWindow.Owner = Application.Current.MainWindow;

            pieChartWindow.Closed += (s, args) => { 
                                                    notifyIcon.ContextMenu.MenuItems[3].Enabled = true;
                                                    Application.Current.MainWindow.Focus();
                                                  };
            pieChartWindow.Show();
            pieChartWindow.Activate();
            pieChartWindow.Topmost = true;
            pieChartWindow.Topmost = false;
            pieChartWindow.Focus();
        }

        /// <summary>
        /// Handle user left click to notify icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = WindowState.Normal;
            }
        }
        #endregion

        #region MainWindow Event Handler
        /// <summary>
        /// Handle MainWindow Closing Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (noteWindow != null) noteWindow.Close();
        }

        /// <summary>
        /// Handle MainWindow Closed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainClosed(object sender, EventArgs e)
        {
            keyboardHook.Uninstall();
            notifyIcon.Visible = false;
        }

        /// <summary>
        /// Handle MainWindow State Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                //notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = "Minimize Sucessful";
                notifyIcon.BalloonTipText = "Minimized the app";
                notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                notifyIcon.ShowBalloonTip(500);
                notifyIcon.ContextMenu.MenuItems[2].Enabled = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                //notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
                notifyIcon.ContextMenu.MenuItems[2].Enabled = false;
            }
        }

        /// <summary>
        /// Handle MainWindow Loaded Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLoaded(object sender, RoutedEventArgs e)
        {
            notifyIcon.Visible = true;
            notifyIcon.ContextMenu.MenuItems[2].Enabled = false;
        }
        #endregion

        #region TreeView ListTag
        /// <summary>
        /// List Tag (Tree View) Change Selected Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Tag tag = e.NewValue as Tag;
            if (tag != null)
            {
                ObservableCollection<Note> list = new ObservableCollection<Note>(databaseManager.GetSpecificListNote(tag));

                listNote.DataContext = new NotesViewModel(list);
                listNote.Items.Refresh();
            }
        }

        /// <summary>
        /// Handle AllNotes Button Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick_AllNotes(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            toggleButton.IsChecked = true;

            UnselectTreeViewItem(listTag);

            dataNotes = new ObservableCollection<Note>(databaseManager.GetListNote());

            listNote.DataContext = new NotesViewModel(dataNotes);
            listNote.Items.Refresh();
        }

        /// <summary>
        /// Handle ListTag (Tree View) Got Focus event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_GotFocus(object sender, RoutedEventArgs e)
        {
            allNotesBtn.IsChecked = false;
        }

        /// <summary>
        /// Function for unselect item in TreeView
        /// </summary>
        /// <param name="pTreeView"></param>
        private void UnselectTreeViewItem(TreeView pTreeView)
        {
            if (pTreeView.SelectedItem == null)
                return;

            if (pTreeView.SelectedItem is TreeViewItem)
            {
                (pTreeView.SelectedItem as TreeViewItem).IsSelected = false;
            }
            else
            {
                TreeViewItem item = pTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsSelected = false;
                }
            }
        }

        private void listTag_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //listTag.Items.Refresh();
            //listTag.Focus();
        }

        private void TVRenameItem_ContextMenu(object sender, RoutedEventArgs e)
        {
            Tag tagSelected = listTag.SelectedItem as Tag;

            if (tagSelected != null)
            {
                popupEditTagName.IsOpen = true;
                txtNewTagName.Clear();
            }
        }

        private void btnChangeTagName_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNewTagName.Text))
            {
                MessageBox.Show(this, "Require new name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Tag tagSelected = listTag.SelectedItem as Tag;

            if (tagSelected != null)
            {
                tagSelected.Name = txtNewTagName.Text;
                bool res = databaseManager.UpdateTag(tagSelected);

                if (res == false)
                {
                    MessageBox.Show(this, "New name is exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    dataTags = new ObservableCollection<QuickNote.Tag>(databaseManager.GetListTag());
                    listTag.DataContext = new TagsViewModel(dataTags);
                    listTag.Items.Refresh();
                    listTag.Focus();

                    OnClick_AllNotes(allNotesBtn, null);

                    popupEditTagName.IsOpen = false;
                }
            }
        }

        private void TVRemoveItem_ContextMenu(object sender, RoutedEventArgs e)
        {
            Tag tagSelected = listTag.SelectedItem as Tag;

            if (tagSelected != null)
            {
                databaseManager.DeleteTag(tagSelected);

                dataTags = new ObservableCollection<QuickNote.Tag>(databaseManager.GetListTag());
                listTag.DataContext = new TagsViewModel(dataTags);
                listTag.Items.Refresh();
                listTag.Focus();

                OnClick_AllNotes(allNotesBtn, null);
            }
        }

        private TreeViewItem lastRightClick = null;
        private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                item.IsSelected = true;
                item.ContextMenu = listTag.Resources["ItemContextMenu"] as ContextMenu;
                e.Handled = true;
                lastRightClick = item;
            }
        }

        private void TreeView_ContextMenuClosed(object sender, RoutedEventArgs e)
        {
            if (lastRightClick != null)
            {
                lastRightClick.ContextMenu = null;
                lastRightClick = null;
            }
        }
        #endregion

        #region ListView ListNote
        /// <summary>
        /// ListView ColumnHeader Click for Sort Items in ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listNoteColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listNote.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            listNote.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        #region Context menu in list view
        /// <summary>
        /// Remove Header in context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LVRemoveItem_ContextMenu(object sender, RoutedEventArgs e)
        {
            NotesViewModel notesViewModel = listNote.DataContext as NotesViewModel;
            ObservableCollection<Note> list = notesViewModel.Notes;
            Note note = listNote.SelectedItem as Note;

            if (note != null)
            {
                databaseManager.DeleteNote(note);
                list.Remove(note);
                listNote.Items.Refresh();

                int index = 0;
                Tag selectedTag = listTag.SelectedItem as Tag;
                if (selectedTag != null)
                {
                    index = listTag.ItemContainerGenerator.IndexFromContainer(listTag.ItemContainerGenerator.ContainerFromItem(selectedTag));
                }

                dataTags = new ObservableCollection<QuickNote.Tag>(databaseManager.GetListTag());
                listTag.DataContext = new TagsViewModel(dataTags);
                listTag.Items.Refresh();

                if (selectedTag != null)
                {
                    TreeViewItem item = listTag.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                }
                else
                {
                    OnClick_AllNotes(allNotesBtn, null);
                }
            }
        }

        /// <summary>
        /// Edit Header in context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LVEditItem_ContextMenu(object sender, RoutedEventArgs e)
        {
            Note note = listNote.SelectedItem as Note;

            if (note != null)
            {
                NoteWindow noteWindow = new NoteWindow(note, databaseManager.GetSpecificListTag(note), databaseManager.GetListTag());

                noteWindow.ShowInTaskbar = false;
                noteWindow.Owner = Application.Current.MainWindow;

                noteWindow.Closed += (s, args) => noteWindow = null;
                noteWindow.NoteUpdate += new NoteWindow.NoteUpdateHandler(NoteWindow_DoneClick);
                noteWindow.Show();
                noteWindow.Activate();
                noteWindow.Topmost = true;
                noteWindow.Topmost = false;
                noteWindow.Focus();
            }
        }

        /// <summary>
        /// Show Header in context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LVShowItem_ContextMenu(object sender, RoutedEventArgs e)
        {
            Note note = listNote.SelectedItem as Note;

            if (note != null)
            {
                NoteWindow showWindow = new NoteWindow(note, databaseManager.GetSpecificListTag(note));

                showWindow.ShowInTaskbar = false;
                showWindow.Owner = Application.Current.MainWindow;

                showWindow.Closed += (s, args) => showWindow = null;
                showWindow.Show();
                showWindow.Activate();
                showWindow.Topmost = true;
                showWindow.Topmost = false;
                showWindow.Focus();
            }
        }
        #endregion

        #endregion
    }
}
