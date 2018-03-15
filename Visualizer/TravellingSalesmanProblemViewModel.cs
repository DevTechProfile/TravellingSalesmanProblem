using Combinatorics;
using PathHelper;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TspOptimizer;

namespace Visualizer
{
    public class TravellingSalesmanProblemViewModel : BindableBase
    {
        private List<Point> _currentPath;
        private List<Point> _initialPath;
        private string _plotTitle;
        private int[] _shuffledTour;
        private EuclideanPath _euclideanPath;
        private string _info;
        private CancellationTokenSource _tokenSource;
        private TspOptimizerAlgorithm _selectedOptimizer;
        private TspPathType _selectedPathType;

        public IEnumerable<TspOptimizerAlgorithm> OptimizerSet
        {
            get { return Enum.GetValues(typeof(TspOptimizerAlgorithm)).Cast<TspOptimizerAlgorithm>(); }
        }

        public IEnumerable<TspPathType> PathTypeSet
        {
            get { return Enum.GetValues(typeof(TspPathType)).Cast<TspPathType>(); }
        }

        public TspOptimizerAlgorithm SelectedOptimizer
        {
            get { return _selectedOptimizer; }
            set { _selectedOptimizer = value; RaisePropertyChanged(); }
        }

        public TspPathType SelectedPathType
        {
            get { return _selectedPathType; }
            set { _selectedPathType = value; RaisePropertyChanged(); }
        }

        public List<Point> CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath = value; RaisePropertyChanged(); }
        }

        public string PlotTitle
        {
            get { return _plotTitle; }
            set { _plotTitle = value; RaisePropertyChanged(); }
        }

        public string Info
        {
            get { return _info; }
            set { _info = value; RaisePropertyChanged(); }
        }

        public ICommand StartPathOptimizationCommand { get; }

        public ICommand StopPathOptimizationCommand { get; }

        public ICommand ShufflePathCommand { get; }

        public ICommand PathTypeChangedCommand { get; }

        public TravellingSalesmanProblemViewModel()
        {
            StartPathOptimizationCommand = new DelegateCommand(OnAlgorithmStart);
            StopPathOptimizationCommand = new DelegateCommand(OnAlgorithmStop);
            ShufflePathCommand = new DelegateCommand(OnShufflePath);
            PathTypeChangedCommand = new DelegateCommand(OnPathTypeChanged);

            SelectedPathType = TspPathType.Ciclre;
            SelectedOptimizer = TspOptimizerAlgorithm.RandomOptimizer;
        }

        private void OnPathTypeChanged()
        {
            var path = PathGeneratorFactory.Create(SelectedPathType, new Size(10, 10), 18);

            if (path == null)
            {
                Info = "Path generator not yet implemented :-(";
                return;
            }

            _initialPath = path.Select(coordinate => coordinate.To2DPoint()).ToList();
            _euclideanPath = new EuclideanPath(_initialPath);
            _shuffledTour = Enumerable.Range(0, _initialPath.Count).ToArray();

            PlotTour(Enumerable.Range(0, _initialPath.Count).ToArray());
        }

        private void OnAlgorithmStop() => _tokenSource.Cancel();

        private void OnShufflePath()
        {
            _shuffledTour = Enumerable.Range(0, _initialPath.Count).ToArray();
            Helper.Shuffle(_shuffledTour);
            PlotTour(_shuffledTour);
        }

        private void OnAlgorithmStart()
        {
            ITspOptimizer optimizer = TspOptimizerFactory.Create(SelectedOptimizer, _shuffledTour, _euclideanPath);

            if (optimizer == null)
            {
                Info = "Algorithm not yet implemented :-(";
                return;
            }

            optimizer.OptimalSequence.Subscribe(PlotTour);

            _tokenSource = new CancellationTokenSource();
            CancellationToken token = _tokenSource.Token;

            var context = TaskScheduler.FromCurrentSynchronizationContext();

            Info = "Optimizer started...";
            Task task = Task.Factory.StartNew(() =>
            {
                optimizer.Start(token);
            }, token).ContinueWith((t) =>
            {
                Info = "Optimizer canceled...";
            }, context);
        }

        private void PlotTour(int[] tour)
        {
            CurrentPath = new List<Point>(tour.Select(i => _initialPath[i]));
        }
    }
}
