using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using hots_quick_build_finder.Classes;
using hots_quick_build_finder.Models;
using hots_quick_build_finder.Repositories.Interfaces;
using hots_quick_build_finder.Services.Interfaces;

namespace hots_quick_build_finder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string ApplicationPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hots_quick_build_finder");

        private readonly IHeroBuildService _heroService;

        private readonly ISettingsRepository _settingsRepo;

        private ObservableCollection<Build> _builds;

        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        private string _heroName;

        private ObservableCollection<string> _heroNames;

        private bool _isLoadingData;

        private Build _selectedBuild;

        private int _currentIndex;

        private int _maxIndex;

        //

        public MainWindowViewModel(IHeroBuildService heroService, ISettingsRepository settingsRepo)
        {
            _heroService = heroService;
            _settingsRepo = settingsRepo;

            FindCommand = new RelayCommand(Find);
            NextCommand = new RelayCommand(Next);
            PreviousCommand = new RelayCommand(Previous);
            RefreshCommand = new RelayCommand(Refresh);
            WebPageCommand = new RelayCommand(Webpage);
            UpdateCommand = new RelayCommand(Update);
            // LoadedCommand = new RelayCommand(CreateTray);

            RegisterService();

            if (IsInDesignMode)
            {
                HeroName = "Arthas";
                Find();
            }
        }

        //

        public RelayCommand FindCommand { get; set; }

        public RelayCommand NextCommand { get; set; }

        public RelayCommand PreviousCommand { get; set; }
        
        public RelayCommand RefreshCommand { get; set; }

        public RelayCommand WebPageCommand { get; set; }

        public RelayCommand LoadedCommand { get; set; }

        public RelayCommand UpdateCommand { get; set; }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { Set(() => CurrentIndex, ref _currentIndex, value); }
        }

        public int MaxIndex
        {
            get { return _maxIndex; }
            set { Set(() => MaxIndex, ref _maxIndex, value); }
        }

        public string Page => Builds?.Count > 0 ? $"{CurrentIndex+1}/{MaxIndex}" : "";

        public Build SelectedBuild
        {
            get { return _selectedBuild; }
            set
            {
                Set(() => SelectedBuild, ref _selectedBuild, value);
                RaisePropertyChanged("Page");
            }
        }

        public ObservableCollection<Build> Builds
        {
            get { return _builds; }
            set { Set(() => Builds, ref _builds, value); }
        }

        public ObservableCollection<string> HeroNames
        {
            get { return _heroNames; }
            set { Set(() => HeroNames, ref _heroNames, value); }
        }
        
        public string HeroName
        {
            get { return _heroName; }
            set
            {
                Set(() => HeroName, ref _heroName, TextInfo.ToTitleCase(value));
                if (value.Length == 0)
                {
                    Builds = new ObservableCollection<Build>();
                    SelectedBuild = null;
                    CurrentIndex = 0;
                    MaxIndex = 0;
                }
            }
        }

        public bool IsLoadingData
        {
            get { return _isLoadingData; }
            set { Set(() => IsLoadingData, ref _isLoadingData, value); }
        }

        //

        private async void RegisterService()
        {
            // Retrieve initial list
            if (!_settingsRepo.RegisteredBuildService.Equals(_heroService.GetType().Name.Replace("Service", "")))
            {
                IsLoadingData = true;
                _settingsRepo.RegisteredBuildService = _heroService.GetType().Name.Replace("Service", "");
                _settingsRepo.Heroes = await _heroService.HeroList();
                _settingsRepo.Save();
                IsLoadingData = false;
            }

            // Update existing hero list (Check every 30 days)
            else if ((DateTime.Now - _settingsRepo.LastUpdated).TotalDays >= 30)
                await UpdateHerolist();

            HeroNames = new ObservableCollection<string>(_settingsRepo.Heroes.Select(hero => hero.Name));
        }

        private void Webpage()
        {
            var hero = _heroService.HeroPage(HeroName);
            if (hero != null)
                Process.Start(hero);
        }

        private async void Refresh()
        {
            await _heroService.Refresh(HeroName, loading => IsLoadingData = loading);
        }

        private void Next()
        {
            if (Builds?.Count > 0)
            {
                CurrentIndex = Math.Min(CurrentIndex + 1, MaxIndex - 1);
                SelectedBuild = Builds[CurrentIndex];
            }
        }

        private void Previous()
        {
            if (Builds?.Count > 0)
            {
                CurrentIndex = Math.Max(CurrentIndex - 1, 0);
                SelectedBuild = Builds[CurrentIndex];
            }
        }

        private async Task UpdateHerolist()
        {
            IsLoadingData = true;
            var heroList = await _heroService.HeroList();
            var newHeroes = heroList.Where(newHero => !_settingsRepo.Heroes.Any(hero => hero.Name.Equals(newHero.Name))).ToList();
            if (newHeroes.Any())
            {
                foreach (var hero in newHeroes)
                    _settingsRepo.Heroes.AddSorted(hero);
                _settingsRepo.LastUpdated = DateTime.Today;
                _settingsRepo.Save();
            }
            IsLoadingData = false;
        }

        private async void Update()
        {
            if ((DateTime.Now - _settingsRepo.LastUpdated).TotalDays >= 3)
                await UpdateHerolist();
        }

        private async void Find()
        {
            if (HeroName?.Length > 0)
            {
                IEnumerable<Build> builds;

                try
                {
                    builds = await _heroService.BuildsFor(HeroName, loading => IsLoadingData = loading);
                }

                catch
                {
                    builds = null;
                    IsLoadingData = false;
                }

                if (builds == null)
                    HeroName = "";

                else
                {
                    Builds = new ObservableCollection<Build>(builds);
                    CurrentIndex = 0;
                    MaxIndex = Builds.Count;
                    SelectedBuild = Builds[CurrentIndex];
                }
            }
        }
    }
}