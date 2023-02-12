using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Project_4.Logic;

public class Bot
{
    #region Поля

    private static string _token = "";

    private readonly TelegramBotClient _botClient = new TelegramBotClient(_token);

    private readonly string _pathFileMessages = "MessagesReceivedInBot.txt";

    private readonly string _pathFileException = "ExceptionInBot.txt";

    #endregion

    #region События

    private event Func<Update, Task> _eventWriteData;

    private event Func<Exception, Task> _eventWriteException; 

    #endregion

    #region Конструктор

    public Bot()
    {
        _eventWriteData += WriteDataInFileAsync;
        _eventWriteException += WriteExceptionInFileAsync;
    }

    #endregion

    #region Основные методы

    /// <summary>
    /// Запуск бота
    /// </summary>
    public void StartBot()
    {
        _botClient.StartReceiving(UpdateBotAsync, ExceptionBotAsync);
    }

    /// <summary>
    /// Примем сообщений
    /// </summary>
    private async Task UpdateBotAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        
        if (message.Text != null)
        {
            switch (message.Text.ToLower())
            {
                case "/start":
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Телеграм бот запущен");
                    break;
                }
                case "/functions":
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "У бота нет идеи для функционала");
                    break;
                }
                default:
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Я не стану обрабатывать этот запрос");
                    break;
                }
            }
        }

        if (message.Audio != null)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы прислали аудиофайл");
        }

        if (message.Photo != null)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы прислали фото");
        }

        if (message.Document != null)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы прислали документ");
        }

        _eventWriteData?.Invoke(update);
    }
    
    /// <summary>
    /// Обработка ошибок
    /// </summary>
    private async Task ExceptionBotAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        //TODO некий функционал

        _eventWriteException?.Invoke(exception);
    }

    #endregion

    #region Дополнительные методы

    /// <summary>
    /// Запись данных в файл
    /// </summary>
    private async Task WriteDataInFileAsync(Update update)
    {
        var textWrite = $"{update.Id} " +
                        $"{update.Message.Date} " +
                        $"{update.Message.Chat.FirstName} " +
                        $"{update.Message.Chat.LastName} " +
                        $"{update.Message.Type} ";

        if (update.Message.Type == MessageType.Text)
        {
            textWrite += $"{update.Message.Text}";
        }
            
        await Write(_pathFileMessages, textWrite);
    }

    /// <summary>
    /// Запись ошибок в файл
    /// </summary>
    private async Task WriteExceptionInFileAsync(Exception exception)
    {
        var exceptionWrite = $"{DateTime.Now} {exception.Message}";
        await Write(_pathFileException, exceptionWrite);
    }

    /// <summary>
    /// Запись
    /// </summary>
    /// <param name="pathFile">Путь да файла для записи</param>
    /// <param name="text">Текст, который нужно записать</param>
    private async Task Write(string pathFile, string text)
    {
        await Task.Run(() =>
        {
            using (StreamWriter file = new StreamWriter(pathFile, append:true, Encoding.UTF8))
            {
                try
                {
                    Console.WriteLine(text);
                    file.WriteLineAsync(text);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    file.Dispose();
                }
            }
        });
    }

    #endregion
}
