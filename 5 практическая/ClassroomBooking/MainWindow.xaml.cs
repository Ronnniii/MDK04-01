using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows;

namespace ClassroomBooking
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Booking> bookings = new ObservableCollection<Booking>();
        private Settings settings = new Settings();

        public MainWindow()
        {
            InitializeComponent();
            BookingsGrid.ItemsSource = bookings;
            DateBox.SelectedDate = DateTime.Today;
            LoadSettings();
            foreach (string room in settings.Rooms) RoomBox.Items.Add(room);
            if (RoomBox.Items.Count > 0) RoomBox.SelectedIndex = 0;
        }

        private void LoadSettings()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!File.Exists(path)) throw new FileNotFoundException("Файл appsettings.json не найден");
                var loaded = new JavaScriptSerializer().Deserialize<Settings>(File.ReadAllText(path));
                if (loaded == null || loaded.Rooms == null || loaded.Rooms.Length == 0 ||
                    !TimeSpan.TryParse(loaded.OpenTime, out _) || !TimeSpan.TryParse(loaded.CloseTime, out _))
                    throw new FormatException("Некорректные настройки");
                settings = loaded;
                StatusMessage.Text = "Настройки загружены из appsettings.json";
            }
            catch (Exception ex)
            {
                settings = new Settings();
                StatusMessage.Text = "Стандартные настройки: " + ex.Message;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string user = UserBox.Text.Trim();
            if (user.Length == 0 || RoomBox.SelectedItem == null || !DateBox.SelectedDate.HasValue)
            { MessageBox.Show("Заполните пользователя, аудиторию и дату."); return; }
            TimeSpan start, end, open, close;
            if (!TimeSpan.TryParseExact(StartBox.Text.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out start) ||
                !TimeSpan.TryParseExact(EndBox.Text.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out end) ||
                !TimeSpan.TryParse(settings.OpenTime, out open) || !TimeSpan.TryParse(settings.CloseTime, out close))
            { MessageBox.Show("Укажите время в формате ЧЧ:ММ."); return; }
            if (start >= end || start < open || end > close)
            { MessageBox.Show("Проверьте время. Доступно с " + settings.OpenTime + " до " + settings.CloseTime); return; }
            DateTime date = DateBox.SelectedDate.Value.Date;
            string room = RoomBox.SelectedItem.ToString();
            if (bookings.Any(b => b.Room == room && b.Date == date && start < b.EndTime && end > b.StartTime))
            { MessageBox.Show("Эта аудитория уже занята в указанное время."); return; }
            bookings.Add(new Booking { User = user, Room = room, Date = date, StartTime = start, EndTime = end });
            StatusMessage.Text = "Бронирование создано. Всего: " + bookings.Count;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selected = BookingsGrid.SelectedItem as Booking;
            if (selected != null) { bookings.Remove(selected); StatusMessage.Text = "Бронирование удалено"; }
        }
    }

    public class Settings
    {
        public string[] Rooms { get; set; } = { "Аудитория 101", "Аудитория 102", "Аудитория 203" };
        public string OpenTime { get; set; } = "08:00";
        public string CloseTime { get; set; } = "20:00";
    }

    public class Booking
    {
        public string User { get; set; }
        public string Room { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Start { get { return StartTime.ToString(@"hh\:mm"); } }
        public string End { get { return EndTime.ToString(@"hh\:mm"); } }
    }
}
