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

        private string CurrentFigureName = "Empty";
        private IGraphic CurrentFigure;
        private List<Button> Buttons = new List<Button>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            this.WhenActivated(disposer =>
            {

                Add = ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    var C = new MiniEditor.Point { X = random.Next(100, 600), Y = random.Next(100, 600) };
                    var C2 = new MiniEditor.Point { X = C.X + random.Next(-100, 100), Y = C.Y + random.Next(-100, 100) };
                    var fig = new MiniEditor.Circle(C, C2);
                    ViewModel.Add.Execute(fig).Subscribe();
                    DrawAll();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    return default;
                }).DisposeWith(disposer);

                Delete = ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    var fig = ViewModel.AllFigures.FirstOrDefault();
                    ViewModel.Delete.Execute(fig).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    Holst.Children.Clear();
                    DrawAll();
                    return default;
                }, ViewModel.Delete.CanExecute).DisposeWith(disposer);

                SaveAll = ReactiveCommand.Create<Unit, Unit>(_ => {
                    ViewModel.SaveAll.Execute().Subscribe();
                    return default;
                }, ViewModel.SaveAll.CanExecute).DisposeWith(disposer);

                LoadAll = ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    ViewModel.LoadAll.Execute().Subscribe();
                    this.Error.Content = ViewModel.error;
                    DrawAll();
                    return default;
                }).DisposeWith(disposer);



                this.RaisePropertyChanged("Add");
                this.RaisePropertyChanged("Delete");
                this.RaisePropertyChanged("SaveAll");
                this.RaisePropertyChanged("LoadAll");
                this.RaisePropertyChanged("Circle");
                this.RaisePropertyChanged("Line");
                this.RaisePropertyChanged("Polygon");
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
                case "Polygon": { break; }
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
            CurrentFigureName = "Polygon";
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Holst.Children.Clear();
                Errorbox.Text = "LeftMouseDrag" + "\n" +
               "X: " + position.X +
               "\n" +
               "Y: " + position.Y;
                Mousepos2 = position;
                BuildFigure bu = new BuildFigure();
                MiniEditor.Point tmp1, tmp2;
                tmp1.X = Mousepos1.X;
                tmp1.Y = Mousepos1.Y;
                tmp2.X = Mousepos2.X;
                tmp2.Y = Mousepos2.Y;
                var fig = new MiniEditor.Circle(tmp1, tmp2);
                ViewModel.Add.Execute(fig).Subscribe();
                DrawAll();

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

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private void DeteleFromList(object sender, MouseButtonEventArgs e)
        {
            ListView temp = sender as ListView;
            var item = temp.SelectedItem as MiniEditor.IFigure;
            ViewModel.Delete.Execute(item).Subscribe();
            ViewModel.Add.Execute(item).Subscribe();
            Holst.Children.Clear();
            DrawAll();
        }

        public void DrawAll()
        {
            var Figures = ViewModel.AllFigures.Reverse().ToArray();
            foreach (var p in Figures)
            {
                if (p.Name == "Circle")
                {
                    Ellipse el = new Ellipse();
                    var par = p.Parameters.ToList();
                    MiniEditor.Point point1 = (MiniEditor.Point)(p[par[0]]);
                    MiniEditor.Point point2 = (MiniEditor.Point)(p[par[1]]);
                    el.Width = 2 * Math.Abs(point1.X - point2.X);
                    el.Height = 2 * Math.Abs(point1.Y - point2.Y);
                    el.Margin = new Thickness(point1.X / 2, point1.Y / 2, 0, 0);
                    el.Fill = Brushes.Green;
                    el.Stroke = Brushes.Red;
                    el.StrokeThickness = 3;
                    Holst.Children.Add(el);
                }
                

            }
        }

    }
}
