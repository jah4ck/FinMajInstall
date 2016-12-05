using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FinMajInstall
{
    public class Trace
    {
        private int status = 3;//fichier
        private int statusParam = 3;//fichier
        private int type = 3;//tout
        public void WriteLog(string arg0, int arg1, string arg2)
        {
            if (File.Exists(@"c:\ProgramData\CtrlPc\SCRIPT\RemLog.nfo"))
            {
                using (FileStream filestream = new FileStream(@"c:\ProgramData\CtrlPc\SCRIPT\RemLog.nfo", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader read = new StreamReader(filestream))
                    {
                        string ligne;
                        while ((ligne = read.ReadLine()) != null)
                        {
                            if (ligne.Length > 2)
                            {
                                string[] colonne = ligne.Split(';');
                                Int32.TryParse(colonne[0], out status);
                                Int32.TryParse(colonne[1], out statusParam);
                                Int32.TryParse(colonne[2], out type);
                            }
                        }
                    }
                }
            }
            SynchroHeure MySynchroHeure = new SynchroHeure();
            DateTime dateTraitement = DateTime.Now;
            try
            {
                dateTraitement = MySynchroHeure.GetNetworkTime();
            }
            catch (Exception err)
            {
                dateTraitement = DateTime.Now;
            }
            if (status == 3 && statusParam == 3)//mode journal
            {
                try
                {
                    string NameDate = dateTraitement.ToString("yyyyMMdd");
                    string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");
                    using (StreamWriter writer = File.AppendText(@"C:\ProgramData\CtrlPc\LOG\JOURNAL_" + NameDate + ".log"))
                    {
                        if (arg1 == 1 && (type == 3 || type == 1))
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + arg0.ToString());
                        }
                        if (arg1 == 2 && (type == 3 || type == 2))
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "INFO : " + arg0.ToString());
                        }
                    }
                }
                catch (Exception err)
                {

                }

            }
            else if (status == 2 && statusParam == 2) //mode WS
            {
                try
                {
                    Object Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                    ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                    string result = "OK";
                    if (arg1 == 1 && (type == 3 || type == 1))
                    {
                        result = ws.TraceLogNew(Guid.ToString(), dateTraitement, arg2, arg1, arg0);
                    }
                    if (arg1 == 2 && (type == 3 || type == 2))
                    {
                        result = ws.TraceLogNew(Guid.ToString(), dateTraitement, arg2, arg1, arg0);
                    }
                    if (result == "RELICA")
                    {
                        string NameDate = dateTraitement.ToString("yyyyMMdd");
                        string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");
                        using (StreamWriter writer = File.AppendText(@"C:\ProgramData\CtrlPc\LOG\RELICA_" + NameDate + ".log"))
                        {
                            if (arg1 == 1 && (type == 3 || type == 1))
                            {
                                writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + arg0.ToString());
                            }
                            if (arg1 == 2 && (type == 3 || type == 2))
                            {
                                writer.WriteLine(Date + "     " + arg2 + "     " + "INFO : " + arg0.ToString());
                            }
                        }

                    }
                }
                catch (Exception err)
                {
                    string NameDate = dateTraitement.ToString("yyyyMMdd");
                    string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");
                    using (StreamWriter writer = File.AppendText(@"C:\ProgramData\CtrlPc\LOG\JOURNAL_ERREUR_" + NameDate + ".log"))
                    {
                        writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + err.Message);
                        if (arg1 == 1 && (type == 3 || type == 1))
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + arg0.ToString());
                        }
                        if (arg1 == 2 && (type == 3 || type == 2))
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "INFO : " + arg0.ToString());
                        }
                    }
                }
            }

            if (status == 3 && statusParam == 2) //mode relica
            {
                try
                {
                    ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                    Object Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);

                    string NameDate = dateTraitement.ToString("yyyyMMdd");
                    string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");

                    using (StreamWriter writer = File.AppendText(@"C:\ProgramData\CtrlPc\LOG\RELICA_" + NameDate + ".log"))
                    {
                        if (arg1 == 1 && (type == 3 || type == 1))
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + arg0.ToString());
                            ws.SetIncrementeRelica(Guid.ToString());
                        }
                        if (arg1 == 2 && (type == 3 || type == 2))
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "INFO : " + arg0.ToString());
                            ws.SetIncrementeRelica(Guid.ToString());
                        }
                    }
                }
                catch (Exception err)
                {
                    string NameDate = dateTraitement.ToString("yyyyMMdd");
                    string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");
                    using (StreamWriter writer = File.AppendText(@"C:\ProgramData\CtrlPc\LOG\JOURNAL_ERREUR_" + NameDate + ".log"))
                    {
                        writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + err.Message);
                    }
                }
            }


        }
    }
}
