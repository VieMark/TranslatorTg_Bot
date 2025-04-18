using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Handlers;

namespace bot_telegram {
    class Program {
        static async Task Main() {
            Console.WriteLine("Введите токен : ");
            var token = Console.ReadLine();
            var botClient = new TelegramBotClient(token);

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            var botMethods = new BotMethods();

            botClient.StartReceiving(
                (bot, update, cancellationToken) => botMethods.HandleUpdateAsync(bot, update, cancellationToken),
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token
            );

            Console.WriteLine("Бот запущен");
            Console.ReadLine();
            cts.Cancel();

            Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken) {
                Console.WriteLine(exception);
                return Task.CompletedTask;
            }
        }
    }
}