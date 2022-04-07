using System;
using System.IO;
using AltoHttp;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;

namespace Minecraft_Server_Hub
{
    public partial class serverform1 : Form
    {
        public class Properties
        {
            private Dictionary<String, String> list;
            private String filename;

            public Properties(String file)
            {
                reload(file);
            }

            public String get(String field, String defValue)
            {
                return (get(field) == null) ? (defValue) : (get(field));
            }
            public String get(String field)
            {
                return (list.ContainsKey(field)) ? (list[field]) : (null);
            }

            public void set(String field, Object value)
            {
                if (!list.ContainsKey(field))
                    list.Add(field, value.ToString());
                else
                    list[field] = value.ToString();
            }

            public void Save()
            {
                Save(this.filename);
            }

            public void Save(String filename)
            {
                this.filename = filename;

                if (!System.IO.File.Exists(filename))
                    System.IO.File.Create(filename);

                System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

                foreach (String prop in list.Keys.ToArray())
                    if (!String.IsNullOrWhiteSpace(list[prop]))
                        file.WriteLine(prop + "=" + list[prop]);

                file.Close();
            }

            public void reload()
            {
                reload(this.filename);
            }

            public void reload(String filename)
            {
                this.filename = filename;
                list = new Dictionary<String, String>();

                if (System.IO.File.Exists(filename))
                    loadFromFile(filename);
                else
                    System.IO.File.Create(filename);
            }

            private void loadFromFile(String file)
            {
                foreach (String line in System.IO.File.ReadAllLines(file))
                {
                    if ((!String.IsNullOrEmpty(line)) &&
                        (!line.StartsWith(";")) &&
                        (!line.StartsWith("#")) &&
                        (!line.StartsWith("'")) &&
                        (line.Contains('=')))
                    {
                        int index = line.IndexOf('=');
                        String key = line.Substring(0, index).Trim();
                        String value = line.Substring(index + 1).Trim();

                        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                            (value.StartsWith("'") && value.EndsWith("'")))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        try
                        {
                            //ignore dublicates
                            list.Add(key, value);
                        }
                        catch { }
                    }
                }
            }
        }
        public string path = Directory.GetCurrentDirectory();
        public bool jarvalid;
        public string servernumberpath = "/server1/";
        public int servernumberline = 1;
        public bool cansavetruefalse = false;
        public int serverprocessid = 0;
        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
                string[] arrLine = File.ReadAllLines(fileName);
                arrLine[line_to_edit - 1] = newText;
                File.WriteAllLines(fileName, arrLine);
        }


        static public string UpperCaseFirstChar(string text)
        {
            return Regex.Replace(text, "^[a-z]", m => m.Value.ToUpper());
        }



        public serverform1()
        {
            InitializeComponent();

            

        }

        private void rename_Click(object sender, EventArgs e)
        {
            string servername = renametext.Text;
            lineChanger(servername, "servernames.txt", servernumberline);
        }

        private void openserverfolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(path + servernumberpath);
        }

        public void ToggleEasyControls(bool toggle)
        {
            changemainport.Enabled = toggle;
            changeseed.Enabled = toggle;
            changemaxplayers.Enabled = toggle;
            changemotd.Enabled = toggle;
            changegamemode.Enabled = toggle;
            changedifficulty.Enabled = toggle;
            Whitelist.Enabled = toggle;
            hardcore.Enabled = toggle;
            enablestatus.Enabled = toggle;
            enablepvp.Enabled = toggle;
            enableflight.Enabled = toggle;
            enablecommandblocks.Enabled = toggle;
        }

        public void CheckJarStatus()
        {
            if (!File.Exists(path + servernumberpath + "server.jar"))
            {
                uploadjar.Enabled = true;
                installselectedjar.Enabled = true;
                serverjarstatus.ForeColor = Color.Red;
                serverjarstatus.Text = "INVALID. CHECK JAR INSTALLATION.";
                startstopbutton.Enabled = false;
                ramselect.Visible = false;
                changerambutton.Visible = false;
            }
            else
            {
                long length = new FileInfo(path + servernumberpath + "server.jar").Length;
                if (length > 500000)
                {
                    uploadjar.Enabled = false;
                    installselectedjar.Enabled = false;
                    serverjarstatus.ForeColor = Color.Green;
                    serverjarstatus.Text = "VALID";
                    startstopbutton.Enabled = true;
                    ramselect.Visible = true;
                    changerambutton.Visible = true;
                }
                else
                {
                    uploadjar.Enabled = true;
                    installselectedjar.Enabled = true;
                    serverjarstatus.ForeColor = Color.Red;
                    serverjarstatus.Text = "INVALID. CHECK JAR INSTALLATION.";
                    startstopbutton.Enabled = false;
                    ramselect.Visible = false;
                    changerambutton.Visible = false;
                }
            }
        }

        public void ToggleBoolInputs(string propertyname, CheckBox ToToggle)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");

            string tempstring = config.get(propertyname, "1");
            bool tempbool = Convert.ToBoolean(tempstring);
            ToToggle.Checked = tempbool;
        }

        public void ToggleDropDownInputs(string propertyname, ComboBox ToToggle)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");


            string tempstring = config.get(propertyname, "1");
            tempstring = UpperCaseFirstChar(tempstring);
            ToToggle.SelectedItem = tempstring;
        }

        public void ToggleTextInputs(string propertyname, TextBox ToEnter)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");

            string tempstring = config.get(propertyname, "1");
            ToEnter.Text = tempstring;
        }
            

        public void CheckPropertiesStatus()
        {
            // NOT GETTING PROPERTIES
            if (!File.Exists(path + servernumberpath + "server.properties"))
            {
                propertiesstatus.ForeColor = Color.Red;
                propertiesstatus.Text = "INVALID. CHECK PROPERTIES INSTALLATION.";
                easycontrols.Enabled = false;
                expertconfigedit.Enabled = false;
                propertiesbutton.Enabled = false;
                whitelistsbutton.Enabled = false;

                ToggleEasyControls(false);
            } 
            else
            {
                long length = new FileInfo(path + servernumberpath + "server.properties").Length;
                if(length > 200)
                {
                    propertiesstatus.ForeColor = Color.Green;
                    propertiesstatus.Text = "VALID";
                    easycontrols.Enabled = true;
                    expertconfigedit.Enabled = true;

                    if (easycontrols.Checked == true)
                    {
                        ToggleEasyControls(true);
                        propertiesbutton.Enabled = false;
                        whitelistsbutton.Enabled = false;
                    }
                    else
                    {
                        ToggleEasyControls(false);
                        propertiesbutton.Enabled = true;
                        whitelistsbutton.Enabled = true;
                    }

                    //Getting Properties
                    Properties config = new Properties(path + servernumberpath + "server.properties");
                    ToggleBoolInputs("pvp", enablepvp);
                    ToggleBoolInputs("hardcore", hardcore);
                    ToggleBoolInputs("enable-status", enablestatus);
                    ToggleBoolInputs("allow-flight", enableflight);
                    ToggleBoolInputs("enable-command-block", enablecommandblocks);
                    ToggleBoolInputs("enforce-whitelist", Whitelist);
                    ToggleDropDownInputs("gamemode", gamemode);
                    ToggleDropDownInputs("difficulty", difficulty);
                    ToggleTextInputs("server-port", mainport);
                    ToggleTextInputs("max-players", maxplayers);
                    ToggleTextInputs("level-seed", seed);
                    ToggleTextInputs("motd", motd);
                    config.Save();

                    cansavetruefalse = true;

                    //Getting properties
                }
                else
                {
                    propertiesstatus.ForeColor = Color.Red;
                    propertiesstatus.Text = "INVALID. CHECK PROPERTIES INSTALLATION.";
                    easycontrols.Enabled = false;
                    expertconfigedit.Enabled = false;
                    propertiesbutton.Enabled = false;
                    whitelistsbutton.Enabled = false;

                    ToggleEasyControls(false);
                }
            }
        }


        private void checkinstallation_Click(object sender, EventArgs e)
        {
            CheckJarStatus();
        }

        private void serverform1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(path + servernumberpath + "startserver.bat"))
            {
                File.WriteAllText(path + servernumberpath + "startserver.bat", "cd %~dp0 \n@echo off \nfor /F %%I in (ramamount.txt) do set /A ram=%%I \nif [%ram%]==[] set /A ram=2 \ncls \njava -Xmx%ram%G -jar server.jar nogui \nPause");
            }
            //string quote = "\"";
            //File.WriteAllText(path + servernumberpath + "serverjarpath.txt", quote + path + servernumberpath + "server.jar" + quote);
            stopserver.Enabled = false;
            CheckPropertiesStatus();
            CheckJarStatus();
            propertiesbutton.Enabled = false;
            whitelistsbutton.Enabled = false;
        }

        private void externalconsole_CheckedChanged(object sender, EventArgs e)
        {
            if (externalconsole.Checked == true)
            {
                externalconsolechecked.Visible = true;
            } else
            {
                externalconsolechecked.Visible = false;
            }
        }

        public void CheckProperties()
        {
            string path = Directory.GetCurrentDirectory();
            using (StreamReader sr = new StreamReader(path + servernumberpath + "server.properties"))
            {
                string propertiescontent = sr.ReadToEnd();

                //PVP TOGGLE
                if (propertiescontent.Contains("pvp=true"))
                {
                    enablepvp.Checked = true;
                }
                else if (propertiescontent.Contains("pvp=false"))
                {
                    enablepvp.Checked = false;
                }
                else { enablepvp.Checked = false; }
                //HARCORE TOGGLE
                if (propertiescontent.Contains("hardcore=true"))
                {
                    hardcore.Checked = true;
                }
                else if (propertiescontent.Contains("hardcore=false"))
                {
                    hardcore.Checked = false;
                }
                else { hardcore.Checked = false; }
                //STATUS TOGGLE
                if (propertiescontent.Contains("enable-status=true"))
                {
                    enablestatus.Checked = true;
                }
                else if (propertiescontent.Contains("enable-status=false"))
                {
                    enablestatus.Checked = false;
                }
                else { enablestatus.Checked = false; }
                //FLIGHT TOGGLE
                if (propertiescontent.Contains("allow-flight=true"))
                {
                    enableflight.Checked = true;
                }
                else if (propertiescontent.Contains("allow-flight=false"))
                {
                    enableflight.Checked = false;
                }
                else { enableflight.Checked = false; }
                //Enable CommandBlocks
                if (propertiescontent.Contains("enable-command-block=true"))
                {
                    enablecommandblocks.Checked = true;                }
                else if (propertiescontent.Contains("enable-command-block=false"))
                {
                    enablecommandblocks.Checked = false;                }
                else { enablecommandblocks.Checked = false; }
                //Enable Whitelist
                if (propertiescontent.Contains("enforce-whitelist=true"))
                {
                    Whitelist.Checked = true;
                }
                else if (propertiescontent.Contains("enforce-whitelist=false"))
                {
                    Whitelist.Checked = false;                }
                else { Whitelist.Checked = false; }
            }
        }

        private void easycontrols_CheckedChanged(object sender, EventArgs e)
        {
            if (easycontrols.Checked == true)
            {
                propertiesbutton.Enabled = false;
                whitelistsbutton.Enabled = false;
            }
            else
            {
                ToggleEasyControls(false);
            }
            CheckProperties();
        }

        private void expertconfigedit_CheckedChanged(object sender, EventArgs e)
        {
            if (expertconfigedit.Checked == true)
            {
                propertiesbutton.Enabled = true;
                whitelistsbutton.Enabled = true;
            }
            else
            {
                ToggleEasyControls(true);
            }
            CheckProperties();
        }

        private void checkproperties_Click(object sender, EventArgs e)
        {
            CheckPropertiesStatus();
        }

        private void propertiesbutton_Click(object sender, EventArgs e)
        {
            Process.Start(path + servernumberpath + "server.properties");
        }

        public static bool CannotFind()
        {
            const string message = "Cannot find the file specified. Did you install your server correctly?";
            const string caption = "Cannot Find File";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                return true;
            } else
            {
                return false;
            }
        }

        private void whitelistsbutton_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(path + servernumberpath + "whitelist.json");
            }
            catch (Win32Exception)
            {
                CannotFind();
            }
        }

        private void uploadjar_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Jar file (*.jar)|*.jar";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string serverjarpath = path + servernumberpath;
                    var fileName = "server.jar";
                    serverjarpath = serverjarpath + fileName;
                    File.Copy(dialog.FileName, serverjarpath);
                    CheckJarStatus();
                }
            }
            catch(Exception) { }
        }
        public void DownloadJar(string paperversionlink, string selectedpaperversion)
        {
            HttpDownloader DownloadJar;
            var selectedjar = jarselect.SelectedItem;

            if ((string)selectedjar == "--- LATEST VERSION ---" || (string)selectedjar == "--- LEGACY VERSIONS ---")
            {
                const string message = "InVaLiD Selection!";
                const string caption = "InVaLiD Selection";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            if ((string)selectedjar == selectedpaperversion)
            {
                DownloadJar = new HttpDownloader(paperversionlink, path + servernumberpath + "server.jar");
                DownloadJar.DownloadCompleted += HttpDownloader_DownloadCompleted;
                DownloadJar.ProgressChanged += HttpDownloader_ProgressChanged;
                DownloadJar.Start();
            }
        }

        private void HttpDownloader_ProgressChanged(object sender, AltoHttp.ProgressChangedEventArgs e)
        {
            downloadprogress.Value = (int)e.Progress;
        }
        private void HttpDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                CheckJarStatus();
            });

        }

        private void installselectedjar_Click(object sender, EventArgs e)
        {
            //!!!!!!!!!!!!!!!!!May need to be updated, and add future versions//!!!!!!!!!!!!!!!!!
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.18.2/builds/278/downloads/paper-1.18.2-278.jar", "1.18.2 Paper");

            //Legacy Versions
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.17.1/builds/409/downloads/paper-1.17.1-409.jar", "1.17.1 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.16.5/builds/794/downloads/paper-1.16.5-794.jar", "1.16.5 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.15.2/builds/393/downloads/paper-1.15.2-393.jar", "1.15.2 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.14.4/builds/245/downloads/paper-1.14.4-245.jar", "1.14.4 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.13.2/builds/657/downloads/paper-1.13.2-657.jar", "1.13.2 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.12.2/builds/1620/downloads/paper-1.12.2-1620.jar", "1.12.2 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.11.2/builds/1106/downloads/paper-1.11.2-1106.jar", "1.11.2 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.10.2/builds/918/downloads/paper-1.10.2-918.jar", "1.10.2 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.9.4/builds/775/downloads/paper-1.9.4-775.jar", "1.9.4 Paper");
            DownloadJar("https://papermc.io/api/v2/projects/paper/versions/1.8.8/builds/445/downloads/paper-1.8.8-445.jar", "1.8.8 Paper");
        }


        private void paperlinkclick_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string message = "Paper is a fork of Spigot, and it aims to have better performance and stability. Vanilla is good, but not optimized. You can install plugins freely, and it also fixes many exploits, and bugs. \n\nHowever, this does not mean that you can't install any other versions, feel free to upload custom server jars if you know what you're doing!";
            const string caption = "Why only Paper?";
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void changegamemode_Click(object sender, EventArgs e)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");

            object gamemodeobject = gamemode.SelectedItem;
            string gamemodestring = gamemodeobject.ToString();
            gamemodestring = gamemodestring.ToLower();

            config.set("gamemode", gamemodestring);
            config.Save();
        }

        private void changedifficulty_Click(object sender, EventArgs e)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");

            object difficultyobject = difficulty.SelectedItem;
            string difficultystring = difficultyobject.ToString();
            difficultystring = difficultystring.ToLower();

            config.set("difficulty", difficultystring);
            config.Save();
        }

        private void changemainport_Click(object sender, EventArgs e)
        {
            int finalportnumber = 0;
            Properties config = new Properties(path + servernumberpath + "server.properties");
            string mainportnumber = mainport.Text.ToString();
            try
            {
                finalportnumber = Convert.ToInt32(mainportnumber);
            }
            catch (FormatException)
            {
                const string message = "You must input an number!";
                const string caption = "Incorrect Format!";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (1 <= finalportnumber && finalportnumber <= 65535)
                {
                    config.set("server-port", finalportnumber);
                    config.Save();
                }
                else
                {
                    const string message = "Port must be within 1 and 65535!";
                    const string caption = "Invalid Port!";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void changemaxplayers_Click(object sender, EventArgs e)
        {
            int maxplayersnumber = 0;
            Properties config = new Properties(path + servernumberpath + "server.properties");
            string mainmaxnumber = maxplayers.Text.ToString();
            try
            {
                maxplayersnumber = Convert.ToInt32(mainmaxnumber);
            }
            catch (FormatException)
            {
                const string message = "You must input an number!";
                const string caption = "Incorrect Format!";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (1 <= maxplayersnumber && maxplayersnumber <= 100)
                {
                    config.set("max-players", maxplayersnumber);
                    config.Save();
                }
                else
                {
                    const string message = "For your safety, we have limited the max player count to 100. If you know what you are doing, click on Expert Editing at the top, and update the count manually. It may consume to many resources for you to handle.";
                    const string caption = "Please lower your max players.";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void changeseed_Click(object sender, EventArgs e)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");
            string seednumber = seed.Text.ToString();
            try
            {
                Convert.ToInt32(seednumber);
            }
            catch (FormatException)
            {
                const string message = "You must input an number!";
                const string caption = "Incorrect Format!";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                config.set("level-seed", seednumber);
                config.Save();
            }
        }

        private void changemotd_Click(object sender, EventArgs e)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");
            string motdtext = motd.Text.ToString();
            config.set("motd", motdtext);
            config.Save();
        }
        private void changerambutton_Click(object sender, EventArgs e)
        {
            object ramselection = ramselect.SelectedItem;
            string ramselectstring = ramselection.ToString();
            File.WriteAllText(path + servernumberpath + "ramamount.txt", ramselectstring);
        }
        public void ChangeCheckedValue(CheckBox checkboxname, string propertyname)
        {
            Properties config = new Properties(path + servernumberpath + "server.properties");

            if (checkboxname.Checked == true)
            {
                config.set(propertyname, true);
            }
            else
            {
                config.set(propertyname, false);
            }
            if (cansavetruefalse == true)
            {
                config.Save();
            }
        }

        private void enablepvp_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheckedValue(enablepvp, "pvp");

        }

        private void hardcore_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheckedValue(hardcore, "hardcore");
        }

        private void enablestatus_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheckedValue(enablestatus, "enable-status");
        }

        private void enableflight_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheckedValue(enableflight, "allow-flight");
        }

        private void enablecommandblocks_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheckedValue(enablecommandblocks, "enable-command-block");
        }

        private void Whitelist_CheckedChanged(object sender, EventArgs e)
        {
            ChangeCheckedValue(Whitelist, "enforce-whitelist");
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


        private void startstopbutton_Click(object sender, EventArgs e)
        {
            string eulapath = path + servernumberpath + "eula.txt";
            if (!File.Exists(eulapath))
            {
                try
                {
                    TextWriter tw = new StreamWriter(eulapath, true);
                    tw.WriteLine("#By changing the setting below to TRUE you are indicating your agreement to our EULA (https://account.mojang.com/documents/minecraft_eula).");
                    tw.WriteLine("eula=true");
                    tw.Close();
                }
                catch
                {
                    Console.Write(e);
                }
            }


            // SERVER START CODE
            if (externalconsole.Checked == true)
            {
                Process serverprocess = new Process();
                string pathName = path + servernumberpath + "startserver.bat";
                serverprocess.StartInfo.FileName = pathName;
                serverprocess.Start();
                Task.Delay(10000).Wait();
                CheckPropertiesStatus();
                serverprocessid = serverprocess.Id;
                stopserver.Enabled = true;
                Console.WriteLine(serverprocessid);
            }
            else
            {
                Process serverprocess = new Process();
                string pathName = path + servernumberpath + "startserver.bat";
                serverprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                serverprocess.StartInfo.FileName = pathName;
                serverprocess.Start();
                Thread.Sleep(500);
                SetParent(serverprocess.MainWindowHandle, internalconsolepanel.Handle);
                CenterToScreen();
                
                Task.Delay(10000).Wait();
                CheckPropertiesStatus();
                serverprocessid = serverprocess.Id;
                stopserver.Enabled = true;
                Console.WriteLine(serverprocessid);


            }
        }
        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
        public static bool ConfirmShutDown()
        {
            const string message = "Are you sure you would like to stop the server? \nWARNING: IT IS RECOMMENDED THAT YOU TYPE \"stop\" IN THE SERVER CONSOLE TO SHUT DOWN THE SERVER CORRECTLY AND AVOID DATA LOSS";
            const string caption = "Shut Down Server";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            bool result = ConfirmShutDown();
            if (result == true)
            {
                KillProcessAndChildren(serverprocessid);
            }
            else
            {

            }
        }

        private void serverform1_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillProcessAndChildren(serverprocessid);
        }

        private void internalconsole_CheckedChanged(object sender, EventArgs e)
        {
            if (internalconsole.Checked == true)
            {
                const string message = "WARNING: This feature is very experimental and 100% subject to change. Beware when using this tool, it may cause unfixable (until restart) bugs in the program.";
                const string caption = "Fair Warning.";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

            }
        }
    }
}
