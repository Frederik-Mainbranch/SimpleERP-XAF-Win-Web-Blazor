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
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.XtraEditors;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]

    public abstract class Aktion : BaseObject
    {
        [Browsable(false)]
        public Aktion(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------


        protected override void OnSaving()
        {
            if(IsDeleted == false)
            {
                ZeitraumHelper2 zeitraumHelper = new ZeitraumHelper2((Aktion)This, AktionStart, AktionEnde);

                zeitraumHelper.IstNeueAktionZulässig();
                ÜberprüfeStatusDerAktion();

                AktionWurdeCommited = true;
            }
            base.OnSaving();
        }


        protected override void OnDeleting()
        {
            base.OnDeleting();
            AktionWurdeGeloescht_NP = true;
        }

        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Methoden ---------------------------------------------

        private void ÜberprüfeStatusDerAktion()
        {
            if (AktionStart != DateTime.MinValue && AktionEnde != DateTime.MinValue)
            {
                //abgelaufen
                if (DateTime.Today > AktionEnde)
                {
                    Status = "abgelaufen";
                }

                //aktiv
                else if (DateTime.Today >= AktionStart && DateTime.Today <= AktionEnde)
                {
                    Status = "aktiv";
                }

                //zukünftig
                else if (DateTime.Today < AktionStart)
                {
                    Status = "zukünftig";
                }

                //Fehler Falle
                else
                {
                    Status = "Fehler beim bestimmen des Status der Aktion";
                }
            }
            else
            {
                Status = "Bitte wählen Sie einen Aktionszeitraum aus!";
            }
        }

        //-------------------------------- Methoden ---------------------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private bool _AktionWurdeCommited;
        [Browsable(false)]
        public bool AktionWurdeCommited
        {
            get { return _AktionWurdeCommited; }
            set { SetPropertyValue<bool>(nameof(AktionWurdeCommited), ref _AktionWurdeCommited, value); }
        }


        private string _Status;
        [ModelDefault("AllowEdit", "false")]
        public string Status
        {
            get { return _Status; }
            set { SetPropertyValue<string>(nameof(Status), ref _Status, value); }
        }


        private DateTime _AktionStart;
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField]
        public DateTime AktionStart  //Für interne Zwecke
        {
            get { return _AktionStart; }
            set { SetPropertyValue<DateTime>(nameof(AktionStart), ref _AktionStart, value); }
        }


        private DateTime _AktionEnde;
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField]
        public DateTime AktionEnde  //Für interne Zwecke
        {
            get { return _AktionEnde; }
            set { SetPropertyValue<DateTime>(nameof(AktionEnde), ref _AktionEnde, value); }
        }


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------

        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------

        private bool _AktionWurdeGeloescht_NP;
        [NonPersistent]
        [Browsable(false)]
        public bool AktionWurdeGeloescht_NP
        {
            get { return _AktionWurdeGeloescht_NP; }
            set { SetPropertyValue<bool>(nameof(AktionWurdeGeloescht_NP), ref _AktionWurdeGeloescht_NP, value); }
        }

        //-------------------------------- Non Persistent Properties ---------------------------------------------

    }
}