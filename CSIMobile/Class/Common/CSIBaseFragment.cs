﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CSIMobile.Class.Common
{
    public class CSIBaseFragment : Android.Support.V4.App.Fragment
    {
        protected CSIBaseActivity BaseActivity;
        protected CSIContext CSISystemContext;
        
        public CSIBaseFragment()
        {
            if (CSISystemContext == null)
            {
                BaseActivity = (CSIBaseActivity)Activity;
                CSISystemContext = new CSIContext();
            }
            else
            {
                if (BaseActivity.GetCSISystemContext() == null)
                {
                    CSISystemContext = new CSIContext();
                }
                else
                {
                    CSISystemContext = BaseActivity.GetCSISystemContext();
                }
            }
        }

        public CSIBaseFragment(CSIBaseActivity activity = null) : base()
        {
            if (activity == null)
            {
                CSISystemContext = new CSIContext();
            }
            else
            {
                BaseActivity = activity;
                if (BaseActivity.GetCSISystemContext() == null)
                {
                    CSISystemContext = new CSIContext();
                }
                else
                {
                    CSISystemContext = BaseActivity.GetCSISystemContext();
                }
            }
        }

        public void SetCSISystemContext(CSIContext SrcContext)
        {
            if (SrcContext == null)
            {
                CSISystemContext = new CSIContext();
            }
            else
            {
                CSISystemContext = SrcContext;
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            if (BaseActivity == null)
            {
                BaseActivity = (CSIBaseActivity)Activity;
                if (BaseActivity.GetCSISystemContext() == null)
                {
                    CSISystemContext = new CSIContext();
                }
                else
                {
                    CSISystemContext = BaseActivity.GetCSISystemContext();
                }
            }
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        protected void WriteErrorLog(Exception Ex)
        {
            if (CSISystemContext.DisplayWhenError)
            {
                Toast.MakeText(Application.Context, Ex.Message, ToastLength.Long).Show();
            }
            CSIErrorLog.WriteErrorLog(Ex);
        }

        protected void WriteLog(string content)
        {
            CSIErrorLog.WriteLog(content);
        }

        protected void WriteLog()
        {
            CSIErrorLog.WriteLog(CSISystemContext);
        }
    }
}