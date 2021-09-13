using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public abstract partial class txtDatenbankViewController : ViewController
    {
        public Random zufallsWertFeld = new Random();

        public txtDatenbankViewController()
        {
            InitializeComponent();
            TargetViewType = ViewType.ListView;
            TargetObjectType = null;

            ParametrizedAction testParametrizedAction = new ParametrizedAction(this, "txtDatenbankViewController", PredefinedCategory.Edit, typeof(int));
            testParametrizedAction.TargetViewType = ViewType.ListView;
            //testParametrizedAction.Id = "zufällige" + GetType().Name;
            //Test

            testParametrizedAction.Id = GetType().Name;
            
            testParametrizedAction.Execute += testParametrizedAction_Execute;
        }

        protected void testParametrizedAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            int anzahlZuerstellenderObjekte = (int)(e.ParameterCurrentValue);

            if (anzahlZuerstellenderObjekte > 0)
            {
                Type type = View.ObjectTypeInfo.Type;
                SetzteZielObject(((XPObjectSpace)this.ObjectSpace).Session, anzahlZuerstellenderObjekte, type);

                if (this.ObjectSpace.IsModified)
                {
                    this.ObjectSpace.CommitChanges();
                    View.Refresh(true);
                }
            }
            else
            {
                throw new UserFriendlyException("Bitte geben Sie einen Wet größer als 0 ein!");
            }
                    ((ParametrizedAction)sender).Value = 0;
        }

        protected virtual void SetzteZielObject(Session session, int anzahl, Type type)
        {
            //wird überschrieben in Child
        }

        //------------------------------------ Zufällige Werte --------------------------------------------

        public string ZufälligerWertString(string imputTxtListe)
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

            int zufallswert = zufallsWertFeld.Next(0, txtListe.Length - 1);
            return txtListe[zufallswert];
        }


        public string[] ZufälligerWertundSeineZeilennummerString(string imputTxtListe)
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

            int zufallswert = zufallsWertFeld.Next(0, txtListe.Length - 1);
            string[] ausgabe = { txtListe[zufallswert], zufallswert + "" };
            return ausgabe;
        }


        public string[] ZufälligerWertundSeineZeilennummerTxt(string speicherort, string dateiname)
        {
            string datei = $"{speicherort}'\'{dateiname}.txt";
            string[] txtDatei = File.ReadAllLines(datei);
            int zeilenNummer = zufallsWertFeld.Next(0, txtDatei.Length);
            string[] ausgabe = new string[] { txtDatei[zeilenNummer], zeilenNummer.ToString() };
            return ausgabe;
        }


        public string BestimmterWertString(string imputTxtListe, int zeilennummer)
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

        public string BestimmterWertTxt(string speicherort, string dateiname, int zeilenNummer)
        {
            string datei = $"{speicherort}'\'{dateiname}.txt";
            string[] txtDatei = File.ReadAllLines(datei);
            string ausgabe = txtDatei[zeilenNummer];
            return ausgabe;
        }
    }
}
