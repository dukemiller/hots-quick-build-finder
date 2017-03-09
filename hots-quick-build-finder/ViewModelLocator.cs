using GalaSoft.MvvmLight.Ioc;
using hots_quick_build_finder.Repositories;
using hots_quick_build_finder.Repositories.Interfaces;
using hots_quick_build_finder.Services;
using hots_quick_build_finder.Services.Interfaces;
using hots_quick_build_finder.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace hots_quick_build_finder
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IHeroBuildService, IcyVeinsService>();
            SimpleIoc.Default.Register<ISettingsRepository>(SettingsRepository.Load);
            SimpleIoc.Default.Register<MainWindowViewModel>();
        }

        public static MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
    }
}
