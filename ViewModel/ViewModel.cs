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
using Prism.Commands;

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
        public string error;
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
                Figures.Insert(0,fig);
                return default;
            });

            Delete = ReactiveCommand.Create<IFigure, Unit>(
            fig =>
            {
                Figures.Remove(fig);
                return default;
            }, Figures.CountChanged.Select(i => i > 0));

            Draw = ReactiveCommand.Create<IGraphic, Unit>(
            graphic =>
            {
                foreach (var figure in Figures.Items)
                    figure.Draw(graphic);
                return default;
            });

            //кол-во фигур
            // тип | количество параметров | тип параметра | значение | значение | ... | тип параметра | | значение | значение | ... 
            SaveAll = ReactiveCommand.Create<IGraphic, Unit>(_ =>
            {
                string path, pathList = Directory.GetCurrentDirectory() + @"\SaveList.txt";
                if (!File.Exists(pathList))
                    using (StreamWriter st = File.CreateText(pathList))
                    {
                        st.WriteLine("AllFigures_0.txt");
                        st.Close();
                        path = Directory.GetCurrentDirectory() + @"\AllFigures_0.txt";
                    }
                else
                {
                    string LastSave = File.ReadLines(pathList).Last();
                    using (StreamWriter st = File.AppendText(pathList))
                    {
                        //было бы проще узнать количество строк в файле, чтобы получить номер следующего файла
                        //если какое-то сохранение удалялось, то можно затереть существующее
                        int i = Int32.Parse(LastSave.Substring(11, (LastSave.Length - 15)));//11 - индекс числа, 15 - длинна без числа
                        path = "AllFigures_" + (i + 1) + ".txt";
                        st.WriteLine(path);
                        st.Close();
                        path = Directory.GetCurrentDirectory() + @"\" + path;
                    }
                }

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(Figures.Count); //количество фигур
                    foreach (var figure in Figures.Items)
                    {
                        sw.Write(figure.Name + "\t"); //тип фигуры
                        foreach (var p in figure.Parameters)
                        {
                            //если параметр - точка, вдруг еще какие то другие будут параметры 
                            if (figure[p].GetType().ToString() == "MiniEditor.Point") 
                            {
                                sw.Write(((MiniEditor.Point)figure[p]).X + "\t");
                                sw.Write(((MiniEditor.Point)figure[p]).Y + "\t");
                            }
                        }
                        sw.Write("\n");
                    }
                    sw.Close();
                }
                return default;
            }, Figures.CountChanged.Select(i => i > 0));

            //пока что загружает последнее сохранение
            LoadAll = ReactiveCommand.Create<IGraphic, Unit>(_ =>
            {
                string path, pathList = Directory.GetCurrentDirectory() + @"\SaveList.txt";
                if (!File.Exists(pathList)) error = "Нет сохранений";
                else
                {
                    string LastSave = File.ReadLines(pathList).Last();
                    path = Directory.GetCurrentDirectory() + @"\" + LastSave;
                    using (StreamReader sw = new StreamReader(path))
                    {
                        int count = Int32.Parse(sw.ReadLine()); //количество фигур
                        for (int i = 0; i < count; i++)
                        {
                            string[] figure = sw.ReadLine().Split("\t");
                            figure = figure.Take(figure.Count() - 1).ToArray();
                            string type = figure[0];
                            int param = NumberOfParameters(type); //количетсво параметров (встроеное)
                            int index = 1;
                            List<Point> Points = new List<Point>(); //считаем, что параметры только точки
                            for (int j = 0; j < param; j++) {
                                Points.Add(new Point { X = Int32.Parse(figure[index]), Y = Int32.Parse(figure[index + 1]) });
                                index += 2;
                            }
                            IFigure NewFigure = Create(type, Points);
                            Figures.Add(NewFigure);
                            }
                        sw.Close();

                        }
                    }
                return default;
            }) ;
        }
    }
}
