using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdventOfCodeUWP
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DayPage : Page
    {
        public ObservableCollection<DataModel> Data { get; } = new ObservableCollection<DataModel>();


        public DayPage()
        {
            InitializeComponent();
            Loaded += DayPage_Loaded;
        }

        private Type dayClassType;
        internal int TypeYear { get; private set; }
        internal int TypeDay { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            dayClassType = e.Parameter as Type;
            TypeYear = dayClassType?.GetCustomAttribute<AoCDayAttribute>()?.Year ?? 0;
            TypeDay = dayClassType?.GetCustomAttribute<AoCDayAttribute>()?.Day ?? 0;
        }
        private void DayPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UpdateTestDataFromDB();
        }


        private void UpdateTestDataFromDB()
        {
            var data = DataAccess.GetTestData(TypeYear, TypeDay);
            testDataCheckBoxPanel.Children.Clear();
            int index = 1;
            foreach (var d in data)
            {
                testDataCheckBoxPanel.Children.Add(new CheckBox { Content = $"Test data {index}", Tag = d });
            }
        }

        private void EditTestData_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => ShowDataControl();




        private void ShowDataControl()
        {
            var itemsToRemove = editDataPivotItem.Items.Where(it => (it as PivotItem).Tag != null).ToList();
            foreach (var i in itemsToRemove)
                editDataPivotItem.Items.Remove(i);

            dataEditWnd_LiveDataText.Text = DataAccess.GetLiveData(TypeYear, TypeDay).Content;
            foreach (var d in DataAccess.GetTestData(TypeYear, TypeDay))
            {
                editDataPivotItem.Items.Add(new PivotItem 
                {
                    Header = d.Name,
                    Tag = d,
                });
            }

            dataEditControl.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void EditData_Discard_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // do not save data
            Data.Clear();
            dataEditControl.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void EditData_Save_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // save data in data access

        }

        private void EditData_RedownloadLive_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void EditData_DeleteSelected_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void EditData_AddNew_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void CommandBar_Closing(object sender, object e) => ((CommandBar)sender).IsOpen = true;
    }
}
