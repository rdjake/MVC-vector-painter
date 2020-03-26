using Microsoft.Win32;
using MiniEditor;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        private IGraphic CurrentCanvas;

        private  System.Windows.Media.SolidColorBrush CurrentBrush;
        private System.Windows.Media.SolidColorBrush CurrentBGColor;
        private double Thickness = 5;

        private string CurrentFigureName = "Empty";
        private List<Button> Buttons = new List<Button>();

        private IFigure fig;
        private int figUpdated = 0;

        public MainWindow()
        {       
            DataContext = this;
            InitializeComponent();
            CurrentCanvas = new BuildFigure(MainCanvas);
            CurrentBrush = new System.Windows.Media.SolidColorBrush(Colors.Black);
            CurrentBGColor = new System.Windows.Media.SolidColorBrush(Colors.White);
            this.WhenActivated(disposer =>
            {

                Add = ReactiveCommand.Create<Unit, Unit>(_ => {
                    //Random random = new Random();
                    //var C = new MiniEditor.Point { X = random.Next(100, 600), Y = random.Next(100, 600) };
                    //var C2 = new MiniEditor.Point { X = C.X + random.Next(-100, 100), Y = C.Y + random.Next(-100, 100) };
                    //var fig = new MiniEditor.Circle(C, C2);
                    //ViewModel.Add.Execute(fig).Subscribe();
                    //DrawAll(true);
                    //this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    return default;
                }).DisposeWith(disposer);

                Delete = ReactiveCommand.Create<Unit, Unit>(_ => {
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
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                    openFileDialog.Filter = "txt files (*.txt)|AllFigures*.txt";
                    if (openFileDialog.ShowDialog() == true)
                        ViewModel.LoadAll.Execute(openFileDialog.FileName).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
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

        //Обработчик нажатия на кнопку линии
        private void Line_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFigureName == "Line")
            {
                LineButton.Background = System.Windows.Media.Brushes.LightGray;
                EllipseButton.Background = System.Windows.Media.Brushes.LightGray;
                PolygonButton.Background = System.Windows.Media.Brushes.LightGray;
                CurrentFigureName = "Empty";
            }
            else
            {
                LineButton.Background = System.Windows.Media.Brushes.DarkRed;
                EllipseButton.Background = System.Windows.Media.Brushes.LightCyan;
                PolygonButton.Background = System.Windows.Media.Brushes.LightCyan;
                CurrentFigureName = "Line";
            }
        }
        //Обработчик нажатия на кнопку круга
        private void Circle_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFigureName == "Circle")
            {
                LineButton.Background = System.Windows.Media.Brushes.LightGray;
                EllipseButton.Background = System.Windows.Media.Brushes.LightGray;
                PolygonButton.Background = System.Windows.Media.Brushes.LightGray;
                CurrentFigureName = "Empty";
            }
            else
            {
                LineButton.Background = System.Windows.Media.Brushes.LightCyan;
                EllipseButton.Background = System.Windows.Media.Brushes.DarkRed;
                PolygonButton.Background = System.Windows.Media.Brushes.LightCyan;
                CurrentFigureName = "Circle";
            }
        }

        //Обработчик нажатия на кнопку полигона
        private void Polygon_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFigureName == "Rectangle")
            {
                LineButton.Background = System.Windows.Media.Brushes.LightGray;
                EllipseButton.Background = System.Windows.Media.Brushes.LightGray;
                PolygonButton.Background = System.Windows.Media.Brushes.LightGray;
                CurrentFigureName = "Empty";
            }
            else
            {
                LineButton.Background = System.Windows.Media.Brushes.LightCyan;
                EllipseButton.Background = System.Windows.Media.Brushes.LightCyan;
                PolygonButton.Background = System.Windows.Media.Brushes.DarkRed;
                CurrentFigureName = "Rectangle";
            }
        }


        //Нажатие на левую кнопку мыши
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
            if (position.Y < MainCanvas.Height && position.Y > Menu.Height)
                Mousepos1 = position;
        }

        //Нажатие правую кнопку мыши
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
        }

        //Перемещение мыши
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
            if (position.Y < MainCanvas.Height && position.Y > Menu.Height)
            {
                //LeftDrag
                if (CurrentFigureName != "Empty")
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {

                        if (position.Y < 700 || position.X > 0) //Чтобы не залазить на панель работы с выпадающим списком
                        {
                            Mousepos2 = position;
                            List<MiniEditor.Point> Points = new List<MiniEditor.Point>();
                            Points.Add(new MiniEditor.Point { X = Mousepos1.X, Y = Mousepos1.Y });
                            Points.Add(new MiniEditor.Point { X = Mousepos2.X, Y = Mousepos2.Y });
                            MiniEditor.Brush br;
                            br.Line.R = CurrentBrush.Color.R;
                            br.Line.G = CurrentBrush.Color.G;
                            br.Line.B = CurrentBrush.Color.B;
                            br.Line.A = CurrentBrush.Color.A;

                            //СЮДА ЦВЕТ ЗАЛИВКИ ИЗ ПАЛИТРЫ
                            br.Fill.R = CurrentBGColor.Color.R;
                            br.Fill.G = CurrentBGColor.Color.G;
                            br.Fill.B = CurrentBGColor.Color.B;
                            br.Fill.A = CurrentBGColor.Color.A;

                            br.Thickness = Thickness;
                            /*IFigure*/
                            fig = ViewModel.Create(CurrentFigureName, Points, br);

                            ViewModel.Add.Execute(fig).Subscribe();
                            this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                            MainCanvas.Children.Clear();
                            DrawAll(false);
                            ViewModel.Delete.Execute(fig).Subscribe();
                            figUpdated = 1;
                        }
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

                    }
                }
                else
                {
                    //тут режим Empty, типа перетаскивание пусть включается
                }
            }
         }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private void MainColorSelect(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CurrentBrush = (SolidColorBrush)button.Background;
            MaincolorRect.Fill = button.Background;
        }
        private void BgColorSelect(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CurrentBGColor = (SolidColorBrush)button.Background;
            BackgroundRect.Fill = button.Background;
        }
        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Thickness = slValue.Value;
            this.Thick.Content = Thickness;
            //this.NumberOfFigures.Content = Thickness;
        }


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
                    MiniEditor.Brush br;
                    foreach (var p in fig.Parameters)
                    {
                        if(fig[p].GetType().ToString() == "MiniEditor.Point") 
                            C.Add(new System.Windows.Point { X = ((MiniEditor.Point)fig[p]).X, Y = ((MiniEditor.Point)fig[p]).Y });
                    }
                    br = ((MiniEditor.Brush)fig["brush"]);
                    if (fig.Name == "Line")
                    {
                      CurrentCanvas.Polyline(C, br);
                    }

                    if (fig.Name == "Circle")
                    {
                        CurrentCanvas.Circle(C, br);
                    }

                    if (fig.Name == "Rectangle")
                    {
                        CurrentCanvas.Rectangle(C, br);
                    }
                }
            }
        }

    }
}
