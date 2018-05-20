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
using Java.Lang;

namespace CSIMobile.Class.Activities
{
    public class Module : CSIBaseObject
    {
        public string ModuleName;
        public ModuleAction[] ModuleActions;
        public bool Visible = true;
        public int DisplayPosition;
        public Module(CSIContext SrcContext = null) : base(SrcContext)
        {
        }
    }

    public class ModuleAction : CSIBaseObject
    {
        public string ActionName;
        public Type ActivityType;
        public string[] InvokeCommands = { "GetToken" };
        public int DrawableId;
        public bool Enabled = true;
        public bool Visible = true;
        public ModuleAction(CSIContext SrcContext = null) : base(SrcContext)
        {
        }
    }

    public class ModuleDeck : CSIBaseObject
    {
        private static Module[] builtInModules =
        {
            new Module
            {
                ModuleName = Application.Context.GetString(Resource.String.MasterData),
                Visible = false,
                DisplayPosition = 0,
                ModuleActions = new ModuleAction[]
                {
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.Items),
                        DrawableId = Resource.Drawable.Logo,
                        //ActivityType = typeof(object),
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.PurchaseOrders),
                        DrawableId = Resource.Drawable.Logo,
                        //ActivityType = typeof(object)
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.SalesOrders),
                        DrawableId = Resource.Drawable.Logo,
                        //ActivityType = typeof(object)
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.JobOrders),
                        DrawableId = Resource.Drawable.Logo,
                        //ActivityType = typeof(object)
                        Enabled = true,
                        Visible = true
                    }
                }
            },
            new Module
            {
                ModuleName = Application.Context.GetString(Resource.String.InventoryActivities),
                Visible = true,
                DisplayPosition = 1,
                ModuleActions = new ModuleAction[]
                {
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.MiscIssue),
                        DrawableId = Resource.Drawable.stockout,
                        InvokeCommands = new string[] { "MiscIssue" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.MiscReceive),
                        DrawableId = Resource.Drawable.stockin,
                        InvokeCommands = new string[] { "MiscReceive" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.SalesShip),
                        DrawableId = Resource.Drawable.shipping,
                        InvokeCommands = new string[] { "OrderShipping" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.SalesReturn),
                        DrawableId = Resource.Drawable.shipping,
                        InvokeCommands = new string[] { "OrderReturn" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.PurchaseReceive),
                        DrawableId = Resource.Drawable.movein,
                        InvokeCommands = new string[] { "PurchaseReceive" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.PurchaseReturn),
                        DrawableId = Resource.Drawable.moveout,
                        InvokeCommands = new string[] { "PurchaseReturn" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.TransferShip),
                        DrawableId = Resource.Drawable.transferout,
                        InvokeCommands = new string[] { "TransferShip" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.TransferReceive),
                        DrawableId = Resource.Drawable.transferin,
                        InvokeCommands = new string[] { "TransferReceive" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.QuantityMove),
                        DrawableId = Resource.Drawable.move,
                        InvokeCommands = new string[] { "QtyMove" },
                        Enabled = true,
                        Visible = true
                    }
                }
            },
            new Module
            {
                ModuleName = Application.Context.GetString(Resource.String.Shopfloor),
                Visible = true,
                DisplayPosition = 2,
                ModuleActions = new ModuleAction[]
                {
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.JobMaterial),
                        DrawableId = Resource.Drawable.jobmaterial,
                        InvokeCommands = new string[] { "JobMaterial" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.JobTransaction),
                        DrawableId = Resource.Drawable.shopfloor,
                        InvokeCommands = new string[] { "JobTransaction" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.JobReceipt),
                        DrawableId = Resource.Drawable.manufacturers,
                        InvokeCommands = new string[] { "JobReceipt" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.WorkCenterTransaction),
                        DrawableId = Resource.Drawable.shopfloor,
                        InvokeCommands = new string[] { "WorkCenterTransaction" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.ProductionSchedule),
                        DrawableId = Resource.Drawable.plan,
                        InvokeCommands = new string[] { "ProductionSchedule" },
                        Enabled = true,
                        Visible = true
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.JustInTime),
                        DrawableId = Resource.Drawable.task,
                        InvokeCommands = new string[] { "JustInTime" },
                        Enabled = true,
                        Visible = true
                    }
                }
            },
            new Module
            {
                ModuleName = Application.Context.GetString(Resource.String.Settings),
                Visible = true,
                DisplayPosition = 3,
                ModuleActions = new ModuleAction[]
                {
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.Login),
                        DrawableId = Resource.Drawable.user,
                        InvokeCommands = new string[] { "ShowSignIn" }
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.Settings),
                        DrawableId = Resource.Drawable.settings,
                        InvokeCommands = new string[] { "ShowSettings" }
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.About),
                        DrawableId = Resource.Drawable.cash,
                        InvokeCommands = new string[] { "ShowAbout" }
                    },
                    new ModuleAction {
                        ActionName = Application.Context.GetString(Resource.String.Log),
                        DrawableId = Resource.Drawable.report,
                        InvokeCommands = new string[] { "ShowLog" }
                    }
                }
            }
        };

        public static Module[] Modules;

        public ModuleDeck(CSIContext SrcContext = null) : base(SrcContext)
        {
            Modules = BuildModulesByLicens();
        }

        public Module this[int i]
        {
            get { return Modules[i]; }
        }
        
        public int NumModules
        {
            get { return Modules.Length; }
        }
        
        public int NumVisibleModules()
        {
            int Count = 0;
            foreach (Module M in Modules)
            {
                if (M.Visible)
                {
                    Count++;
                }
            }
            return Count;
        }

        public Module[] BuildModulesByLicens()
        {
            Module[] LicensedModules = null;
            return LicensedModules ?? builtInModules;
        }
    }
}