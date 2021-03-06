﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.R.Components.ContentTypes;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.R.Package.Debugger.DataTips {
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType(RContentTypeDefinition.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]
    internal sealed class DataTipTextViewConnectionListener : IWpfTextViewConnectionListener {
        public DataTipTextViewConnectionListener() {
        }

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers) {
            if (!textView.TextBuffer.ContentType.IsOfType(RContentTypeDefinition.ContentType)) {
                return;
            }

            var adapterService = VsAppShell.Current.ExportProvider.GetExportedValue<IVsEditorAdaptersFactoryService>();
            var debugger = VsAppShell.Current.GetGlobalService<IVsDebugger>();
            DataTipTextViewFilter.GetOrCreate(textView, adapterService, debugger);
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers) {
            if (!textView.TextBuffer.ContentType.IsOfType(RContentTypeDefinition.ContentType)) {
                var filter = DataTipTextViewFilter.TryGet(textView);
                if (filter != null) {
                    filter.Dispose();
                }
            }
        }
    }
}
