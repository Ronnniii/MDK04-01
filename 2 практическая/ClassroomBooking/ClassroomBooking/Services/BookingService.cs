using System.Collections.Generic;
using System.Linq;
using ClassroomBooking.Models;

namespace ClassroomBooking.Services
{
    public class BookingService
    {
        private readonly List<Booking> bookings = new List<Booking>();

        public bool AddBooking(Booking booking)
        {
            // Бизнес-проверка: аудитория, дата совпадают, а время пересекается
            bool conflict = bookings.Any(x =>
                x.RoomId == booking.RoomId &&
                x.Date.Date == booking.Date.Date &&
                x.StartTime < booking.EndTime &&
                booking.StartTime < x.EndTime);

            if (conflict)
            {
                return false; // Найдено пересечение времени, отклоняем бронь
            }

            booking.Id = bookings.Count + 1;
            bookings.Add(booking);
            return true; // Конфликтов нет, успешно добавлено
        }

        public List<Booking> GetBookings()
        {
            return bookings;
        }
    }
}
