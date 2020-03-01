using ReactiveUI;
using System;
using DynamicData;
using MiniEditor;
using System.Reactive;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Composition.Hosting;
using System.Reactive.Linq;
namespace ViewModel
{
    public class ViewModel:ReactiveObject
    {
        class ImportClass
        {
            [ImportMany]
            public IEnumerable<Lazy<IFigureDescriptor, FigureDescriptorMetadata>> AvailableFigures { get; set; }
        }
        static ImportClass importClass;
        SourceList<IFigure> Figures=new SourceList<IFigure>();
        ReadOnlyObservableCollection<IFigure> allFigures;
        public ReadOnlyObservableCollection<IFigure> AllFigures => allFigures;
        public IEnumerable<string> FigureTypes => importClass.AvailableFigures.Select(fig => fig.Metadata.Name);

        public int NumberOfParameters(string type)
        {
           return importClass.AvailableFigures.First(f => f.Metadata.Name == type).Value.NumberOfPoints;
        }
        public IFigure Create(string type,IEnumerable<Point> points)
        {
            return importClass.AvailableFigures.First(f => f.Metadata.Name == type).Value.Create(points);
        }

        public ReactiveCommand<IFigure,Unit> Add { get; }
        public ReactiveCommand<IFigure, Unit> Delete { get; }
        public ReactiveCommand<IGraphic, Unit> Draw { get; }
        static ViewModel()
        {
         System.Reflection.Assembly[] assemblies = { typeof(Point).Assembly};
                var conf = new ContainerConfiguration();
                try
                {
                    conf = conf.WithAssemblies(assemblies);
                }
                catch (Exception) { }

                var cont = conf.CreateContainer();
            importClass = new ImportClass();
                cont.SatisfyImports(importClass);
        }

        public ViewModel()
        {
            Figures.Connect().Bind(out allFigures);
            Add = ReactiveCommand.Create<IFigure, Unit>(
                fig => 
                {
                    Figures.Add(fig);
                    return default;
                });
            Delete = ReactiveCommand.Create<IFigure, Unit>(
                fig =>
                {
                    Figures.Remove(fig);
                    return default;
                },Figures.CountChanged.Select(i=>i>0));
            Draw = ReactiveCommand.Create<IGraphic, Unit>(graphic =>
            {
                foreach (var figure in Figures.Items)
                    figure.Draw(graphic);
                return default;
            });
        }
    }
}
