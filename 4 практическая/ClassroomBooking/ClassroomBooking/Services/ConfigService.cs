using System;
using System.IO;
using ClassroomBooking.Configuration;

namespace ClassroomBooking.Services
{
    public class ConfigService
    {
        public AppSettings Load()
        {
        
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            
            if (!File.Exists(path))
                return new AppSettings();

            try
            {
                
                string[] lines = File.ReadAllLines(path);
                AppSettings settings = new AppSettings();

                
                foreach (string line in lines)
                {
                    if (!line.Contains(":")) continue;

                    string[] parts = line.Split(new char[] { ':' }, 2);
                    string key = parts[0].Trim(new char[] { ' ', '"', ',', '{', '}' });
                    string value = parts[1].Trim(new char[] { ' ', '"', ',', '{', '}' });

                    if (key == "AppTitle") settings.AppTitle = value;
                    else if (key == "WorkDayStart") settings.WorkDayStart = value;
                    else if (key == "WorkDayEnd") settings.WorkDayEnd = value;
                    else if (key == "StatusMessage") settings.StatusMessage = value;
                    else if (key == "MinBookingMinutes") { int.TryParse(value, out int min); settings.MinBookingMinutes = min; }
                    else if (key == "AllowUnavailableRooms") { bool.TryParse(value, out bool allow); settings.AllowUnavailableRooms = allow; }
                }
                return settings;
            }
            catch
            {
               
                return new AppSettings();
            }
        }
    }
}
