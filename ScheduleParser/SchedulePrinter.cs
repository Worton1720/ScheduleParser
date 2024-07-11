using Newtonsoft.Json;

public static class SchedulePrinter
{
    public static void PrintSchedule(List<Dictionary<string, string>> scheduleData)
    {
        if (scheduleData != null)
        {
            foreach (var dateEntry in scheduleData)
            {
                Console.WriteLine($"Дата: {dateEntry["date"]}");
                Console.WriteLine($"Урок: {dateEntry["lesson"]}");
                Console.WriteLine($"Длительность пары: {dateEntry["started_at"]}|{dateEntry["finished_at"]}");
                Console.WriteLine($"Преподаватель: {dateEntry["teacher_name"]}");
                Console.WriteLine($"Предмет: {dateEntry["subject_name"]}");
                Console.WriteLine($"Аудитория: {dateEntry["room_name"]}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Расписание на указанный день отсутствует.");
        }
    }

    public static void SaveToJson(List<Dictionary<string, string>> scheduleData, string filename)
    {
        var json = JsonConvert.SerializeObject(scheduleData, Formatting.Indented);
        File.WriteAllText(filename, json);
    }
}