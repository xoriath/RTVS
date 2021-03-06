﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.R.Components.Plots {
    public interface IPlotHistory: IDisposable {
        int PlotCount { get; }
        int ActivePlotIndex { get; }
        IPlotContentProvider PlotContentProvider { get; }
        Task RefreshHistoryInfo();

        event EventHandler HistoryChanged;
    }
}
