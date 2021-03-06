﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Common.Core.Shell;
using Microsoft.Common.Core.Test.Script;
using Microsoft.R.Components.Plots;
using Microsoft.R.Host.Client;
using Microsoft.R.Host.Client.Test.Script;
using Microsoft.UnitTests.Core.XUnit;
using Microsoft.VisualStudio.R.Package.Plots;
using Microsoft.VisualStudio.R.Package.Plots.Definitions;
using Microsoft.VisualStudio.R.Package.Shell;
using Xunit;

namespace Microsoft.VisualStudio.R.Package.Test.Plots {
    [ExcludeFromCodeCoverage]
    [Collection(CollectionNames.NonParallel)]   // required for tests using R Host 
    public class PlotHistoryTest {
        private static string[] _commands = new string[] {
            "plot(AirPassengers)",
            "plot(BJsales)",
            "plot(BJsales.lead)",
            "plot(BOD)",
            "plot(CO2)",
            "plot(ChickWeight)",
            "plot(DNase)",
            "plot(EuStockMarkets)",
            "plot(Formaldehyde)",
            "plot(HairEyeColor)",
            "plot(Harman23.cor)",
            "plot(Harman74.cor)",
            "plot(Indometh)",
            "plot(InsectSprays)",
            "plot(JohnsonJohnson)",
            "plot(LakeHuron)",
            "plot(LifeCycleSavings)",
            "plot(Loblolly)",
            "plot(Nile)",
            "plot(Orange)",
            "plot(OrchardSprays)",
            "plot(PlantGrowth)",
            "plot(Puromycin)",
            "plot(Seatbelts)",
            "plot(Theoph)",
            "plot(Titanic)",
            "plot(ToothGrowth)",
            "plot(UCBAdmissions)",
            "plot(UKDriverDeaths)",
            "plot(UKgas)",
            "plot(USAccDeaths)",
            "plot(USArrests)",
            "plot(USJudgeRatings)",
            "plot(USPersonalExpenditure)",
            "plot(UScitiesD)",
            "plot(VADeaths)",
            "plot(WWWusage)",
            "plot(WorldPhones)",
            "plot(ability.cov)",
            "plot(airmiles)",
            "plot(airquality)",
            "plot(anscombe)",
            "plot(attenu)",
            "plot(attitude)",
            "plot(austres)",
            "plot(beaver1)",
            "plot(beaver2)",
            "plot(cars)",
            "plot(chickwts)",
            "plot(co2)",
            "plot(crimtab)",
            "plot(discoveries)",
            "plot(esoph)",
            "plot(euro)",
            "plot(euro.cross)",
            "plot(eurodist)",
            "plot(faithful)",
            "plot(fdeaths)",
            "plot(freeny)",
            "plot(freeny.x)",
            "plot(freeny.y)",
            "plot(infert)",
            "plot(iris)",
            "plot(iris3)",
            "plot(islands)",
            "plot(ldeaths)",
            "plot(lh)",
            "plot(longley)",
            "plot(lynx)",
            "plot(mdeaths)",
            "plot(morley)",
            "plot(mtcars)",
            "plot(nhtemp)",
            "plot(nottem)",
            "plot(npk)",
            "plot(occupationalStatus)",
            "plot(precip)",
            "plot(presidents)",
            "plot(pressure)",
            "plot(quakes)",
            "plot(randu)",
            "plot(rivers)",
            "plot(rock)",
            "plot(sleep)",
            "plot(stack.loss)",
            "plot(stack.x)",
            "plot(stackloss)",
            "plot(state.abb)",
            "plot(state.area)",
            "plot(state.center)",
            "plot(state.division)",
            "plot(state.name)",
            "plot(state.region)",
            "plot(state.x77)",
            "plot(sunspot.month)",
            "plot(sunspot.year)",
            "plot(sunspots)",
            "plot(swiss)",
            "plot(treering)",
            "plot(trees)",
            "plot(uspop)",
            "plot(volcano)",
            "plot(warpbreaks)",
            "plot(women)"
        };

        [Test]
        [Category.Plots]
        public async Task PlotALot() {
            var sessionProvider = VsAppShell.Current.ExportProvider.GetExportedValue<IRSessionProvider>();
            var app = new RHostClientPlotTestApp();
            using (var script = new RHostScript(sessionProvider, app)) {
                var history = new PlotHistory(script.Session);
                app.History = history;

                foreach (var c in _commands) {
                    using (var interaction = await script.Session.BeginInteractionAsync()) {
                        await interaction.RespondAsync(c + Environment.NewLine);
                        EventsPump.DoEvents(100);
                    }
                }

                for (int i = _commands.Length - 1; i >= 0; i--) {
                    await history.PlotContentProvider.PreviousPlotAsync();
                    EventsPump.DoEvents(100);
                }

                for (int i = 0; i < _commands.Length; i++) {
                    await history.PlotContentProvider.NextPlotAsync();
                    EventsPump.DoEvents(500);
                }

                EventsPump.DoEvents(1000);
            }
        }

        class RHostClientPlotTestApp : IRSessionCallback {
            public IPlotHistory History { get; set; }

            public virtual string CranUrlFromName(string name) {
                return "https://cran.rstudio.com";
            }

            public virtual Task Plot(string filePath, CancellationToken ct) {
                VsAppShell.Current.DispatchOnUIThread(() => {
                    History.PlotContentProvider.LoadFile(filePath);
                });
                return Task.CompletedTask;
            }

            public virtual Task ShowErrorMessage(string message) {
                return Task.CompletedTask;
            }

            public virtual Task ShowHelp(string url) {
                return Task.CompletedTask;
            }

            public virtual Task<MessageButtons> ShowMessage(string message, MessageButtons buttons) {
                return Task.FromResult(MessageButtons.OK);
            }

            public Task<string> ReadUserInput(string prompt, int maximumLength, CancellationToken ct) {
                return Task.FromResult("\n");
            }

            public void ViewObject(string expression, string title) { }

            public Task ViewLibrary() {
                return Task.CompletedTask;
            }

            public Task ViewFile(string fileName, string tabName, bool deleteFile) {
                return Task.CompletedTask;
            }

            public Task<LocatorResult> Locator(CancellationToken ct) {
                throw new NotImplementedException();
            }
        }
    }
}
