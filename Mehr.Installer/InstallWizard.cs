using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using DevExpress.XtraLayout.Utils;
using System.Data.Sql;
using System.IO;
using Mehr.Installer.Properties;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using MehrNet.MySqlServer;
using Microsoft.Win32;

namespace Mehr.Installer
{
    public partial class InstallWizard : DevExpress.XtraEditors.XtraForm
    {
        public InstallWizard()
        {
            MehrNet.MySqlServer.MyConnection.R_ConnectionUserPass = MehrSqlSetting.mehruserpass();
            InitializeComponent();
            EditorHelpers.CreateLanguagePrefixImageComboBox(LanguageComboBox.Properties, null);
            changecustomculture();
            MyInitialize();
        }

        #region Fields
        int LangInfFile = 1;
        bool ResetApp = false;
        string DbName = "";
        string OldMehrPath = "";
        string InstallPath = "D:\\";
        string DefaulInstalPath = "";
        string ApplicationPath = Application.StartupPath + "\\";
        string SqlInstanceName = "";
        string DefaultInstanceName = "MehrSql2014";
        List<string> ErrList;
        List<string> InstanceNameList;
        InstallType installType = InstallType.New_Mehr;
        Files ManageFile;
        bool ShowDowloadPage = true;
        long MehrFileSizeTotal = 0;
        long MehrFileSizeReseived = 0;
        long FontSizeTotal = 0;
        long FontSizeReseived = 0;
        long SqlserverSizeTotal = 0;
        long SqlserverSizeReseived = 0;
        bool IsFontDownloadCompleted = false;
        bool IsMehrDownloadCompleted = false;
        bool IsSqlDownloadCompleted = false;
        bool SqlInstalledSuccessfully = false;
        int BugRepoertId = 0;
        long compressedSize = 0;
        long extractedSizeTotal = 0;
        long fileSize = 0;
        string compressedFileName = "";
        #endregion

        #region Default Value and Initialize
        public void MyInitialize()
        {
            SetDefault_DbName();
            //SqlInstanceNameTxt.EditValue = SqlInstanceName;
            SetAndCreateDefaultPath();
            InstallKindCommentLay.Text = Resources.InstallKindComment;
            InstallKindComment1Lay.Text = Resources.InstallKindComment1;
            NewInstanceCommentLay.Text = Resources.NewInstanceComment;
            NewInstanceComment1Lay.Text = Resources.NewInstanceComment1;
            OldInstanceCommentLay.Text = Resources.OldInstanceComment;
            InstanceNameTxt.EditValue = DefaultInstanceName;
            FontInstallChk.Checked = false;
            VersionLbl.Text = $"Version:{Application.ProductVersion}";
        }
        public void SetDefault_DbName()
        {
            if (LangInfFile == 1)
            {
                DbName = $"h{ShamsiDate.GetCurrentYear()}";
            }
            else
            {
                DbName = $"h{DateTime.Now.Year}";
            }
            DbNameTxt.EditValue = DbName;
        }

        public void SetAndCreateDefaultPath()
        {
            string defaultPath = $"D:\\Mehr_Folder\\Mehr_{DbName}";
            DefaulInstalPath = defaultPath;
            if (Directory.Exists(defaultPath))
            {
                InstallPathTxt.EditValue = "";
            }
            else
            {
                InstallPathTxt.EditValue = defaultPath;
            }
        }
        #endregion

        #region Language
        private void changecustomculture()
        {
            string culturenamestr = checkcultureincommandline();
            CultureInfo systemculture = Thread.CurrentThread.CurrentCulture;
            CultureInfo currentculture = new CultureInfo(culturenamestr);
            currentculture.NumberFormat = systemculture.NumberFormat;
            currentculture.DateTimeFormat = systemculture.DateTimeFormat;
            Thread.CurrentThread.CurrentUICulture = currentculture;
            LanguageComboBox.SelectedIndex = LangInfFile;
            //AppVar.Culcure = currentculture
        }
        private string checkcultureincommandline()
        {
            try
            {

                if (!File.Exists(ApplicationPath + "Language.inf"))
                {
                    FileStream LanFile = File.Create(ApplicationPath + "Language.inf");
                    LanFile.Dispose();
                    File.WriteAllText(ApplicationPath + "Language.inf", "1");
                }
                int Language = Convert.ToInt32(File.ReadAllText(ApplicationPath + "Language.inf"));
                LangInfFile = Language;
                string Res = "fa";
                switch (Language)
                {
                    case 1:
                        Res = "fa";
                        break;
                    case 0:
                        Res = "en";
                        break;
                    case 2:
                        Res = "ar";
                        break;
                    default:
                        break;
                }
                return Res;
            }
            catch (Exception)
            {
                LangInfFile = 1;
                return "fa";
            }

        }
        public void SaveLanguage()
        {
            try
            {
                File.WriteAllText(Application.StartupPath + "\\Language.inf", LanguageComboBox.SelectedIndex.ToString());
            }
            catch (Exception)
            {

            }
        }
        private void LanguageComboBox_SelectedValueChanged_1(object sender, EventArgs e)
        {
            SaveLanguage();
            if (ResetApp)
            {
                Application.Restart();
            }
            ResetApp = true;
        }
        #endregion

        #region Page1 actions
        public void Fill_FirstPage()
        {
            InstallPath = InstallPathTxt.Text.Trim();
            OldMehrPath = OldMehrPathTxt.Text.Trim();
            DbName = DbNameTxt.Text.Trim();
        }
        public bool IsValid_Page1()
        {
            Fill_FirstPage();
            bool result = true;
            ErrList = new List<string>();
            if (installType == InstallType.New_Mehr)
            {
                if (InstallPath == "" || InstallPath == null)
                {
                    ErrList.Add(Resources.InstallPathNotInsertError);
                    return false;
                }
                if (NewMehrPathIsExists())
                {
                    if (!NewMehrPathIsEpmty())
                    {
                        ErrList.Add(Resources.NewMehrPathMustBeEmptyError);
                        result = false;
                    }
                }
                else
                {
                    try
                    {
                        CreateNewPathDirectory();
                    }
                    catch (Exception)
                    {
                        ErrList.Add(Resources.ErrorWhileCreateNewMehrDirectory);
                        result = false;
                    }
                }
            }
            if (installType == InstallType.ExistsBefore)
            {
                if (OldMehrPath == "" || OldMehrPath == null)
                {
                    ErrList.Add(Resources.OldMehrPathNotSelectedError);
                    result = false;
                }
                if (!ValidOldPath())
                {
                    ErrList.Add(Resources.OldPathWrongDataNotfountError);
                    result = false;
                }
            }
            return result;
        }
        public bool NewMehrPathIsExists()
        {
            if (Directory.Exists(InstallPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool NewMehrPathIsEpmty()
        {
            if (NewMehrPathIsExists())
            {
                string[] dir = Directory.GetDirectories(InstallPath);
                string[] files = Directory.GetFiles(InstallPath);
                if (dir.Length != 0 || files.Length != 0)
                {
                    return false;
                }
            }
            return true;
        }
        public void CreateNewPathDirectory()
        {
            try
            {
                Directory.CreateDirectory(InstallPath);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ValidOldPath()
        {
            string dataPath = Path.Combine(OldMehrPath, "data", "MehrData.mdf");
            if (File.Exists(dataPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetDatabasePath
        {
            get
            {
                if (installType == InstallType.New_Mehr)
                {
                    return Path.Combine(InstallPath, "data", "MehrData.mdf");
                }
                else
                {
                    return Path.Combine(OldMehrPath, "data", "MehrData.mdf");
                }
            }
        }
        public string GetMehrExePath
        {
            get
            {
                if (installType == InstallType.New_Mehr)
                {
                    return Path.Combine(InstallPath, "Mehr.net.exe");
                }
                else
                {
                    return Path.Combine(OldMehrPath, "Mehr.net.exe");
                }
            }
        }
        public string GetLocalSettinPath
        {
            get
            {
                if (installType == InstallType.New_Mehr)
                {
                    return Path.Combine(InstallPath, "LocalSetting.inf");
                }
                else
                {
                    return Path.Combine(OldMehrPath, "LocalSetting.inf");
                }
            }
        }

        private void InstallPathTxt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (InstallPathFolderBrowseDialog.ShowDialog() == DialogResult.OK)
                InstallPathTxt.EditValue = InstallPathFolderBrowseDialog.SelectedPath;
        }

        private void OldMehrPathTxt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (InstallPathFolderBrowseDialog.ShowDialog() == DialogResult.OK)
                OldMehrPathTxt.EditValue = InstallPathFolderBrowseDialog.SelectedPath;
        }
        private void InstallTypeRadioGroup_EditValueChanged(object sender, EventArgs e)
        {
            short SelectedValue = Convert.ToInt16(InstallTypeRadioGroup.EditValue);
            if (SelectedValue == 100)
            {
                InstallPathLay.Visibility = LayoutVisibility.Always;
                OldMehrPathLay.Visibility = LayoutVisibility.Never;
                installType = InstallType.New_Mehr;
            }
            else if (SelectedValue == 200)
            {
                InstallPathLay.Visibility = LayoutVisibility.Never;
                OldMehrPathLay.Visibility = LayoutVisibility.Always;
                installType = InstallType.ExistsBefore;
            }
        }
        public enum InstallType
        {
            New_Mehr = 100,
            ExistsBefore = 200
        }
        private void welcomeWizardPage1_PageValidating(object sender, DevExpress.XtraWizard.WizardPageValidatingEventArgs e)
        {

            FirstPageErrorMemo.Text = "";
            e.Valid = IsValid_Page1();
            if (ErrList.Count > 0)
            {
                FirstPageErrorMemoLay.Visibility = LayoutVisibility.Always;
                foreach (var R in ErrList)
                {
                    FirstPageErrorMemo.Text += R + Environment.NewLine;
                }
            }

        }

        #endregion

        #region Action_SecondPage
        private void Second_DbConfig_WizardPage_PageInit(object sender, EventArgs e)
        {
            var h = Loading.Show(this);
            GetDataSources();
            Loading.Close(h);
        }
        public void CheckInstanceName()
        {

        }

        private void GetDataSources()
        {
            InstanceNameList = new List<string>();
            RegistryKey localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey sqlKey32 = localMachine32.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
            if (sqlKey32 != null)
            {
                var list32 = sqlKey32.GetValueNames();
                if (list32.Any())
                    InstanceNameList.AddRange(list32);
            }

            RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey sqlKey64 = localMachine64.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");

            if (sqlKey64 != null)
            {
                var list64 = sqlKey64.GetValueNames();
                if (list64.Any())
                    InstanceNameList.AddRange(list64);
            }
            if (InstanceNameList.Any())
            {
                int i = 0;
                int Index = 0;
                foreach (string instance in InstanceNameList)
                {
                    SqlInstanceCMB.Properties.Items.Add(instance);
                    if (instance.ToLower() == DefaultInstanceName.ToLower())
                    {
                        InstanceNameTxt.EditValue = "";
                        Index = i;
                    }
                    i += 1;
                }
                InstallSqlserverCHK.Checked = false;
                SqlInstanceCMB.SelectedIndex = Index;
            }
            else
            {
                InstallSqlserverCHK.Checked = true;
            }

            //DataTable instances = SqlDataSourceEnumerator.Instance.GetDataSources();
            //if (instances != null)
            //{
            //    InstanceNameList = new List<string>();
            //    int i = 0;
            //    int Index = 0;
            //    foreach (DataRow instance in instances.AsEnumerable())
            //    {
            //        string Ins = instance["InstanceName"].ToString();
            //        InstanceNameList.Add(Ins);
            //        SqlInstanceCMB.Properties.Items.Add(Ins);
            //        if (Ins.ToLower() == DefaultInstanceName.ToLower())
            //        {
            //            InstanceNameTxt.EditValue = "";
            //            Index = i;
            //        }
            //        i += 1;
            //    }
            //    InstallSqlserverCHK.Checked = false;
            //    SqlInstanceCMB.SelectedIndex = Index;
            //}
            //else
            //{
            //    InstallSqlserverCHK.Checked = true;
            //}
        }

        private void InstallSqlserverCHK_CheckedChanged(object sender, EventArgs e)
        {
            if (InstallSqlserverCHK.Checked)
            {
                InstanceNameTxtLay.Visibility = LayoutVisibility.Always;
                SqlInstanceCMBLay.Visibility = LayoutVisibility.Never;
                NewInstanceCommentLay.Visibility = LayoutVisibility.Always;
                NewInstanceComment1Lay.Visibility = LayoutVisibility.Always;
                OldInstanceCommentLay.Visibility = LayoutVisibility.Never;
            }
            else
            {
                InstanceNameTxtLay.Visibility = LayoutVisibility.Never;
                SqlInstanceCMBLay.Visibility = LayoutVisibility.Always;
                NewInstanceCommentLay.Visibility = LayoutVisibility.Never;
                NewInstanceComment1Lay.Visibility = LayoutVisibility.Never;
                OldInstanceCommentLay.Visibility = LayoutVisibility.Always;
            }
        }

        public void Fill_SecondPage()
        {
            if (InstallSqlserverCHK.Checked)
            {
                SqlInstanceName = InstanceNameTxt.Text.Trim();
            }
            else
            {
                SqlInstanceName = SqlInstanceCMB.Text.Trim();
            }
        }
        public bool IsValid_Page2()
        {
            Fill_SecondPage();
            bool result = true;
            ErrList = new List<string>();
            if (DataTools.IsNullOrEmpty(SqlInstanceName))
            {
                ErrList.Add(Resources.SqlInstanceNotInsertErr);
                result = false;
            }
            if (InstanceNameList.Count > 0 & InstallSqlserverCHK.Checked)
            {
                foreach (var item in InstanceNameList)
                {
                    if (item.ToLower() == SqlInstanceName.ToLower())
                    {
                        ErrList.Add(Resources.SameInstanceNameErr);
                        result = false;
                    }
                }
            }
            return result;
        }

        private void Second_DbConfig_WizardPage_PageValidating(object sender, DevExpress.XtraWizard.WizardPageValidatingEventArgs e)
        {
            SecondPageErrorMemo.Text = "";
            e.Valid = IsValid_Page2();
            if (ErrList.Count > 0)
            {
                SecondPageErrorMemoLay.Visibility = LayoutVisibility.Always;
                foreach (var R in ErrList)
                {
                    SecondPageErrorMemo.Text += R + Environment.NewLine;
                }
            }
            else
            {
                SecondPageErrorMemoLay.Visibility = LayoutVisibility.Never;
            }
        }

        #endregion

        #region Action_ThirdPage
        private void Third_Download_WizardPage_PageInit(object sender, EventArgs e)
        {
            Third_Download_WizardPage.AllowBack = false;
            Third_Download_WizardPage.AllowNext = false;
            int bit = Info.GetOperationSystemBit();
            ManageFile = new Files();
            Font_ShowProgressbar_Setting();
            Mehr_ShowProgressbar_Setting();
            Sql_ShowProgressbar_Setting();
            if (IsSqlDownloadCompleted && IsMehrDownloadCompleted && IsFontDownloadCompleted)
            {
                wizardControl1.SetNextPage();
            }
        }
        public void Font_ShowProgressbar_Setting()
        {
            if (FontInstallChk.Checked & !ManageFile.IsExistsFarsFontZipFile())
            {
                FontDownloadLbl.Visibility = LayoutVisibility.Always;
                FontDownloadPrgBarLay.Visibility = LayoutVisibility.Always;
                FontFileSizelbl.Visibility = LayoutVisibility.Always;
                ShowDowloadPage = true;
                FontbackgroundWorker.RunWorkerAsync();
            }
            else
            {
                FontDownloadLbl.Visibility = LayoutVisibility.Never;
                FontDownloadPrgBarLay.Visibility = LayoutVisibility.Never;
                FontFileSizelbl.Visibility = LayoutVisibility.Never;
                ShowDowloadPage = false;
                IsFontDownloadCompleted = true;
            }
        }
        public void Mehr_ShowProgressbar_Setting()
        {
            if (!ManageFile.IsExistsMehrZipFile())
            {
                MehrDownloadLbl.Visibility = LayoutVisibility.Always;
                MehrDownloadProbarLay.Visibility = LayoutVisibility.Always;
                MehrFileSizelbl.Visibility = LayoutVisibility.Always;
                ShowDowloadPage = true;
                MehrbackgroundWorker.RunWorkerAsync();
            }
            else
            {
                MehrDownloadLbl.Visibility = LayoutVisibility.Never;
                MehrDownloadProbarLay.Visibility = LayoutVisibility.Never;
                MehrFileSizelbl.Visibility = LayoutVisibility.Never;
                ShowDowloadPage = false;
                IsMehrDownloadCompleted = true;
            }
        }
        public void Sql_ShowProgressbar_Setting()
        {
            int bit = Info.GetOperationSystemBit();
            bool result = (bit == 64 ? ManageFile.IsExistsSql64BitZipFile() : ManageFile.IsExistsSql32BitZipFile());
            if (!result && InstallSqlserverCHK.Checked)
            {
                SqlDownloadLbl.Visibility = LayoutVisibility.Always;
                SqlDownloadProBarLay.Visibility = LayoutVisibility.Always;
                SqlFileSizelbl.Visibility = LayoutVisibility.Always;
                ShowDowloadPage = true;
                SqlbackgroundWorker.RunWorkerAsync();
            }
            else
            {
                SqlDownloadLbl.Visibility = LayoutVisibility.Never;
                SqlDownloadProBarLay.Visibility = LayoutVisibility.Never;
                SqlFileSizelbl.Visibility = LayoutVisibility.Never;
                ShowDowloadPage = false;
                IsSqlDownloadCompleted = true;
            }
        }
        private void FontbackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MehrDownloadProbar.Properties.Maximum = 100;
            using (WebClient wc = new WebClient())
            {
                var notifier = new AutoResetEvent(false);
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers.Add("Cache-Control", "no-cache");
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                wc.DownloadProgressChanged += (s, ee) =>
                {
                    FontbackgroundWorker.ReportProgress(ee.ProgressPercentage);
                    FontSizeReseived = ee.BytesReceived;
                    FontSizeTotal = ee.TotalBytesToReceive;
                    if (FontSizeReseived >= FontSizeTotal) notifier.Set();
                };

                wc.DownloadFileAsync(new Uri(ManageFile.FontFileDownloadAddress), ManageFile.FontFileZipPath);
                // wait for signal to proceed
                notifier.WaitOne();
            }
        }
        private void MehrbackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MehrDownloadProbar.Properties.Maximum = 100;
            using (WebClient wc = new WebClient())
            {
                var notifier = new AutoResetEvent(false);
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers.Add("Cache-Control", "no-cache");
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                wc.DownloadProgressChanged += (s, ee) =>
                {
                    MehrbackgroundWorker.ReportProgress(ee.ProgressPercentage);
                    MehrFileSizeReseived = ee.BytesReceived;
                    MehrFileSizeTotal = ee.TotalBytesToReceive;
                    if (MehrFileSizeReseived >= MehrFileSizeTotal) notifier.Set();
                };
                wc.DownloadFileAsync(new Uri(ManageFile.MehrFileDownloadAddress), ManageFile.MehrFileZipPath);

                // wait for signal to proceed
                notifier.WaitOne();
            }
        }
        private void SqlbackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SqlDownloadProBar.Properties.Maximum = 100;
            using (WebClient wc = new WebClient())
            {
                var notifier = new AutoResetEvent(false);
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers.Add("Cache-Control", "no-cache");
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                wc.DownloadProgressChanged += (s, ee) =>
                {
                    SqlbackgroundWorker.ReportProgress(ee.ProgressPercentage);
                    SqlserverSizeReseived = ee.BytesReceived;
                    SqlserverSizeTotal = ee.TotalBytesToReceive;
                    if (SqlserverSizeReseived >= SqlserverSizeTotal) notifier.Set();
                };
                string downlaodAddress = (Info.GetOperationSystemBit() == 64 ? ManageFile.Sql64BitDownloadAddress : ManageFile.Sql32BitDownloadAddress);
                string savePath = (Info.GetOperationSystemBit() == 64 ? ManageFile.Sql64BitZipPath : ManageFile.Sql32BitZipPath);
                wc.DownloadFileAsync(new Uri(downlaodAddress), savePath);

                // wait for signal to proceed
                notifier.WaitOne();
            }
        }
        private void MehrbackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MehrDownloadProbar.Position = e.ProgressPercentage;
            MehrFileSizelbl.Text = $"{(MehrFileSizeReseived / 1024D / 1024D).ToString("0.00")} MB's/{(MehrFileSizeTotal / 1024D / 1024D).ToString("0.00")} MB's";
        }
        private void FontbackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FontDownloadPrgBar.Position = e.ProgressPercentage;
            FontFileSizelbl.Text = $"{(FontSizeReseived / 1024D / 1024D).ToString("0.00")} MB's/{(FontSizeTotal / 1024D / 1024D).ToString("0.00")} MB's";
        }
        private void SqlbackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SqlDownloadProBar.Position = e.ProgressPercentage;
            SqlFileSizelbl.Text = $"{(SqlserverSizeReseived / 1024D / 1024D).ToString("0.00")} MB's/{(SqlserverSizeTotal / 1024D / 1024D).ToString("0.00")} MB's";
        }
        private void MehrbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsMehrDownloadCompleted = true;
            if (IsFontDownloadCompleted & IsMehrDownloadCompleted & IsSqlDownloadCompleted)
            {
                wizardControl1.SetNextPage();
            }
        }
        private void FontbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsFontDownloadCompleted = true;
            if (IsFontDownloadCompleted & IsMehrDownloadCompleted & IsSqlDownloadCompleted)
            {
                wizardControl1.SetNextPage();
            }
        }
        private void SqlbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsSqlDownloadCompleted = true;
            if (IsFontDownloadCompleted & IsMehrDownloadCompleted & IsSqlDownloadCompleted)
            {
                wizardControl1.SetNextPage();
            }
        }

        #endregion

        #region Action_FourthPage
        private void Fourth_Install_WizardPage_PageInit(object sender, EventArgs e)
        {
            Fourth_Install_WizardPage.AllowNext = false;
            Fourth_Install_WizardPage.AllowBack = false;
            ErrList = new List<string>();
            // if (installType == InstallType.New_Mehr) ManageFile.UnzipMehrFile();

            MehrProgressBarSetting();
            SqlProgressBarSetting();
            MehrInstallProgressBar.Properties.Maximum = int.MaxValue;
            MehrExtractIndividualPB.Properties.Maximum = int.MaxValue;
            MehrInstallLblLay.Text = Resources.WhileExtractMehrFileText;
            MehrExtractBackgroundWorker.RunWorkerAsync();
            //MehrInstallBackgroundWorker.RunWorkerAsync();
        }
        private void MehrExtractBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (installType == InstallType.New_Mehr)
                {

                    ManageFile.ExtractProgressEventHandler += (count) =>
                    {
                        MehrExtractBackgroundWorker.ReportProgress((int)count);
                    };
                    ManageFile.UnzipMehrFile(ref fileSize, ref extractedSizeTotal, ref compressedSize, ref compressedFileName);
                }
            }
            catch (Exception ex)
            {
                ErrList.Add($"1018-{ex.Message}-{ex.InnerException.Message}");
            }
        }
        private void MehrExtractBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            MehrExtractIndividualPB.Position = e.ProgressPercentage;
            MehrExtractFileNamelbl.Text = compressedFileName;
            //calculate the totalPercent
            long totalPercent = ((long)e.ProgressPercentage * compressedSize + extractedSizeTotal * int.MaxValue) / fileSize;
            if (totalPercent > int.MaxValue)
                totalPercent = int.MaxValue;
            MehrInstallProgressBar.Position = (int)totalPercent;
        }
        private void MehrExtractBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MehrExtractFileNamelblLay.Visibility = LayoutVisibility.Never;
            MehrExtractIndividualPBLay.Visibility = LayoutVisibility.Never;
            MehrInstallLblLay.Text = Resources.WhileInstallMehrFileText;
            ManageFile = new Files();
            MehrInstallBackgroundWorker.RunWorkerAsync();
            MehrInstallProgressBar.Properties.Maximum = ManageFile.GetCountFile(ManageFile.MehrFilePath);
        }

        private void MehrInstallBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (installType == InstallType.New_Mehr)
                {
                    ManageFile.CopyProgressEventHnadler += (count) =>
                    {
                        MehrInstallBackgroundWorker.ReportProgress(count);
                    };

                    ManageFile.InstallMehr(InstallPathTxt.Text);
                }
            }
            catch (Exception ex)
            {
                ErrList.Add($"1018-{ex.Message}");
            }

        }
        private void MehrInstallBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MehrInstallProgressBar.Position = e.ProgressPercentage;
        }

        private void MehrInstallBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (InstallSqlserverCHK.Checked)
            {
                SqlInstallProgressBar.Properties.Maximum = int.MaxValue;
                SqlExtractIndividualPB.Properties.Maximum = int.MaxValue;
                SqlInstallProgressBar.Position = 0;
                SqlExtractIndividualPB.Position = 0;
                SqlExtractIndividualPBLay.Visibility = LayoutVisibility.Always;
                SqlExtractFileNamelblLay.Visibility = LayoutVisibility.Always;
                SqlInstallLay.Text = Resources.WhileExtractSqlFileText;
            }
            SqlExtractBackgroundWorker.RunWorkerAsync();
        }

        public void MehrProgressBarSetting()
        {
            if (installType == InstallType.New_Mehr)
            {
                // MehrInstallProgressBar.Properties.Maximum = ManageFile.GetCountFile(ManageFile.MehrFilePath);
                MehrInstallProgressBarLay.Visibility = LayoutVisibility.Always;
                MehrInstallLblLay.Visibility = LayoutVisibility.Always;
            }
            else
            {
                MehrInstallProgressBarLay.Visibility = LayoutVisibility.Never;
                MehrInstallLblLay.Visibility = LayoutVisibility.Never;
            }
        }
        public void SqlProgressBarSetting()
        {
            if (InstallSqlserverCHK.Checked)
            {
                SqlInstallProgressBar.Properties.Maximum = 100;
                SqlInstallProgressBarLay.Visibility = LayoutVisibility.Always;
                SqlInstallLay.Visibility = LayoutVisibility.Always;
            }
            else
            {
                SqlInstallProgressBarLay.Visibility = LayoutVisibility.Never;
                SqlInstallLay.Visibility = LayoutVisibility.Never;
            }
        }

        #region SqlExtractBackgroundWorker
        private void SqlExtractBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InstallSqlserverCHK.Checked)
            {
                ManageFile.ExtractProgressEventHandler += (progress) =>
                {
                    SqlExtractBackgroundWorker.ReportProgress(progress);
                };
                if (Info.GetOperationSystemBit() == 64)
                {
                    ManageFile.UnzipSql64File(ref fileSize, ref extractedSizeTotal, ref compressedSize, ref compressedFileName);
                }
                else
                {
                    ManageFile.UnzipSql32File(ref fileSize, ref extractedSizeTotal, ref compressedSize, ref compressedFileName);
                }

            }
        }
        private void SqlExtractBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SqlExtractIndividualPB.Position = e.ProgressPercentage;
            SqlExtractFileNamelbl.Text = compressedFileName;
            //calculate the totalPercent
            long totalPercent = ((long)e.ProgressPercentage * compressedSize + extractedSizeTotal * int.MaxValue) / fileSize;
            if (totalPercent > int.MaxValue)
                totalPercent = int.MaxValue;
            SqlInstallProgressBar.Position = (int)totalPercent;
        }

        private void SqlExtractBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SqlExtractIndividualPBLay.Visibility = LayoutVisibility.Never;
            SqlExtractFileNamelblLay.Visibility = LayoutVisibility.Never;
            SqlInstallLay.Text = Resources.WhileInstallSqlFileText;
            SqlInstallProgressBar.Properties.Maximum = 100;
            SqlInstallBackgroundWorker.RunWorkerAsync();
        }
        #endregion
        private void SqlInstallBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InstallSqlserverCHK.Checked)
            {
                try
                {
                    SqlExpInstall sql = new SqlExpInstall();
                    sql.INSTANCENAME = SqlInstanceName;
                    sql.INSTANCEID = SqlInstanceName;
                    if (Info.GetOperationSystemBit() == 64)
                    {
                        sql.FilePath = Path.Combine(ManageFile.Sql64BitPath, "setup.exe");
                    }
                    else
                    {
                        sql.FilePath = Path.Combine(ManageFile.Sql32BitPath, "setup.exe");
                    }
                    sql.InstallProgressEventHnadler += (count) =>
                    {
                        SqlInstallBackgroundWorker.ReportProgress(count);
                    };
                    sql.InstallExpress();
                    SqlInstalledSuccessfully = true;
                }
                catch (Exception ex)
                {
                    ErrList.Add(ex.Message);
                    SqlInstalledSuccessfully = false;
                    PublicTools.SendBugReport(ex, System.Reflection.MethodBase.GetCurrentMethod(), "");
                }
            }
            else
            {
                SqlInstalledSuccessfully = true;
            }
        }
        private void SqlInstallBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SqlInstallProgressBar.Position = e.ProgressPercentage;
        }
        private void SqlInstallBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SqlInstalledSuccessfully & ErrList.Count == 0)
            {
                Thread.Sleep(3000);
                AttachDb();
            }
            else
            {
                wizardControl1.SetNextPage();
            }
        }
        public void AttachDb()
        {
            var h = Loading.Show(this);
            try
            {
                bool res = MehrSqlSetting.AttachDataBastToMehr(SqlInstanceName, DbName, GetDatabasePath);
                if (res)
                {
                    MyConnection connection = new MyConnection(".", SqlInstanceName, DbName, 15);
                    if (connection.TestConectivity() == MyConnection.ConectionConectivityEnum.Connected)
                    {
                        MehrSqlSetting.SqlSettingInit(connection);
                        if (installType == InstallType.New_Mehr)
                        {
                            MehrSqlSetting.SetAsFirstTimeAppOpen(connection);
                        }
                        Files.SaveConnectionSetting(connection.CnnStringDisplay().ToLower(), GetLocalSettinPath);
                    }
                    {

                    }
                }
                else
                {
                    ErrList.Add(Resources.ErrorWhileAttachingDataBase);
                }
            }
            catch (Exception ex)
            {
                ErrList.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    ErrList.Add(ex.InnerException.Message);
                }
                PublicTools.SendBugReport(ex, System.Reflection.MethodBase.GetCurrentMethod(), "");
            }
            finally
            {
                Loading.Close(h);
                wizardControl1.SetNextPage();

            }
        }

        #endregion

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //PublicTools.SendBugReport(new Exception("Test"), System.Reflection.MethodBase.GetCurrentMethod(), "sql2014");
            //PublicTools.InstallerLog("", 0, true);
            try
            {
                bool res = MehrSqlSetting.AttachDataBastToMehr("mehrsqlserver", DbNameTxt.Text, InstallPathTxt.Text + @"\Data\MehrData.mdf");
                if (res)
                {
                    MyConnection connection = new MyConnection(".", "mehrsqlserver", DbNameTxt.Text, 15);
                    if (connection.TestConectivity() == MyConnection.ConectionConectivityEnum.Connected)
                    {
                        //MehrSqlSetting.MakeDatabaseUser(connection);
                        MehrSqlSetting.SqlSettingInit(connection);
                        Files.SaveConnectionSetting(connection.CnnStringDisplay().ToLower(), InstallPathTxt.Text + @"\LocalSetting.inf");
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Connect error");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Attach Error ");

                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message);
            }
            //finally
            //{
            //    wizardControl1.SetNextPage();
            //}
            // PublicTools.CreateShortcut(@"D:\Mehr_Folder\New folder (4)\Mehr.net.exe","h1402");
        }



        #region Compelete Page
        private void completionPage_PageInit(object sender, EventArgs e)
        {
            var h = Loading.Show(this);
            try
            {
                if (ErrList.Count == 0)
                {
                    completionPage.Text = Resources.InstallSuccessfulMsg;
                    FinalErrorMemo.Visible = false;
                    PublicTools.CreateShortcut(GetMehrExePath, DbName);
                    Thread.Sleep(1000);
                    Process.Start(GetMehrExePath);
                    this.Close();
                }
                else
                {
                    completionPage.Text = Resources.SomeErrorRunWhileInstallationError;
                    foreach (var R in ErrList)
                    {
                        FinalErrorMemo.Text += R + Environment.NewLine;
                    }
                    completionPage.AllowBack = false;
                }
            }
            catch (Exception ex)
            {
                PublicTools.SendBugReport(ex, System.Reflection.MethodBase.GetCurrentMethod(), "sql2014");
            }
            finally
            {
                bool isSuccess = (ErrList.Count == 0 ? true : false);
                PublicTools.InstallerLog(FinalErrorMemo.Text, BugRepoertId, isSuccess);
                Loading.Close(h);
            }

        }
        #endregion

        private void wizardControl1_FinishClick(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        private void wizardControl1_HelpClick(object sender, DevExpress.XtraWizard.WizardButtonClickEventArgs e)
        {
            WebApiCls webApi = new WebApiCls();
            StringBuilder sb = new StringBuilder();
            sb.Append($"{webApi.BaseUrl}document/Index/1");
            // sb.Append("Document/1");
            Process.Start(sb.ToString());
        }


    }
}