using System.Globalization;

public class ScheduleManager
{
    private readonly ScheduleFetcher _scheduleFetcher;

    public ScheduleManager(ScheduleFetcher scheduleFetcher)
    {
        _scheduleFetcher = scheduleFetcher;
    }

    public string GetWeekdayByDate(string date)
    {
        var dateObj = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        return dateObj.ToString("dddd", new CultureInfo("ru-RU"));
    }

    public Dictionary<string, List<Dictionary<string, string>>> GetScheduleByDate(string date)
    {
        return _scheduleFetcher.FetchSchedule("/get-by-date", new Dictionary<string, string> { { "date_filter", date } });
    }

    public Dictionary<string, List<Dictionary<string, string>>> GetCurrentWeekSchedule()
    {
        var currentWeekDates = GetCurrentWeekDates();
        var currentWeekSchedule = new Dictionary<string, List<Dictionary<string, string>>>();

        foreach (var date in currentWeekDates)
        {
            var scheduleData = GetScheduleByDate(date);

            if (scheduleData != null && scheduleData.ContainsKey(date))
            {
                currentWeekSchedule[date] = scheduleData[date];
            }
            else
            {
                currentWeekSchedule[date] = new List<Dictionary<string, string>>(); // или можно оставить пустым, в зависимости от требований
            }
        }

        return currentWeekSchedule;
    }


    private List<string> GetCurrentWeekDates()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var dates = new List<string>();
        for (var i = 0; i < 7; i++)
        {
            dates.Add(startOfWeek.AddDays(i).ToString("yyyy-MM-dd"));
        }
        return dates;
    }
}