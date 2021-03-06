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
using System.Threading.Tasks;
using static CSIMobile.Class.Common.CSIMessageDialog;
using CSIMobile.Class.Business;
using System.Data;
using System.IO;
using Android.Util;
using CSIMobile.Class.Business.IO;

namespace CSIMobile.Class.Fragments
{
    public class DCJobReceiptFragment : CSIBaseDialogFragment
    {
        CSIDcjms SLDcjms;

        ImageButton JobScanButton;
        ImageButton SuffixScanButton;
        ImageButton OperNumScanButton;
        ImageButton QtyScanButton;
        ImageButton LocScanButton;
        ImageButton LotScanButton;
        EditText WhseEdit;
        TextView TransDateText;
        EditText JobEdit;
        EditText SuffixEdit;
        TextView JobDescText;
        EditText QtyEdit;
        EditText OperNumEdit;
        EditText LocEdit;
        EditText LotEdit;
        TextView ItemText;
        TextView ItemDescText;
        TextView ItemUMText;
        TextView QtyReleasedText;
        TextView WorkCenterText;
        TextView LocDescText;
        LinearLayout QtyLinearLayout;
        LinearLayout OperNumLinearLayout;
        LinearLayout LocationLinearLayout;
        LinearLayout LotLinearLayout;
        Button SNButton;
        Button ProcessButton;

        ImageView CloseImage;
        ProgressBar ProgressBar;
        LinearLayout Layout;

        bool LotTracked = false, SNTracked = false;

        bool JobValidated = false, SuffixValidated = false, QtyValidated = false, OperNumValidated = false, LocValidated = false, LotValidated = false;
        List<string> SNs = new List<string>();
        bool SNPicked = true;

        public DCJobReceiptFragment() : base()
        {

        }

        public DCJobReceiptFragment(CSIBaseActivity activity = null) : base(activity)
        {
            Title = Application.Context.GetString(Resource.String.JobReceipt);

            CSISystemContext.ReadConfigurations();
            SLDcjms = new CSIDcjms(CSISystemContext);
            SLDcjms.AddProperty("TransNum");
            SLDcjms.AddProperty("TransType");
            SLDcjms.AddProperty("Stat");
            SLDcjms.AddProperty("Termid");
            SLDcjms.AddProperty("TransDate");
            SLDcjms.AddProperty("EmpNum");
            SLDcjms.AddProperty("Job");
            SLDcjms.AddProperty("Suffix");
            SLDcjms.AddProperty("JobItem");
            SLDcjms.AddProperty("CoProductMix");
            SLDcjms.AddProperty("DerCoItem");
            SLDcjms.AddProperty("OperNum");
            SLDcjms.AddProperty("JobrtWc");
            SLDcjms.AddProperty("JobItemLotTracked");
            SLDcjms.AddProperty("JobItemSerialTracked");
            SLDcjms.AddProperty("ItemSerialPrefix");
            SLDcjms.AddProperty("Whse");
            SLDcjms.AddProperty("Loc");
            SLDcjms.AddProperty("Lot");
            SLDcjms.AddProperty("Qty");
            SLDcjms.AddProperty("DocumentNum");
            SLDcjms.AddProperty("ErrorMessage");

            SLDcjms.SetFilter("1=0");
            SLDcjms.UseAsync(false);
            SLDcjms.LoadIDO();
            SLDcjms.UseAsync(true);
            SLDcjms.SaveDataSetCompleted += SLDcjms_SaveDataSetCompleted;
            SLDcjms.LoadDataSetCompleted += SLDcjms_LoadDataSetCompleted;
            SLDcjms.CallMethodCompleted += SLDcjms_CallMethodCompleted;
        }

        private void SLDcjms_SaveDataSetCompleted(object sender, SaveDataSetCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    //check result status
                    if (SLDcjms.CurrentTable.Rows.Count <= 0)
                    {
                        //nothing happen or just delete rows
                    }
                    else
                    {
                        string RowStatus = SLDcjms.GetCurrentPropertyValueOfString("Stat");
                        string ErrorMessage = SLDcjms.GetCurrentPropertyValueOfString("ErrorMessage");

                        if ((RowStatus != "E") || string.IsNullOrEmpty(ErrorMessage))
                        {
                            if (CSISystemContext.ForceAutoPost)
                            {
                                //Ready to Post -- calling DcjrPSp
                                ShowProgressBar(true);
                                string strParmeters = "";
                                strParmeters = CSIBaseInvoker.BuildXMLParameters(strParmeters, SLDcjms.GetCurrentPropertyValueOfString("TransNum"));
                                strParmeters = CSIBaseInvoker.BuildXMLParameters(strParmeters, "");
                                strParmeters = CSIBaseInvoker.BuildXMLParameters(strParmeters, "", true);
                                SLDcjms.InvokeMethod("DcjrPSp", strParmeters);
                            }
                            else
                            {
                                if (CSISystemContext.ShowSuccessMessage)
                                {
                                    ShowProcessedMessage();
                                }
                                //Clear Result if no error.
                                Initialize();
                            }
                        }
                        else
                        {
                            //delete first before prompt message.
                            SLDcjms.CurrentTable.Rows[0].Delete();
                            ShowProgressBar(true);
                            SLDcjms.DeleteIDO();

                            //Populate Error
                            FragmentTransaction ft = FragmentManager.BeginTransaction();
                            CSIMessageDialog DeleteDialog = (CSIMessageDialog)FragmentManager.FindFragmentByTag("DeleteDialog");

                            if (DeleteDialog != null)
                            {
                                ft.Show(DeleteDialog);
                            }
                            else
                            {
                                DeleteDialog = new CSIMessageDialog(GetString(Resource.String.app_name), ErrorMessage, DialogTypes.OK);
                                DeleteDialog.Show(ft, "DeleteDialog");
                            }
                        }
                    }
                }
                else
                {
                    WriteErrorLog(e.Error);
                }
            }catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
            ShowProgressBar(false);
        }

        private void SLDcjms_CallMethodCompleted(object sender, CallMethodCompletedEventArgs e)
        {
            try
            {
                //throw new NotImplementedException();
                if (e.Error == null)
                {
                    if (e.Result.ToString() == "0")
                    {
                        if (CSISystemContext.ShowSuccessMessage)
                        {
                            ShowProcessedMessage();
                        }
                        Initialize();
                    }
                    else
                    {
                        //get error - delete first.
                        SLDcjms.CurrentTable.Rows[0].Delete();
                        ShowProgressBar(true);
                        SLDcjms.DeleteIDO();
                        WriteErrorLog(new Exception(CSIBaseInvoker.GetXMLParameters(e.strMethodParameters,1)));
                    }
                }
                else
                {
                    //try to delete post
                    SLDcjms.CurrentTable.Rows[0].Delete();
                    ShowProgressBar(true);
                    SLDcjms.DeleteIDO();
                    WriteErrorLog(e.Error);
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
            ShowProgressBar(false);
        }

        private void SLDcjms_LoadDataSetCompleted(object sender, LoadDataSetCompletedEventArgs e)
        {
            try
            {
                //throw new NotImplementedException();
                if (e.Error == null)
                {
                }
                else
                {
                }
            }
            catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
            ShowProgressBar(false);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                var view = inflater.Inflate(Resource.Layout.CSIJobReceipt, container, false);
                Cancelable = false;

                WhseEdit = view.FindViewById<EditText>(Resource.Id.WhseEdit);
                TransDateText = view.FindViewById<TextView>(Resource.Id.TransDateText);
                JobScanButton = view.FindViewById<ImageButton>(Resource.Id.JobScanButton);
                SuffixScanButton = view.FindViewById<ImageButton>(Resource.Id.SuffixScanButton);
                JobEdit = view.FindViewById<EditText>(Resource.Id.JobEdit);
                SuffixEdit = view.FindViewById<EditText>(Resource.Id.SuffixEdit);
                QtyScanButton = view.FindViewById<ImageButton>(Resource.Id.QtyScanButton);
                QtyEdit = view.FindViewById<EditText>(Resource.Id.QtyEdit);
                OperNumScanButton = view.FindViewById<ImageButton>(Resource.Id.OperNumScanButton);
                OperNumEdit = view.FindViewById<EditText>(Resource.Id.OperNumEdit);
                LocScanButton = view.FindViewById<ImageButton>(Resource.Id.LocScanButton);
                LocEdit = view.FindViewById<EditText>(Resource.Id.LocEdit);
                LotScanButton = view.FindViewById<ImageButton>(Resource.Id.LotScanButton);
                LotEdit = view.FindViewById<EditText>(Resource.Id.LotEdit);

                QtyLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.QtyLinearLayout);
                OperNumLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.OperNumLinearLayout);
                LocationLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.LocationLinearLayout);
                LotLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.LotLinearLayout);

                SNButton = view.FindViewById<Button>(Resource.Id.SNButton);
                ProcessButton = view.FindViewById<Button>(Resource.Id.ProcessButton);
                Layout = view.FindViewById<LinearLayout>(Resource.Id.LinearLayout);

                JobDescText = view.FindViewById<TextView>(Resource.Id.JobDescText);
                ItemText = view.FindViewById<TextView>(Resource.Id.ItemText);
                ItemDescText = view.FindViewById<TextView>(Resource.Id.ItemDescText);
                ItemUMText = view.FindViewById<TextView>(Resource.Id.ItemUMText);
                QtyReleasedText = view.FindViewById<TextView>(Resource.Id.QtyReleasedText);
                WorkCenterText = view.FindViewById<TextView>(Resource.Id.WorkCenterText);
                LocDescText = view.FindViewById<TextView>(Resource.Id.LocDescText);

                CloseImage = view.FindViewById<ImageView>(Resource.Id.CloseImage);
                ProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);

                JobScanButton.Click += JobScanButton_Click;
                SuffixScanButton.Click += SuffixScanButton_Click;
                QtyScanButton.Click += QtyScanButton_Click;
                OperNumScanButton.Click += OperNumScanButton_Click;
                LocScanButton.Click += LocScanButton_Click;
                LotScanButton.Click += LotScanButton_Click;

                JobEdit.FocusChange += JobEdit_FocusChange;
                SuffixEdit.FocusChange += SuffixEdit_FocusChange;
                QtyEdit.FocusChange += QtyEdit_FocusChange;
                OperNumEdit.FocusChange += OperNumEdit_FocusChange;
                LocEdit.FocusChange += LocEdit_FocusChange;
                LotEdit.FocusChange += LotEdit_FocusChange;

                JobEdit.KeyPress += JobEdit_KeyPress;
                SuffixEdit.KeyPress += SuffixEdit_KeyPress;
                QtyEdit.KeyPress += QtyEdit_KeyPress;
                OperNumEdit.KeyPress += OperNumEdit_KeyPress;
                LocEdit.KeyPress += LocEdit_KeyPress;
                LotEdit.KeyPress += LotEdit_KeyPress;

                JobEdit.RequestFocus();

                SNButton.Click += SNButton_Click;
                ProcessButton.Click += ProcessButton_Click;

                CloseImage.Click += (sender, args) =>
                {
                    Dismiss();
                    Dispose();
                };

                ShowProgressBar(false);
                Initialize();
                EnableDisableComponents();

                return view;
            }catch (Exception Ex)
            {
                WriteErrorLog(Ex);
                Dismiss();
                Dispose();
                return null;
            }
        }

        private void LotEdit_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Tab)
            {
                if (e.Event.Action == KeyEventActions.Up)
                {
                    ValidateLot();
                    ProcessButton.RequestFocus();
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                LotValidated = false;
            }
        }

        private void LocEdit_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Tab)
            {
                if (e.Event.Action == KeyEventActions.Up)
                {
                    ValidateLoc();
                    if (LotTracked)
                    {
                        LotEdit.RequestFocus();
                    }
                    else
                    {
                        ProcessButton.RequestFocus();
                    }
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                LocValidated = false;
            }

        }

        private void QtyEdit_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Tab)
            {
                if (e.Event.Action == KeyEventActions.Up)
                {
                    ValidateQty();
                    LocEdit.RequestFocus();
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                QtyValidated = false;
            }
        }

        private void OperNumEdit_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Tab)
            {
                if (e.Event.Action == KeyEventActions.Up)
                {
                    ValidateOperNum();
                    QtyEdit.RequestFocus();
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                OperNumValidated = false;
            }
        }

        private void SuffixEdit_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Tab)
            {
                if (e.Event.Action == KeyEventActions.Up)
                {
                    ValidateSuffix();
                    OperNumEdit.RequestFocus();
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                SuffixValidated = false;
            }
        }

        private void JobEdit_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Tab)
            {
                if (e.Event.Action == KeyEventActions.Up)
                {
                    ValidateJob();
                    SuffixEdit.RequestFocus();
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                JobValidated = false;
            }
        }

        private void Initialize()
        {
            WhseEdit.Text = CSISystemContext.DefaultWarehouse;
            TransDateText.Text = string.Format("{0} {1}",DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            JobEdit.Text = string.Empty;
            JobDescText.Text = string.Empty;
            SuffixEdit.Text = string.Empty;
            QtyEdit.Text = string.Empty;
            OperNumEdit.Text = string.Empty;
            LocEdit.Text = string.Empty;
            LotEdit.Text = string.Empty;
            ItemText.Text = string.Empty;
            ItemDescText.Text = string.Empty;
            ItemUMText.Text = string.Empty;
            QtyReleasedText.Text = string.Empty;
            WorkCenterText.Text = string.Empty;
            LocDescText.Text = string.Empty;

            JobValidated = false;
            SuffixValidated = false;
            QtyValidated = false;
            OperNumValidated = false;
            LocValidated = false;
            LotValidated = false;

            SetSNLabel();
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            PerformValidation();
            if (JobValidated && SuffixValidated && QtyValidated && OperNumValidated && LocValidated && (LotValidated || !LotTracked) && SNPicked)
            {
                SLDcjms.CurrentTable.Rows.Clear();
                DataRow Row = SLDcjms.CurrentTable.NewRow();
                Row["TransNum"] = SLDcjms.NextTransNum();//TransNum
                Row["TransType"] = "3";//TransType
                Row["Stat"] = "U";//Stat
                Row["Termid"] = CSISystemContext.GetDeviceId();//Termid
                Row["TransDate"] = DateTime.Now;//TransDate
                Row["Whse"] = CSISystemContext.DefaultWarehouse;//Whse
                Row["EmpNum"] = CSISystemContext.EmpNum;//EmpNum
                Row["Job"] = JobEdit.Text;//Item
                Row["Suffix"] = SuffixEdit.Text;//UM
                Row["Qty"] = QtyEdit.Text;//QtyMoved
                Row["OperNum"] = OperNumEdit.Text;//OperNum
                Row["Loc"] = LocEdit.Text;//Loc
                Row["Lot"] = LotEdit.Text;//Lot
                SLDcjms.CurrentTable.Rows.Add(Row);
                //Row.BeginEdit();
                //Row.EndEdit();
                //Row.AcceptChanges();

                SetKeyValues(GetString(Resource.String.JobOrder), JobEdit.Text, GetString(Resource.String.Suffix), SuffixEdit.Text);

                SLDcjms.InsertIDO();
                ShowProgressBar(true);
            }
        }

        private void SNButton_Click(object sender, EventArgs e)
        {
            SerialGenerator.RunFragment(this);
        }

        private void LotEdit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {//gain focus
                LotEdit.SelectAll();
            }
            else
            {//lose focus
                ValidateLot();
            }
        }

        private void LocEdit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {//gain focus
                LocEdit.SelectAll();
            }
            else
            {//lose focus
                ValidateLoc();
            }
        }

        private void OperNumEdit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {//gain focus
                OperNumEdit.SelectAll();
            }
            else
            {//lose focus
                ValidateOperNum ();
            }
        }

        private void QtyEdit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {//gain focus
                QtyEdit.SelectAll();
            }
            else
            {//lose focus
                ValidateQty();
            }
        }

        private void SuffixEdit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {//gain focus
                SuffixEdit.SelectAll();
            }
            else
            {//lose focus
                ValidateSuffix();
            }
        }

        private void JobEdit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {//gain focus
                JobEdit.SelectAll();
            }
            else
            {//lose focus
                ValidateJob();
            }
        }

        private bool ValidateJob()
        {
            if (!JobValidated)
            {
                if (string.IsNullOrEmpty(JobEdit.Text))
                {
                    JobValidated = false;
                }
                else
                {
                    try
                    {
                        string Job = JobEdit.Text, Suffix = SuffixEdit.Text, Desc = JobDescText.Text, Item = ItemText.Text
                            , ItemDesc = ItemDescText.Text, ItemUM = ItemUMText.Text, QtyReleased = QtyReleasedText.Text
                            , QtyComplete = "", QtyRequired = "";

                        //validate Job and Suffix
                        JobValidated = CSIJobs.GetJobInfor(CSISystemContext, ref Job, ref Suffix, ref Desc, ref Item
                            , ref ItemDesc, ref ItemUM, ref QtyReleased, ref QtyComplete, ref QtyRequired, ref LotTracked, ref SNTracked);
                        if (JobValidated == true)
                        {
                            JobEdit.Text = Job;
                            SuffixEdit.Text =  Suffix;
                            JobDescText.Text = Desc;
                            ItemText.Text = Item;
                            ItemDescText.Text = ItemDesc;
                            ItemUMText.Text = ItemUM;
                            QtyReleasedText.Text = QtyReleased;

                            SuffixValidated = true;

                            //validate Qty
                            if (string.IsNullOrEmpty(QtyEdit.Text) || decimal.Parse(QtyEdit.Text) == 0)
                            {
                                QtyEdit.Text = QtyRequired;
                                QtyValidated = true;
                            }

                            //Validate OperNum
                            string Operation = OperNumEdit.Text, Wc = WorkCenterText.Text, QtyReceived = "";
                            bool RtnSLJobRoutes = CSIJobRoutes.GetOperationInfor(CSISystemContext, JobEdit.Text, SuffixEdit.Text, ref Operation, ref Wc, ref QtyReceived);
                            if (RtnSLJobRoutes == true)
                            {
                                OperNumEdit.Text = Operation;
                                OperNumValidated = false;
                                ValidateOperNum();
                                WorkCenterText.Text = Wc;
                            }

                            //Validate ItemLoc
                            string Loc = LocEdit.Text, LocDescription = "", Qty = "";
                            bool RtnCSIItemLocs = CSIItemLocs.GetItemLocInfor(CSISystemContext, ItemText.Text, WhseEdit.Text, ref Loc, ref LocDescription, ref Qty);
                            if (RtnCSIItemLocs == true)
                            {
                                LocEdit.Text = Loc;
                                LocDescText.Text = LocDescription;
                                LocValidated = false;
                                ValidateLoc();
                                //OnHandQuantityText.Text = Qty; //used for validate Qty
                            }
                        }
                        else
                        {
                            JobDescText.Text = string.Empty;
                            ItemText.Text = string.Empty;
                            ItemDescText.Text = string.Empty;
                            ItemUMText.Text = string.Empty;
                        }

                    }
                    catch (Exception Ex)
                    {
                        WriteErrorLog(Ex);
                        JobValidated = false;
                    }
                }
            }
            EnableDisableComponents();
            return JobValidated;
        }

        private bool ValidateSuffix()
        {
            if (!SuffixValidated)
            {
                if (string.IsNullOrEmpty(SuffixEdit.Text))
                {
                    SuffixValidated = false;
                }
                else
                {
                    try
                    {
                        string Job = JobEdit.Text, Suffix = SuffixEdit.Text, Desc = JobDescText.Text, Item = ItemText.Text
                            , ItemDesc = ItemDescText.Text, ItemUM = ItemUMText.Text, QtyReleased = QtyReleasedText.Text
                            , QtyComplete = "", QtyRequired = "";
                        //validate Job and Suffix
                        SuffixValidated = CSIJobs.GetJobInfor(CSISystemContext, ref Job, ref Suffix, ref Desc, ref Item
                            , ref ItemDesc, ref ItemUM, ref QtyReleased, ref QtyComplete, ref QtyRequired, ref LotTracked, ref SNTracked);
                        if (SuffixValidated == true)
                        {
                            JobEdit.Text = Job;
                            SuffixEdit.Text = Suffix;
                            JobDescText.Text = Desc;
                            ItemText.Text = Item;
                            ItemDescText.Text = ItemDesc;
                            ItemUMText.Text = ItemUM;
                            QtyReleasedText.Text = QtyReleased;

                            //validate OperNum
                            if (string.IsNullOrEmpty(QtyEdit.Text) || decimal.Parse(QtyEdit.Text) == 0)
                            {
                                QtyEdit.Text = QtyRequired;
                                OperNumValidated = false;
                                ValidateQty();
                            }

                            //Validate OperNum
                            string Operation = OperNumEdit.Text, Wc = WorkCenterText.Text, QtyReceived = "";
                            bool RtnSLJobRoutes = CSIJobRoutes.GetOperationInfor(CSISystemContext, JobEdit.Text, SuffixEdit.Text, ref Operation, ref Wc, ref QtyReceived);
                            if (RtnSLJobRoutes == true)
                            {
                                OperNumEdit.Text = Operation;
                                OperNumValidated = false;
                                ValidateOperNum();
                                WorkCenterText.Text = Wc;
                            }

                            //Validate ItemLoc
                            string Loc = LocEdit.Text, LocDescription = "", Qty = "";
                            bool RtnCSIItemLocs = CSIItemLocs.GetItemLocInfor(CSISystemContext, ItemText.Text, WhseEdit.Text, ref Loc, ref LocDescription, ref Qty);
                            if (RtnCSIItemLocs == true)
                            {
                                LocEdit.Text = Loc;
                                LocDescText.Text = LocDescription;
                                LocValidated = false;
                                ValidateLoc();
                            //OnHandQuantityText.Text = Qty; //used for validate Qty
                            }
                        }
                        else
                        {
                            JobDescText.Text = string.Empty;
                            ItemText.Text = string.Empty;
                            ItemDescText.Text = string.Empty;
                            ItemUMText.Text = string.Empty;
                        }

                    }
                    catch (Exception Ex)
                    {
                        WriteErrorLog(Ex);
                        SuffixValidated = false;
                    }
                }
            }
            EnableDisableComponents();
            return SuffixValidated;
        }

        private bool ValidateQty()
        {
            if (!QtyValidated)
            {
                if (string.IsNullOrEmpty(QtyEdit.Text))
                {
                    QtyValidated = false;
                }
                else
                {
                    QtyValidated = true;
                }
            }
            EnableDisableComponents();
            return QtyValidated;
        }

        private bool ValidateOperNum()
        {
            if (!OperNumValidated)
            {
                if (string.IsNullOrEmpty(OperNumEdit.Text))
                {
                    OperNumValidated = false;
                }
                else
                {
                    string Operation = OperNumEdit.Text, Wc = WorkCenterText.Text, QtyReceived = "";
                    OperNumValidated = CSIJobRoutes.GetOperationInfor(CSISystemContext, JobEdit.Text, SuffixEdit.Text, ref Operation, ref Wc, ref QtyReceived);
                    if (OperNumValidated)
                    {
                        OperNumEdit.Text = Operation;
                        WorkCenterText.Text = Wc;
                    }
                    else
                    {
                        WorkCenterText.Text = string.Empty;
                    }
                }
            }
            EnableDisableComponents();
            return OperNumValidated;
        }

        private bool ValidateLoc()
        {
            if (!LocValidated)
            {
                if (string.IsNullOrEmpty(LocEdit.Text))
                {
                    LocValidated = false;
                }
                else
                {
                    string Loc = LocEdit.Text, LocDescription = "", Lot = LotEdit.Text, Qty = "";
                    LocValidated = CSIItemLocs.GetItemLocInfor(CSISystemContext, ItemText.Text, WhseEdit.Text, ref Loc, ref LocDescription, ref Qty);
                    if (LocValidated)
                    {
                        LocDescText.Text = LocDescription;
                    }
                    else
                    {
                        try
                        {
                            CSILocations SLLoc = new CSILocations(CSISystemContext);
                            SLLoc.UseAsync(false);
                            SLLoc.AddProperty("Loc");
                            SLLoc.AddProperty("Description");
                            SLLoc.SetFilter(string.Format("Loc = N'{0}'", Loc));
                            SLLoc.LoadIDO();
                            if (SLLoc.CurrentTable.Rows.Count <= 0)
                            {
                                LocDescText.Text = string.Empty;
                                LocValidated = false;
                            }
                            else
                            {
                                LocEdit.Text = SLLoc.GetCurrentPropertyValueOfString("Loc"); ;
                                LocDescText.Text = SLLoc.GetCurrentPropertyValueOfString("Description"); ;
                                LocValidated = true;
                            }
                        }catch (Exception Ex)
                        {
                            WriteErrorLog(Ex);
                            LocValidated = false;
                        }
                        //LocEdit.Text = Loc;
                        //LocDescText.Text = LocDescription;
                        //bool RtnCSILotLocs = CSILotLocs.GetItemLotLocInfor(CSISystemContext, JobEdit.Text, WhseEdit.Text, Loc, ref Lot, ref Qty);
                        //if (RtnCSILotLocs)
                        //{
                        //    LotEdit.Text = Lot;
                        //    LotValidated = false;
                        ValidateLot();
                        //    //ReleasedQuantityText.Text = Qty; //used for validate Qty
                        //}
                    }
                }
            }
            EnableDisableComponents();
            return LocValidated;
        }

        private bool ValidateLot()
        {
            if (!LotValidated)
            {
                if (string.IsNullOrEmpty(LotEdit.Text))
                {
                    if (LotTracked)
                    {
                        string Message = "", Key = "";
                        LotValidated = CSIItems.GetNextLotSp(CSISystemContext, ItemText.Text, "", ref Message, ref Key);
                        if (string.IsNullOrEmpty(Message))
                        {
                            LotEdit.Text = Key;
                        }
                        else
                        {
                            //LotValidated = false;
                        }
                    }
                    else
                    {
                        LotValidated = true;
                    }
                }
                else
                {
                    LotValidated = true;
                }
            }
            EnableDisableComponents();
            return LotValidated;
        }

        private bool ValidateSN()
        {
            if (SNTracked)
            {
                SNPicked = SNs.Count == int.Parse(QtyEdit.Text);
            }
            else
            {
                SNPicked = true;
            }
            return SNPicked;
        }

        private void EnableDisableComponents()
        {
            try
            {
                if (string.IsNullOrEmpty(JobEdit.Text))
                {
                    //QtyLinearLayout.Visibility = ViewStates.Gone;
                    //FromLinearLayout.Visibility = ViewStates.Gone;
                    //ToLinearLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    //QtyLinearLayout.Visibility = ViewStates.Visible;
                    //FromLinearLayout.Visibility = ViewStates.Visible;
                    //ToLinearLayout.Visibility = ViewStates.Visible;
                }
                LotLinearLayout.Visibility = LotTracked ? ViewStates.Visible : ViewStates.Gone;
                LotScanButton.Enabled = LotTracked;
                SNButton.Visibility = SNTracked ? ViewStates.Visible : ViewStates.Gone;
                SNButton.Enabled = SNTracked;
                ProcessButton.Enabled = JobValidated && SuffixValidated && QtyValidated && OperNumValidated
                    && LocValidated && ((LotTracked && LotValidated) || !LotTracked) && ((SNTracked && SNPicked) || !SNTracked);
            }catch (Exception Ex)
            {
                WriteErrorLog(Ex);
            }
        }

        private async void LotScanButton_Click(object sender, EventArgs e)
        {
            string ScanResult = await CSISanner.ScanAsync();
            if (string.IsNullOrEmpty(ScanResult))
            {
                return;
            }
            if (!AnalysisScanResult(ScanResult))
            {
                LotEdit.Text = ScanResult;
                if (!ValidateLot())
                {
                    LotEdit.RequestFocus();
                }
                else
                {
                    if (SNTracked)
                    {
                        SNButton.RequestFocus();
                    }
                    else
                    {
                        ProcessButton.RequestFocus();
                    }
                }
            }
        }

        private async void LocScanButton_Click(object sender, EventArgs e)
        {
            string ScanResult = await CSISanner.ScanAsync();
            if (string.IsNullOrEmpty(ScanResult))
            {
                return;
            }
            if (!AnalysisScanResult(ScanResult))
            {
                LocEdit.Text = ScanResult;
                if (!ValidateLoc())
                {
                    LocEdit.RequestFocus();
                }
                else
                {
                    if (LotTracked)
                    {
                        LotEdit.RequestFocus();
                    }
                    else
                    {
                        if (SNTracked)
                        {
                            SNButton.RequestFocus();
                        }
                        else
                        {
                            ProcessButton.RequestFocus();
                        }
                    }
                }
            }
        }

        private async void QtyScanButton_Click(object sender, EventArgs e)
        {
            string ScanResult = await CSISanner.ScanAsync();
            if (string.IsNullOrEmpty(ScanResult))
            {
                return;
            }
            if (!AnalysisScanResult(ScanResult))
            {
                QtyEdit.Text = ScanResult;
                if (!ValidateQty())
                {
                    QtyEdit.RequestFocus();
                }
                else
                {
                    LocEdit.RequestFocus();
                }
            }
        }

        private async void OperNumScanButton_Click(object sender, EventArgs e)
        {
            string ScanResult = await CSISanner.ScanAsync();
            if (string.IsNullOrEmpty(ScanResult))
            {
                return;
            }
            if (!AnalysisScanResult(ScanResult))
            {
                OperNumEdit.Text = ScanResult;
                if (!ValidateOperNum())
                {
                    OperNumEdit.RequestFocus();
                }
                else
                {
                    QtyEdit.RequestFocus();
                }
            }
        }

        private async void SuffixScanButton_Click(object sender, EventArgs e)
        {
            string ScanResult = await CSISanner.ScanAsync();
            if (string.IsNullOrEmpty(ScanResult))
            {
                return;
            }
            if (!AnalysisScanResult(ScanResult))
            {
                SuffixEdit.Text = ScanResult;
                if (!ValidateSuffix())
                {
                    SuffixEdit.RequestFocus();
                }
                else
                {
                    OperNumEdit.RequestFocus();
                }
            }
        }

        private async void JobScanButton_Click(object sender, EventArgs e)
        {
            string ScanResult = await CSISanner.ScanAsync();
            if (string.IsNullOrEmpty(ScanResult))
            {
                return;
            }
            if (!AnalysisScanResult(ScanResult))
            {
                JobEdit.Text = ScanResult;
                if (!ValidateJob())
                {
                    JobEdit.RequestFocus();
                }
                else
                {
                    SuffixEdit.RequestFocus();
                }
            }
        }

        private bool AnalysisScanResult(string Result)
        {
            //this is designed for future scan enhancement, such as scan one code to fill in all stuff...
            bool rtn = CSIDcJsonObjects.ReadJobReceiptJson(Result, out string Job, out string Suffix, out string OperNum, out string Qty, out string Loc, out string Lot);
            if (rtn)
            {
                if (!string.IsNullOrEmpty(Job))
                {
                    JobEdit.Text = Job;
                    JobValidated = false;
                    ValidateJob();
                }
                if (!string.IsNullOrEmpty(Suffix))
                {
                    SuffixEdit.Text = Suffix;
                    SuffixValidated = false;
                    ValidateSuffix();
                }
                if (!string.IsNullOrEmpty(OperNum))
                {
                    OperNumEdit.Text = OperNum;
                    OperNumValidated = false;
                    ValidateOperNum();
                }
                if (!string.IsNullOrEmpty(Qty))
                {
                    QtyEdit.Text = Qty;
                    QtyValidated = false;
                    ValidateQty();
                }
                if (!string.IsNullOrEmpty(Loc))
                {
                    LocEdit.Text = Loc;
                    LocValidated = false;
                    ValidateLoc();
                }
                if (!string.IsNullOrEmpty(Lot))
                {
                    LotEdit.Text = Lot;
                    LotValidated = false;
                    ValidateLot();
                }
                if (SNTracked)
                {
                    SNPicked = false;
                    ValidateSN();
                }
                EnableDisableComponents();
                try
                {
                    ProcessButton_Click(null, null);
                }catch(Exception Ex)
                {
                    WriteErrorLog(Ex);
                }
            }
            return rtn;
        }

        private bool PerformValidation()
        {
            TransDateText.Text = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            return ValidateJob() && ValidateSuffix() && ValidateQty() && ValidateOperNum() && ValidateLoc() && ValidateLot();
        }

        private void SetSNLabel()
        {
            SNButton.Text = string.Format(GetString(Resource.String.SNPicked), SNs.Count.ToString());
        }

        private void ShowProgressBar(bool show)
        {
            if (show)
            {
                ProcessCount += 1;
                ProgressBar.Visibility = ViewStates.Visible;
                CSIBaseObject.DisableEnableControls(false, Layout);
            }
            else
            {
                ProcessCount -= ProcessCount == 0 ? 0 : 1;
                if (ProcessCount == 0)
                {
                    ProgressBar.Visibility = ViewStates.Invisible;
                    CSIBaseObject.DisableEnableControls(true, Layout);

                    EnableDisableComponents();
                }
            }
            CloseImage.Visibility = HasTitle ? ViewStates.Gone : ViewStates.Visible;
        }

        public static void RunFragment(CSIBaseActivity activity)
        {
            try
            {
                FragmentTransaction ft = activity.FragmentManager.BeginTransaction();

                DCJobReceiptFragment JobReceiptDialog = (DCJobReceiptFragment)activity.FragmentManager.FindFragmentByTag("JobReceipt");
                if (JobReceiptDialog != null)
                {
                    ft.Show(JobReceiptDialog);
                    //ft.AddToBackStack(null);
                }
                else
                {
                    // Create and show the dialog.
                    JobReceiptDialog = new DCJobReceiptFragment(activity);
                    //Add fragment
                    JobReceiptDialog.Show(ft, "JobReceipt");
                }
            }
            catch (Exception Ex)
            {
                CSIErrorLog.WriteErrorLog(Ex);
            }
        }
    }
}
