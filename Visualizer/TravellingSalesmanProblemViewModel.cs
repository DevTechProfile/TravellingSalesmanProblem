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

        public IEnumerable<TspOptimizerAlgorithm> OptimizerSet
        {
            get { return Enum.GetValues(typeof(TspOptimizerAlgorithm)).Cast<TspOptimizerAlgorithm>(); }
        }

        public TspOptimizerAlgorithm SelectedOptimizer
        {
            get { return _selectedOptimizer; }
            set { _selectedOptimizer = value; RaisePropertyChanged(); }
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

        public TravellingSalesmanProblemViewModel()
        {
            StartPathOptimizationCommand = new DelegateCommand(OnAlgorithmStart);
            StopPathOptimizationCommand = new DelegateCommand(OnAlgorithmStop);
            ShufflePathCommand = new DelegateCommand(OnShufflePath);

            _initialPath = PathGenerator.GetCirclePath(20d, 10d).Path;
            _euclideanPath = new EuclideanPath(_initialPath);
            _shuffledTour = Enumerable.Range(0, _initialPath.Count).ToArray();

            SelectedOptimizer = TspOptimizerAlgorithm.RandomOptimizer;

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
            Info = "Optimizer started...";

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
