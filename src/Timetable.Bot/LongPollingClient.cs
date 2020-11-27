// <copyright file="LongPollingClient.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Represents the handler of web hook requests.
    /// </summary>
    public class LongPollingClient
    {
        private readonly ITelegramBotClient client;
        private readonly Configuration configuration;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LongPollingClient"/> class.
        /// </summary>
        /// <param name="configuration">The instance of a application configuration.</param>
        /// <param name="logger">The instance of an application logger.</param>
        public LongPollingClient(Configuration configuration, ILogger<LongPollingClient> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.client = new TelegramBotClient(configuration.Token);
        }

        /// <summary>
        /// The main entry point for the bot.
        /// </summary>
        public void Run()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            this.client.OnMessage += this.OnMessageHandler;
            this.client.OnCallbackQuery += this.OnCallbackQueryHandler;
            this.client.StartReceiving();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            this.client.StopReceiving();
        }

        private async void OnMessageHandler(object? sender, MessageEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.Text))
            {
                return;
            }

            this.Log(
                $"[{e.Message.Chat.Id}] "
                + $"[{e.Message.Chat.Username}] "
                + $"[{e.Message.Chat.FirstName} "
                + $"{e.Message.Chat.LastName}]");

            switch (e.Message.Text.Trim())
            {
                case "/start":
                case "/timetable":
                    await this.OnTimetableAsync(e.Message.Chat.Id)
                        .ConfigureAwait(false);
                    break;
                default:
                    await this.client.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Invalid command: {e.Message.Text}")
                        .ConfigureAwait(false);

                    break;
            }
        }

        private async void OnCallbackQueryHandler(object? sender, CallbackQueryEventArgs e)
        {
            if (string.IsNullOrEmpty(e.CallbackQuery.Data))
            {
                return;
            }

            this.Log(
                $"[{e.CallbackQuery.Message.Chat.Id}] "
                + $"[{e.CallbackQuery.Message.Chat.Username}] "
                + $"[{e.CallbackQuery.Message.Chat.FirstName} "
                + $"{e.CallbackQuery.Message.Chat.LastName}] "
                + $"[{e.CallbackQuery.Message.MessageId}]");

            try
            {
                await this.client.EditMessageReplyMarkupAsync(
                    e.CallbackQuery.Message.Chat.Id,
                    e.CallbackQuery.Message.MessageId).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                this.logger.LogError(ex.ToString());
            }

            switch (e.CallbackQuery.Data.Trim())
            {
                case "start":
                case "timetable":
                    await this.OnTimetableAsync(e.CallbackQuery.Message.Chat.Id)
                        .ConfigureAwait(false);
                    break;
                default:
                    await this.client.SendTextMessageAsync(
                        chatId: e.CallbackQuery.Message.Chat,
                        text: $"Invalid command: {e.CallbackQuery.Data}")
                        .ConfigureAwait(false);
                    break;
            }
        }

        private async Task OnTimetableAsync(long chatId)
        {
            var timetable = await TimetableParser.GetAsync(this.configuration.Login ?? string.Empty, this.configuration.Password ?? string.Empty);
            timetable = TimetableFormatter.Format(timetable);

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton() { Text = "Повторить запрос", CallbackData = "timetable" },
                },
            });

            await this.client.SendTextMessageAsync(
                chatId: chatId,
                text: (timetable.Length > 0) ? timetable : "Расписание для студенческой группы отсутствует",
                replyMarkup: keyboard,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                disableWebPagePreview: true).ConfigureAwait(false);
        }

        private void Log(string message)
        {
            MissionMonitor.Publish($"{nameof(Timetable)} {message}");
            this.logger.LogInformation(message);
        }
    }
}
