using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main()
    {
        string url = "https://msapi.top-academy.ru/api/v2";
        Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "authorization", "" },
            { "referer", "https://journal.top-academy.ru/" }
        };

        string refresh_token = LoadRefreshTokenFromDB();

        if (string.IsNullOrEmpty(refresh_token) || !CheckTokenValidity(url, headers))
        {
            Console.Write("Введите ваш логин: ");
            string username = Console.ReadLine().Trim();
            string password = GetPassword("Введите ваш пароль(пароль скрыт при вводе): ").Trim();

            refresh_token = GetRefreshToken(username, password);
            if (!string.IsNullOrEmpty(refresh_token))
            {
                SaveRefreshTokenToDB(refresh_token);
            }
            else
            {
                Console.WriteLine("Не удалось получить refresh token. Пожалуйста, повторите попытку.");
                return;
            }
        }

        headers["authorization"] = $"Bearer {refresh_token}";

        var httpClient = new HttpClient();
        ScheduleFetcher scheduleFetcher = new ScheduleFetcher(httpClient, url, headers);
        ScheduleManager scheduleManager = new ScheduleManager(scheduleFetcher, headers);

        Console.Write("Введите дату (гггг-мм-дд): ");
        string inputDate = Console.ReadLine().Trim();

        List<Dictionary<string, string>> scheduleForDay = scheduleManager.GetScheduleForDay(inputDate);

        if (scheduleForDay != null)
        {
            SchedulePrinter.PrintSchedule(scheduleForDay);
            SchedulePrinter.SaveToJson(scheduleForDay, $"schedule_{inputDate}.json");
            Console.WriteLine($"Расписание на {inputDate} успешно выведено и сохранено.");
        }
        else
        {
            Console.WriteLine($"Не удалось получить расписание на {inputDate}.");
        }
    }

    static string LoadRefreshTokenFromDB()
    {
        // Реализуйте метод загрузки refresh_token из базы данных
        // Здесь может быть логика подключения к базе данных и выполнения запроса
        // Пример:
        // return YourDatabaseManager.LoadRefreshToken();
        return null;
    }

    static void SaveRefreshTokenToDB(string refresh_token)
    {
        // Реализуйте метод сохранения refresh_token в базу данных
        // Здесь может быть логика подключения к базе данных и выполнения запроса
        // Пример:
        // YourDatabaseManager.SaveRefreshToken(refresh_token);
    }

    static bool CheckTokenValidity(string url, Dictionary<string, string> headers)
    {
        try
        {
            // Проверяем релевантность refresh_token
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var response = client.SendAsync(request).Result;
                return response.IsSuccessStatusCode;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при проверке токена: {ex.Message}");
            return false;
        }
    }

    static string GetPassword(string prompt)
    {
        Console.Write(prompt);
        var password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, (password.Length - 1));
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    public class LoginResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in_refresh { get; set; }
        public int expires_in_access { get; set; }
        public int user_type { get; set; }
        public CityData city_data { get; set; }
        public string user_role { get; set; }
    }

    public class CityData
    {
        public int id_city { get; set; }
        public string prefix { get; set; }
        public string translate_key { get; set; }
        public string timezone_name { get; set; }
        public string country_code { get; set; }
        public int market_status { get; set; }
        public string name { get; set; }
    }


    static string GetRefreshToken(string username, string password)
    {
        var urlLogin = "https://msapi.top-academy.ru/api/v2/auth/login";
        var payload = new
        {
            application_key = "6a56a5df2667e65aab73ce76d1dd737f7d1faef9c52e8b8c55ac75f565d8e8a6",
            id_city = (object)null,
            username = username,
            password = password
        };
        var headersLogin = new Dictionary<string, string>
    {
        { "Referer", "https://journal.top-academy.ru/" }
    };

        using (var client = new HttpClient())
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, urlLogin);

                foreach (var header in headersLogin)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;
                var responseData = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = JObject.Parse(responseData);

                    if (responseObject["refresh_token"] != null)
                    {
                        return responseObject["refresh_token"].ToString();
                    }
                    else
                    {
                        Console.WriteLine("Refresh token не найден в ответе.");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка при отправке запроса: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Произошла ошибка при отправке запроса: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
            }
        }

        return null;
    }
}