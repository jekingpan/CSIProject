﻿using Android.App;
using Android.OS;
using Android.Widget;
using CSIMobile.Class.Common;
using CSIMobile.Class.Fragments;
using System;
using System.Threading.Tasks;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.View;
using CSIMobile.Class.Activities;
using CSIMobile.Class.Fragments.Adapter;
using System.Collections.Generic;
using static CSIMobile.Class.Common.CSIMessageDialog;
using Android.Content.PM;

namespace CSIMobile
{
    [Activity(Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : CSIBaseActivity
    {
        public TextView[] MoudleButton = { null, null, null, null };
        public ModuleDeck Modules;
        public ModuleDeckAdapter DeckAdapter;
        public ViewPager ModulePage;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);

                if (CSISystemContext == null)
                {
                    CSISystemContext = new CSIContext();
                }
                CSISystemContext.ReadConfigurations();

                SetContentView(Resource.Layout.CSIMain);

                ModulePage = FindViewById<ViewPager>(Resource.Id.ModulePage);

                // Instantiate the deck of flash cards:
                Modules = new ModuleDeck(CSISystemContext);

                // Instantiate the adapter and pass in the deck of flash cards:
                DeckAdapter = new ModuleDeckAdapter(SupportFragmentManager, Modules, this);

                // Find the ViewPager and plug in the adapter:
                ModulePage.Adapter = DeckAdapter;
                ModulePage.PageSelected += (o, e) => { GetModuleDeck(); };

                MoudleButton[0] = FindViewById<TextView>(Resource.Id.MoudleButton1);
                MoudleButton[1] = FindViewById<TextView>(Resource.Id.MoudleButton2);
                MoudleButton[2] = FindViewById<TextView>(Resource.Id.MoudleButton3);
                MoudleButton[3] = FindViewById<TextView>(Resource.Id.MoudleButton4);
                MoudleButton[0].Visibility = ViewStates.Gone;
                MoudleButton[1].Visibility = ViewStates.Gone;
                MoudleButton[2].Visibility = ViewStates.Gone;
                MoudleButton[3].Visibility = ViewStates.Gone;
                int usedPosition = 0;
                for (int index = 0; index < Modules.NumModules; index++)
                {
                    if (Modules[index].Visible)
                    {
                        MoudleButton[usedPosition].Visibility = ViewStates.Visible;
                        MoudleButton[usedPosition].Text = Modules[index].ModuleName;
                        Modules[index].DisplayPosition = usedPosition;
                        MoudleButton[usedPosition].Tag = usedPosition;
                        usedPosition++;
                    }
                }
                MoudleButton[0].Click += (o, e) => { SetModuleDeck(int.Parse(MoudleButton[0].Tag.ToString())); };
                MoudleButton[1].Click += (o, e) => { SetModuleDeck(int.Parse(MoudleButton[1].Tag.ToString())); };
                MoudleButton[2].Click += (o, e) => { SetModuleDeck(int.Parse(MoudleButton[2].Tag.ToString())); };
                MoudleButton[3].Click += (o, e) => { SetModuleDeck(int.Parse(MoudleButton[3].Tag.ToString())); };

                SetModuleDeck(CSISystemContext.CurrentPageIndex);
                GetModuleDeck();

                //Show SignIn
                if (string.IsNullOrEmpty(CSISystemContext.CSIWebServerName))
                {
                    ShowSettingsDialog();
                }
                else
                {
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                    }
                }
                //Task startupWork = new Task(() => { ShowSignInDialog(); });
                //startupWork.Start();
            }
            catch (Exception Ex) {
                WriteErrorLog(Ex);
            }
        }

        private void ShowNoEmpMessage()
        {
            FragmentTransaction ft = FragmentManager.BeginTransaction();

            CSIMessageDialog SignOutDialog = (CSIMessageDialog)FragmentManager.FindFragmentByTag("NoEmp");

            if (SignOutDialog != null)
            {
                ft.Show(SignOutDialog);
            }
            else
            {
                SignOutDialog = new CSIMessageDialog(GetString(Resource.String.app_name), GetString(Resource.String.NoEmployee), DialogTypes.OK, this);
                SignOutDialog.Show(ft, "NoEmp");
            }
        }

        private void ShowSettingsDialog()
        {
            try
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                SettingsDialogFragment SettingsDialog = (SettingsDialogFragment)FragmentManager.FindFragmentByTag("Settings");
                if (SettingsDialog != null)
                {
                    ft.Show(SettingsDialog);
                    //ft.AddToBackStack(null);
                }
                else
                {
                    // Create and show the dialog.
                    SettingsDialog = new SettingsDialogFragment(this);
                    //Add fragment
                    SettingsDialog.Show(ft, "Settings");
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
        }

        private void ShowChangeWarehouse()
        {
            try
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                ChangeWarehouseFragment ChangeWhseDialog = (ChangeWarehouseFragment)FragmentManager.FindFragmentByTag("ChangeWarehouse");
                if (ChangeWhseDialog != null)
                {
                    ft.Show(ChangeWhseDialog);
                }
                else
                {
                    // Create and show the dialog.
                    ChangeWhseDialog = new ChangeWarehouseFragment(this);
                    //Add fragment
                    ChangeWhseDialog.Show(ft, "ChangeWarehouse");
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
        }

        private void ShowAbout()
        {
            try
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                AboutDialogFragment AboutDialog = (AboutDialogFragment)FragmentManager.FindFragmentByTag("About");
                if (AboutDialog != null)
                {
                    ft.Show(AboutDialog);
                }
                else
                {
                    // Create and show the dialog.
                    AboutDialog = new AboutDialogFragment(this);
                    //Add fragment
                    AboutDialog.Show(ft, "About");
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
        }

        private void ShowLog()
        {
            try
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                LogDialogFragment LogDialog = (LogDialogFragment)FragmentManager.FindFragmentByTag("Settings");
                if (LogDialog != null)
                {
                    ft.Show(LogDialog);
                    //ft.AddToBackStack(null);
                }
                else
                {
                    // Create and show the dialog.
                    LogDialog = new LogDialogFragment(this);
                    //Add fragment
                    LogDialog.Show(ft, "Settings");
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
        }

        private void ShowSignInDialog()
        {
            if (string.IsNullOrEmpty(CSISystemContext.CSIWebServerName))
            {
                ShowSettingsDialog();
                return;
            }
            try
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                SignInDialogFragment SignInDialog = (SignInDialogFragment)FragmentManager.FindFragmentByTag("SignIn");
                if (SignInDialog != null)
                {
                    ft.Show(SignInDialog);
                    //ft.AddToBackStack(null);
                }
                else
                {
                    // Create and show the dialog.
                    SignInDialog = new SignInDialogFragment(this);
                    //Add fragment
                    SignInDialog.Show(ft, "SignIn");
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                CSIMessageDialog SignOutDialog = (CSIMessageDialog)FragmentManager.FindFragmentByTag("Exit");

                if (SignOutDialog != null)
                {
                    ft.Show(SignOutDialog);
                }
                else
                {
                    SignOutDialog = new CSIMessageDialog(GetString(Resource.String.app_name), string.Format(GetString(Resource.String.AskForExit), string.IsNullOrEmpty(CSISystemContext.UserDesc) ? CSISystemContext.User : CSISystemContext.UserDesc), DialogTypes.OKCancel, this);
                    SignOutDialog.OkHandler += (sender, args) =>
                    {
                        Finish();
                    };
                    SignOutDialog.Show(ft, "Exit");
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public void GetModuleDeck()
        {
            for (int i = 0; i < MoudleButton.Length; i++)
            {
                if (i == ModulePage.CurrentItem)
                {
                    MoudleButton[i].SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Android.Resource.Drawable.ButtonOnoffIndicatorOn), null, null);
                }
                else
                {
                    MoudleButton[i].SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Android.Resource.Drawable.ButtonOnoffIndicatorOff), null, null);
                }
            }
        }

        public void SetModuleDeck(int Position)
        {
            ModulePage.SetCurrentItem(Position, true);
            CSISystemContext.CurrentPageIndex = Position;
            GetModuleDeck();
        }

        public override bool InvokeCommand(string Command, Dictionary<string, object> ParmList = null)
        {
            bool Success = false;
            switch (Command)
            {
                case "ShowSettings":
                    ShowSettingsDialog();
                    Success = true;
                    break;
                case "ShowAbout":
                    ShowAbout();
                    Success = true;
                    break;
                case "ShowLog":
                    ShowLog();
                    Success = true;
                    break;
                case "GetToken":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else
                    {
                        Success = true;
                    }
                    break;
                case "CreateToken":
                case "ShowSignIn":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                        ShowSignInDialog();
                    else
                    {
                        FragmentTransaction ft = FragmentManager.BeginTransaction();
                        CSIMessageDialog AlertDialog = new CSIMessageDialog(GetString(Resource.String.app_name), string.Format(GetString(Resource.String.AlreadySignIn), string.IsNullOrEmpty(CSISystemContext.UserDesc) ? CSISystemContext.User : CSISystemContext.UserDesc), DialogTypes.OKCancel, this);
                        AlertDialog.OkHandler += (sender, args) =>
                        {
                            CSISystemContext.Token = "";
                            ShowSignInDialog();
                        };
                        AlertDialog.Show(ft, "Exit");
                    }
                    Success = true;
                    break;
                case "ShowChangeWarehouse":
                    ShowChangeWarehouse();
                    Success = true;
                    break;
                case "QtyMove":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if(string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCQuantityMoveFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "MiscIssue":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCMiscIssueFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "MiscReceive":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCMiscReceiveFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "JobReceipt":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCJobReceiptFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "JobMaterial":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCJobMaterialTransactionsFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "JobTransaction":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCJobRunFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "OrderShipping":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCOrderShippingFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "OrderReturn":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCOrderReturnFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "PurchaseReceive":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCPurchaseReceiveFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "PurchaseReturn":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCPurchaseReturnFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "TransferShip":
                    
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCTransferOrderShipFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "TransferReceive":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        DCTransferOrderReceiveFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                case "Items":
                    if (string.IsNullOrEmpty(CSISystemContext.Token))
                    {
                        ShowSignInDialog();
                        Success = false;
                    }
                    else if (string.IsNullOrEmpty(CSISystemContext.EmpNum))
                    {
                        ShowNoEmpMessage();
                        return false;
                    }
                    else
                    {
                        ItemListFragment.RunFragment(this);
                        Success = true;
                    }
                    break;
                default:
                    break;
            }
            return Success;
        }
        
        private ProgressFragment ShowProcess()
        {
            try
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                ProgressFragment progress = (ProgressFragment)FragmentManager.FindFragmentByTag("Progress");
                if (progress != null)
                {
                    ft.Show(progress);
                    //ft.AddToBackStack(null);
                }
                else
                {
                    // Create and show the dialog.
                    progress = new ProgressFragment(this);
                    //Add fragment
                    progress.Show(ft, "Progress");
                }
                return progress;
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
            return null;
        }
            
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Main_Menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.Menu_Settings)
            {
                InvokeCommand("ShowSettings");
            }
            else if (item.ItemId == Resource.Id.Menu_ChangeWarehouse)
            {
                InvokeCommand("ShowChangeWarehouse");
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}