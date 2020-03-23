using MiniEditor;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Drawing;
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
    public partial class MainWindow : Window,IViewFor<ViewModel.ViewModel>,IReactiveObject
    {
        public ReactiveCommand<Unit,Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ReactiveCommand<Unit, Unit> Draw { get; set; }
        public ReactiveCommand<Unit, Unit> SaveAll { get; set; }
        public ReactiveCommand<Unit, Unit> LoadAll { get; set; }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            IGraphic graphic; //Задел под нашу графику
            Graphics gra;
            this.WhenActivated(disposer => 
            {

                Add = ReactiveCommand.Create<Unit,Unit>(_ => {
                    Random random = new Random();
                    ViewModel.Add.Execute(new MiniEditor.Circle(
                    new MiniEditor.Point { X = random.Next(300), Y = random.Next(300) },
                    new MiniEditor.Point { X = random.Next(300), Y = random.Next(300) })).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    return default;
                }).DisposeWith(disposer);

                Delete= ReactiveCommand.Create<Unit, Unit>(_ => {
                    ViewModel.Delete.Execute(ViewModel.AllFigures.FirstOrDefault()).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    return default;
                }, ViewModel.Delete.CanExecute).DisposeWith(disposer);

                SaveAll = ReactiveCommand.Create<Unit, Unit>(_ => {
                    ViewModel.SaveAll.Execute().Subscribe();
                    return default;
                }, ViewModel.SaveAll.CanExecute).DisposeWith(disposer);

                LoadAll = ReactiveCommand.Create<Unit, Unit>(_ => {
                    ViewModel.LoadAll.Execute().Subscribe();
                    this.Error.Content = ViewModel.error;
                    return default;
                }).DisposeWith(disposer);

                

             //< Button Content = "Circle" Command = "{Binding LoadAll}" Width = "55" Canvas.Left = "250" />
             //< Button Content = "Line" Command = "{Binding LoadAll}" Width = "55" Canvas.Left = "300" />
             //< Button Content = "Triangle" Command = "{Binding LoadAll}" Width = "55" Canvas.Left = "350" />
             //< Button Content = "Polygon" Command = "{Binding LoadAll}" Width = "55" Canvas.Left = "400" />

                this.RaisePropertyChanged("Add");
                this.RaisePropertyChanged("Delete");
                this.RaisePropertyChanged("SaveAll");
                this.RaisePropertyChanged("LoadAll");
                this.RaisePropertyChanged("Circle");
                this.RaisePropertyChanged("Line");
                this.RaisePropertyChanged("Triangle");
                this.RaisePropertyChanged("Polygon");
            });
            
        }
        ViewModel.ViewModel viewModel=new ViewModel.ViewModel();
        public ViewModel.ViewModel ViewModel { get=>viewModel; set { } }
        object IViewFor.ViewModel { get=> ViewModel; set=> ViewModel=(ViewModel.ViewModel)value; }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging.Invoke(this, args);
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged.Invoke(this, args);
        }
        
        //Нажатие на левую кнопку мыши
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = Mouse.GetPosition(MainCanvas);
            Errorbox.Text = "LeftMousePressed" + "\n" +
               "X: " + position.X +
               "\n" +
               "Y: " + position.Y;
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
            //LeftDrag
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point position = Mouse.GetPosition(MainCanvas);
                Errorbox.Text = "LeftMouseDrag" + "\n" +
               "X: " + position.X +
               "\n" +
               "Y: " + position.Y;
            }
            //RightDrag
            if (e.RightButton == MouseButtonState.Pressed)
            {
                System.Windows.Point position = Mouse.GetPosition(MainCanvas);
                Errorbox.Text = "RightMouseDrag" + "\n" +
               "X: " + position.X +
               "\n" +
               "Y: " + position.Y;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

    }
}
