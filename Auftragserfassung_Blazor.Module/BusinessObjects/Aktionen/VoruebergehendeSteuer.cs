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

    public class VoruebergehendeSteuer : Aktion
    {

        public VoruebergehendeSteuer(Session session)
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
                if (propertyName == nameof(Steuer) && Steuer != null && AktionWurdeCommited == false) //Debug: after Construction Ersatz?
                {
                    AktionsSteuer = Steuer.StandardSteuersatz;
                }
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving();
            AktionsHelper2000 AktionsHelper = new AktionsHelper2000(Session);

            if (IsDeleted == false)
            {
                AktionsHelper.UpdateAktuelleSteuer(Steuer);
            }
            else
            {
                AktionsHelper.UpdateAlleAktuelleSteuern();
            }
        }



        //---------------------------- Override Methode -------------------------------
        //---------------------------- Methoden ----------------------------------------

        #region //alt
        //private void ÜberprüfeDatumseingabe()
        //{
        //    bool EintragGefunden = false;
        //    string pufferUserStart = (BenutzerEingabeAktionsstart.ToString()).Substring(0, 10);
        //    string pufferUserEnde = (BenutzerEingabeAktionsende.ToString()).Substring(0, 10);

        //    //Überprüfung, ob die Eingabe in einen Aktionszeitraum liegt
        //    foreach (Aktionssteuer aktSteuer in AktionsArtikel.Aktionssteuern)
        //    {
        //        string pufferStart = (aktSteuer.AktionStart.ToString()).Substring(0, 10);
        //        string pufferEnde = (aktSteuer.AktionEnde.ToString()).Substring(0, 10);

        //        if (EintragGefunden == false && aktSteuer.AktionStart != dummyDatime && aktSteuer.AktionEnde != dummyDatime)
        //        {
        //            //liegt Start/Ende in einen Zeitraum?
        //            //Start
        //            if ((BenutzerEingabeAktionsstart >= aktSteuer.AktionStart) && (BenutzerEingabeAktionsstart <= aktSteuer.AktionEnde))
        //            {
        //                BenutzerEingabeAktionsstart = dummyDatime;
        //                BenutzerEingabeAktionsende = dummyDatime;
        //                EintragGefunden = true;
        //                throw new UserFriendlyException("Fehler: Der Aktionsstart liegt in der bereits vorhandenen Aktion vom " + pufferStart + " bis " + pufferEnde + "!");
        //            }
        //            //Ende
        //            else if ((BenutzerEingabeAktionsende >= aktSteuer.AktionStart) && (BenutzerEingabeAktionsende <= aktSteuer.AktionEnde))
        //            {
        //                BenutzerEingabeAktionsstart = dummyDatime;
        //                BenutzerEingabeAktionsende = dummyDatime;
        //                EintragGefunden = true;
        //                throw new UserFriendlyException("Fehler: Das Aktionsende liegt in der bereits vorhandenen Aktion vom " + pufferStart + " bis " + pufferEnde + "!");
        //            }
        //            //wird ein Aktionszeitraum "umschlossen"?
        //            else if ((BenutzerEingabeAktionsstart <= aktSteuer.AktionStart) && (BenutzerEingabeAktionsende >= aktSteuer.AktionEnde))
        //            {
        //                BenutzerEingabeAktionsstart = dummyDatime;
        //                BenutzerEingabeAktionsende = dummyDatime;
        //                EintragGefunden = true;
        //                throw new UserFriendlyException("Fehler: Es wird die bereits vorhandene Aktion vom " + pufferStart + " bis " + pufferEnde + " eingeschlossen!");
        //            }
        //            //liegt das Ende vor dem Anfang?
        //            else if (BenutzerEingabeAktionsstart > BenutzerEingabeAktionsende)
        //            {
        //                BenutzerEingabeAktionsstart = dummyDatime;
        //                BenutzerEingabeAktionsende = dummyDatime;
        //                EintragGefunden = true;
        //                throw new UserFriendlyException("Fehler: Das Aktionsende " + pufferUserEnde + " liegt vor dem Aktionsanfang " + pufferUserStart + "!");
        //            }
        //        }
        //    }

        //    //Wenn die User Eingaben keinen anderen Aktionszeitraum verletzen
        //    if (EintragGefunden == false)
        //    {
        //        AktionStart = BenutzerEingabeAktionsstart;
        //        AktionEnde = BenutzerEingabeAktionsende;
        //        AnzeigeStatus = Status;
        //    }
        //}

        #endregion
        //---------------------------- Methoden ----------------------------------------
        //-------------------------------- Properties ---------------------------------------------




        private double _AktionsSteuer;
        [ModelDefault("DisplayFormat", "{0:#0.# '%'}")]
        [ModelDefault("EditMask", "#0.#")]
        public double AktionsSteuer
        {
            get { return _AktionsSteuer; }
            set { SetPropertyValue<double>(nameof(AktionsSteuer), ref _AktionsSteuer, value); }
        }


        private Steuer _Steuer;
        [Association("Steuer-VoruebergehendeSteuer")]
        public Steuer Steuer
        {
            get { return _Steuer; }
            set { SetPropertyValue<Steuer>(nameof(Steuer), ref _Steuer, value); }
        }


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------

        #region
        //private DateTime _BenutzerEingabeAktionsstart;
        //[NonPersistent]
        //[ImmediatePostData]
        //[ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        //[ModelDefault("EditMask", "dd/MM/yyyy")]
        //public DateTime BenutzerEingabeAktionsstart
        //{
        //    get { return _BenutzerEingabeAktionsstart; }
        //    set { SetPropertyValue<DateTime>(nameof(BenutzerEingabeAktionsstart), ref _BenutzerEingabeAktionsstart, value); }
        //}

        //private DateTime _BenutzerEingabeAktionsende;
        //[NonPersistent]
        //[ImmediatePostData]
        //[ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        //[ModelDefault("EditMask", "dd/MM/yyyy")]
        //public DateTime BenutzerEingabeAktionsende
        //{
        //    get { return _BenutzerEingabeAktionsende; }
        //    set { SetPropertyValue<DateTime>(nameof(BenutzerEingabeAktionsende), ref _BenutzerEingabeAktionsende, value); }
        //}


        //private Artikel _AktionsArtikel;
        //[NonPersistent]
        //[Browsable(false)]
        //public Artikel AktionsArtikel
        //{
        //    get { return _AktionsArtikel; }
        //    set { SetPropertyValue<Artikel>(nameof(AktionsArtikel), ref _AktionsArtikel, value); }
        //}

        #endregion

        //-------------------------------- Non Persistent Properties ---------------------------------------------
    }
}