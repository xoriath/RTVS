﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.R.Package.Commands;
#if VS14
using Microsoft.VisualStudio.ProjectSystem.Utilities;
#endif

namespace Microsoft.VisualStudio.R.Package.ProjectSystem.Commands {
    [ExportCommandGroup("AD87578C-B324-44DC-A12A-B01A6ED5C6E3")]
    [AppliesTo(Constants.RtvsProjectCapability)]
    internal sealed class AddRMarkdownCommand : AddItemCommand {

        [ImportingConstructor]
        public AddRMarkdownCommand(UnconfiguredProject unconfiguredProject): 
            base(unconfiguredProject, RPackageCommandId.icmdAddRMarkdown, "emptyrmd", "markdown", "rmd") {
        }
    }
}
