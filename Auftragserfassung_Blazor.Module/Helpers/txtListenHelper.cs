using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Reflection;


namespace Auftragserfassung_Blazor.Module.Helpers
{

    class txtListenHelper
    {
        public txtListenHelper(string speicherort)
        {
            Speicherort = speicherort + "\\";
        }

        public txtListenHelper()
        {
        }
        public string Speicherort { get; set; } = @"C:\Users\frederik\Documents\Debug\Auftragserfassung_Blazor\";

        // string speicherortDebugDateien = @"C:\Users\frederik\Documents\Debug\AuftragserfassungXAF\";

        //Die .txt Liste muss unter %Projektname%.module => Eigenschaften => Ressourcen => Dateien als .txt Liste hinzugefügt werden. Jeder Eintrag muss durch ein Zeilenumbruch (\r\n) vom nächsten getrennt werden.
        //Beispiel:
        //Adam\r\n
        //Bernd\r\n
        //Christoph\r\n
        //Beim letzten Eintrag ist das \r\n optional (wird automatisch entfernt)
        //Sollte die letzte Zeile leer sein, wird diese automatisch entfernt.

        Random zufälligeZahl = new Random();
        public string[] ZufälligerWertundSeineZeilennummer(string imputTxtListe)
        {
            // beim Methodenaufruf muss mit ZufälligerWert(Properties.Resources.%Listenname%) die korrekte Liste ausgewählt werden
            // die Rückgabe erfolgt als Stringarray, wobei [0] der ermittelte Wert ist und [1] die Zeilennummer(bezogen auf Startindex von Null des Arrays. Achtung: Im Standard Windows txt Editor ist der Startindex bei 1!!)
            
            imputTxtListe = imputTxtListe.Replace("\r\n", "#");
            string[] txtListe = imputTxtListe.Split('#');

            if (txtListe[txtListe.Length - 1] == "") //Falls der letzte Eintrag leer ist, wird dieser entfernt
            {
                List<string> puffer = txtListe.ToList();
                puffer.RemoveAt(puffer.Count - 1);
                txtListe = puffer.ToArray();
            }

            int zufallswert = zufälligeZahl.Next(0, txtListe.Length - 1);
            string[] ausgabe = { txtListe[zufallswert], zufallswert + "" };
            return ausgabe;
        }


        public string ZufälligerWert(string imputTxtListe)
        {
            // beim Methodenaufruf muss mit ZufälligerWert(Properties.Resources.%Listenname%) die korrekte Liste ausgewählt werden
            // die Rückgabe erfolgt als String
            imputTxtListe = imputTxtListe.Replace("\r\n", "#");
            string[] txtListe = imputTxtListe.Split('#');

            if (txtListe[txtListe.Length - 1] == "") //Falls der letzte Eintrag leer ist, wird dieser entfernt
            {
                List<string> puffer = txtListe.ToList();
                puffer.RemoveAt(puffer.Count - 1);
                txtListe = puffer.ToArray();
            }

            int zufallswert = zufälligeZahl.Next(0, txtListe.Length - 1);
            return txtListe[zufallswert];
        }


        public string BestimmterWert(string imputTxtListe, int zeilennummer)
        {
            // beim Methodenaufruf muss mit ZufälligerWert(Properties.Resources.%Listenname%, zeilennummer) die korrekte Liste und die ausgesuchte Zeilennummer (bezogen auf Startindex von 0) ausgewählt werden. (Achtung: Im Standard Windows txt Editor ist der Startindex bei 1!!)
            // die Rückgabe erfolgt als String
            imputTxtListe = imputTxtListe.Replace("\r\n", "#");
            string[] txtListe = imputTxtListe.Split('#');

            if (txtListe[txtListe.Length - 1] == "") //Falls der letzte Eintrag leer ist, wird dieser entfernt
            {
                List<string> puffer = txtListe.ToList();
                puffer.RemoveAt(puffer.Count - 1);
                txtListe = puffer.ToArray();
            }

            return txtListe[zeilennummer];
        }

        public string[] KonvertiereTxtStringzuStringArray(string imput)
        {
            imput = imput.Replace("\r\n", "#");
            string[] export = imput.Split('#');

            if (export[export.Length - 1] == "") //Falls der letzte Eintrag leer ist, wird dieser entfernt
            {
                List<string> puffer = export.ToList();
                puffer.RemoveAt(puffer.Count - 1);
                export = puffer.ToArray();
            }

            return export;
        }

        public void SchreibeInVorhandeneDatei(string dateiname, string text)
        {
            string speicherortExport = Speicherort + dateiname + ".txt";

            using (StreamWriter datei = new StreamWriter(speicherortExport, true))
            {
                datei.WriteLine(text);
            }
        }

        public void SchreibeInNeueDatei(string dateiname, string text)
        {
            LeereDebugDatei(dateiname);
            string speicherortExport = Speicherort + dateiname + ".txt";

            using (StreamWriter datei = new StreamWriter(speicherortExport, true))
            {
                datei.WriteLine(text);
            }
        }

        public void LeereDebugDatei(string dateiname)
        {
            string speicherort_LeereDebugDatei = $@"{Speicherort}{dateiname}.txt";
            File.WriteAllText(speicherort_LeereDebugDatei, "");
        }

        public void ÜberprüfeAufVorkommenInListe(string className, string text)
        {
            string dateiName = $"{ className }.txt";
            string datei = Speicherort + dateiName;
            if (File.Exists(datei) == false)
            {
                using (StreamWriter streamWriter = new StreamWriter(datei, true))
                {
                    streamWriter.Write("");
                }
            }

            string[] zeilen = File.ReadAllLines(datei);

            bool gefunden = false;
            foreach (string zeile in zeilen)
            {
                if(zeile == text)
                {
                    gefunden = true;
                    break;
                }
            }

            if (gefunden == false)
            {
                using (StreamWriter streamWriter = new StreamWriter(datei, true))
                {
                    streamWriter.WriteLine(text);
                }
            }
        }


        public string[] BesorgeGanzeListe(string dateiname)
        {
            string speicherortExport = Speicherort + dateiname + ".txt";
            return File.ReadAllLines(speicherortExport);
        }

        public void ÜberprüfeAufVorkommenInListeMitExtraEigenschaften(string dateiname, string bezeichnung, string[] eigenschaften_typ, string[] eigenschaften_inhalt)
        {
            //string speicherOrt = @"C:\Users\frederik\Documents\Debug\AuftragserfassungXAF\";
            string dateiname_mitEndung = $"{ dateiname }.txt";
            string datei_mitPfad = Speicherort + dateiname_mitEndung;

            if (File.Exists(datei_mitPfad) == false)
            {
                using (StreamWriter streamWriter = new StreamWriter(datei_mitPfad, true))
                {
                    streamWriter.Write("");
                }

                for (int i = 0; i < eigenschaften_typ.Length; i++)
                {
                    using (StreamWriter streamWriter = new StreamWriter($"{Speicherort}{ dateiname }_{eigenschaften_typ[i]}.txt", true))
                    {
                        streamWriter.Write("");
                    }
                }
            }

            string[] zeilen = File.ReadAllLines(datei_mitPfad);

            bool gefunden = false;
            foreach (string zeile in zeilen)
            {
                if (zeile == bezeichnung)
                {
                    gefunden = true;
                    break;
                }
            }

            if (gefunden == false)
            {
                using (StreamWriter streamWriter = new StreamWriter(datei_mitPfad, true))
                {
                    streamWriter.WriteLine(bezeichnung);
                }

                for (int i = 0; i < eigenschaften_typ.Length; i++)
                {
                    using (StreamWriter streamWriter = new StreamWriter($"{Speicherort}{ dateiname }_{eigenschaften_typ[i]}.txt", true))
                    {
                        streamWriter.WriteLine(eigenschaften_inhalt[i]);
                    }
                }
            }
        }

    }
}

