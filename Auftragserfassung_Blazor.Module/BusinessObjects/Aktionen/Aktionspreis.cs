using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using Auftragserfassung_Blazor.Module.Interfaces;
using Auftragserfassung_Blazor.Module.Helpers;


namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Aktionspreis : Aktion
    {
        public Aktionspreis(Session session)
            : base(session)
        {
        }

        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false)
            {
                if (propertyName == "AktionsArtikel" && AktionsArtikel != null && AktionWurdeCommited == false)      //besorgt beim ersten Anzeigen der Aktion den Preis des Artikels aus der Datenbank
                {
                    AktionPreis = AktionsArtikel.StandardPreis;
                }
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving();

            if(IsDeleted == false)
            {
                AktionsHelper2000 aktionsHelper = new AktionsHelper2000(Session);
                aktionsHelper.UpdateAktuellenPreis(AktionsArtikel);
                //AktionsArtikel.BerechneAktuellenPreis();
            }
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            AktionPreis = -1; //-1, damit beim Neuberechnen des Aktuellen Preises die Aktion uebersprungen wird

            AktionsHelper2000 aktionsHelper = new AktionsHelper2000(Session);
            aktionsHelper.UpdateAktuellenPreis(AktionsArtikel);
            //AktionsArtikel.BerechneAktuellenPreis();
        }


        //---------------------------- Override Methode -------------------------------
        //---------------------------- Methoden ----------------------------------------

        #region //alt
        /*private void ÜberprüfeDatumseingabe()
        {
            bool EintragGefunden = false;
            string pufferUserStart = (BenutzerEingabeAktionsstart.ToString()).Substring(0, 10);
            string pufferUserEnde = (BenutzerEingabeAktionsende.ToString()).Substring(0, 10);

            //Überprüfung, ob die Eingabe in einen Aktionszeitraum liegt
            foreach (Aktionspreis aktPreis in AktionsArtikel.Aktionspreise)
            {
                string pufferStart = (aktPreis.AktionStart.ToString()).Substring(0, 10);
                string pufferEnde = (aktPreis.AktionEnde.ToString()).Substring(0, 10);

                if (EintragGefunden == false && aktPreis.AktionStart != dummyDatime && aktPreis.AktionEnde != dummyDatime)
                {
                    //liegt Start/Ende in einen Zeitraum?
                    //Start
                    if ((BenutzerEingabeAktionsstart >= aktPreis.AktionStart) && (BenutzerEingabeAktionsstart <= aktPreis.AktionEnde))
                    {
                        BenutzerEingabeAktionsstart = dummyDatime;
                        BenutzerEingabeAktionsende = dummyDatime;
                        EintragGefunden = true;
                        throw new UserFriendlyException("Fehler: Der Aktionsstart liegt in der bereits vorhandenen Aktion vom " + pufferStart + " bis " + pufferEnde + "!");
                    }
                    //Ende
                    else if ((BenutzerEingabeAktionsende >= aktPreis.AktionStart) && (BenutzerEingabeAktionsende <= aktPreis.AktionEnde))
                    {
                        BenutzerEingabeAktionsstart = dummyDatime;
                        BenutzerEingabeAktionsende = dummyDatime;
                        EintragGefunden = true;
                        throw new UserFriendlyException("Fehler: Das Aktionsende liegt in der bereits vorhandenen Aktion vom " + pufferStart + " bis " + pufferEnde + "!");
                    }
                    //wird ein Aktionszeitraum "umschlossen"?
                    else if ((BenutzerEingabeAktionsstart <= aktPreis.AktionStart) && (BenutzerEingabeAktionsende >= aktPreis.AktionEnde))
                    {
                        BenutzerEingabeAktionsstart = dummyDatime;
                        BenutzerEingabeAktionsende = dummyDatime;
                        EintragGefunden = true;
                        throw new UserFriendlyException("Fehler: Es wird die bereits vorhandene Aktion vom " + pufferStart + " bis " + pufferEnde + " eingeschlossen!");
                    }
                    //liegt das Ende vor dem Anfang?
                    else if (BenutzerEingabeAktionsstart > BenutzerEingabeAktionsende)
                    {
                        BenutzerEingabeAktionsstart = dummyDatime;
                        BenutzerEingabeAktionsende = dummyDatime;
                        EintragGefunden = true;
                        throw new UserFriendlyException("Fehler: Das Aktionsende " + pufferUserEnde + " liegt vor dem Aktionsanfang " + pufferUserStart + "!");
                    }
                }
            }

            //Wenn die User Eingaben keinen anderen Aktionszeitraum verletzen
            if (EintragGefunden == false)
            {
                AktionStart = BenutzerEingabeAktionsstart;
                AktionEnde = BenutzerEingabeAktionsende;
                AnzeigeStatus = Status;
            }
        }*/
        #endregion

        //---------------------------- Methoden ----------------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private double _AktionPreis;
        [ModelDefault("DisplayFormat", "{0:#,###,##0.00 €}")]
        [ModelDefault("EditMask", "#,###,##0.00 €")]
        public double AktionPreis
        {
            get { return _AktionPreis; }
            set { SetPropertyValue<double>(nameof(AktionPreis), ref _AktionPreis, value); }
        }

        private Artikel _AktionsArtikel;
        [Association("Artikel-Aktionspreis")]
        [Browsable(false)]
        public Artikel AktionsArtikel
        {
            get { return _AktionsArtikel; }
            set { SetPropertyValue<Artikel>(nameof(AktionsArtikel), ref _AktionsArtikel, value); }
        }


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------



        //-------------------------------- Non Persistent Properties ---------------------------------------------
    }
}