using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    //public class MyConnection
    //{
    //    public enum ConectionConectivityEnum
    //    {
    //        NotAvailable,
    //        Connected,
    //    }
    //    public string ComputerName;

    //    public string ServerName;

    //    public string DatabaseName;

    //    public int ConnectionTimeOut;

    //    public bool SuspendUser;

    //    public bool ClearPoolEachConnection;

    //    public bool SilentConnection;

    //    private const string ConnectionUserID = "mehruser";

    //    public static string R_ConnectionUserPass =  "bastan.net.bastanuser7943";

    //    public static string R_ConnectionUserName = "mehruser";

    //    public static int R_CurrentUserID;

    //    public string CnnStringDisplay()
    //    {
    //        return ComputerName + "\\" + ServerName + " ; " + DatabaseName + " ; " + ConnectionTimeOut.ToString();
    //    }

    //    public string ComputerServer()
    //    {
    //        return (ComputerName + ("\\" + ServerName));
    //    }

    //    public MyConnection(string _ComputerName, string _ServerName, string _DatabaseName, int _ConnectionTimeOut, bool _SuspendUser = false, bool _ClearPoolEachConnection = false, bool _SilentConnection = false, int _QueryTimeOut = 30, int _CurrentUserID = 0)
    //    {
    //        ComputerName = _ComputerName.Trim();
    //        ServerName = _ServerName.Trim();
    //        DatabaseName = _DatabaseName.Trim();
    //        ConnectionTimeOut = _ConnectionTimeOut;
    //        if (ConnectionTimeOut < 5) ConnectionTimeOut = 5;
    //        if (ConnectionTimeOut > 500) ConnectionTimeOut = 500;
    //        if (ComputerName == "" || ComputerName == "192.168.1.1") ComputerName = ".";
    //        SuspendUser = _SuspendUser;
    //        ClearPoolEachConnection = _ClearPoolEachConnection;
    //        SilentConnection = _SilentConnection;
    //        R_CurrentUserID = _CurrentUserID;
    //    }

    //    public MyConnection(string _CnnStringDisplay, bool _SuspendUser = false, bool _ClearPoolEachConnection = false, bool _SilentConnection = false, int _QueryTimeOut = 30, int _CurrentUserID = 0)
    //    {
    //        if (_CnnStringDisplay != null && _CnnStringDisplay.Length != 0)
    //        {
    //            ComputerName = _CnnStringDisplay.Split('\\')[0].Trim();
    //            ServerName = _CnnStringDisplay.Split('\\')[1].Split(';')[0].Trim();
    //            DatabaseName = _CnnStringDisplay.Split(';')[1].Trim();
    //            ConnectionTimeOut = Convert.ToInt32(_CnnStringDisplay.Split(';')[2].Trim());
    //            if (ConnectionTimeOut < 5) ConnectionTimeOut = 5;
    //            if (ConnectionTimeOut > 500) ConnectionTimeOut = 500;
    //            if (ComputerName == "" || ComputerName == "") ComputerName = ".";
    //            SuspendUser = _SuspendUser;
    //            ClearPoolEachConnection = _ClearPoolEachConnection;
    //            SilentConnection = _SilentConnection;
    //            R_CurrentUserID = _CurrentUserID;
    //        }

    //    }

    //    // '' <summary>
    //    // '' make and return connection string to use for sqlconnection
    //    // '' </summary>
    //    // '' <param name="ComputerName">nothing  , "." ,"" or any valid computer name </param>
    //    public static string CnnStringMaker(string ComputerName, string ServerName, string DatabaseName, int ConnectionTimeOut, int _CurrentUserID, bool _SuspendUser)
    //    {
    //        if (ComputerName == null || ComputerName == "") ComputerName = ".";
    //        string SecuritySection;
    //        if (R_ConnectionUserName == null || R_ConnectionUserName.Length == 0) R_ConnectionUserName = ConnectionUserID;

    //        if (_SuspendUser)
    //        {
    //            SecuritySection = ";Integrated Security=true";
    //        }
    //        else
    //        {
    //            SecuritySection = ";Integrated Security=false;" + "user id=" + R_ConnectionUserName + ";password=" + R_ConnectionUserPass + ";application name=" + _CurrentUserID.ToString() + ";Max Pool Size=1000";
    //        }

    //        return "Data Source=" + ComputerName + "\\" + ServerName + ";Initial Catalog=" + DatabaseName + SecuritySection + ";Connect Timeout=" + ConnectionTimeOut.ToString();
    //    }

    //    // '' <returns> None open connection for this instance's datas</returns>
    //    public SqlConnection SqlConnection()
    //    {
    //        return new SqlConnection(ConnectionString());
    //    }

    //    // '' <returns> An open connection for this instance's datas</returns>
    //    public SqlConnection LiveConnection()
    //    {
    //        SqlConnection CNN = this.SqlConnection();
    //        CNN.Open();
    //        return CNN;
    //    }

    //    // '' <returns> A SqlCommand ready to execute regards to instant's data and sqltext</returns>
    //    public SqlCommand LiveSQLCMD(string Text)
    //    {
    //        SqlConnection Connection = this.LiveConnection();
    //        SqlCommand CMD = new SqlCommand(Text, Connection);
    //        CMD.CommandType = CommandType.Text;
    //        return CMD;
    //    }

    //    public bool IsSqlClient()
    //    {
    //        return ((ComputerName != "") && (ComputerName != "."));
    //    }

    //    public bool NoUser()
    //    {
    //        return SuspendUser;
    //    }

    //    // '' <returns>Make and open the connection .Execute sqltext for instant's datas and return data back.Close connection</returns>
    //    public string ExecuteScalar(string Text)
    //    {
    //        SqlCommand CMD = null;
    //        try
    //        {
    //            CMD = LiveSQLCMD(Text);
    //            return Convert.ToString(CMD.ExecuteScalar());
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            try
    //            {
    //                CMD.Connection.Close();
    //            }
    //            catch (System.Exception End)
    //            {
    //                throw End;
    //            }
    //        }

    //    }

    //    public int Execute(string Text)
    //    {
    //        SqlCommand CMD = null;
    //        try
    //        {
    //            CMD = LiveSQLCMD(Text);
    //            return CMD.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            try
    //            {
    //                CMD.Connection.Close();
    //            }
    //            catch (Exception ex)
    //            {
    //            }

    //        }

    //    }

    //    public string PhyisicalName()
    //    {
    //        try
    //        {
    //            return this.ExecuteScalar("select physical_name from sys.database_files");
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }

    //    }

    //    public string ConnectionString()
    //    {
    //        return MyConnection.CnnStringMaker(ComputerName, ServerName, DatabaseName, ConnectionTimeOut, R_CurrentUserID, this.NoUser());
    //    }

    //    public ConectionConectivityEnum TestConectivity()
    //    {
    //        //MouseCursor MyMouse = new MouseCursor();
    //        try
    //        {
    //            if (ServerName == null || DatabaseName == null || ServerName == "" || DatabaseName == "")
    //            {
    //                return ConectionConectivityEnum.NotAvailable;
    //            }
    //            SqlConnection Cnn = this.LiveConnection();
    //            System.Data.SqlClient.SqlConnection.ClearPool(Cnn);
    //            Cnn.Close();
    //            return ConectionConectivityEnum.Connected;
    //        }
    //        catch (Exception ex)
    //        {
    //            return ConectionConectivityEnum.NotAvailable;
    //        }
    //        finally
    //        {
    //            //MyMouse.Release();
    //        }

    //    }

    //    public static MyConnection GetConnectionFromCnnStr(string Cnnstr)
    //    {
    //        try
    //        {
    //            if (Cnnstr != null && Cnnstr.Length != 0)
    //            {
    //                string Str;
    //                int TimeOut = 30;
    //                int StrIndex;
    //                bool NoUser = false;
    //                Cnnstr = Cnnstr.ToLower();
    //                StrIndex = (Cnnstr.IndexOf("data source=") + "data source=".Length);
    //                Str = Cnnstr.Substring(StrIndex).Split(';')[0];
    //                StrIndex = (Cnnstr.IndexOf("initial catalog=") + "initial catalog=".Length);
    //                Str = (Str + (";" + Cnnstr.Substring(StrIndex).Split(';')[0]));
    //                StrIndex = Cnnstr.IndexOf("integrated security=");
    //                if (StrIndex != -1)
    //                {
    //                    NoUser = bool.Parse(Cnnstr.Substring((StrIndex + "integrated security=".Length)).Split(';')[0]);
    //                }
    //                StrIndex = Cnnstr.IndexOf("connect timeout=");
    //                if (StrIndex != -1)
    //                {
    //                    TimeOut = Convert.ToInt32(Cnnstr.Substring((StrIndex + "connect timeout=".Length)).Split(';')[0]);
    //                }
    //                MyConnection mc = new MyConnection(Str + ";" + TimeOut.ToString());
    //                mc.SuspendUser = NoUser;
    //                return mc;
    //            }
    //            else
    //            {
    //                return null;
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }

    //    }
    //}
}
