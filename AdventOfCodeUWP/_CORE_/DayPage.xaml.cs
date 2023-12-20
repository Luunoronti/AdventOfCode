using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdventOfCodeUWP
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DayPage : Page
    {
        public DayPage()
        {
            this.InitializeComponent();
        }

        private void EditTestData_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var data = DataAccess.GetTestData(2023, 20);
            var live = DataAccess.GetLiveData(2023, 20);
            foreach(var d in data)
            {

            }

            // test data for this day will contain:
            // date (year) and date (day)
            // number of test (0...10) (we support maximum of 10 tests per day)
            // the contents of test (string)
            // expected value (in string form, because of BigInt)
            // is this test applicable to Part1, Part 2 or both?




        }
    }
}
