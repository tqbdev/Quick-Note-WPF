using Final.QuickNote.PieChart.Converter;
using Final.QuickNote.PieChart.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Final.QuickNote.PieChart
{
    /// <summary>
    /// Interaction logic for PieChartWindow.xaml
    /// </summary>
    public partial class PieChartWindow : Window
    {
        private ObservableCollection<Tag> listTag = null;

        public PieChartWindow(IEnumerable<Tag> listAllTag)
        {
            InitializeComponent();

            listTag = new ObservableCollection<Tag>(listAllTag);

            double HoleSize = 0.3;

            double halfWidth = canvas.Width / 2;
            double innerRadius = halfWidth * HoleSize;

            // compute the total for the property which is being plotted

            double total = 0;
            foreach (var item in listTag)
            {
                total += item.Amount;
            }

            // add the pie pieces
            canvas.Children.Clear();
            List<PiePiece> piePieces = new List<PiePiece>();
            piePieces.Clear();

            double accumulativeAngle = 0;
            Random rnd = new Random(Guid.NewGuid().ToString().GetHashCode());
            foreach (var item in listTag)
            {
                double wedgeAngle = item.Amount * 360 / total;

                PiePiece piece = new PiePiece()
                {
                    Radius = halfWidth,
                    InnerRadius = innerRadius,
                    CentreX = halfWidth,
                    CentreY = halfWidth,
                    WedgeAngle = wedgeAngle,
                    PieceValue = item,
                    RotationAngle = accumulativeAngle,
                    Fill = Final.QuickNote.PieChart.Utils.PickRandomBrush(rnd),
                    Tag = listTag.IndexOf(item),
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    ToolTip = new ToolTip()
                };

                piece.ToolTipOpening += new ToolTipEventHandler(PiePieceToolTipOpening);

                piePieces.Add(piece);
                canvas.Children.Insert(0, piece);

                accumulativeAngle += wedgeAngle;
            }

            listViewTag.ItemsSource = piePieces;
        }

        /// <summary>
        /// Handles the event which occurs just before a pie piece tooltip opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PiePieceToolTipOpening(object sender, ToolTipEventArgs e)
        {
            PiePiece piece = (PiePiece)sender;
            
            int index = (int)piece.Tag;
            if (piece.ToolTip != null)
            {
                ToolTip tip = (ToolTip)piece.ToolTip;
                Tag tag = listTag.ElementAt(index) as Tag;

                tip.Content = "Name: " + tag.Name + "\r\nAmount: " + tag.Amount + "\r\nPercent: " + string.Format(null, @"{0:0%}", piece.Percentage); ;
            }
        }
    }
}
