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

namespace CSIMobile.Class.Fragments
{
    public class ProgressFragment : CSIBaseDialogFragment
    {
        public ProgressFragment() : base()
        {
        }

        public ProgressFragment(CSIBaseActivity activity = null) : base(activity)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);
                Cancelable = false;

                var view = inflater.Inflate(Resource.Layout.CSIProgressBar, container, false);
                
                return view;
            }catch (Exception Ex)
            {
                WriteErrorLog(Ex);
                return null;
            }
        }
    }
}