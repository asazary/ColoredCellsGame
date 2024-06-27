using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ColoredCells
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game _game;
        private int _size = 3;

        private const int BlockBorder = 2;
        private readonly Brush _blockBorderBrush = Brushes.Gray;

        private Dictionary<int, Brush> _blockBrushes = new()
        {
            { 0, Brushes.White },
            { 1, Brushes.LightSeaGreen },
            { 2, Brushes.RoyalBlue },
            { 3, Brushes.IndianRed },
            { 4, Brushes.YellowGreen },
            { 5, Brushes.DarkViolet }
        };

        private Rectangle[,] Cells = new Rectangle[0, 0];


        public MainWindow()
        {
            InitializeComponent();

            _game = new Game(_size, 2, 1, 1);
            Cells = new Rectangle[_size, _size];

            FieldSizeList.Items.Clear();
            FieldSizeList.ItemsSource = new int[] { 3, 4, 5 }; 
            FieldSizeList.SelectedIndex = 0;

            GameLevelList.Items.Clear();
            GameLevelList.ItemsSource = new int[] { 1, 2, 3, 4 };
            GameLevelList.SelectedIndex = 0;

            ColorsCountList.Items.Clear();
            ColorsCountList.ItemsSource = new int[] { 2, 3, 4 };
            ColorsCountList.SelectedIndex = 1;

            FieldCanvas.Background = Brushes.LightGray;           
        }

        private void MainWindowOnLoad(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e) => StartNewGame();

        private void StartNewGame()
        {
            _size = (int)FieldSizeList.SelectedItem;
            var level = (int)GameLevelList.SelectedItem;
            var colorsCount = (int)ColorsCountList.SelectedItem;
            var randomMoves = (int)RandMovesCntSlider.Value;

            _game = new Game(_size, colorsCount, level, randomMoves);
            Cells = new Rectangle[_size, _size];
                        
            _game.StartGame();
            WinLabel.Visibility = Visibility.Collapsed;
            DrawField(_size);
        }

        private void FinishGame()
        {
            _game.EndGame();
            WinLabel.Visibility = Visibility.Visible;
        }

        private void DrawField(int size)
        {
            FieldCanvas.Children.Clear();
            var actWidth = FieldCanvas.ActualWidth;

            var blockWidth = actWidth / size;
            var blockSideSize = blockWidth;            

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var rect = CreateBlock(blockSideSize, i, j);
                    FieldCanvas.Children.Add(rect);
                    Cells[i, j] = rect;
                    Canvas.SetTop(rect, i * blockSideSize);
                    Canvas.SetLeft(rect, j * blockSideSize);
                }
            }
        }

        private Rectangle CreateBlock(double blockSideSize, int row, int col)
        {
            var cellValue = _game.Field[row, col];
            var rect = new Rectangle
            {
                Fill = _blockBrushes[cellValue],  // _blockZeroBrush,
                Stroke = _blockBorderBrush,
                StrokeThickness = BlockBorder,
                Height = blockSideSize,
                Width = blockSideSize,
                Tag = new CellCoords { Row = row, Col = col }
            };
            rect.MouseLeftButtonDown += Block_OnClick;
            
            return rect;
        }

        private void Block_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Rectangle rect)
                ChangeCell((CellCoords)rect.Tag);
        }

        private void ChangeCell(CellCoords cellCoords)
        {
            if (_game.GameStatus != GameState.Continues) return;
            _game.PauseGame();

            var changedCells = _game.ChangeCell(cellCoords);
            ChangeCellsColors(changedCells);
            if (_game.CheckWin()) FinishGame();
        }

        private void ChangeCellsColors(IEnumerable<CellCoords> cellsCoords)
        {
            foreach (var cell in cellsCoords)
            {
                Cells[cell.Row, cell.Col].Fill = _blockBrushes[_game.Field[cell.Row, cell.Col]];
            }

            _game.ContinueGame();
        }                
    }

    public struct CellCoords(int row, int col)
    {
        public int Row = row;
        public int Col = col;

        public override readonly string ToString() => $"{Row}, {Col}";
    }
}