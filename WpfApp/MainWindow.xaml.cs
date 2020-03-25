using MiniEditor;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<ViewModel.ViewModel>, IReactiveObject

    {
        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteSpec { get; set; }
        public ReactiveCommand<Unit, Unit> SaveAll { get; set; }
        public ReactiveCommand<Unit, Unit> LoadAll { get; set; }

        public ReactiveCommand<Unit, Unit> Draw { get; set; }

        //Обработчик мышки
        private System.Windows.Point Mousepos1, Mousepos2;
        private int RightPressed = 0, LeftPressed = 0;

        public IGraphic CurrentCanvas;

        private string CurrentFigureName = "Empty";
        private List<Button> Buttons = new List<Button>();

        private IFigure fig;
        private int figUpdated = 0;

        public MainWindow()
        {       
            DataContext = this;
            InitializeComponent();
            CurrentCanvas = new BuildFigure(MainCanvas);
            this.WhenActivated(disposer =>
            {
                
                Add = ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    var C = new MiniEditor.Point { X = random.Next(100, 600), Y = random.Next(100, 600) };
                    var C2 = new MiniEditor.Point { X = C.X + random.Next(-100, 100), Y = C.Y + random.Next(-100, 100) };
                    var fig = new MiniEditor.Circle(C, C2);
                    ViewModel.Add.Execute(fig).Subscribe();
                    DrawAll(true);
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    return default;
                }).DisposeWith(disposer);

                Delete = ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    var fig = ViewModel.AllFigures.FirstOrDefault();
                    ViewModel.Delete.Execute(fig).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    MainCanvas.Children.Clear();
                    DrawAll(false);
                    return default;
                }, ViewModel.Delete.CanExecute).DisposeWith(disposer);

                SaveAll = ReactiveCommand.Create<Unit, Unit>(_ => {
                    ViewModel.SaveAll.Execute().Subscribe();
                    return default;
                }, ViewModel.SaveAll.CanExecute).DisposeWith(disposer);

                LoadAll = ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    ViewModel.LoadAll.Execute().Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    this.Error.Content = ViewModel.error;
                    DrawAll(false);
                    return default;
                }).DisposeWith(disposer);

                this.RaisePropertyChanged("Add");
                this.RaisePropertyChanged("Delete");
                this.RaisePropertyChanged("SaveAll");
                this.RaisePropertyChanged("LoadAll");
                this.RaisePropertyChanged("Circle");
                this.RaisePropertyChanged("Line");
                this.RaisePropertyChanged("Rectangle");
            });

        }
        ViewModel.ViewModel viewModel = new ViewModel.ViewModel();
        public ViewModel.ViewModel ViewModel { get => viewModel; set { } }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (ViewModel.ViewModel)value; }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging.Invoke(this, args);
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged.Invoke(this, args);
        }

        //Функция для передачи координат ModelView
        private void DrawFigure()
        {
            switch (CurrentFigureName)
            {
                case "Emplty": { break; }
                case "Line": { break; }
                case "Circle": { break; }
                case "Rectangle": { break; }
            }
        }

        //Обработчик нажатия на кнопку линии
        private void Line_Button_Click(object sender, RoutedEventArgs e)
        {
            LineButton.Background = System.Windows.Media.Brushes.DarkRed;
            EllipseButton.Background = System.Windows.Media.Brushes.LightCyan;
            PolygonButton.Background = System.Windows.Media.Brushes.LightCyan;
            CurrentFigureName = "Line";
        }
        //Обработчик нажатия на кнопку круга
        private void Circle_Button_Click(object sender, RoutedEventArgs e)
        {
            LineButton.Background = System.Windows.Media.Brushes.LightCyan;
            EllipseButton.Background = System.Windows.Media.Brushes.DarkRed;
            PolygonButton.Background = System.Windows.Media.Brushes.LightCyan;
            CurrentFigureName = "Circle";
        }

        //Обработчик нажатия на кнопку полигона
        private void Polygon_Button_Click(object sender, RoutedEventArgs e)
        {
            LineButton.Background = System.Windows.Media.Brushes.LightCyan;
            EllipseButton.Background = System.Windows.Media.Brushes.LightCyan;
            PolygonButton.Background = System.Windows.Media.Brushes.DarkRed;
            CurrentFigureName = "Rectangle";
        }


        //Нажатие на левую кнопку мыши
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
            Errorbox.Text = "LeftMousePressed" + "\n" +
               "X: " + position.X +
               "\n" +
               "Y: " + position.Y;
            Mousepos1 = position;
        }

        //Нажатие правую кнопку мыши
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
            Errorbox.Text = "RightMousePressed" + "\n" +
                "X: " + position.X +
                "\n" +
                "Y: " + position.Y;
            
        }

        //Перемещение мыши
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
            //LeftDrag
            if (CurrentFigureName != "Empty") {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Errorbox.Text = "LeftMouseDrag" + "\n" +
                   "X: " + position.X +
                   "\n" +
                   "Y: " + position.Y;
                    Mousepos2 = position;
                    List<MiniEditor.Point> Points = new List<MiniEditor.Point>();
                    Points.Add(new MiniEditor.Point { X = Mousepos1.X, Y = Mousepos1.Y });
                    Points.Add(new MiniEditor.Point { X = Mousepos2.X, Y = Mousepos2.Y});
                    /*IFigure*/ fig = ViewModel.Create(CurrentFigureName, Points);

                    ViewModel.Add.Execute(fig).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    MainCanvas.Children.Clear();
                    DrawAll(false);
                    ViewModel.Delete.Execute(fig).Subscribe();
                    figUpdated = 1;
                }
                else
                {
                    if (figUpdated == 1)
                    {
                        ViewModel.Add.Execute(fig).Subscribe();
                        DrawAll(false);
                        figUpdated = 0;
                    }
                }

                //RightDrag
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    Errorbox.Text = "RightMouseDrag" + "\n" +
                    "X: " + position.X +
                     "\n" +
                     "Y: " + position.Y;
                }
            }
            else 
            {
                //тут режим Empty, типа перетаскивание пусть включается
            }
         }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private void DeteleFromList(object sender, MouseButtonEventArgs e)
        {
            ListView temp = sender as ListView;
            var item = temp.SelectedItem as MiniEditor.IFigure;
            ViewModel.Delete.Execute(item).Subscribe();
            ViewModel.Add.Execute(item).Subscribe();
            MainCanvas.Children.Clear();
            DrawAll(true);
        }

        public void DrawAll(bool clearall)
        {
            var Figures = ViewModel.AllFigures.Reverse().ToArray();
            if (clearall) MainCanvas.Children.Clear();
            foreach (var fig in Figures)
            {
                if (fig != null)
                {
                    List<System.Windows.Point> C = new List<System.Windows.Point>(); //здесь будут точки текущей фигуры
                    foreach (var p in fig.Parameters)
                    {
                        C.Add(new System.Windows.Point { X = ((MiniEditor.Point)fig[p]).X, Y = ((MiniEditor.Point)fig[p]).Y });

                    }
                    if (fig.Name == "Line")
                    {
                        CurrentCanvas.Polyline(C, Colors.Red, 5);
                    }

                    if (fig.Name == "Circle")
                    {
                        CurrentCanvas.Circle(C, Colors.Red, 5);
                    }

                    if (fig.Name == "Rectangle")
                    {
                        CurrentCanvas.Rectangle(C, Colors.Red, 5);
                    }
                }
            }
        }

    }
}
