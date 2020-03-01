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

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            this.WhenActivated(disposer => 
            {
                Add = ReactiveCommand.Create<Unit,Unit>(_ => {
                    ViewModel.Add.Execute(new MiniEditor.Line(
                    new MiniEditor.Point { X = 0, Y = 0 },
                    new MiniEditor.Point { X = 10, Y = 10 })).Subscribe();
                    return default;
                }).DisposeWith(disposer);
                Delete= ReactiveCommand.Create<Unit, Unit>(_ => {
                    ViewModel.Delete.Execute(ViewModel.AllFigures.FirstOrDefault()).Subscribe();
                    return default;
                }, ViewModel.Delete.CanExecute).DisposeWith(disposer);
                this.RaisePropertyChanged("Add");
                this.RaisePropertyChanged("Delete");
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
