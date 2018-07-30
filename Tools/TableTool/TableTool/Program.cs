using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using ProtoBuf;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace TableTool {
    class Program {

        /// <summary>
        /// 格式化模版
        /// </summary>
        static string format = @"using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using ProtoBuf;

[ProtoBuf.ProtoContract]
public class [NAME]
{
    [ProtoBuf.ProtoMember(1)]
    public string name { get; set; }

    private List<[NAME]Item> _items = new List<[NAME]Item>();
    [ProtoBuf.ProtoMember(2)]
    public List<[NAME]Item> items { get { return _items; } }
}

[ProtoBuf.ProtoContract]
public class [NAME]Item
{
    [BODY]
}
        ";

        /// <summary>
        /// 生成类模版
        /// </summary>
        static string template = @"
public class [NAME]Creator {
    public void Execute() {
        [NAME] obj = new [NAME]();
        obj.name = [XNAME];

        [BODY]

        using (MemoryStream ms = new MemoryStream()) { 
            Serializer.Serialize<[NAME]>(ms, obj);
            var data = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(data, 0, data.Length);
            File.WriteAllBytes([PATH], data);
        }
    }
}
";
        static string codetemplate = @"
using System;
using System.IO;
using ProtoBuf;
using UnityEngine;
using FirClient.Manager;

public class [NAME]Data {
    private static [NAME]Data instance;
    private [NAME] tableData;
    private ResourceManager resMgr;

    public static [NAME]Data Instance {
        get {
            if (instance == null)
                instance = new [NAME]Data();
            return instance;
        }
    }

	public [NAME]Data() {
		this.LoadData([PATH]);
	}

	public void LoadData(string path) {
		var data = resMgr.LoadAsset<TextAsset>(path);
		if (data != null) {
            using (MemoryStream ms = new MemoryStream(data.bytes)) {
		        tableData = Serializer.Deserialize<[NAME]>(ms);
            }
		}
	}

	public [NAME]Item[] Get[NAME]Items() {
		return tableData.items.ToArray();
	}
}
";
        /// <summary>
        /// 管理器调用表ITEM代码
        /// </summary>
        static string managerItemCode = @"
    public [NAME]Item[] Get[NAME]Items() {
        return [NAME]Data.Instance.Get[NAME]Items();
    }
";
        /// <summary>
        /// 管理器代码模版
        /// </summary>
        static string managerCode = @"
using UnityEngine;
using System.Collections;

public class TableManager : MonoBehaviour {
    [BODY]
}

";

        static string toolPath = string.Empty;
        static string mgrCode = string.Empty;
        static Dictionary<string, string> valueType = new Dictionary<string, string>();

        static void Main(string[] args) {
            if (args.Length != 3) return;
            mgrCode = string.Empty;
            toolPath = args[0].Trim();
            string[] files = Directory.GetFiles(args[1].Trim());

            for (int i = 0; i < files.Length; i++) {
                Start(files[i], args[2].Trim());
            }

            ///生成TableManager类
            var destDir = args[2].Trim();
            string mgrPath = destDir.Replace("Scripts\\Tables", "Scripts\\Manager");
            var content = managerCode.Replace("[BODY]", mgrCode);
            File.WriteAllText(mgrPath + "\\TableManager.cs", content);
        }

        static void Start(string excelPath, string destDir) {
            valueType.Clear();
            string name = Path.GetFileNameWithoutExtension(excelPath);
            name += "Table";

            ///读取数据
            DataSet dataSet = GetDataTable(excelPath, "Sheet1$");
            DataTable table = dataSet.Tables[0];
            var types = table.Rows[0];
            var header = table.Rows[1];
            string body = string.Empty;

            for (int i = 0; i < table.Columns.Count; i++) {
                DataColumn col = table.Columns[i];
                string key = header[col].ToString();
                string value = types[col].ToString().ToLower();
                valueType.Add(key, value);

                body += "    [ProtoBuf.ProtoMember(" + (i + 1) + ")]\n";
                body += "    public " + value + " " + key + " { get; set; } \n\n";
            }
            string txtCode = format.Replace("[NAME]", name).Replace("[BODY]", body.Trim());
            File.WriteAllText(destDir + name + ".cs", txtCode);

            ///生成二进制文件
            string code = string.Empty;
            for (int i = 2; i < table.Rows.Count; i++) {
                var row = table.Rows[i];
                code += "        " + name + "Item item = new " + name + "Item();\n";
                for (int j = 0; j < table.Columns.Count; j++) {
                    DataColumn col = table.Columns[j];
                    string prop = header[col].ToString();
                    var type = valueType[prop];
                    var value = row[col].ToString();

                    switch (type) {
                        case "int":
                            code += "        item." + prop + " = " + value + ";\n";
                        break;
                        case "string":
                            code += "        item." + prop + " = \"" + value + "\";\n";
                        break;
                    }
                }
                code += "        obj.items.Add(item);\n\n";
            }
            ///序列化成数据文件
            string binPath = destDir.Replace("Scripts\\Tables", "Resources\\Tables");
            string dataPath = "\"" + (binPath + name + ".bytes").Replace('\\', '/') + "\"";

            string content = template.Replace("[NAME]", name).Replace("[BODY]", code.Trim());
            content = content.Replace("[XNAME]", "\"" + name + "\"").Replace("[PATH]", dataPath);
            content = (txtCode + content).Trim();
            //File.WriteAllText(destDir + name + "Creator.cs", content);
            CompileCode(name, content);

            ///生成工程调用类
            content = codetemplate.Replace("[NAME]", name).Replace("[PATH]", "\"Tables/" + name + ".bytes\"");
            File.WriteAllText(destDir + name + "Data.cs", content);

            ///收集管理器代码
            mgrCode += managerItemCode.Replace("[NAME]", name);
        }

        static void ExecuteOne(string proc, string args, string dir) {
            Console.WriteLine(proc + " " + args);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = proc;
            info.Arguments = args;
            info.WindowStyle = ProcessWindowStyle.Minimized;
            info.UseShellExecute = true;
            info.WorkingDirectory = dir;

            Process pro = Process.Start(info);
            pro.WaitForExit();
        }

        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="className"></param>
        /// <param name="filename"></param>
        static void CompileCode(string className, string classCode) {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp"); 
            CompilerParameters paras = new CompilerParameters();
            paras.ReferencedAssemblies.Add("System.dll");
            paras.ReferencedAssemblies.Add("System.Xml.dll");
            paras.ReferencedAssemblies.Add(toolPath + "\\ProtoBuf-net.dll");
            paras.GenerateExecutable = false;
            paras.GenerateInMemory = true; 

            CompilerResults result = provider.CompileAssemblyFromSource(paras, classCode);
            if (result.Errors.HasErrors) {
                string ErrorMessage = "";
                foreach (CompilerError err in result.Errors) {
                    ErrorMessage += err.ErrorText;
                }
                Console.WriteLine(ErrorMessage);
            } else {
                string clsName = className + "Creator";
                object instance = result.CompiledAssembly.CreateInstance(clsName);
                Type classType = result.CompiledAssembly.GetType(clsName);
                try {
                    classType.GetMethod("Execute").Invoke(instance, null);
                } catch (Exception) {
                    Console.WriteLine("Execute Error!");
                } 
            }
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        static void SetProperty(object objClass, string propertyName, object value) {
            PropertyInfo[] infos = objClass.GetType().GetProperties();
            foreach (PropertyInfo info in infos) {
                if (info.Name == propertyName && info.CanWrite) {
                    info.SetValue(objClass, value, null);
                }
            }
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="propertyName"></param>
        static object GetProperty(object objClass, string propertyName) {
            PropertyInfo[] infos = objClass.GetType().GetProperties();
            foreach (PropertyInfo info in infos) {
                if (info.Name == propertyName && info.CanRead) {
                    return info.GetValue(objClass, null);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        static DataSet GetDataTable(string path, string tblName) {
            string connStr = string.Empty;
            string sqlstr = "Select * FROM [{0}]";

            if (path.EndsWith(".xls"))
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

            OleDbConnection conn = null;
            OleDbDataAdapter da = null;
            DataSet dsItem = new DataSet();
            try {
                conn = new OleDbConnection(connStr);
                conn.Open();

                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                da = new OleDbDataAdapter();
                for (int i = 0; i < dtSheetName.Rows.Count; i++) {
                    var sheetName = (string)dtSheetName.Rows[i]["TABLE_NAME"];

                    if (sheetName.Equals(tblName)) {
                        da.SelectCommand = new OleDbCommand(string.Format(sqlstr, tblName), conn);
                        da.Fill(dsItem, tblName);
                        break;
                    }
                }
            } finally {
                if (da != null) da.Dispose();
                if (conn != null) conn.Dispose();
            }
            return dsItem;
        }
    }
}
