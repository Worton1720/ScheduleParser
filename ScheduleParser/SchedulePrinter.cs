using Newtonsoft.Json;

public static class SchedulePrinter
{
    public static void PrintSchedule(Dictionary<string, List<Dictionary<string, string>>> scheduleData)
    {
        if (scheduleData != null)
        {
            foreach (var dateEntry in scheduleData)
            {
                Console.WriteLine($"Дата: {dateEntry.Key}");
                foreach (var entry in dateEntry.Value)
                {
                    Console.WriteLine($"Урок: {entry["lesson"]}");
                    Console.WriteLine($"Длительность пары: {entry["started_at"]}|{entry["finished_at"]}");
                    Console.WriteLine($"Преподаватель: {entry["teacher_name"]}");
                    Console.WriteLine($"Предмет: {entry["subject_name"]}");
                    Console.WriteLine($"Аудитория: {entry["room_name"]}");
                    Console.WriteLine();
                }
            }
        }
        else
        {
            Console.WriteLine("Расписание на указанный день отсутствует.");
        }
    }

    public static void SaveToJson(Dictionary<string, List<Dictionary<string, string>>> scheduleData, string filename)
    {
        var json = JsonConvert.SerializeObject(scheduleData, Formatting.Indented);
        File.WriteAllText(filename, json);
    }
}