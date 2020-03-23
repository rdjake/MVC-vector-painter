using MiniEditor;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
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
            this.WhenActivated(disposer => 
            {

                Add = ReactiveCommand.Create<Unit,Unit>(_ => {
                    Random random = new Random();
                    var C = new MiniEditor.Point { X = random.Next(100,600), Y = random.Next(100,600) };
                    var C2 = new MiniEditor.Point { X = C.X + random.Next(-100,100), Y = C.Y + random.Next(-100, 100) };
                    var fig = new MiniEditor.Circle(C,C2);
                    ViewModel.Add.Execute(fig).Subscribe();

                    if (fig.Name == "Circle")
                    {
                        Ellipse el = new Ellipse();
                        var par = fig.Parameters.ToList();
                        MiniEditor.Point point1 = (MiniEditor.Point)(fig[par[0]]);
                        MiniEditor.Point point2 = (MiniEditor.Point)(fig[par[1]]);
                        el.Width = 2 * Math.Abs(point1.X - point2.X);
                        el.Height = 2 * Math.Abs(point1.Y - point2.Y);
                        el.Margin = new Thickness(point1.X/2, point1.Y/2, 0, 0);
                        el.Fill = Brushes.Green;
                        el.Stroke = Brushes.Red;
                        el.StrokeThickness = 3;
                        Holst.Children.Add(el);
                    }
                   
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    return default;
                }).DisposeWith(disposer);

                Delete= ReactiveCommand.Create<Unit, Unit>(_ => {
                    Random random = new Random();
                    var fig = ViewModel.AllFigures.Last();
                    ViewModel.Delete.Execute(fig).Subscribe();
                    this.NumberOfFigures.Content = ViewModel.AllFigures.Count();
                    Holst.Children.Clear();
                    foreach (var p in ViewModel.AllFigures)
                    {
                        if (p.Name == "Circle")
                        {
                            Ellipse el = new Ellipse();
                            var par = p.Parameters.ToList();
                            MiniEditor.Point point1 = (MiniEditor.Point)(p[par[0]]);
                            MiniEditor.Point point2 = (MiniEditor.Point)(p[par[1]]);
                            el.Width = 2 * Math.Abs(point1.X - point2.X);
                            el.Height = 2 * Math.Abs(point1.Y - point2.Y);
                            el.Margin = new Thickness(point1.X/2, point1.Y/2, 0, 0);
                            el.Fill = Brushes.Green;
                            el.Stroke = Brushes.Red;
                            el.StrokeThickness = 3;
                            Holst.Children.Add(el);
                        }
                    }
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
                    foreach (var p in ViewModel.AllFigures)
                    {
                        if (p.Name == "Circle")
                        {
                            Ellipse el = new Ellipse();
                            var par = p.Parameters.ToList();
                            MiniEditor.Point point1 = (MiniEditor.Point)(p[par[0]]);
                            MiniEditor.Point point2 = (MiniEditor.Point)(p[par[1]]);
                            el.Width = 2 * Math.Abs(point1.X - point2.X);
                            el.Height = 2 * Math.Abs(point1.Y - point2.Y);
                            el.Margin = new Thickness(point1.X/2, point1.Y/2, 0, 0);
                            el.Fill = Brushes.Green;
                            el.Stroke = Brushes.Red;
                            el.StrokeThickness = 3;
                            Holst.Children.Add(el);
                        }

                    }
                    return default;
                }).DisposeWith(disposer);

                this.RaisePropertyChanged("Add");
                this.RaisePropertyChanged("Delete");
                this.RaisePropertyChanged("SaveAll");
                this.RaisePropertyChanged("LoadAll");
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

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

    }
}
