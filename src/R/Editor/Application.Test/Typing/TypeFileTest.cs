﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Common.Core;
using Microsoft.R.Components.ContentTypes;
using Microsoft.R.Editor.Application.Test.TestShell;
using Microsoft.R.Support.RD.ContentTypes;
using Microsoft.UnitTests.Core.XUnit;
using Xunit;

namespace Microsoft.R.Editor.Application.Test.Typing {
    [ExcludeFromCodeCoverage]
    [Collection(CollectionNames.NonParallel)]
    public class TypeFileTest {
        private readonly EditorAppTestFilesFixture _files;

        public TypeFileTest(EditorAppTestFilesFixture files) {
            _files = files;
        }

        //[Test]
        //[Category.Interactive]
        public void TypeFile_R() {
            string actual = TypeFileInEditor("check.r", RContentTypeDefinition.ContentType);
            string expected = "";
            actual.Should().Be(expected);
        }

        //[Test]
        //[Category.Interactive]
        public void TypeFile_RD() {
            TypeFileInEditor("01.rd", RdContentTypeDefinition.ContentType);
        }

        /// <summary>
        /// Opens file in an editor window
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="contentType">File content type</param>
        private string TypeFileInEditor(string fileName, string contentType) {
            using (var script = new TestScript(contentType)) {
                string text = _files.LoadDestinationFile(fileName);
                string[] lines = text.Split(CharExtensions.LineBreakChars);
                for (int i = 0; i < lines.Length; i++) {
                    string lineText = lines[i];
                    if (!lineText.TrimStart().StartsWith("#", System.StringComparison.Ordinal)) {
                        lineText = lineText.Replace(" ", string.Empty);
                    }
                    script.Type(lineText, idleTime: 10);
                    script.Enter();
                    script.DoIdle(300);
                }
                return script.EditorText;
            }
        }
    }
}
