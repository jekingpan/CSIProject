﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CSIMobile.Class.Common;

namespace CSIMobile.Class.Business 
{
    public class CSIUserNames : CSIBaseDataObject
    {
        public CSIUserNames(CSIContext SrcContext = null) : base(SrcContext)
        {
            IDOName = "UserNames";
        }

        protected override void InitialPreopertyList()
        {
            base.InitialPreopertyList();
            PreSetPropertyList.Add("UserId");
            PreSetPropertyList.Add("Username");
            PreSetPropertyList.Add("UserDesc");
            PreSetPropertyList.Add("UseEmailReplyTo");
        }
    }
}