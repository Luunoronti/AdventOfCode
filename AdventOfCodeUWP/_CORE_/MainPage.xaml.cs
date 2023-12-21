using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdventOfCodeUWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            navView.ItemInvoked += NavView_ItemInvoked;

            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;


            ReloadNavigationPane();
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
            }
            else
            {
                frame.Navigate(typeof(DayPage), args.InvokedItemContainer?.Tag);
            }
        }

        private void ReloadNavigationPane(bool navigateToCurrentDay = true)
        {
            var year = 2023; //TODO: get from settings later

            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Select(t => (type: t, dayAtt: t.GetCustomAttribute<AoCDayAttribute>()))
                .Where(t => t.dayAtt != null)
                .Where(t => t.dayAtt.Year == year)
                .ToList();

            types.Sort((a, b) => b.dayAtt.Day - a.dayAtt.Day);

            foreach (var type in types)
            {
                var item = new NavigationViewItem() { Content = type.dayAtt.Day.ToString(), Tag = type.type };
                navView.MenuItems.Add(item);
                if(navigateToCurrentDay && DateTime.Now.Year == year && DateTime.Now.Day == type.dayAtt.Day)
                {
                    navView.SelectedItem = item;
                    frame.Navigate(typeof(DayPage), type.type);
                }
            }


        }

    }

   

}
