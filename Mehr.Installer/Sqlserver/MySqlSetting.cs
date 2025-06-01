using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Mehr.Installer.Properties;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using MehrNet.MySqlServer;

namespace Mehr.Installer
{
    public class MehrSqlSetting
    {
        public static string mehruserpass()
        {
            string S = "et.bas";
            return "bas" + "tan" + ".n123456tanuser".Replace("123456", S) + (7 * 1000 + 940 + 9 / 3).ToString();
        }
        public static void MakeDatabaseUser(MehrNet.MySqlServer.MyConnection CNN, bool ForceCreate = false, bool CreateOnMehrDBToo = true)
        {
            string UserName = "mehruser";
            try
            {
                try
                {
                    if (ForceCreate)
                    {
                        CNN.Execute("DROP USER [" + UserName + "]");
                    }
                }
                catch (Exception ex)
                {

                }
                CNN.Execute("CREATE LOGIN [" + UserName + "] WITH PASSWORD=N'" + mehruserpass() + "', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF");
            }
            catch (Exception)
            {

            }
            if (CreateOnMehrDBToo)
            {
                try
                {
                    MehrNet.MySqlServer.CustomCommand UserCmd = new MehrNet.MySqlServer.CustomCommand("CreateBastanUser", CNN);
                    UserCmd.Parameters("@userName", UserName);
                    UserCmd.Execute();
                    UserCmd.Dispose();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        internal static MyConnection ForceToGetOneConnection()
        {
            var Cnn = GetDefaultConnection(true);
            if (Cnn == null)
            {
                Application.Exit();
            }
            return Cnn;
        }

        public static bool AttachDataBastToMehr(string Server, string DataBaseName, string Path)
        {
            try
            {
                AttachDB(Server, DataBaseName, Path);
                var Cnn = new MehrNet.MySqlServer.MyConnection(".", Server, DataBaseName, 30, true);
                MehrSqlSetting.MakeDatabaseUser(Cnn);
                return true;
            }
            catch (Exception ex)
            {
                throw;
                // throw new Exception(Resources.ErrorWhileAttachingDataBase, ex);
            }
        }
        public static MyConnection GetDefaultConnection(bool ForceToSelectOne = false)
        {
            MyConnection Cnn = null;
            try
            {
                try
                {
                    string DefCnnStr = GetDefaultCnnString();

                    // if there is no any saved setting we check first old version standard
                    if (DefCnnStr is null || DefCnnStr.Length == 0)
                    {
                        try
                        {
                            string Str = GetOldVersionCnnString();
                            if (Str != null & Str.Length > 0)
                            {
                                MyConnection TempCnn = MyConnection.GetConnectionFromCnnStr(Str);
                                if (TempCnn != null)
                                {
                                    SaveAsDefaultCnnString(TempCnn.CnnStringDisplay());
                                    DefCnnStr = TempCnn.CnnStringDisplay();
                                }
                            }

                            // try to delete oldversion file
                            try
                            {
                                File.Delete(Application.StartupPath + "setting.inf");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        catch
                        {
                        }
                    }

                    // at this time we may have setting
                    if (DefCnnStr != null || DefCnnStr.Length > 0)
                    {
                        Cnn = new MyConnection(DefCnnStr);
                    }
                }


                catch (Exception ex)
                {
                    Cnn = null;
                }


                if (ForceToSelectOne & (Cnn is null || Cnn.TestConectivity() != MyConnection.ConectionConectivityEnum.Connected))
                {
                    do
                    {
                        // Cnn = ServerSelectForm.UserMakesNewConnection();
                        //    if (Cnn != null)
                        //return Cnn;
                        //    if (MyMsg.MyMsgBox(global::My.Resources.ConnectionErrorMsg, MsgBoxStyle.Question | MsgBoxStyle.RetryCancel) != MsgBoxResult.Retry)
                        //    {
                        //        if (MyMsg.MyMsgBox(global::My.Resources.NoConnectionExitMsg, MsgBoxStyle.YesNo | MsgBoxStyle.DefaultButton2) == MsgBoxResult.Yes)
                        //            return default;
                        //    }
                    }
                    while (true);
                }
                else
                {
                    return Cnn;
                }
            }
            catch (Exception ex)
            {
                //MyMsg.MyErrorMsgBox(ex, default, default, EmailErrMsg(ex, MethodBase.GetCurrentMethod()));
                return null;
            }
        }

        public static string GetDefaultCnnString()
        {
            return "";// MySetting.FileGetSetting("cnnstring00", null, MySetting.SettingType.Program);
        }

        public static void SaveAsDefaultCnnString(string CnnStringDisplay)
        {
            // MySetting.FileSaveSetting("cnnstring00", CnnStringDisplay, MySetting.SettingType.Program);
        }

        private static string GetOldVersionCnnString()
        {
            try
            {
                string fileContents;
                string[] S;
                string Resault = null;
                //fileContents = global::My.Computer.FileSystem.ReadAllText(myTools.IOTools.AppCurrentDir + "setting.inf");
                //S = fileContents.Split(new string[] { "*/" }, StringSplitOptions.RemoveEmptyEntries);
                //foreach (string c in S)
                //{
                //    if (c.StartsWith("ConnectionString" + "//"))
                //        Resault = c.Remove(0, String.("ConnectionString" + "//")); // must find last setting value
                //}
                return Resault;
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                //MyMsg.MyErrorMsgBox(ex, default, default, EmailErrMsg(ex, MethodBase.GetCurrentMethod()));
                return null;
            }
        }

        public static void AttachDB(string SqlServer, string DBName, string FileName)
        {
            string DBPath = new DirectoryInfo(FileName).FullName;
            if (DBPath.ToLower().EndsWith(".mdf"))
                DBPath = DBPath.Substring(0, DBPath.Length - 4);
            string SqlCmd = "CREATE DATABASE " + DBName + " ON  ( FILENAME = N'" + DBPath + ".mdf' ) , (FILENAME = N'" + DBPath + "_log.ldf') FOR   ATTACH  ";
            var CurCnn = new MyConnection(".", SqlServer, "master", 30, true);
            CurCnn.Execute(SqlCmd);
        }

        public static void DetachDB(string ServerName, string DbName)
        {
            var newConnection = new MyConnection("", ServerName, "master", 30, true);
            CustomCommand.ClearAllPools();
            string SqlCmd = "EXEC master.dbo.sp_detach_db @dbname ='" + DbName + "'";
            newConnection.Execute(SqlCmd);
            // Dim Cmd As New CustomCommand(SqlCmd, newConnection, CommandType.Text, , True, True)
            // Cmd.Execute()
        }

        public static void ShrinkDataBase(MyConnection Cnn)
        {
            try
            {
                string Sql = "DBCC SHRINKDATABASE(N'" + Cnn.DatabaseName + "',0 )";
                Cnn.Execute(Sql);
            }
            catch (Exception ex)
            {
            }
        }
        public static void SqlSettingInit(MyConnection connection)
        {
            ExecSqlError(connection);
        }

        public static void ExecSqlError(MyConnection connection)
        {
            CustomCommand CMD = new CustomCommand("STPDatabaseErrorMSGRegister", connection);
            try
            {
                CMD.Execute();
            }
            catch (Exception)
            {

            }
            finally
            {
                CMD.Dispose();
            }
        }

        //this method set mehr as first time opening in database for init setting
        public static void SetAsFirstTimeAppOpen(MyConnection connection)
        {
            CustomCommand CMD =
                new CustomCommand("Delete DataBaseSettingTBL where KeyName = 'IsFirstTimeAppOpen';" +
                "insert into DataBaseSettingTBL values ('IsFirstTimeAppOpen','True',0);"
                ,connection
                ,CommandType.Text);
            try
            {
                CMD.Execute();
            }
            catch (Exception)
            {

            }
            finally
            {
                CMD.Dispose();
            }
        }

    }

    //public partial class CustomCommand
    //{
    //    private MyConnection ConnectionObject;
    //    private System.Data.DataRow DataRow;
    //    private CustomCommand ParrentCmd;
    //    private bool Silent;
    //    private bool ClearPool;
    //    private SqlTransaction Trans = null;
    //    public SqlCommand Command = new SqlCommand();
    //    public bool DontShowValuedParamInGetParamValues;
    //    public object Tag;
    //    /// <summary>
    //    /// default connection if there is no declare conection when u r using customcommand
    //    /// </summary>
    //    public static MyConnection R_DefaultConnection;
    //    /// <summary>
    //    /// default query timeout . Default is 30
    //    /// </summary>
    //    public static int R_QueryTimeOut = 30;
    //    public event OutputParameterChangedEventHandler OutputParameterChanged;
    //    public delegate void OutputParameterChangedEventHandler(string ParamName, object Value);
    //    public static event R_GetParametersEventHandler R_GetParameters;
    //    public delegate void R_GetParametersEventHandler(CustomCommand CMD);

    //    public CustomCommand(string CmdStr, MyConnection _ConnectionObj = null, System.Data.CommandType CommandType = CommandType.StoredProcedure, CustomCommand CurrentCmd = null, bool SilentMode = false, bool ClearPoolVar = false, int QueryTimeOut = 0)
    //    {
    //        ConnectionObject = (_ConnectionObj == null ? R_DefaultConnection : _ConnectionObj);
    //        if (CurrentCmd != null)
    //        {
    //            Connection = CurrentCmd.Connection;
    //            CurrentTrans = CurrentCmd.CurrentTrans;
    //        }
    //        else
    //        {
    //            Connection = ConnectionObject.SqlConnection();
    //        }
    //        Command.CommandText = CmdStr;
    //        Command.CommandType = CommandType;
    //        Command.CommandTimeout = (QueryTimeOut == 0 ? R_QueryTimeOut : QueryTimeOut);
    //        Silent = SilentMode | ConnectionObject.SilentConnection;
    //        ClearPool = ClearPoolVar | ConnectionObject.ClearPoolEachConnection;
    //    }

    //    public void Dispose()
    //    {
    //        // to be compatible for last version
    //        if (CurrentTrans == null)
    //        {
    //            Connection.Dispose();
    //        }
    //    }

    //    private SqlConnection Connection
    //    {
    //        get
    //        {
    //            return Command.Connection;
    //        }
    //        set
    //        {
    //            Command.Connection = value;
    //        }
    //    }

    //    public SqlParameterCollection CmdParameters
    //    {
    //        get
    //        {
    //            return Command.Parameters;
    //        }
    //    }

    //    public string Commandtext
    //    {
    //        get
    //        {
    //            return Command.CommandText;
    //        }
    //    }

    //    private SqlTransaction CurrentTrans
    //    {
    //        get
    //        {
    //            return Trans;
    //        }
    //        set
    //        {
    //            Command.Transaction = value;
    //            Trans = value;
    //        }
    //    }

    //    public static void ClearAllPools()
    //    {
    //        SqlConnection.ClearAllPools();
    //    }

    //    private void Open()
    //    {
    //        if (Connection.State == ConnectionState.Open)
    //            return;
    //        do
    //        {
    //            try
    //            {
    //                ClearAllPools();
    //                Connection.Open();
    //                break;
    //            }
    //            catch (Exception ex)
    //            {
    //                if (Silent)
    //                {
    //                    throw new Exception(ex.Message);
    //                }
    //                else
    //                {
    //                    ClearAllPools();

    //                    //if (MehrNet.MyMsg.MyMsgBox(global::My.Resources.ConnectionErrorMsg, MsgBoxStyle.Question | MsgBoxStyle.RetryCancel, ex.Message, default, EmailErrMsg(ex, MethodBase.GetCurrentMethod(), "CmdTxt=" + Commandtext)) != MsgBoxResult.Retry)
    //                    //{
    //                    //    if (MehrNet.MyMsg.MyMsgBox(global::My.Resources.NoConnectionExitMsg, MsgBoxStyle.YesNo | MsgBoxStyle.DefaultButton2) == MsgBoxResult.Yes)
    //                    //        throw new NoConnectionException(global::My.Resources.CnnTestNotSuccess, ex);
    //                    //}
    //                }
    //            }
    //        }
    //        while (true);
    //    }
    //    private string EmailErrMsg(Exception ex, MethodBase ErrMethod, string AdditionalMsg = "")
    //    {
    //        string S;
    //        S = DateTime.Now.ToString();
    //        S += "Error in Sqlserver.Dll";
    //        //S += Constants.vbCrLf + "OS Version: " + global::this.Computer.Info.OSFullName + " - " + global::My.Computer.Info.OSVersion;
    //        //S += Constants.vbCrLf + "Caller Assembly: " + Assembly.GetCallingAssembly().GetName().FullName;
    //        //S += Constants.vbCrLf + "EXE Version: " + global::My.Application.Info.Version.ToString;
    //        //S += Constants.vbCrLf + "Exe Path: " + Application.ExecutablePath;
    //        //S += Constants.vbCrLf + "ConnectionString: " + Connection.ConnectionString.Replace("bastan", "mehr").Replace("bastanuser", "mehr").Replace((7000 + 943).ToString(), "2000");
    //        //S += Constants.vbCrLf + "Exeption.Function Name: " + ErrMethod.ReflectedType.FullName + "." + ErrMethod.Name;
    //        //S += Constants.vbCrLf + "Exeption.Message: " + ex.Message;
    //        //S += Constants.vbCrLf + "Exeption.InnerException: " + ex.InnerException.Message;
    //        //S += Constants.vbCrLf + AdditionalMsg;
    //        return S;
    //    }
    //    public void Parameters(params object[] Param_Name_Value)
    //    {
    //        for (int i = Param_Name_Value.GetLowerBound(0), loopTo = Param_Name_Value.GetUpperBound(0); i <= loopTo; i += 2)
    //            Command.Parameters.Add(new SqlParameter(Param_Name_Value[i].ToString(), Param_Name_Value[i + 1]));
    //    }
    //    public void ChangeParameters(params object[] Param_Name_Value)
    //    {

    //        for (int i = Param_Name_Value.GetLowerBound(0); (i <= Param_Name_Value.GetUpperBound(0)); i = (i + 2))
    //        {
    //            foreach (SqlParameter P in Command.Parameters)
    //            {
    //                if (P.ParameterName == Param_Name_Value[i].ToString())
    //                {
    //                    P.Value = Param_Name_Value[i + 1];
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    public void Parameter(string ParName, object ParValue, System.Data.ParameterDirection Direction)
    //    {
    //        var P = new SqlParameter(ParName, ParValue);
    //        P.Direction = Direction;
    //        Command.Parameters.Add(P);
    //    }
    //    public void Parameter(string ParName, object ParValue, int Paramsize, System.Data.ParameterDirection Direction)
    //    {
    //        var P = new SqlParameter(ParName, SqlDbType.NVarChar);
    //        P.Size = Paramsize;
    //        P.Direction = Direction;
    //        P.IsNullable = true;
    //        P.Value = ParValue;
    //        Command.Parameters.Add(P);
    //    }
    //    public void Parameter(string ParName, object ParValue, SqlDbType SqlType)
    //    {
    //        var P = new SqlParameter(ParName, SqlType);
    //        P.Value = ParValue;
    //        Command.Parameters.Add(P);
    //    }
    //    public object Parameter(string ParName)
    //    {
    //        return Command.Parameters[ParName].Value;
    //    }
    //    private void Close()
    //    {
    //        if (CurrentTrans != null)
    //            return;
    //        Connection.Close();
    //        if (ClearPool)
    //            SqlConnection.ClearPool(Connection);
    //    }

    //    public DataTable Fill()
    //    {
    //        using (var DA = new SqlDataAdapter(Command))
    //        {
    //            using (var DataTbl = new DataTable())
    //            {
    //                try
    //                {
    //                    Open();
    //                    do
    //                    {
    //                        try
    //                        {
    //                            DA.Fill(DataTbl);
    //                            break;
    //                        }
    //                        catch (SqlException ex)
    //                        {
    //                            //if (ex.Number != -2)
    //                            //    throw ex; // time out
    //                            //if (MyMsg.MyMsgBox(global::My.Resources.QueryTimeOutMSG, MsgBoxStyle.YesNo | MsgBoxStyle.Critical) != MsgBoxResult.Yes)
    //                            //    throw ex;
    //                            //Command.CommandTimeout = 2 * Command.CommandTimeout;
    //                        }
    //                    }
    //                    while (true);
    //                }

    //                catch (Exception ex)
    //                {
    //                    throw ex;
    //                }
    //                finally
    //                {
    //                    Close();
    //                }
    //                return DataTbl;
    //            }
    //        }
    //    }
    //    private DataTable _Read_DTRows = null;
    //    private short _Read_Index = 0;
    //    public bool Read()
    //    {
    //        if (_Read_DTRows is null)
    //        {
    //            _Read_DTRows = Fill();
    //        }
    //        if (_Read_DTRows.Rows.Count > _Read_Index)
    //        {
    //            DataRow = _Read_DTRows.Rows[_Read_Index];
    //            _Read_Index = (short)(_Read_Index + 1);
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    public object Field(string FieldName, object NullToVal = null)
    //    {
    //        var Value = DataRow[FieldName];
    //        if (Value == null)
    //        {
    //            return NullToVal;
    //        }
    //        return Value;
    //    }
    //    public int Execute(bool MustAffect = false)
    //    {
    //        try
    //        {
    //            Open();
    //            int AffectedRows;

    //            try
    //            {
    //                AffectedRows = Command.ExecuteNonQuery();
    //            }
    //            catch (SqlException ex)
    //            {
    //                throw;
    //                //if (ex.Number != -2)
    //                //    throw ex; // time out
    //                //if (MyMsg.MyMsgBox(global::My.Resources.QueryTimeOutMSG, MsgBoxStyle.YesNo | MsgBoxStyle.Critical) != MsgBoxResult.Yes)
    //                //    throw ex;
    //                //Command.CommandTimeout = 2 * Command.CommandTimeout;
    //            }

    //            if (MustAffect & AffectedRows == 0)
    //            {
    //                throw new EXCNoRowAffected();
    //            }
    //            return AffectedRows;
    //        }
    //        catch (Exception)
    //        {
    //            throw ;
    //        }
    //        finally
    //        {
    //            Close();
    //        }
    //    }
    //    public void BeginTrans()
    //    {
    //        if (CurrentTrans != null)
    //        {
    //            throw new Exception();
    //        }
    //        Open();
    //        CurrentTrans = Connection.BeginTransaction();
    //    }
    //    public void RollBack(string SavePointName = "")
    //    {
    //        CurrentTrans.Rollback();
    //        CurrentTrans.Dispose();
    //        CurrentTrans = null;
    //        Close();
    //    }
    //    public void Commit()
    //    {
    //        CurrentTrans.Commit();
    //        CurrentTrans.Dispose();
    //        CurrentTrans = null;
    //        Close();
    //    }
    //    public object ExecuteScalar()
    //    {
    //        try
    //        {
    //            Open();
    //            do
    //            {
    //                try
    //                {
    //                    return Command.ExecuteScalar();
    //                    break;
    //                }
    //                catch (SqlException ex)
    //                {
    //                    //if (ex.Number != -2)
    //                    //    throw ex; // time out
    //                    //if (MyMsg.MyMsgBox(global::My.Resources.QueryTimeOutMSG, MsgBoxStyle.YesNo | MsgBoxStyle.Critical) != MsgBoxResult.Yes)
    //                    //    throw ex;
    //                    //Command.CommandTimeout = 2 * Command.CommandTimeout;
    //                }
    //            }
    //            while (true);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            Close();
    //        }
    //    }

    //    /// <param name="CMD"></param>
    //    /// If cmd is nothing it will be changed with new one , other else it will be used for fill
    //    public DataTable SmartFill([Optional, DefaultParameterValue(null)] ref CustomCommand CMD)
    //    {
    //        DataTable SmartFillRet = null;
    //        try
    //        {
    //            R_GetParameters?.Invoke(this);
    //            SmartFillRet = Fill();
    //            foreach (SqlParameter P in CmdParameters)
    //            {
    //                if (P.Direction != ParameterDirection.Input)
    //                {
    //                    OutputParameterChanged?.Invoke(P.ParameterName.TrimStart('@'), P.Value);
    //                }
    //            }
    //        }
    //        finally
    //        {

    //        }
    //        return SmartFillRet;
    //    }

    //    public partial class NoConnectionException : Exception
    //    {
    //        public NoConnectionException(string MSG, Exception EX) : base(MSG, EX)
    //        {
    //        }
    //    }
    //    public partial class EXCNoRowAffected : Exception
    //    {
    //        public EXCNoRowAffected() : base("")
    //        {
    //        }
    //    }

    //}

}
