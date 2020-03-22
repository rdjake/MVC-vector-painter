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
using System.IO;
using System.Diagnostics;

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
        public ReactiveCommand<IGraphic, Unit> SaveAll { get; }
        public ReactiveCommand<IGraphic, Unit> LoadAll { get; }
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
            Figures.Connect().Bind(out allFigures).Subscribe();

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
            //кол-во фигур
            // тип | количество параметров | тип параметра | значение | значение | ... | тип параметра | | значение | значение | ... 
            SaveAll = ReactiveCommand.Create<IGraphic, Unit>(_ =>
            {
                int i = 0;
                //подбор имени файла
                string path = Directory.GetCurrentDirectory() + @"\AllFigures_" + i + ".txt"; ;
                while (File.Exists(path)) {
                    i++;
                    path = Directory.GetCurrentDirectory() + @"\AllFigures_" + i + ".txt"; 
                } 

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(Figures.Count); //количество фигур
                    foreach (var figure in Figures.Items)
                    {
                        sw.Write(figure.Name + "\t"); //тип фигуры
                        sw.Write(figure.Parameters.Count()+ "\t"); //количество параметров
                        foreach (var p in figure.Parameters) {
                            if (figure[p].GetType().ToString() == "MiniEditor.Point") //если параметр - точка
                            {
                                sw.Write("MiniEditor.Point\t"); 
                                sw.Write(((MiniEditor.Point)figure[p]).X + "\t");
                                sw.Write(((MiniEditor.Point)figure[p]).Y + "\t");
                            }
                        }
                        sw.Write("\n");
                    }
                    sw.Close();
                }
                
                return default;
            },Figures.CountChanged.Select(i => i > 0));

            LoadAll = ReactiveCommand.Create<IGraphic, Unit>(_ =>
            {
                var file = Process.Start("explorer.exe", @"/n,/select," + Directory.GetCurrentDirectory());   
                return default;
            });
        }
    }
}
