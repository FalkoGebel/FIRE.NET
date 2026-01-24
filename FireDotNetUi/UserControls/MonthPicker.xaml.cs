using System.Windows;
using System.Windows.Controls;

namespace FireDotNetUi.UserControls
{
    /// <summary>
    /// Interaktionslogik für MonthPicker.xaml
    /// </summary>
    public partial class MonthPicker : UserControl
    {
        public MonthPicker()
        {
            InitializeComponent();
        }

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);

            set
            {
                SetValue(SelectedDateProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate",
                                                                                                    typeof(DateTime),
                                                                                                    typeof(MonthPicker));

        public int TextBoxWidth
        {
            get => (int)GetValue(TextBoxWidthProperty);
            set => SetValue(TextBoxWidthProperty, value);
        }

        public static readonly DependencyProperty TextBoxWidthProperty = DependencyProperty.Register("TextBoxWidth",
                                                                                                    typeof(int),
                                                                                                    typeof(MonthPicker));

        private void MonthPickerCalendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            if (e.OldMode == CalendarMode.Year && e.NewMode != CalendarMode.Decade)
            {
                CalendarPopup.IsOpen = false;
                SelectedDate = new DateTime(MonthPickerCalendar.DisplayDate.Year, MonthPickerCalendar.DisplayDate.Month, 1);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MonthPickerCalendar.DisplayMode = CalendarMode.Year;
            MonthPickerCalendar.DisplayDate = SelectedDate;
            CalendarPopup.IsOpen = true;
        }

        private void MonthPickerCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            if (MonthPickerCalendar.ActualWidth == 0)
                return;

            MonthPickerCalendar.DisplayMode = CalendarMode.Year;
        }
    }
}