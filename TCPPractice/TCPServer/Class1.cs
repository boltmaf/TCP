using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Media;
using System.Linq;

namespace TCPServer
{
    public class ClientObject
    {
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;
            NetworkStream stream1 = null; // for sound
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[256]; // буфер для получаемых данных

                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);

                    message = message.Substring(message.IndexOf(':') + 1); // Убирает имя пользователя для более точного определения команды

                    message = AI(message); /* // TODO; смотреь 1. кости со звуком
                2.подброс монетки 3.ролл(нахуй нужен если кости будут)
                4.мб перевод бабок из евро и из доллара в рубли и наоборот(хуй знает как реализовать вообще)
                5.анекдотик травануть рандомный
                6.таймер со временем */
                    Console.WriteLine(message);
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
        static string AI(string _message)
        {
            string message = _message;

            if (message.Contains("Помощь") || message.Contains("помощь") || message.Contains("Помогите") || message.Contains("помогите"))
            {
                message = CMD();
                return message;
            }

            if (message.Contains("Погода") || message.Contains("погода"))
            {
                message = AI_Weather(message);
                return message;
            }

            if (message.Contains("Время") || message.Contains("время") || message.Contains("Дата") || message.Contains("дата"))
            {
                message = DateTime();
                return message;
            }

            if (message.Contains("Кости") || message.Contains("кости") || message.Contains("Игра в кости") || message.Contains("игра в кости"))
            {
                message = Dice(message);
                return message;
            }

            if (message.Contains("Анекдот") || message.Contains("Шутка") || message.Contains("анекдот") || message.Contains("шутка"))
            {
                message = Humor();
                return message;
            }

            if (message.Contains("Я хачу пиццы") || message.Contains("я хачу пиццы"))
            {
                message = "Я тоже хачу";
                return message;
            }
            if (message.Contains("Как дела?") || message.Contains("как дела?"))
            {
                message = "Круто!";
                return message;
            }
            if (message.Contains("Кем ты создан?") || message.Contains("кем ты создан?"))
            {
                message = "Двумя программистами - Артуром Ивановым и Ирой Косых";
                return message;
            }
            if (message.Contains("Сколько тебе лет?") || message.Contains("сколько тебе лет?"))
            {
                message = "Мне 11110 лет!";
                return message;
            }
            if (message.Contains("Разве искусственный интелект сделает из обычного листа шедевр искусства?") || message.Contains("разве искусственный интелект сделает из обычного листа шедевр искусства?"))
            {
                message = "Абоба";
                return message;
            }
            return message = "Я не понимаю тебя";
        }
        static string CMD()
        {
            string message;

            return message = "Список доступных команд: Погода - показывает текущую погоду в Казани \\ Время - показывает текущее время \n" +
                "Кости - игра в кости(вам будет предложено угадать число, которое получится в сумме от подброса 2-ух костей)\n" +
                "Важно написать боту в следующем формате: Кости {ваше число}\n" +
                "Шутка - увидеть случайный анекдот\n";
        }
        static string AI_Weather(string _message)
        {
            string message = _message;
            string response;
            string url = "http://api.openweathermap.org/data/2.5/weather?q=Kazan&units=metric&appid=d821bc49ba332d568b7f194dd04a7064";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);
            message = "Погода в " + Convert.ToString(weatherResponse.Name) + " составляет " + Convert.ToString(weatherResponse.Main.Temp) + " °C";
            return message;
        }
        static string DateTime()
        {
            string message;
            DateTime DateTime;
            message = DateTime.Now.ToString();
            return message;
        }

        static string Dice(string _message) // Кости
        {
            string message = _message;
            int value;
            Random rand = new Random();
            int rand1;

            int.TryParse(string.Join("", message.Where(c => char.IsDigit(c))), out value); // Убирает всё кроме цифр
            rand1 = rand.Next(1, 13);
            if (value > 0 && value < 13)
            {
                if (value == rand1)
                {
                    return message = "Поздравляем, вы выиграли! Числа совпали:" + value + " и " + rand1;
                }
                else return message = "К сожалению, вы проиграли. Числа не совпали: " + value + " и " + rand1;
            }
            else
            {
                return message = "Вы указали цифру <1 или >12";
            }
        }

        static string Humor()
        {
            string message;
            Random rand = new Random();
            string[] Humors = File.ReadAllLines(@"Humors.txt");
            int i = rand.Next(0, Humors.Length);
            return message = Humors[i];
        }

    }
}
