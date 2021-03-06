﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.R.Components.Extensions {
    public static class TextViewExtensions {
        /// <summary>
        /// Maps down to the buffer using positive point tracking and successor position affinity
        /// </summary>
        public static SnapshotPoint? MapDownToBuffer(this ITextView textView, int position, ITextBuffer buffer) {
            if (textView.BufferGraph == null) {
                // Unit test case
                if (position <= buffer.CurrentSnapshot.Length) {
                    return new SnapshotPoint(buffer.CurrentSnapshot, position);
                }
                return null;
            }
            if (position <= textView.TextBuffer.CurrentSnapshot.Length) {
                return textView.BufferGraph.MapDownToBuffer(
                    new SnapshotPoint(textView.TextBuffer.CurrentSnapshot, position),
                    PointTrackingMode.Positive,
                    buffer,
                    PositionAffinity.Successor
                );
            }
            return null;
        }

        public static SnapshotPoint? MapUpToView(this ITextView textView, SnapshotPoint position) {
            if (textView.BufferGraph == null) {
                // Unit test case
                return position;
            }
            return textView.BufferGraph.MapUpToBuffer(
                position,
                PointTrackingMode.Positive,
                PositionAffinity.Successor,
                textView.TextBuffer
             );
        }
    }
}
