using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Net.Http;
using System.Text.Json;

namespace Handlers {
    public class BotMethods {
        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken) {
            // проверяем пришло ли нам какое-то сообщение
            if (update.Type != UpdateType.Message || update.Message is null)
                return;

            var chatId = update.Message.Chat.Id;
            
            if (update.Message.Type == MessageType.Text && update.Message.Text != null) {
                var inputText = update.Message.Text;
                
                if (inputText.StartsWith("/start")) {
                    var username = update.Message.From.Username ?? update.Message.From.FirstName ?? "пользователь";
                    var reply = $"Привет, {username}! Этот бот позволяет переводить сообщения на английский и корейский языки.";
                    await bot.SendTextMessageAsync(chatId, reply, cancellationToken: cancellationToken);
                }
                
                // переводим сообщение на корейский и английский языки \(о-о)/
                else {
                    var translatedEn = await TranslateAsync(inputText, "en");
                    var translatedKo = await TranslateAsync(inputText, "ko");
                    var reply = $"англ: {translatedEn}\nкор: {translatedKo}";
                    await bot.SendTextMessageAsync(chatId, reply, cancellationToken: cancellationToken);
                }
            }
        }
        // функция для перевода сообщений
        async Task<string> TranslateAsync(string text, string lang) {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={lang}&dt=t&q={Uri.EscapeDataString(text)}";
            using var http = new HttpClient();
            var result = await http.GetStringAsync(url);
            var json = JsonDocument.Parse(result);
            var translated = json.RootElement[0][0][0].GetString();

            if (string.IsNullOrWhiteSpace(translated) || translated.Trim().ToLower() == text.Trim().ToLower())
                return "Я не понял.";
            return translated;
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken) {
            Console.WriteLine($"Polling error: {exception}");
            return Task.CompletedTask;
        }
    }
}