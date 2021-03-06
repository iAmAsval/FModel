﻿using System.Reflection;
using CUE4Parse.UE4.Objects.Core.Misc;

namespace FModel
{
    public static class Constants
    {
        public static readonly string APP_VERSION = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public const string ZERO_64_CHAR = "0000000000000000000000000000000000000000000000000000000000000000";
        public static readonly FGuid ZERO_GUID = new(0U);

        public const string WHITE = "#DAE5F2";
        public const string RED = "#E06C75";
        public const string GREEN = "#98C379";
        public const string YELLOW = "#E5C07B";
        public const string BLUE = "#528BCC";

        public const string DONATE_LINK = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=EP9SSWG8MW4UC&source=url";
        public const string CHANGELOG_LINK = "https://github.com/iAmAsval/FModel/releases/latest";
        public const string ISSUE_LINK = "https://github.com/iAmAsval/FModel/issues/new";
        public const string DISCORD_LINK = "https://discord.gg/fdkNYYQ";

        public const string _FN_LIVE_TRIGGER = "fortnite-live.manifest";
        public const string _VAL_LIVE_TRIGGER = "valorant-live.manifest";
    }
}