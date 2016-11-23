using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace FinMajInstall
{
    class Program
    {
        static void Main(string[] args)
        {
            ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
            Trace MyTrace = new Trace();
            string codeappli = "INSTALLATION";
            Thread.Sleep(5000);
            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
            Object versionStation = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "Version", null);
            Object Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
            Object identifiant = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "Identifiant", null);
            String hostName = Dns.GetHostName();
            try
            {
                MyTrace.WriteLog("Inscription du guid en BDD", 2, codeappli);
                ws.CreateGuid(hostName, Guid.ToString(), identifiant.ToString(), versionStation.ToString());
                
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("Inscription du guid en erreur : "+err.Message,1 , codeappli);
            }


            //Renome DeleteFTP AutoExtractibleServiceCtrlPc.exe
            try
            {
                //Controle regedit
                MyTrace.WriteLog("Vérification de la monté de version", 2, codeappli);
                string versionMaj = File.ReadAllText(@"C:\ProgramData\CtrlPc\TEMP\version.txt");
                if (versionMaj == versionStation.ToString())
                {
                    MyTrace.WriteLog("La mise à jour s'est bien déroulée", 2, codeappli);
                    MyTrace.WriteLog("Suppression de l'autoextractible", 2, codeappli);
                    File.Delete(@"C:\ProgramData\CtrlPc\SCRIPT\AutoExtractibleServiceCtrlPc.exe");
                }
                else
                {
                    MyTrace.WriteLog("La station est restée dans l'ancienne version :" + versionStation, 1, codeappli);
                }

            }
            catch (Exception err)
            {
                MyTrace.WriteLog(err.Message, 1, codeappli);
            }

            //remonté du log d'installation
            MyTrace.WriteLog("Début récupération log d'installation ", 2, codeappli);
            DateTime dateTraitement = DateTime.Now;
            string pathFile = @"C:\ProgramData\CtrlPc\LOG\Install.log";
            if (File.Exists(pathFile))
            {
                string[] ligne = File.ReadAllLines(pathFile, Encoding.Default);
                foreach (string line in ligne)
                {
                    try
                    {
                        if (line.Length > 5)
                        {
                            string ligneImport = line;
                            if (ligneImport.Contains("'"))
                            {
                                ligneImport = ligneImport.Replace("'", "''");
                            }
                            string colonne1 = ligneImport.Substring(0, 22);
                            string colonne2 = ligneImport.Substring(25);
                            colonne1 = colonne1.Replace(",", ".");
                            try
                            {
                                dateTraitement = Convert.ToDateTime(colonne1);
                                ws.TraceLog(Guid.ToString(), dateTraitement, codeappli, 2, colonne2);
                            }
                            catch (Exception)
                            {
                                ws.TraceLog(Guid.ToString(), dateTraitement, codeappli, 2, ligneImport);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
            }
                try
                {
                    MyTrace.WriteLog("Suppression du log d'installation ", 2, codeappli);
                    File.Delete(pathFile);
                }
                catch (Exception err)
                {
                    MyTrace.WriteLog(err.Message, 1, codeappli);
                }
            }
            MyTrace.WriteLog("Fin récupération log d'installation ", 2, codeappli);


            //suppression du répertoire temps
            MyTrace.WriteLog("Suppression du répertoire TEMP", 2, codeappli);
            try
            {
                Process.Start("cmd.exe", @"/C RMDIR /S /Q C:\ProgramData\CtrlPc\TEMP");
            }
            catch (Exception err)
            {
                MyTrace.WriteLog(err.Message, 1, codeappli);
            }

            //remonté du journal
            string pathJournal = @"C:\ProgramData\CtrlPc\LOG\Journal.log";
            string pathJournalTransfert = @"C:\ProgramData\CtrlPc\LOG\Journal_transfert.log";
            if (File.Exists(pathJournal))
            {
                try
                {
                    File.Move(pathJournal, pathJournalTransfert);
                    if (File.Exists(pathJournalTransfert))
                    {
                        string[] ligne = File.ReadAllLines(pathJournalTransfert);
                        string colonne1 = "";
                        string codeappli2 = "";
                        string statut = "";
                        string colonne4 = "";
                        int codeerreur = 0;
                        foreach (string line in ligne)
                        {
                            if (line.Length > 5)
                            {
                                if (line.Substring(0, 5).Contains("/"))
                                {
                                    colonne1 = line.Substring(0, 19);
                                    codeappli2 = line.Substring(24, line.LastIndexOf("    ") - 24);
                                    statut = line.Substring(line.LastIndexOf("     ") + 5, line.IndexOf(" : ", line.LastIndexOf("     ") + 5) - line.LastIndexOf("     ") - 5);
                                    colonne4 = line.Substring(line.IndexOf(" : ", line.LastIndexOf("     ") + 5) + 3);
                                    colonne1 = colonne1.Trim();
                                    codeappli2 = codeappli2.Trim();
                                    statut = statut.Trim();
                                    if (colonne4.Contains("'"))
                                    {
                                        colonne4 = colonne4.Replace("'", "''");
                                    }
                                    
                                    if (statut == "INFO")
                                    {
                                        codeerreur = 2;
                                    }
                                    else
                                    {
                                        codeerreur = 1;
                                    }
                                    // Console.WriteLine(colonne4);
                                    try
                                    {
                                        dateTraitement = Convert.ToDateTime(colonne1);
                                        ws.TraceLog(Guid.ToString(), dateTraitement, codeappli2, codeerreur, colonne4);
                                        //Console.WriteLine(dateTraitement.ToString("yyyyMMddHHmmss") + ";" + colonne2);
                                    }
                                    catch (Exception)
                                    {
                                        ws.TraceLog(Guid.ToString(), dateTraitement, codeappli2, codeerreur, line);
                                        //Console.WriteLine(dateTraitement.ToString("yyyyMMddHHmmss")+";"+line);
                                    }
                                }
                                
                            }
                            //Console.WriteLine(colonne1);
                        }
                    }
                    File.Delete(pathJournalTransfert);
                }
                catch (Exception err)
                {
                    
                }
            }
            


        }
    }
}
