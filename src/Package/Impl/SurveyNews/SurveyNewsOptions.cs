﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Microsoft.Common.Core;
using Microsoft.R.Support.Settings;
using Microsoft.R.Support.Settings.Definitions;
using Microsoft.VisualStudio.R.Package.Options.R;
using Microsoft.VisualStudio.R.Packages.R;

namespace Microsoft.VisualStudio.R.Package.SurveyNews {
    [Export(typeof(ISurveyNewsOptions))]
    internal class SurveyNewsOptions : ISurveyNewsOptions {
        public SurveyNewsPolicy SurveyNewsCheck {
            get { return RToolsSettings.Current.SurveyNewsCheck; }
        }

        public DateTime SurveyNewsLastCheck {
            get { return RToolsSettings.Current.SurveyNewsLastCheck; }
            set {
                // We go through the dialog page, instead of going straight to RToolsSettings,
                // to ensure that the value gets saved.  Otherwise the option would only get saved
                // if the user opens the dialog page during the VS session.
                using (var p = RPackage.Current.GetDialogPage(typeof(RToolsOptionsPage)) as RToolsOptionsPage) {
                    p.SurveyNewsLastCheck = value;
                    p.SaveSettings();
                }
            }
        }

        public string IndexUrl { get { return RToolsSettings.Current.SurveyNewsIndexUrl; } }

        public string FeedUrl { get { return RToolsSettings.Current.SurveyNewsFeedUrl; } }

        public string CannotConnectUrl {
            get {
                string assemblyPath = Assembly.GetExecutingAssembly().GetAssemblyPath();
                string url = Path.Combine(Path.GetDirectoryName(assemblyPath), "SurveyNews", "NoSurveyNewsFeed.mht");
                return url;
            }
        }
    }
}
