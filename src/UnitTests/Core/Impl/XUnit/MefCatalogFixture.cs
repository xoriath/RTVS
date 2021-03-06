﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.UnitTests.Core.XUnit {
    [ExcludeFromCodeCoverage]
    public abstract class MefCatalogFixture : IDisposable {
        private readonly Lazy<ComposablePartCatalog> _catalogLazy;
        private readonly ConcurrentQueue<CompositionContainer> _containers = new ConcurrentQueue<CompositionContainer>();

        protected MefCatalogFixture() {
            _catalogLazy = new Lazy<ComposablePartCatalog>(CreateCatalog);
        }

        protected abstract ComposablePartCatalog CreateCatalog();

        protected virtual void AddValues(CompositionContainer container) {}

        public ExportProvider CreateExportProvider() {
            var container = new CompositionContainer(_catalogLazy.Value, CompositionOptions.DisableSilentRejection);
            AddValues(container);
            _containers.Enqueue(container);
            return container;
        }

        public ExportProvider CreateExportProvider(CompositionBatch additionalValues) {
            var container = new CompositionContainer(_catalogLazy.Value, CompositionOptions.DisableSilentRejection);
            AddValues(container);
            container.Compose(additionalValues);
            _containers.Enqueue(container);
            return container;
        }

        void IDisposable.Dispose() {
            CompositionContainer container;
            while (_containers.TryDequeue(out container)) {
                container.Dispose();
            }
        }
    }
}