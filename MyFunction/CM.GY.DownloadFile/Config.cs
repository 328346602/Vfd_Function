using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Configuration;

using System.IO;
using System.Data;
using ICSharpCode.SharpZipLib.Zip;

namespace CM.GY.DownloadFile
{
    public class Config
    {
        private static string connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getConnectionString()
        {
            try
            {

                string sysconfigPath = AppDomain.CurrentDomain.BaseDirectory.ToString()+"Content//sysconfig.xml";
                XmlDocument doc = new XmlDocument();
                doc.Load(sysconfigPath);
                XmlElement rootElem = doc.DocumentElement;
                XmlNodeList dbNodes = doc.DocumentElement.ChildNodes;
                foreach (XmlElement element in dbNodes)
                {
                    if (element.Name.ToString() == "DbConn_File")
                    {
                        
                        connectionString = element.InnerText;
                        //Tool.WriteLog(connectionString);
                    }
                }
                return connectionString;
               
            }
            catch (Exception ex)
            {
                Tool.WriteLog("Config.getConnectionString>>>>>"+ ex.Message);
                throw ex;
            }
        }

        public static DatabaseORC setConnectionString()
        {
            try
            {
                DatabaseORC db = new DatabaseORC(getConnectionString());
                return db;
            }
            catch(Exception ex)
            {
                Tool.WriteLog("Config.setConnectionString>>>>>"+ex.Message);
                throw ex;
            }
        } 

    }

    public class Tool
    {
        public static void WriteLog(string sMsg)
        {
            try
            {
                string sUrl = System.Web.HttpContext.Current.Server.MapPath("~/TempFile");
                string sPath = sUrl + "/Test" + System.DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
                WebUse.Logs.WriteLog(sPath, sMsg);
            }
            catch (Exception oExcept)
            {
                WebUse.Logs.WriteLog(System.Web.HttpContext.Current.Server.MapPath("~/TempFile") + ".txt", "写日志方法出错--------" + oExcept.Message);
            }
        }


        /// <summary>
        /// 将指定目录下的所有文件压缩，打包成一个名为fileName的rar文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        public static void BuildZipFile(string dir,string fileName)
        {
            try
            {
                fileName += ".rar";//压缩文件名
                if (!File.Exists(fileName))
                {
                    string[] fileNames = Directory.GetFiles(dir);//待打包附件名的集合

                    #region 生成压缩文件
                    //Tool.WriteLog("生成压缩文件");
                    using (ZipOutputStream s = new ZipOutputStream(File.Create(fileName)))
                    {
                        s.SetLevel(1);
                        byte[] buffer = new byte[4096];
                        foreach (string file in fileNames)
                        {
                            ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                            entry.DateTime = DateTime.Now;
                            s.PutNextEntry(entry);
                            using (FileStream fs = File.OpenRead(file))
                            {

                                // Using a fixed size buffer here makes no noticeable difference for output
                                // but keeps a lid on memory usage.
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    s.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                            //try
                            //{
                            //    Tool.WriteLog(file);
                            //    File.Delete(file);
                            //}
                            //catch(Exception ex)
                            //{
                            //    throw ex;
                            //}
                        }

                        s.Finish();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WriteLog("Tool.BuildZipFile>>>>>" + ex.Message);
                throw ex;
            }
            
        }
       

        /// <summary>
        /// 从数据库中查找到关联记录，并将二进制字段输出成本地文件
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <returns></returns>
        public static string OutputFiles(string CaseNo)
        {
            try
            {
                DatabaseORC db = new DatabaseORC(Config.getConnectionString());//从配置文件读取数据库配置，并建立连接
                string SQL = "select t.MaterialName,t.FileName from upfileslist t where t.CASENO='" + CaseNo + "'";//数据查询SQL，不读取二进制文件以提高查询效率
                DataSet ds = db.GetDataSet(SQL);//获取DataSet
                string filePath =string.Empty;
                //每一个CaseNo对应一个专有文件夹存放文件，首次下载时生成，以后下载无需重复生成
                string folderName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "//TempFile//" + CaseNo;//临时文件夹命名
                if (ds.Tables.Count != 0)
                {
                    DataTable dt = ds.Tables[0];
                    if(dt.Rows.Count>0)
                    {
                        #region rar文件不存在时，从数据库中提取二进制文件，若存在则不提取
                        if ((!File.Exists(folderName + "\\" + dt.Rows[0][0].ToString() + ".rar")))
                        {
                            SQL = "select t.MaterialName,t.FileName,t.FileContent from upfileslist t where t.CASENO='" + CaseNo + "'";//数据查询SQL，读取二进制文件以便生成下载数据
                            dt = db.GetDataSet(SQL).Tables[0];
                            byte[] temp;//保存二进制临时文件
                            string tempName = string.Empty;//临时文件名
                            System.IO.Directory.CreateDirectory(folderName);//创建临时文件夹
                            //Tool.WriteLog("rar文件不存在时，从数据库中提取二进制文件，若存在则不提取");
                            #region 遍历数据库提取文件
                            for (int i = 0; i != dt.Rows.Count; ++i)
                            {
                                //System.IO.Directory.CreateDirectory(folderName+"//"+dt.Rows[i][0]);
                                temp = (byte[])dt.Rows[i][2];
                                tempName = folderName + "//" + dt.Rows[i][1];
                                File.WriteAllBytes(tempName, temp);
                            }
                            #endregion

                            //BuildZipFile(folderName, folderName + "//" + dt.Rows[0][0].ToString());//将所有文件打包成一个rar文件
                            BuildZipFile(folderName, folderName + "//" + CaseNo);
                            #region 生成rar文件后将缓存文件删除
                            string[] fileNames = Directory.GetFiles(folderName);
                            foreach (string file in fileNames)
                            {
                                if (!file.EndsWith(".rar"))
                                {
                                    File.Delete(file);
                                }
                            }
                            #endregion

                        }
                        #endregion
                    }
                    else
                    {
                        return "0";
                    }
                    filePath = "//TempFile//" + CaseNo + "//" + CaseNo + ".rar";//生成返回值，代表rar文件的相对路径
                }
                else
                {
                    return "0";
                }
                return filePath;
            }
            catch (Exception ex)
            {
                WriteLog("Tool.OutputFiles>>>>>"+ex.Message);
                throw ex;
            }
        }

        public static string OutputRarFile(string CaseNo)
        {
            try
            {
                #region 定义变量
                DatabaseORC db = new DatabaseORC(Config.getConnectionString());//建立数据库连接
                string SQL = "select t.MaterialName,t.FileName from upfileslist t where t.CASENO='" + CaseNo + "'";//数据查询SQL，不读取二进制文件以提高查询效率
                DataSet ds = db.GetDataSet(SQL);//获取SQL查询到的数据
                #endregion

                if (ds.Tables.Count>0)//判断查询到的ds中是否有数据
                {
                    DataTable dt = ds.Tables[0];
                    if(dt.Rows.Count>0)//判断目标dt中是否有数据
                    {
                        string filePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "/TempFile/" + CaseNo;//文件保存路径
                        string rarName = filePath + "/" + CaseNo + ".rar";//生成打包文件的路径
                        
                        if (!File.Exists(rarName))
                        {
                            if(!File.Exists(rarName))
                            {
                                #region 定义变量以及创建必要的路径
                                System.IO.Directory.CreateDirectory(filePath);//创建临时文件夹
                                byte[] tempBytes;//保存二进制临时文件
                                System.IO.Directory.CreateDirectory(filePath);//创建临时文件夹
                                SQL = "select t.MaterialName,t.FileName,t.FileContent from upfileslist t where t.CASENO='" + CaseNo + "'";//数据查询SQL，读取二进制文件以便生成下载数据
                                dt = db.GetDataSet(SQL).Tables[0];//保存新查到的数据
                                ZipOutputStream zos = new ZipOutputStream(File.Create(rarName));//创建rar文件
                                zos.SetLevel(1);
                                byte[] buffer = new byte[4096];
                                #endregion

                                #region 遍历数据库提取文件，文件存放在对应的目录
                                for (int i = 0; i != dt.Rows.Count; ++i)
                                {
                                    ZipEntry entry = new ZipEntry(dt.Rows[i][0] + "/" + dt.Rows[i][1]);//rar中新建压缩文件入口以存放数据
                                    zos.PutNextEntry(entry);
                                    tempBytes = (byte[])dt.Rows[i][2];
                                    /*
                                    using (MemoryStream ms = new MemoryStream((byte[])dt.Rows[i][2]))
                                    {
                                        int sourceBytes;
                                        do
                                        {
                                            sourceBytes = ms.Read(buffer, 0, buffer.Length);
                                            zos.Write(buffer, 0, sourceBytes);
                                        } while (sourceBytes > 0);
                                    }
                                    zos.Finish();
                                     * */
                                    zos.Write(tempBytes,0,tempBytes.Count());
                                }
                                //zos.Finish();
                                #endregion

                                zos.Close();
                               
                            }
                        }
                        string rarPath = "/TempFile/" + CaseNo + "/" + CaseNo + ".rar";
                        return rarPath;
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch(Exception ex)
            {
                WriteLog("rarFile>>>>>"+ex.Message);
                throw ex;
            }
        }
    }
}