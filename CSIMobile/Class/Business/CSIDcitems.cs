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
    public class CSIDcitems : CSIDcBase
    {
        public CSIDcitems(CSIContext SrcContext = null) : base(SrcContext)
        {
            IDOName = "SLDcitems";
        }

        protected override void InitialPreopertyList()
        {
            base.InitialPreopertyList();
            PreSetPropertyList.Add("TransNum");
            PreSetPropertyList.Add("TransType");
            PreSetPropertyList.Add("Stat"); 
            PreSetPropertyList.Add("Termid");
            PreSetPropertyList.Add("TransDate");
            PreSetPropertyList.Add("EmpNum"); 
            PreSetPropertyList.Add("Item");
            PreSetPropertyList.Add("ItemLotTracked");
            PreSetPropertyList.Add("ItemSerialTracked");
            PreSetPropertyList.Add("Whse");
            PreSetPropertyList.Add("Loc");
            PreSetPropertyList.Add("Lot");
            PreSetPropertyList.Add("CountQty");
            PreSetPropertyList.Add("UM");
            PreSetPropertyList.Add("ReasonCode");
            PreSetPropertyList.Add("RsnDescription");
            PreSetPropertyList.Add("DocumentNum");
            PreSetPropertyList.Add("ErrorMessage");
            PreSetPropertyList.Add("Override");
            PreSetPropertyList.Add("CanOverride");
            PreSetPropertyList.Add("ItemSerialPrefix"); 
        }
    }
}