﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using CSIMobile.Class.Activities;
using CSIMobile.Class.Fragments.Adapter;
using static Android.Widget.AdapterView;
using CSIMobile.Class.Common;

namespace CSIMobile.Class.Fragments
{
    public class ModuleDeckFragment : CSIBaseFragment
    {
        private static string MODULE_NAME = "module_name";
        private Module Module;

        public ModuleDeckFragment() : base()
        {
        }

        public ModuleDeckFragment(CSIBaseActivity activity = null) : base(activity)
        {
        }

        public static ModuleDeckFragment NewInstance(Module Module, CSIBaseActivity activity = null)
        {
            ModuleDeckFragment fragment = new ModuleDeckFragment(activity);

            Bundle args = new Bundle();
            args.PutString(MODULE_NAME, Module.ModuleName);
            fragment.Arguments = args;
            fragment.Module = Module;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string question = Arguments.GetString(MODULE_NAME, "");

            base.OnCreateView(inflater, container, savedInstanceState);

            Context contextThemeWrapper;
            switch (CSISystemContext.Theme)
            {
                case "Light":
                    contextThemeWrapper = new ContextThemeWrapper(BaseActivity, Resource.Style.MyTheme_Light_Base);
                    break;
                default:
                    contextThemeWrapper = new ContextThemeWrapper(BaseActivity, Resource.Style.MyTheme_Base);
                    break;
            }

            // clone the inflater using the ContextThemeWrapper
            LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

            // Inflate this fragment from the "flashcard_layout"
            View view = localInflater.Inflate(Resource.Layout.CSIModule, container, false);

            GridView ModuleGrid = view.FindViewById<GridView>(Resource.Id.ModuleGrid);

            ModuleGridViewerAdapter GridAdapter = new ModuleGridViewerAdapter((Android.Support.V4.App.Fragment)this, ModuleGrid);
            if (Module == null)
            {
                MainActivity act = (MainActivity)BaseActivity;
                Module = act.Modules[act.ModulePage.CurrentItem];
            }
            foreach (ModuleAction Action in Module.ModuleActions)
            {
                if (Action.Visible)
                {
                    GridAdapter.ActionItems.Add(Action);
                }
            }
            ModuleGrid.Adapter = GridAdapter;


            ModuleGrid.ItemClick += delegate (object sender, ItemClickEventArgs args)
            {
                ModuleAction Action = (ModuleAction)GridAdapter.ActionItems[args.Position];
                //Toast.MakeText(ModuleGrid.Context, Action.ActionName, ToastLength.Short).Show();
                if (Action.InvokeCommands.Length > 0)
                {
                    foreach (string command in Action.InvokeCommands)
                    {
                        if (Action.Enabled)
                        {
                            if (!BaseActivity.InvokeCommand(command))
                            {
                                return;
                            }
                        }
                    }
                }
                else if (Action.ActivityType != null)
                {
                    Bundle bundle = CSISystemContext.BuildBundle();
                    Intent intent = new Intent(Application.Context, Action.ActivityType);
                    intent.PutExtra("CSISystemContext", bundle);
                    StartActivity(intent);
                }
            };

            return view;
        }
    }
}