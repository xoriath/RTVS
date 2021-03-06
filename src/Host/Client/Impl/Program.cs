﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Common.Core;
using Microsoft.Common.Core.Shell;
using static System.FormattableString;

namespace Microsoft.R.Host.Client {
    class Program : IRCallbacks {
        private static IRExpressionEvaluator _evaluator;
        private int _nesting;

        static void Main(string[] args) {
            Console.CancelKeyPress += Console_CancelKeyPress;
            var host = new RHost("Program", new Program());
            host.CreateAndRun(args[0]).GetAwaiter().GetResult();
            _evaluator = host;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
        }

        public void Dispose() {
        }

        public Task Busy(bool which, CancellationToken ct) {
            return Task.FromResult(true);
        }

        public Task Connected(string rVersion) {
            return Task.CompletedTask;
        }

        public Task Disconnected() {
            return Task.CompletedTask;
        }

        public async Task<string> ReadConsole(IReadOnlyList<IRContext> contexts, string prompt, int len, bool addToHistory, CancellationToken ct) {
            return (await ReadLineAsync(prompt, ct)) + "\n";
        }

        public async Task ShowMessage(string s, CancellationToken ct) {
            await Console.Error.WriteLineAsync(s);
        }

        public async Task WriteConsoleEx(string buf, OutputType otype, CancellationToken ct) {
            var writer = otype == OutputType.Output ? Console.Out : Console.Error;
            await writer.WriteAsync(buf);
        }

        /// <summary>
        /// Called as a result of R calling R API 'YesNoCancel' callback
        /// </summary>
        /// <returns>Codes that match constants in RApi.h</returns>
        public async Task<YesNoCancel> YesNoCancel(IReadOnlyList<IRContext> contexts, string s, CancellationToken ct) {
            MessageButtons buttons = await ShowDialog(contexts, s, MessageButtons.YesNoCancel, ct);
            switch (buttons) {
                case MessageButtons.No:
                    return Client.YesNoCancel.No;
                case MessageButtons.Cancel:
                    return Client.YesNoCancel.Cancel;
            }
            return Client.YesNoCancel.Yes;
        }

        /// <summary>
        /// Called when R wants to display generic Windows MessageBox. 
        /// Graph app may call Win32 API directly rather than going via R API callbacks.
        /// </summary>
        /// <returns>Pressed button code</returns>
        public async Task<MessageButtons> ShowDialog(IReadOnlyList<IRContext> contexts, string s, MessageButtons buttons, CancellationToken ct) {
            await Console.Error.WriteAsync(s);
            while (true) {
                string r = await ReadLineAsync(" [yes/no/cancel]> ", ct);

                if (r.StartsWithIgnoreCase("y")) {
                    return MessageButtons.Yes;
                }
                if (r.StartsWithIgnoreCase("n")) {
                    return MessageButtons.No;
                }
                if (r.StartsWithIgnoreCase("c")) {
                    return MessageButtons.Cancel;
                }

                await Console.Error.WriteAsync("Invalid input, try again!");
            }
        }

        public async Task Plot(string filePath, CancellationToken ct) {
            await Console.Error.WriteLineAsync(filePath);
        }

        public async Task Browser(string url) {
            await Console.Error.WriteLineAsync("Browser: " + url);
        }

        public async void DirectoryChanged() {
            await Console.Error.WriteLineAsync("Directory changed.");
        }

        public void ViewObject(string x, string title) {
            Console.Error.WriteLineAsync(Invariant($"ViewObject({title}): {x}"));
        }

        public async Task ViewLibrary() {
            await Console.Error.WriteLineAsync("ViewLibrary");
        }

        public async Task ShowFile(string fileName, string tabName, bool deleteFile) {
            await Console.Error.WriteAsync(Invariant($"ShowFile({fileName}, {tabName}, {deleteFile})"));
        }

        private async Task<string> ReadLineAsync(string prompt, CancellationToken ct) {
            while (true) {
                await Console.Out.WriteAsync($"|{_nesting}| {prompt}");
                ++_nesting;
                try {
                    string s = await Console.In.ReadLineAsync();

                    if (s.StartsWithIgnoreCase("$$")) {
                        s = s.Remove(0, 1);
                    } else if (s.StartsWithIgnoreCase("$")) {
                        s = s.Remove(0, 1);

                        var kind = REvaluationKind.Normal;
                        if (s.StartsWithIgnoreCase("!")) {
                            kind |= REvaluationKind.Reentrant;
                            s = s.Remove(0, 1);
                        }

                        var er = await _evaluator.EvaluateAsync(s, kind, ct);
                        await Console.Out.WriteLineAsync(er.ToString());
                        continue;
                    }

                    return s;
                } finally {
                    --_nesting;
                }
            }
        }

        public async Task<LocatorResult> Locator(CancellationToken ct) {
            await Console.Error.WriteLineAsync("Locator called.");
            return LocatorResult.CreateNotClicked();
        }
    }
}
