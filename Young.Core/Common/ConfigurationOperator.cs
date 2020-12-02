using System.Configuration;
using System.Windows.Forms;

namespace buliding.Common
{ 
    /// <summary> 
    /// 说明：Config文件类型枚举， 
   /// 分别为asp.net网站的config文件和WinForm的config文件 
    /// 作者：周公 
    /// 日期：2008-08-23 
    /// 首发地址：http://blog.csdn.net/zhoufoxcn/archive/2008/08/24/2823508.aspx 
    /// </summary> 
    public enum ConfigType 
    { 
        /// <summary> 
        /// asp.net网站的config文件 
        /// </summary> 
        WebConfig = 1, 
        /// <summary> 
        /// Windows应用程序的config文件 
        /// </summary> 
        ExeConfig = 2 
    } 

    /// <summary> 
    /// 说明：本类主要负责对程序配置文件(.config)进行修改的类， 
    /// 可以对网站和应用程序的配置文件进行修改 
    /// 作者：周公 
    /// 日期：2008-08-23 
    /// 首发地址：http://blog.csdn.net/zhoufoxcn/archive/2008/08/24/2823508.aspx 
    /// </summary> 
    public class ConfigurationOperator 
    { 
        private Configuration config; 
        private string configPath; 
        private ConfigType configType; 

        /// <summary> 
        /// 对应的配置文件 
        /// </summary> 
        public Configuration Configuration 
        { 
            get { return config; } 
            set { config = value; } 
        } 
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="configType">.config文件的类型，只能是网站配置文件或者应用程序配置文件</param> 
        public ConfigurationOperator(ConfigType configType) 
        { 
            this.configType = configType; 
            if (configType == ConfigType.ExeConfig) 
            { 
                configPath = Application.ExecutablePath; //AppDomain.CurrentDomain.BaseDirectory; 
            } 
            //else 
            //{ 
            //    configPath = HttpContext.Current.Request.ApplicationPath; 
            //} 
            Initialize(); 
        } 
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="configPath">.config文件的位置</param> 
        /// <param name="configType">.config文件的类型，只能是网站配置文件或者应用程序配置文件</param> 
        public ConfigurationOperator(string configPath, ConfigType configType) 
        { 
            this.configPath = configPath; 
            this.configType = configType; 
            Initialize(); 
        } 
        //实例化configuration,根据配置文件类型的不同，分别采取了不同的实例化方法 
        private void Initialize() 
        { 
            //如果是WinForm应用程序的配置文件 
            if (configType == ConfigType.ExeConfig) 
            { 
                config = System.Configuration.ConfigurationManager.OpenExeConfiguration(configPath); 
            } 
            //else//WebForm的配置文件 
            //{ 
            //    config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(configPath); 
            //} 
        } 

        /// <summary> 
        /// 添加应用程序配置节点，如果已经存在此节点，则会修改该节点的值 
        /// </summary> 
        /// <param name="key">节点名称</param> 
        /// <param name="value">节点值</param> 
        public void AddAppSetting(string key, string value) 
        { 
            AppSettingsSection appSetting = (AppSettingsSection)config.GetSection("appSettings"); 
            if (appSetting.Settings[key] == null)//如果不存在此节点，则添加 
            { 
                appSetting.Settings.Add(key, value); 
            } 
            else//如果存在此节点，则修改 
            { 
                ModifyAppSetting(key, value); 
            } 
        } 
        /// <summary> 
        /// 添加数据库连接字符串ConnectionString节点，如果已经存在此节点，则会修改该节点的值 
        /// </summary> 
        /// <param name="key">节点名称</param> 
        /// <param name="connectionString">节点值</param> 
        /// <param name="proName">节点值</param> 
        public void AddConnectionString(string key, string connectionString,string proName) 
        { 
            ConnectionStringsSection connectionSetting = (ConnectionStringsSection)config.GetSection("connectionStrings"); 
            if (connectionSetting.ConnectionStrings[key] == null)//如果不存在此节点，则添加 
            { 
                ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings(key, connectionString); 
                connectionSetting.ConnectionStrings.Add(connectionStringSettings); 
            } 
            else//如果存在此节点，则修改 
            { 
                ModifyConnectionString(key, connectionString,proName); 
            } 
        } 
        /// <summary> 
        /// 修改应用程序配置AppSetting节点，如果不存在此节点，则会添加此节点及对应的值 
        /// </summary> 
        /// <param name="key">节点名称</param> 
        /// <param name="newValue">节点值</param> 
        public void ModifyAppSetting(string key, string newValue) 
        { 
            AppSettingsSection appSetting = (AppSettingsSection)config.GetSection("appSettings"); 
            if (appSetting.Settings[key] != null)//如果存在此节点，则修改 
            { 
                appSetting.Settings[key].Value = newValue; 
            } 
            else//如果不存在此节点，则添加 
            { 
                AddAppSetting(key, newValue); 
            } 
        } 
        /// <summary> 
        /// 修改数据库连接字符串ConnectionString节点，如果不存在此节点，则会添加此节点及对应的值 
        /// </summary> 
        /// <param name="key">节点名称</param> 
        /// <param name="connectionString">节点值</param> 
        /// <param name="proName">节点值</param> 
        public void ModifyConnectionString(string key, string connectionString,string proName) 
        { 
            ConnectionStringsSection connectionSetting = (ConnectionStringsSection)config.GetSection("connectionStrings"); 
            if (connectionSetting.ConnectionStrings[key] != null)//如果存在此节点，则修改 
            { 
                connectionSetting.ConnectionStrings[key].ConnectionString = connectionString;
                connectionSetting.ConnectionStrings[key].ProviderName = proName;
            } 
            else//如果不存在此节点，则添加 
            { 
                AddConnectionString(key, connectionString,proName);
            } 
        } 
        /// <summary> 
        /// 保存所作的修改 
        /// </summary> 
        public void Save() 
        { 
            config.Save(); 
        } 
    } 
}
